using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;

using Codestetic.Web.Data;
using Codestetic.Web.Models;
using Codestetic.Web.Models.Map;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Configuration;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Core.Caching;

using Codestetic.Web.Services.Logging;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Core.Events;
using Codestetic.Web.Core;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Infrastructure.Cache;
using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Services.Media;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.Localization;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Services.Common;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Core.Localization;
using Codestetic.Web.Services.Devices;
using Codestetic.Web.Models.Common;
using Codestetic.Web.Framework.Localization;
using System.Globalization;

namespace Codestetic.Web.Controllers
{
    public class CommonController : BaseController
    {
        #region Fields
        private readonly ILanguageService _languageService;
        //private readonly ICurrencyService _currencyService;
        private readonly IWorkContext _workContext;
        //private readonly IQueuedEmailService _queuedEmailService;
        //private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        //private readonly IMobileDeviceHelper _mobileDeviceHelper;
        private readonly ICacheManager _cacheManager;
        private readonly IUserActivityService _userActivityService;

        private readonly UserSettings _userSettings;
        private readonly CommonSettings _commonSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILocalizationService _localizationService;

        private readonly ISettingService _settingService;

        private readonly IDeviceIndicatorService deviceIndicatorService;
        private readonly ISiteContext _siteContext;
        #endregion Fields

        #region Constructors
        public CommonController(
            ILanguageService languageService,
            ILocalizationService localizationService,
            //ICurrencyService currencyService,
            IWorkContext workContext, 
            //IQueuedEmailService queuedEmailService, 
            //IEmailAccountService emailAccountService,
            //ISitemapGenerator sitemapGenerator, 
            IGenericAttributeService genericAttributeService, IWebHelper webHelper, IPermissionService permissionService, 
            //IMobileDeviceHelper mobileDeviceHelper,
            ICacheManager cacheManager,
            IUserActivityService userActivityService, UserSettings userSettings,
            //EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings, LocalizationSettings localizationSettings,
            IDeviceIndicatorService deviceIndicatorService,
            ISettingService settingService, ISiteContext siteContext)
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            //this._currencyService = currencyService;
            this._workContext = workContext;
            //this._queuedEmailService = queuedEmailService;
            //this._emailAccountService = emailAccountService;
            //this._sitemapGenerator = sitemapGenerator;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            //this._mobileDeviceHelper = mobileDeviceHelper;
            this._cacheManager = cacheManager;
            this._userActivityService = userActivityService;

            this._userSettings = userSettings;
            this._commonSettings = commonSettings;
            this._localizationSettings = localizationSettings;

            this._settingService = settingService;
            this._siteContext = siteContext;

            this.deviceIndicatorService = deviceIndicatorService;
        }
        #endregion Constructors

        #region Properties
        public Localizer T { get; set; }
        #endregion Properties

        #region Page not found
        public ActionResult PageNotFound()
        {
            this.Response.StatusCode = 404;
            this.Response.TrySkipIisCustomErrors = true;

            return View();
        }
        #endregion Page not found

        #region Methods
        public ActionResult Index()
        {
            return View();
        }

