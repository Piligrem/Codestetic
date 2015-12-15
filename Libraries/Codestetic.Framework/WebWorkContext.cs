using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Codestetic.Web.Collections;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.Localization;
using Codestetic.Web.Core.Fakes;
using Codestetic.Web.Services.Authentication;
using Codestetic.Web.Services.Common;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Framework.Localization;
using System.Diagnostics;

namespace Codestetic.Web.Framework
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        private const string UserCookieName = "specter.user";

        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILanguageService _languageService;
        //private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _attrService;
        //private readonly CurrencySettings _currencySettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;

        private Language _cachedLanguage;
        private User _cachedUser;
        //private Currency _cachedCurrency;
        private User _originalUserIfImpersonated;

        public WebWorkContext(Func<string, ICacheManager> cacheManager,
            HttpContextBase httpContext,
            IUserService userService,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            //ICurrencyService currencyService,
            IGenericAttributeService attrService,
            //CurrencySettings currencySettings,
            LocalizationSettings localizationSettings,
            IWebHelper webHelper, 
            ISettingService settingService)
        {
            this._cacheManager = cacheManager("static");
            this._httpContext = httpContext;
            this._userService = userService;
            this._authenticationService = authenticationService;
            this._languageService = languageService;
            this._attrService = attrService;
            //this._currencyService = currencyService;
            //this._currencySettings = currencySettings;
            this._localizationSettings = localizationSettings;
            this._webHelper = webHelper;
            this._settingService = settingService;
        }

        protected HttpCookie GetUserCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[UserCookieName];
        }

        protected void SetUserCookie(Guid userGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserCookieName);
                cookie.HttpOnly = true;
                cookie.Value = userGuid.ToString();
                if (userGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24 * 365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(UserCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public User CurrentUser
        {
            get
            {
                if (_cachedUser != null)
                {
                    return _cachedUser;
                }

                User user = null;
                if (_httpContext == null || _httpContext is FakeHttpContext)
                {
                    //check whether request is made by a background task
                    //in this case return built-in user record for background task
                    user = _userService.GetUserBySystemName(SystemUserNames.BackgroundTask);
                }

                //check whether request is made by a search engine
                //in this case return built-in user record for search engines 
                //or comment the following two lines of code in order to disable this functionality
                if (user == null || user.Deleted || !user.Active)
                {
                    if (_webHelper.IsSearchEngine(_httpContext))
                        user = _userService.GetUserBySystemName(SystemUserNames.SearchEngine);
                }

                //registered user
                if (user == null || user.Deleted || !user.Active)
                {
                    user = _authenticationService.GetAuthenticatedUser();
                }

                //impersonate user if required (currently used for 'phone order' support)
                if (user != null && !user.Deleted && user.Active)
                {
                    int? impersonatedUserId = user.GetAttribute<int?>(SystemUserAttributeNames.ImpersonatedUserId);
                    if (impersonatedUserId.HasValue && impersonatedUserId.Value > 0)
                    {
                        var impersonatedUser = _userService.GetUserById(impersonatedUserId.Value);
                        if (impersonatedUser != null && !impersonatedUser.Deleted && impersonatedUser.Active)
                        {
                            //set impersonated user
                            _originalUserIfImpersonated = user;
                            user = impersonatedUser;
                        }
                    }
                }

                //load guest user
                if (user == null || user.Deleted || !user.Active)
                {
                    var userCookie = GetUserCookie();
                    if (userCookie != null && !String.IsNullOrEmpty(userCookie.Value))
                    {
                        Guid userGuid;
                        if (Guid.TryParse(userCookie.Value, out userGuid))
                        {
                            var userByCookie = _userService.GetUserByGuid(userGuid);
                            if (userByCookie != null &&
                                //this user (from cookie) should not be registered
                                !userByCookie.IsRegistered() &&
                                //it should not be a built-in 'search engine' user account
                                !userByCookie.IsSearchEngineAccount())
                                user = userByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (user == null || user.Deleted || !user.Active)
                {
                    user = _userService.InsertGuestUser();
                }


                //validation
                if (!user.Deleted && user.Active)
                {
                    SetUserCookie(user.UserGuid);
                    _cachedUser = user;
                }

                return _cachedUser;
            }
            set
            {
                SetUserCookie(value.UserGuid);
                _cachedUser = value;
            }
        }

        /// <summary>
        /// Gets or sets the original user (in case the current one is impersonated)
        /// </summary>
        public User OriginalUserIfImpersonated
        {
            get
            {
                return _originalUserIfImpersonated;
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                long userLangId = 0;

                if (this.CurrentUser != null)
                    userLangId = this.CurrentUser.GetAttribute<int>(SystemUserAttributeNames.LanguageId, _attrService);

                #region Get language from URL (if possible)
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && _httpContext != null && _httpContext.Request != null)
                {
                    var helper = new LocalizedUrlHelper(_httpContext.Request, true);
                    string seoCode;
                    if (helper.IsLocalizedUrl(out seoCode))
                    {
                        if (this.IsPublishedLanguage(seoCode))
                        {
                            // the language is found. now we need to save it
                            var langBySeoCode = _languageService.GetLanguageBySeoCode(seoCode);
                            if (this.CurrentUser != null && userLangId != langBySeoCode.Id)
                            {
                                userLangId = langBySeoCode.Id;
                                this.SetUserLanguage(langBySeoCode.Id);
                            }
                            _cachedLanguage = langBySeoCode;
                            return langBySeoCode;
                        }
                    }
                }
                #endregion

                if (_localizationSettings.DetectBrowserUserLanguage && (userLangId == 0 || !this.IsPublishedLanguage(userLangId)))
                {
                    #region Get Browser UserLanguage

                    // Fallback to browser detected language
                    Language browserLanguage = null;

                    if (_httpContext != null && _httpContext.Request != null && _httpContext.Request.UserLanguages != null)
                    {
                        var userLangs = _httpContext.Request.UserLanguages.Select(x => x.Split(new[] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries)[0]);
                        if (userLangs.HasItems())
                        {
                            foreach (var culture in userLangs)
                            {
                                browserLanguage = _languageService.GetLanguageByCulture(culture);
                                if (browserLanguage != null && this.IsPublishedLanguage(browserLanguage.Id))
                                {
                                    // the language is found. now we need to save it
                                    if (this.CurrentUser != null && userLangId != browserLanguage.Id)
                                    {
                                        userLangId = browserLanguage.Id;
                                        SetUserLanguage(userLangId);
                                    }
                                    _cachedLanguage = browserLanguage;
                                    return browserLanguage;
                                }
                            }
                        }
                    }

                    #endregion
                }

                if (userLangId > 0 && this.IsPublishedLanguage(userLangId))
                {
                    _cachedLanguage = _languageService.GetLanguageById(userLangId);
                    return _cachedLanguage;
                }

                // Fallback
                userLangId = this.GetDefaultLanguageId();
                SetUserLanguage(userLangId);

                _cachedLanguage = _languageService.GetLanguageById(userLangId);
                return _cachedLanguage;
            }
            set
            {
                var languageId = value != null ? value.Id : 0;
                this.SetUserLanguage(languageId);
                _cachedLanguage = null;
            }
        }

        private void SetUserLanguage(long languageId)
        {
            _attrService.SaveAttribute(this.CurrentUser, SystemUserAttributeNames.LanguageId, languageId);
        }

        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        //public Currency WorkingCurrency
        //{
        //    get
        //    {
        //        if (_cachedCurrency != null)
        //            return _cachedCurrency;

        //        bool fixPrimarySiteCurrency = false;
        //        Currency currency = null;
        //        // return primary store currency when we're in admin area/mode
        //        if (this.IsAdmin)
        //        {
        //            currency = _currencyService.GetCurrencyById(_currencySettings.PrimarySiteCurrencyId);
        //        }

        //        if (currency == null)
        //        {
        //            // find current user language
        //            var user = this.CurrentUser;

        //            if (user != null && !user.IsSearchEngineAccount())
        //            {
        //                // search engines should always crawl by primary store currency
        //                var userCurrencyId = user.GetAttribute<int?>(SystemUserAttributeNames.CurrencyId, _attrService);
        //                if (userCurrencyId.GetValueOrDefault() > 0)
        //                {
        //                    currency = VerifyCurrency(_currencyService.GetCurrencyById(userCurrencyId.Value));
        //                    if (currency == null)
        //                    {
        //                        _attrService.SaveAttribute<int?>(user, SystemUserAttributeNames.CurrencyId, null);
        //                    }
        //                }
        //            }

        //            // find currency by domain ending
        //            if (currency == null && _httpContext != null && _httpContext.Request != null && _httpContext.Request.Url != null)
        //            {
        //                currency = _currencyService
        //                    .GetAllCurrencies()
        //                    .GetByDomainEnding(_httpContext.Request.Url.Authority);
        //            }

        //            // get PrimarySiteCurrency
        //            if (currency == null)
        //            {
        //                currency = VerifyCurrency(_currencyService.GetCurrencyById(_currencySettings.PrimarySiteCurrencyId));
        //                fixPrimarySiteCurrency = (currency == null);
        //            }

        //            // get the first published currency for current store
        //            if (currency == null)
        //            {
        //                var allSiteCurrencies = _currencyService.GetAllCurrencies();
        //                if (allSiteCurrencies.Count > 0)
        //                {
        //                    currency = allSiteCurrencies.FirstOrDefault();
        //                }
        //            }
        //        }

        //        // if not found in currencies filtered by the current store, then return any currency
        //        if (currency == null)
        //        {
        //            currency = _currencyService.GetAllCurrencies().FirstOrDefault();
        //        }

        //        // no published currency available (fix it)
        //        if (currency == null)
        //        {
        //            currency = _currencyService.GetAllCurrencies(true).FirstOrDefault();
        //            if (currency != null)
        //            {
        //                currency.Published = true;
        //                _currencyService.UpdateCurrency(currency);
        //            }
        //        }

        //        if (fixPrimarySiteCurrency)
        //        {
        //            _currencySettings.PrimarySiteCurrencyId = currency.Id;
        //            _settingService.UpdateSetting(_currencySettings, x => x.PrimarySiteCurrencyId, true);
        //        }


        //        _cachedCurrency = currency;
        //        return _cachedCurrency;
        //    }
        //    set
        //    {
        //        long? id = value != null ? value.Id : (long?)null;
        //        _attrService.SaveAttribute<long?>(this.CurrentUser, SystemUserAttributeNames.CurrencyId, id);
        //        _cachedCurrency = null;
        //    }
        //}

        //private Currency VerifyCurrency(Currency currency)
        //{
        //    if (currency != null && !currency.Published)
        //    {
        //        return null;
        //    }
        //    return currency;
        //}

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        public bool IsAdmin { get; set; }

        public bool IsPublishedLanguage(string seoCode)
        {

            var map = this.GetLanguageMap();
            return map.Any(x => x.Value.ToString() == seoCode);
        }
        internal bool IsPublishedLanguage(long languageId)
        {
            if (languageId <= 0)
                return false;

            var map = this.GetLanguageMap();
            return map.Any(x => x.Key == languageId);
        }

        public string GetDefaultLanguageSeoCode()
        {
            var map = this.GetLanguageMap();
            return map.FirstOrDefault().Value.ToString();
        }
        internal long GetDefaultLanguageId()
        {
            var map = this.GetLanguageMap();
            return map.FirstOrDefault().Key;
        }

        /// <summary>
        /// Gets a map of active/published store languages
        /// </summary>
        /// <returns>A map of store languages where key is the store id and values are tuples of language ids and seo codes</returns>
        protected virtual Multimap<long, string> GetLanguageMap()
        {
            var result = _cacheManager.Get(FrameworkCacheConsumer.LANGUAGE_MAP_KEY, () =>
            {
                var map = new Multimap<long, string>();

                var languages = _languageService.GetAllLanguages(false);
                if (!languages.Any())
                {
                    // language-less sites aren't allowed but could exist accidentally. Correct this.
                    var firstLang = _languageService.GetAllLanguages(true).FirstOrDefault();
                    if (firstLang == null)
                    {
                        // absolute fallback
                        firstLang = _languageService.GetAllLanguages(true).FirstOrDefault();
                    }
                    map.Add(firstLang.Id, firstLang.UniqueSeoCode);
                }
                else
                {
                    foreach (var lang in languages)
                    {
                        map.Add(lang.Id, lang.UniqueSeoCode);
                    }
                }

                return map;
            }, 1440 /* 24 hrs */);

            return result;
        }
    }
}