        #region Child actions
        [ChildActionOnly]
        public ActionResult Header()
        {
            SiteHeaderModel model = null;
            var user = _workContext.CurrentUser;
            var isAdmin = user.IsSuperAdmin() || user.IsAdmin();

            if (!_webHelper.GetThisPageUrl(false).EndsWith("map"))
            {
                model = _cacheManager.Get(ModelCacheEventConsumer.SITEHEADER_MODEL_KEY.FormatWith(0), () =>
                {
                    var pictureService = EngineContext.Current.Resolve<IPictureService>();
                    var siteSettings = _settingService.LoadSetting<SiteSettings>();
                    var logoPictureId = siteSettings.LogoPictureId;

                    Picture picture = null;
                    if (logoPictureId > 0)
                    {
                        picture = pictureService.GetPictureById(logoPictureId);
                    }

                    string logoUrl = null;
                    var logoSize = new Size();
                    if (picture != null)
                    {
                        logoUrl = pictureService.GetPictureUrl(picture);
                        logoSize = pictureService.GetPictureSize(picture);
                    }

                    //var socialSettings = EngineContext.Current.Resolve<SocialSettings>();

                    //var showSocialLinks = socialSettings.ShowSocialLinksInFooter;
                    //var facebookLink = socialSettings.FacebookLink;
                    //var googlePlusLink = socialSettings.GooglePlusLink;
                    //var twitterLink = socialSettings.TwitterLink;
                    //var pinterestLink = socialSettings.PinterestLink;
                    var modelView = new SiteHeaderModel()
                    {
                        HomeUrl = !isAdmin ? "/" : "/Admin/Dashboard",
                        LogoUploaded = picture != null,
                        LogoUrl = logoUrl,
                        LogoWidth = logoSize.Width,
                        LogoHeight = logoSize.Height,
                        LogoTitle = siteSettings.Name,
                        EnableMenu = true,
                    };
                    modelView.CustomProperties.Add("Class", "main-logo");
                    return modelView;
                });
            }
            else
            {
                model = _cacheManager.Get(ModelCacheEventConsumer.SITEHEADER_MAP_MODEL_KEY.FormatWith(0), () =>
                {
                    var pictureService = EngineContext.Current.Resolve<IPictureService>();
                    var siteSettings = _settingService.LoadSetting<SiteSettings>();
                    long logoPictureId = siteSettings.LogoWidePictureId;

                    Picture picture = null;
                    if (logoPictureId > 0)
                    {
                        picture = pictureService.GetPictureById(logoPictureId);
                    }

                    string logoUrl = null;
                    var logoSize = new Size();
                    if (picture != null)
                    {
                        logoUrl = pictureService.GetPictureUrl(picture);
                        logoSize = pictureService.GetPictureSize(picture);
                    }

                    var modelView = new SiteHeaderModel()
                    {
                        LogoUploaded = picture != null,
                        LogoUrl = logoUrl,
                        LogoWidth = logoSize.Width,
                        LogoHeight = logoSize.Height,
                        LogoTitle = siteSettings.Name,
                        UserName = user.GetFullName(),
                        EnableMenu = false,
                    };

                    modelView.CustomProperties.Add("Class", "map-logo");
                    return modelView;
                });
            }

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Menu(bool flat)
        {
            var user = _workContext.CurrentUser;
            var model = new PermissionsMenu()
            {
                IsAdmin = user.IsAdmin(),
                IsSuperAdmin = user.IsSuperAdmin(),
                IsRegistered = user.IsRegistered(),
            };

            return flat ? PartialView("MenuFlat", model) : PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult MenuPanel()
        {
            var user = _workContext.CurrentUser;
            var model = new PermissionsMenu()
            {
                IsAdmin = user.IsAdmin(),
                IsSuperAdmin = user.IsSuperAdmin(),
                IsRegistered = user.IsRegistered(),
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            return View();
        }

        //footer
        [ChildActionOnly]
        public ActionResult JavaScriptDisabledWarning()
        {
            if (!_commonSettings.DisplayJavaScriptDisabledWarning)
                return Content("");

            return PartialView();
        }
        #endregion Child actions

        #region Language
        [ChildActionOnly]
        public ActionResult LanguageSelector()
        {
            var model = PrepareLanguageSelectorModel();
            return PartialView(model);
        }
        public ActionResult SetLanguage(int langid, string returnUrl = "")
        {
            var language = _languageService.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _workContext.WorkingLanguage = language;
            }

            // url referrer
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = _webHelper.GetUrlReferrer();
            }

            // home page
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.RouteUrl("HomePage");
            }

            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var helper = new LocalizedUrlHelper(HttpContext.Request.ApplicationPath, returnUrl, true);
                helper.PrependSeoCode(_workContext.WorkingLanguage.UniqueSeoCode, true);
                returnUrl = helper.GetAbsolutePath();
            }

            return Redirect(returnUrl);
        }

        [HttpPost]
        public JsonResult GetResourceLanguage()
        {
            if (!_workContext.CurrentUser.IsRegistered())
            {
                return Json( new {
                    success = false 
                });
            }

            var languageId = _workContext.WorkingLanguage.Id;
            var resources = _localizationService.GetAllResources(languageId).Select(x => new { key = x.ResourceName, value = x.ResourceValue }).ToDictionary(x => x.key, x => x.value);

            return Json(new
            {
                success = true,
                data = resources
            });
        }
        #endregion Language
        #endregion Methods

        #region Utilities
        [NonAction]
        protected LanguageSelectorModel PrepareLanguageSelectorModel()
        {
            var availableLanguages = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, 0), () =>
            {
                var result = _languageService
                    .GetAllLanguages()
                    .Select(x => new LanguageModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NativeName = GetLanguageNativeName(x.LanguageCulture) ?? x.Name,
                        ISOCode = x.LanguageCulture,
                        SeoCode = x.UniqueSeoCode,
                        FlagImageFileName = x.FlagImageFileName
                    })
                    .ToList();
                return result;
            });

            var workingLanguage = _workContext.WorkingLanguage;

            var model = new LanguageSelectorModel()
            {
                CurrentLanguageId = workingLanguage.Id,
                AvailableLanguages = availableLanguages,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            string defaultSeoCode = _workContext.GetDefaultLanguageSeoCode();

            foreach (var lang in model.AvailableLanguages)
            {
                var helper = new LocalizedUrlHelper(HttpContext.Request, true);

                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    if (lang.SeoCode == defaultSeoCode && (int)(_localizationSettings.DefaultLanguageRedirectBehaviour) > 0)
                    {
                        helper.StripSeoCode();
                    }
                    else
                    {
                        helper.PrependSeoCode(lang.SeoCode, true);
                    }
                }

                model.ReturnUrls[lang.SeoCode] = HttpUtility.UrlEncode(helper.GetAbsolutePath());
            }

            return model;
        }
        private string GetLanguageNativeName(string locale)
        {
            try
            {
                if (!string.IsNullOrEmpty(locale))
                {
                    var info = CultureInfo.GetCultureInfoByIetfLanguageTag(locale);
                    if (info == null)
                    {
                        return null;
                    }
                    return info.NativeName;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion Utilities

        public ActionResult TestPage()
        {
            return View();
        }
    }
}