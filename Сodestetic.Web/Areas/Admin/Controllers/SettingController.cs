using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Codestetic.Web.Areas.Admin.Models.Common;
using Codestetic.Web.Areas.Admin.Models.Settings;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Domain;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.Directory;
//using Specter.Web.Core.Domain.Forums;
using Codestetic.Web.Core.Domain.Localization;
using Codestetic.Web.Core.Domain.Media;
//using Specter.Web.Core.Domain.News;
//using Specter.Web.Core.Domain.Orders;
using Codestetic.Web.Core.Domain.Security;
using Codestetic.Web.Services.Common;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Services.Directory;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Core.Logging;
using Codestetic.Web.Services.Media;
//using Specter.Web.Services.Orders;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Framework;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Framework.Localization;
using Codestetic.Web.Framework.UI.Captcha;
using Codestetic.Web.Framework.Settings;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Areas.Admin.Infrastructure;
using Codestetic.Web.Core.Domain.Seo;
//using Specter.Web.Core.Domain.Seo;

namespace Codestetic.Web.Areas.Admin.Controllers
{
	[AdminAuthorize]
    public partial class SettingController : AdminControllerBase
	{
		#region Fields
        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly IAddressService _addressService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        //private readonly IOrderService _orderService;
        private readonly IEncryptionService _encryptionService;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IWebHelper _webHelper;
        //private readonly IFulltextService _fulltextService;
        private readonly IMaintenanceService _maintenanceService;
		private readonly IWorkContext _workContext;
		private readonly IGenericAttributeService _genericAttributeService;

        private DependingSettingHelper _dependingSettings;
        #endregion Fields

        #region Constructors
        public SettingController(ISettingService settingService,
            ICountryService countryService, IAddressService addressService,
            ICurrencyService currencyService, IPictureService pictureService, 
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            //IOrderService orderService, IFulltextService fulltextService,
            IEncryptionService encryptionService, IUserService userService, 
            IUserActivityService userActivityService, IPermissionService permissionService,
            IWebHelper webHelper, IMaintenanceService maintenanceService,
			IWorkContext workContext, IGenericAttributeService genericAttributeService)
        {
            this._settingService = settingService;
            this._countryService = countryService;
            this._addressService = addressService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            //this._orderService = orderService;
            this._encryptionService = encryptionService;
            this._userService = userService;
            this._userActivityService = userActivityService;
            this._permissionService = permissionService;
            this._webHelper = webHelper;
            //this._fulltextService = fulltextService;
            this._maintenanceService = maintenanceService;
			this._workContext = workContext;
			this._genericAttributeService = genericAttributeService;
        }

		#endregion 

        private DependingSettingHelper DependingSettings
        {
            get
            {
                if (_dependingSettings == null)
                    _dependingSettings = new DependingSettingHelper(this.ViewData);
                return _dependingSettings;
            }
        }

        #region Methods
        #region Forum
        //public ActionResult Forum()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
        //        return AccessDeniedView();

        //    //load settings for a chosen store scope
        //    var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
        //    var model = forumSettings.ToModel();

        //    DependingSettings.GetOverrideKeys(forumSettings, model, storeScope, _settingService);

        //    model.ForumEditorValues = forumSettings.ForumEditor.ToSelectList();
			
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult Forum(ForumSettingsModel model, FormCollection form)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
        //        return AccessDeniedView();

        //    //load settings for a chosen store scope
        //    var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    var forumSettings = _settingService.LoadSetting<ForumSettings>(storeScope);
        //    forumSettings = model.ToEntity(forumSettings);

        //    DependingSettings.UpdateSettings(forumSettings, form, storeScope, _settingService);

        //    //now clear settings cache
        //    _settingService.ClearCache();

        //    //activity log
        //    _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

        //    NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
        //    return RedirectToAction("Forum");
        //}
        #endregion Forum

        #region News
        //public ActionResult News()
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
        //        return AccessDeniedView();

        //    //load settings for a chosen store scope
        //    var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    var newsSettings = _settingService.LoadSetting<NewsSettings>(storeScope);
        //    var model = newsSettings.ToModel();

        //    DependingSettings.GetOverrideKeys(newsSettings, model, storeScope, _settingService);

        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult News(NewsSettingsModel model, FormCollection form)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
        //        return AccessDeniedView();

        //    //load settings for a chosen store scope
        //    var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    var newsSettings = _settingService.LoadSetting<NewsSettings>(storeScope);
        //    newsSettings = model.ToEntity(newsSettings);

        //    DependingSettings.UpdateSettings(newsSettings, form, storeScope, _settingService);

        //    //now clear settings cache
        //    _settingService.ClearCache();

        //    //activity log
        //    _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

        //    NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
        //    return RedirectToAction("News");
        //}
        #endregion News

        #region Order
        //public ActionResult Order(string selectedTab)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
        //        return AccessDeniedView();

        //    //load settings for a chosen store scope
        //    var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    var orderSettings = _settingService.LoadSetting<OrderSettings>(storeScope);
        //    var model = orderSettings.ToModel();

        //    DependingSettings.GetOverrideKeys(orderSettings, model, storeScope, _settingService);

        //    var currencySettings = _settingService.LoadSetting<CurrencySettings>(storeScope);
        //    model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

        //    //gift card activation/deactivation
        //    model.GiftCards_Activated_OrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
        //    //model.GiftCards_Activated_OrderStatuses.Insert(0, new SelectListItem() { Text = "---", Value = "0" }); // codehint: sm-delete
        //    model.GiftCards_Deactivated_OrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
        //    //model.GiftCards_Deactivated_OrderStatuses.Insert(0, new SelectListItem() { Text = "---", Value = "0" }); // codehint: sm-delete

        //    //parse return request actions
        //    for (int i = 0; i < orderSettings.ReturnRequestActions.Count; i++)
        //    {
        //        model.ReturnRequestActionsParsed += orderSettings.ReturnRequestActions[i];
        //        if (i != orderSettings.ReturnRequestActions.Count - 1)
        //            model.ReturnRequestActionsParsed += ",";
        //    }
        //    //parse return request reasons
        //    for (int i = 0; i < orderSettings.ReturnRequestReasons.Count; i++)
        //    {
        //        model.ReturnRequestReasonsParsed += orderSettings.ReturnRequestReasons[i];
        //        if (i != orderSettings.ReturnRequestReasons.Count - 1)
        //            model.ReturnRequestReasonsParsed += ",";
        //    }

        //    //order ident
        //    model.OrderIdent = _maintenanceService.GetTableIdent<Order>();

        //    ViewData["SelectedTab"] = selectedTab;
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult Order(OrderSettingsModel model, string selectedTab, FormCollection form)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
        //        return AccessDeniedView();

        //    if (ModelState.IsValid)
        //    {
        //        //load settings for a chosen store scope
        //        var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //        var orderSettings = _settingService.LoadSetting<OrderSettings>(storeScope);
        //        orderSettings = model.ToEntity(orderSettings);

        //        DependingSettings.UpdateSettings(orderSettings, form, storeScope, _settingService);

        //        //parse return request actions
        //        orderSettings.ReturnRequestActions.Clear();
        //        foreach (var returnAction in model.ReturnRequestActionsParsed.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //            orderSettings.ReturnRequestActions.Add(returnAction);
        //        _settingService.SaveSetting(orderSettings, x => x.ReturnRequestActions, 0, false);		// codehint: sm-edit
				
        //        //parse return request reasons
        //        orderSettings.ReturnRequestReasons.Clear();
        //        foreach (var returnReason in model.ReturnRequestReasonsParsed.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //            orderSettings.ReturnRequestReasons.Add(returnReason);
        //        _settingService.SaveSetting(orderSettings, x => x.ReturnRequestReasons, 0, false);		// codehint: sm-edit

        //        // codehint: sm-edit
        //        if (model.GiftCards_Activated_OrderStatusId.HasValue)
        //            _settingService.SaveSetting(orderSettings, x => x.GiftCards_Activated_OrderStatusId, 0, false);
        //        else
        //            _settingService.DeleteSetting(orderSettings, x => x.GiftCards_Activated_OrderStatusId);

        //        if (model.GiftCards_Deactivated_OrderStatusId.HasValue)
        //            _settingService.SaveSetting(orderSettings, x => x.GiftCards_Deactivated_OrderStatusId, 0, false);
        //        else
        //            _settingService.DeleteSetting(orderSettings, x => x.GiftCards_Deactivated_OrderStatusId);

        //        //now clear settings cache
        //        _settingService.ClearCache();

        //        //order ident
        //        if (model.OrderIdent.HasValue)
        //        {
        //            try
        //            {
        //                _maintenanceService.SetTableIdent<Order>(model.OrderIdent.Value);
        //            }
        //            catch (Exception exc)
        //            {
        //                NotifyError(exc.Message);
        //            }
        //        }

        //        //activity log
        //        _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

        //        NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
        //    }
        //    else
        //    {
        //        //If we got this far, something failed, redisplay form
        //        foreach (var modelState in ModelState.Values)
        //            foreach (var error in modelState.Errors)
        //                NotifyError(error.ErrorMessage);
        //    }
        //    return RedirectToAction("Order", new { selectedTab = selectedTab });
        //}
        #endregion Order

        #region Media
        public ActionResult Media()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var mediaSettings = _settingService.LoadSetting<MediaSettings>();
            var model = mediaSettings.ToModel();

            DependingSettings.GetOverrideKeys(mediaSettings, model, _settingService);

            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;

            var resKey = "Admin.Configuration.Settings.Media.PictureZoomType.";

            model.AvailablePictureZoomTypes.Add(new SelectListItem
            {
                Text = _localizationService.GetResource(resKey + "Window"),
                Value = "window",
                Selected = model.PictureZoomType.Equals("window")
            });
            model.AvailablePictureZoomTypes.Add(new SelectListItem
            {
                Text = _localizationService.GetResource(resKey + "Inner"),
                Value = "inner",
                Selected = model.PictureZoomType.Equals("inner")
            });
            model.AvailablePictureZoomTypes.Add(new SelectListItem
            {
                Text = _localizationService.GetResource(resKey + "Lens"),
                Value = "lens",
                Selected = model.PictureZoomType.Equals("lens")
            });

            return View(model);
        }
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Media(MediaSettingsModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var mediaSettings = _settingService.LoadSetting<MediaSettings>();
            mediaSettings = model.ToEntity(mediaSettings);

            DependingSettings.UpdateSettings(mediaSettings, form, _settingService);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }
        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public ActionResult ChangePictureStorage()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
            //    return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }
        #endregion Media

        #region User
        public ActionResult Users(string selectedTab)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

			var userSettings = _settingService.LoadSetting<UserSettings>();
			var addressSettings = _settingService.LoadSetting<AddressSettings>();
			var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>();
			//var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>();

            //merge settings
            var model = new AdvanceUserSettingsModel();
            model.UserSettings = userSettings.ToModel();
            model.AddressSettings = addressSettings.ToModel();

            model.DateTimeSettings.AllowUsersToSetTimeZone = dateTimeSettings.AllowUsersToSetTimeZone;
            model.DateTimeSettings.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultSiteTimeZone.Id;
            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem()
                    {
                        Text = timeZone.DisplayName,
                        Value = timeZone.Id,
                        Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultSiteTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                    });
            }

            //model.ExternalAuthenticationSettings.AutoRegisterEnabled = externalAuthenticationSettings.AutoRegisterEnabled;

            ViewData["SelectedTab"] = selectedTab;
            return View(model);
        }
        [HttpPost]
        public ActionResult AdvanceUser(AdvanceUserSettingsModel model, string selectedTab)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

			var userSettings = _settingService.LoadSetting<UserSettings>();
			var addressSettings = _settingService.LoadSetting<AddressSettings>();
			var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>();
			//var externalAuthenticationSettings = _settingService.LoadSetting<ExternalAuthenticationSettings>();

            userSettings = model.UserSettings.ToEntity(userSettings);
            _settingService.SaveSetting(userSettings);

            addressSettings = model.AddressSettings.ToEntity(addressSettings);
            _settingService.SaveSetting(addressSettings);

            dateTimeSettings.DefaultSiteTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            dateTimeSettings.AllowUsersToSetTimeZone = model.DateTimeSettings.AllowUsersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            //externalAuthenticationSettings.AutoRegisterEnabled = model.ExternalAuthenticationSettings.AutoRegisterEnabled;
            //_settingService.SaveSetting(externalAuthenticationSettings);

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("AdvanceUser", new { selectedTab = selectedTab });
        }
        #endregion User

        #region GeneralCommon
        public ActionResult GeneralCommon(string selectedTab)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

			var model = new GeneralCommonSettingsModel();

			DependingSettings.CreateViewDataObject();

            #region Store information
            var siteInformationSettings = _settingService.LoadSetting<SiteInformationSettings>();
            model.SiteInformationSettings.SiteClosed = siteInformationSettings.SiteClosed;
            model.SiteInformationSettings.SiteClosedAllowForAdmins = siteInformationSettings.SiteClosedAllowForAdmins;

			var storeInformationSettings = _settingService.LoadSetting<SiteInformationSettings>();
			model.SiteInformationSettings.SiteClosed = storeInformationSettings.SiteClosed;
            model.SiteInformationSettings.SiteClosedAllowForAdmins = storeInformationSettings.SiteClosedAllowForAdmins;

			DependingSettings.GetOverrideKeys(storeInformationSettings, model.SiteInformationSettings, _settingService, false);
            #endregion Store information

            #region Seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>();
            model.SeoSettings.PageTitleSeoAdjustment = seoSettings.PageTitleSeoAdjustment.ToString();
            model.SeoSettings.AvailablePageTitleSeoAdjustment.AddRange(new List<SelectListItem>() 
            { 
                new SelectListItem() { Text = _localizationService.GetResource(PageTitleSeoAdjustment.PagenameAfterSitename.ToString()), Value = PageTitleSeoAdjustment.PagenameAfterSitename.ToString()}, 
                new SelectListItem() { Text = _localizationService.GetResource(PageTitleSeoAdjustment.SitenameAfterPagename.ToString()), Value = PageTitleSeoAdjustment .SitenameAfterPagename.ToString()}
            });
            model.SeoSettings.PageTitleSeparator = seoSettings.PageTitleSeparator;
            model.SeoSettings.DefaultTitle = seoSettings.DefaultTitle;
            model.SeoSettings.DefaultMetaKeywords = seoSettings.DefaultMetaKeywords;
            model.SeoSettings.DefaultMetaDescription = seoSettings.DefaultMetaDescription;
            model.SeoSettings.ConvertNonWesternChars = seoSettings.ConvertNonWesternChars;
            model.SeoSettings.CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled;

			DependingSettings.GetOverrideKeys(seoSettings, model.SeoSettings, _settingService, false);
            #endregion Seo settings

            #region Security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>();
			var captchaSettings = _settingService.LoadSetting<CaptchaSettings>();
			model.SecuritySettings.EncryptionKey = securitySettings.EncryptionKey;
			if (securitySettings.AdminAreaAllowedIpAddresses != null)
			{
				for (int i = 0; i < securitySettings.AdminAreaAllowedIpAddresses.Count; i++)
				{
					model.SecuritySettings.AdminAreaAllowedIpAddresses += securitySettings.AdminAreaAllowedIpAddresses[i];
					if (i != securitySettings.AdminAreaAllowedIpAddresses.Count - 1)
						model.SecuritySettings.AdminAreaAllowedIpAddresses += ",";
				}
			}
			model.SecuritySettings.HideAdminMenuItemsBasedOnPermissions = securitySettings.HideAdminMenuItemsBasedOnPermissions;
			model.SecuritySettings.CaptchaEnabled = captchaSettings.Enabled;
			model.SecuritySettings.CaptchaShowOnLoginPage = captchaSettings.ShowOnLoginPage;
			model.SecuritySettings.CaptchaShowOnRegistrationPage = captchaSettings.ShowOnRegistrationPage;
			model.SecuritySettings.CaptchaShowOnContactUsPage = captchaSettings.ShowOnContactUsPage;
			model.SecuritySettings.CaptchaShowOnEmailWishlistToFriendPage = captchaSettings.ShowOnEmailWishlistToFriendPage;
			model.SecuritySettings.CaptchaShowOnEmailProductToFriendPage = captchaSettings.ShowOnEmailProductToFriendPage;
			model.SecuritySettings.CaptchaShowOnAskQuestionPage = captchaSettings.ShowOnAskQuestionPage;
			model.SecuritySettings.CaptchaShowOnBlogCommentPage = captchaSettings.ShowOnBlogCommentPage;
			model.SecuritySettings.CaptchaShowOnNewsCommentPage = captchaSettings.ShowOnNewsCommentPage;
			model.SecuritySettings.CaptchaShowOnProductReviewPage = captchaSettings.ShowOnProductReviewPage;
			model.SecuritySettings.ReCaptchaPublicKey = captchaSettings.ReCaptchaPublicKey;
			model.SecuritySettings.ReCaptchaPrivateKey = captchaSettings.ReCaptchaPrivateKey;
            #endregion Security settings

            #region PDF settings
			var pdfSettings = _settingService.LoadSetting<PdfSettings>();
			model.PdfSettings.Enabled = pdfSettings.Enabled;
			model.PdfSettings.LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled;
			model.PdfSettings.LogoPictureId = pdfSettings.LogoPictureId;

			DependingSettings.GetOverrideKeys(pdfSettings, model.PdfSettings, _settingService, false);
            #endregion PDF settings

            #region Localization
            //localization
			var localizationSettings = _settingService.LoadSetting<LocalizationSettings>();
			model.LocalizationSettings.UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection;
			model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
			model.LocalizationSettings.LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup;
            model.LocalizationSettings.DefaultLanguageRedirectBehaviour = localizationSettings.DefaultLanguageRedirectBehaviour;
            model.LocalizationSettings.InvalidLanguageRedirectBehaviour = localizationSettings.InvalidLanguageRedirectBehaviour;
            model.LocalizationSettings.DetectBrowserUserLanguage = localizationSettings.DetectBrowserUserLanguage;
            #endregion Localization

            #region Full-text support
            //var commonSettings = _settingService.LoadSetting<CommonSettings>();
            //model.FullTextSettings.Supported = _fulltextService.IsFullTextSupported();
            //model.FullTextSettings.Enabled = commonSettings.UseFullTextSearch;
            //model.FullTextSettings.SearchMode = commonSettings.FullTextMode;
            //model.FullTextSettings.SearchModeValues = commonSettings.FullTextMode.ToSelectList();
            #endregion Full-text support

            #region Company information
            var companySettings = _settingService.LoadSetting<CompanyInformationSettings>();
			model.CompanyInformationSettings.CompanyName = companySettings.CompanyName;
			model.CompanyInformationSettings.Salutation = companySettings.Salutation;
			model.CompanyInformationSettings.Title = companySettings.Title;
			model.CompanyInformationSettings.Firstname = companySettings.Firstname;
			model.CompanyInformationSettings.Lastname = companySettings.Lastname;
			model.CompanyInformationSettings.CompanyManagementDescription = companySettings.CompanyManagementDescription;
			model.CompanyInformationSettings.CompanyManagement = companySettings.CompanyManagement;
			model.CompanyInformationSettings.Street = companySettings.Street;
			model.CompanyInformationSettings.Street2 = companySettings.Street2;
			model.CompanyInformationSettings.ZipCode = companySettings.ZipCode;
			model.CompanyInformationSettings.City = companySettings.City;
			model.CompanyInformationSettings.CountryId = companySettings.CountryId;
			model.CompanyInformationSettings.Region = companySettings.Region;
			model.CompanyInformationSettings.VatId = companySettings.VatId;
			model.CompanyInformationSettings.CommercialRegister = companySettings.CommercialRegister;
			model.CompanyInformationSettings.TaxNumber = companySettings.TaxNumber;

			DependingSettings.GetOverrideKeys(companySettings, model.CompanyInformationSettings, _settingService, false);

			foreach (var c in _countryService.GetAllCountries(true))
			{
				model.CompanyInformationSettings.AvailableCountries.Add(
					new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.CompanyInformationSettings.CountryId)
				});
			}

            model.CompanyInformationSettings.Salutations.Add(ResToSelectListItem("Admin.Address.Salutation.Mr"));
            model.CompanyInformationSettings.Salutations.Add(ResToSelectListItem("Admin.Address.Salutation.Mrs"));

			model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Manager"));
			model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Shopkeeper"));
			model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Procurator"));
			model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Shareholder"));
			model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.AuthorizedPartner"));
			model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.Director"));
			model.CompanyInformationSettings.ManagementDescriptions.Add(
                ResToSelectListItem("Admin.Configuration.Settings.GeneralCommon.CompanyInformationSettings.ManagementDescriptions.ManagingPartner"));
            #endregion Company information

            #region Contact data
			var contactDataSettings = _settingService.LoadSetting<ContactDataSettings>();
			model.ContactDataSettings.CompanyTelephoneNumber = contactDataSettings.CompanyTelephoneNumber;
			model.ContactDataSettings.HotlineTelephoneNumber = contactDataSettings.HotlineTelephoneNumber;
			model.ContactDataSettings.MobileTelephoneNumber = contactDataSettings.MobileTelephoneNumber;
			model.ContactDataSettings.CompanyFaxNumber = contactDataSettings.CompanyFaxNumber;
			model.ContactDataSettings.CompanyEmailAddress = contactDataSettings.CompanyEmailAddress;
			model.ContactDataSettings.WebmasterEmailAddress = contactDataSettings.WebmasterEmailAddress;
			model.ContactDataSettings.SupportEmailAddress = contactDataSettings.SupportEmailAddress;
			model.ContactDataSettings.ContactEmailAddress = contactDataSettings.ContactEmailAddress;

			DependingSettings.GetOverrideKeys(contactDataSettings, model.ContactDataSettings, _settingService, false);
            #endregion Contact data

            #region Bank connection
			var bankConnectionSettings = _settingService.LoadSetting<BankConnectionSettings>();
			model.BankConnectionSettings.Bankname = bankConnectionSettings.Bankname;
			model.BankConnectionSettings.Bankcode = bankConnectionSettings.Bankcode;
			model.BankConnectionSettings.AccountNumber = bankConnectionSettings.AccountNumber;
			model.BankConnectionSettings.AccountHolder = bankConnectionSettings.AccountHolder;
			model.BankConnectionSettings.Iban = bankConnectionSettings.Iban;
			model.BankConnectionSettings.Bic = bankConnectionSettings.Bic;

			DependingSettings.GetOverrideKeys(bankConnectionSettings, model.BankConnectionSettings, _settingService, false);
            #endregion Bank connection

            #region Social
            //var socialSettings = _settingService.LoadSetting<SocialSettings>();
            //model.SocialSettings.ShowSocialLinksInFooter = socialSettings.ShowSocialLinksInFooter;
            //model.SocialSettings.FacebookLink = socialSettings.FacebookLink;
            //model.SocialSettings.GooglePlusLink = socialSettings.GooglePlusLink;
            //model.SocialSettings.TwitterLink = socialSettings.TwitterLink;
            //model.SocialSettings.PinterestLink = socialSettings.PinterestLink;
            //model.SocialSettings.YoutubeLink = socialSettings.YoutubeLink;

            //DependingSettings.GetOverrideKeys(socialSettings, model.SocialSettings, _settingService, false);
            #endregion Social

            ViewData["SelectedTab"] = selectedTab;
            return View(model);
        }

        private SelectListItem ResToSelectListItem(string resourceKey)
        {
            string value = _localizationService.GetResource(resourceKey).EmptyNull();
            return new SelectListItem() { Text = value, Value = value };
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult GeneralCommon(GeneralCommonSettingsModel model, string selectedTab, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

			#region Site information
			var siteInformationSettings = _settingService.LoadSetting<SiteInformationSettings>();
			siteInformationSettings.SiteClosed = model.SiteInformationSettings.SiteClosed;
			siteInformationSettings.SiteClosedAllowForAdmins = model.SiteInformationSettings.SiteClosedAllowForAdmins;

			DependingSettings.UpdateSettings(siteInformationSettings, form, _settingService);
            #endregion Site information

            #region SEO settings
            //var seoSettings = _settingService.LoadSetting<SeoSettings>();
            //seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            //seoSettings.PageTitleSeoAdjustment = model.SeoSettings.PageTitleSeoAdjustment;
            //seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            //seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
            //seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
            //seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            //seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;

            //DependingSettings.UpdateSettings(seoSettings, form, _settingService);
            #endregion SEO settings

            #region Security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>();
			var captchaSettings = _settingService.LoadSetting<CaptchaSettings>();
			if (securitySettings.AdminAreaAllowedIpAddresses == null)
				securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
			securitySettings.AdminAreaAllowedIpAddresses.Clear();
			if (model.SecuritySettings.AdminAreaAllowedIpAddresses.HasValue())
			{
				foreach (string s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					if (!String.IsNullOrWhiteSpace(s))
						securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
				}
			}
			securitySettings.HideAdminMenuItemsBasedOnPermissions = model.SecuritySettings.HideAdminMenuItemsBasedOnPermissions;
			_settingService.SaveSetting(securitySettings);

			captchaSettings.Enabled = model.SecuritySettings.CaptchaEnabled;
			captchaSettings.ShowOnLoginPage = model.SecuritySettings.CaptchaShowOnLoginPage;
			captchaSettings.ShowOnRegistrationPage = model.SecuritySettings.CaptchaShowOnRegistrationPage;
			captchaSettings.ShowOnContactUsPage = model.SecuritySettings.CaptchaShowOnContactUsPage;
			captchaSettings.ShowOnEmailWishlistToFriendPage = model.SecuritySettings.CaptchaShowOnEmailWishlistToFriendPage;
			captchaSettings.ShowOnEmailProductToFriendPage = model.SecuritySettings.CaptchaShowOnEmailProductToFriendPage;
			captchaSettings.ShowOnAskQuestionPage = model.SecuritySettings.CaptchaShowOnAskQuestionPage;
			captchaSettings.ShowOnBlogCommentPage = model.SecuritySettings.CaptchaShowOnBlogCommentPage;
			captchaSettings.ShowOnNewsCommentPage = model.SecuritySettings.CaptchaShowOnNewsCommentPage;
			captchaSettings.ShowOnProductReviewPage = model.SecuritySettings.CaptchaShowOnProductReviewPage;
			captchaSettings.ReCaptchaPublicKey = model.SecuritySettings.ReCaptchaPublicKey;
			captchaSettings.ReCaptchaPrivateKey = model.SecuritySettings.ReCaptchaPrivateKey;

			_settingService.SaveSetting(captchaSettings);

			if (captchaSettings.Enabled && (String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
			{
				NotifyError(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.CaptchaEnabledNoKeys"));
            }
            #endregion Security settings

            #region PDF settings
            var pdfSettings = _settingService.LoadSetting<PdfSettings>();
			pdfSettings.Enabled = model.PdfSettings.Enabled;
			pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
			pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;

			DependingSettings.UpdateSettings(pdfSettings, form, _settingService);
            #endregion PDF settings

            #region Localization settings
            var localizationSettings = _settingService.LoadSetting<LocalizationSettings>();
			localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
			if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
			{
				localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
				//clear cached values of routes
				System.Web.Routing.RouteTable.Routes.ClearSeoFriendlyUrlsCachedValueForRoutes();
			}
			localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
            localizationSettings.DefaultLanguageRedirectBehaviour = model.LocalizationSettings.DefaultLanguageRedirectBehaviour;
            localizationSettings.InvalidLanguageRedirectBehaviour = model.LocalizationSettings.InvalidLanguageRedirectBehaviour;
            localizationSettings.DetectBrowserUserLanguage = model.LocalizationSettings.DetectBrowserUserLanguage;

			_settingService.SaveSetting(localizationSettings);
            #endregion Localization settings

            #region Full-text
            var commonSettings = _settingService.LoadSetting<CommonSettings>();
			commonSettings.FullTextMode = model.FullTextSettings.SearchMode;

			_settingService.SaveSetting(commonSettings);
            #endregion Full-text

            #region Company information
            var companySettings = _settingService.LoadSetting<CompanyInformationSettings>();
			companySettings.CompanyName = model.CompanyInformationSettings.CompanyName;
			companySettings.Salutation = model.CompanyInformationSettings.Salutation;
			companySettings.Title = model.CompanyInformationSettings.Title;
			companySettings.Firstname = model.CompanyInformationSettings.Firstname;
			companySettings.Lastname = model.CompanyInformationSettings.Lastname;
			companySettings.CompanyManagementDescription = model.CompanyInformationSettings.CompanyManagementDescription;
			companySettings.CompanyManagement = model.CompanyInformationSettings.CompanyManagement;
			companySettings.Street = model.CompanyInformationSettings.Street;
			companySettings.Street2 = model.CompanyInformationSettings.Street2;
			companySettings.ZipCode = model.CompanyInformationSettings.ZipCode;
			companySettings.City = model.CompanyInformationSettings.City;
			companySettings.CountryId = model.CompanyInformationSettings.CountryId;
			companySettings.Region = model.CompanyInformationSettings.Region;
			if (model.CompanyInformationSettings.CountryId != 0)
			{
				companySettings.CountryName = _countryService.GetCountryById(model.CompanyInformationSettings.CountryId).Name;
			}
			companySettings.VatId = model.CompanyInformationSettings.VatId;
			companySettings.CommercialRegister = model.CompanyInformationSettings.CommercialRegister;
			companySettings.TaxNumber = model.CompanyInformationSettings.TaxNumber;

			DependingSettings.UpdateSettings(companySettings, form, _settingService);
            #endregion Company information

            #region Contact data
            var contactDataSettings = _settingService.LoadSetting<ContactDataSettings>();
			contactDataSettings.CompanyTelephoneNumber = model.ContactDataSettings.CompanyTelephoneNumber;
			contactDataSettings.HotlineTelephoneNumber = model.ContactDataSettings.HotlineTelephoneNumber;
			contactDataSettings.MobileTelephoneNumber = model.ContactDataSettings.MobileTelephoneNumber;
			contactDataSettings.CompanyFaxNumber = model.ContactDataSettings.CompanyFaxNumber;
			contactDataSettings.CompanyEmailAddress = model.ContactDataSettings.CompanyEmailAddress;
			contactDataSettings.WebmasterEmailAddress = model.ContactDataSettings.WebmasterEmailAddress;
			contactDataSettings.SupportEmailAddress = model.ContactDataSettings.SupportEmailAddress;
			contactDataSettings.ContactEmailAddress = model.ContactDataSettings.ContactEmailAddress;

			DependingSettings.UpdateSettings(contactDataSettings, form, _settingService);
            #endregion Contact data

            #region Bank connection
            var bankConnectionSettings = _settingService.LoadSetting<BankConnectionSettings>();
			bankConnectionSettings.Bankname = model.BankConnectionSettings.Bankname;
			bankConnectionSettings.Bankcode = model.BankConnectionSettings.Bankcode;
			bankConnectionSettings.AccountNumber = model.BankConnectionSettings.AccountNumber;
			bankConnectionSettings.AccountHolder = model.BankConnectionSettings.AccountHolder;
			bankConnectionSettings.Iban = model.BankConnectionSettings.Iban;
			bankConnectionSettings.Bic = model.BankConnectionSettings.Bic;

			DependingSettings.UpdateSettings(bankConnectionSettings, form, _settingService);
            #endregion Bank connection

            #region Social
            //var socialSettings = _settingService.LoadSetting<SocialSettings>();
            //socialSettings.ShowSocialLinksInFooter = model.SocialSettings.ShowSocialLinksInFooter;
            //socialSettings.FacebookLink = model.SocialSettings.FacebookLink;
            //socialSettings.GooglePlusLink = model.SocialSettings.GooglePlusLink;
            //socialSettings.TwitterLink = model.SocialSettings.TwitterLink;
            //socialSettings.PinterestLink = model.SocialSettings.PinterestLink;
            //socialSettings.YoutubeLink = model.SocialSettings.YoutubeLink;

            //DependingSettings.UpdateSettings(socialSettings, form, _settingService);
            #endregion Social

            //now clear settings cache
			_settingService.ClearCache();

			//activity log
			_userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            NotifySuccess(_localizationService.GetResource("Admin.Configuration.Updated"));
            return RedirectToAction("GeneralCommon", new { selectedTab = selectedTab });
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public ActionResult ChangeEnryptionKey(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

			var securitySettings = _settingService.LoadSetting<SecuritySettings>();

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = "";

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (String.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new SpecterException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

                string oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new SpecterException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));

                //update encrypted order info
                //var orders = _orderService.LoadAllOrders();
                //foreach (var order in orders)
                //{
                //    // new credit card encryption
                //    string decryptedCardType = _encryptionService.DecryptText(order.CardType, oldEncryptionPrivateKey);
                //    string decryptedCardName = _encryptionService.DecryptText(order.CardName, oldEncryptionPrivateKey);
                //    string decryptedCardNumber = _encryptionService.DecryptText(order.CardNumber, oldEncryptionPrivateKey);
                //    string decryptedMaskedCreditCardNumber = _encryptionService.DecryptText(order.MaskedCreditCardNumber, oldEncryptionPrivateKey);
                //    string decryptedCardCvv2 = _encryptionService.DecryptText(order.CardCvv2, oldEncryptionPrivateKey);
                //    string decryptedCardExpirationMonth = _encryptionService.DecryptText(order.CardExpirationMonth, oldEncryptionPrivateKey);
                //    string decryptedCardExpirationYear = _encryptionService.DecryptText(order.CardExpirationYear, oldEncryptionPrivateKey);

                //    string encryptedCardType = _encryptionService.EncryptText(decryptedCardType, newEncryptionPrivateKey);
                //    string encryptedCardName = _encryptionService.EncryptText(decryptedCardName, newEncryptionPrivateKey);
                //    string encryptedCardNumber = _encryptionService.EncryptText(decryptedCardNumber, newEncryptionPrivateKey);
                //    string encryptedMaskedCreditCardNumber = _encryptionService.EncryptText(decryptedMaskedCreditCardNumber, newEncryptionPrivateKey);
                //    string encryptedCardCvv2 = _encryptionService.EncryptText(decryptedCardCvv2, newEncryptionPrivateKey);
                //    string encryptedCardExpirationMonth = _encryptionService.EncryptText(decryptedCardExpirationMonth, newEncryptionPrivateKey);
                //    string encryptedCardExpirationYear = _encryptionService.EncryptText(decryptedCardExpirationYear, newEncryptionPrivateKey);

                //    order.CardType = encryptedCardType;
                //    order.CardName = encryptedCardName;
                //    order.CardNumber = encryptedCardNumber;
                //    order.MaskedCreditCardNumber = encryptedMaskedCreditCardNumber;
                //    order.CardCvv2 = encryptedCardCvv2;
                //    order.CardExpirationMonth = encryptedCardExpirationMonth;
                //    order.CardExpirationYear = encryptedCardExpirationYear;

                //    // new direct debit encryption
                //    string decryptedAccountHolder = _encryptionService.DecryptText(order.DirectDebitAccountHolder, oldEncryptionPrivateKey);
                //    string decryptedAccountNumber = _encryptionService.DecryptText(order.DirectDebitAccountNumber, oldEncryptionPrivateKey);
                //    string decryptedBankCode = _encryptionService.DecryptText(order.DirectDebitBankCode, oldEncryptionPrivateKey);
                //    string decryptedBankName = _encryptionService.DecryptText(order.DirectDebitBankName, oldEncryptionPrivateKey);
                //    string decryptedBic = _encryptionService.DecryptText(order.DirectDebitBIC, oldEncryptionPrivateKey);
                //    string decryptedCountry = _encryptionService.DecryptText(order.DirectDebitCountry, oldEncryptionPrivateKey);
                //    string decryptedIban = _encryptionService.DecryptText(order.DirectDebitIban, oldEncryptionPrivateKey);

                //    string encryptedAccountHolder = _encryptionService.EncryptText(decryptedAccountHolder, newEncryptionPrivateKey);
                //    string encryptedAccountNumber = _encryptionService.EncryptText(decryptedAccountNumber, newEncryptionPrivateKey);
                //    string encryptedBankCode = _encryptionService.EncryptText(decryptedBankCode, newEncryptionPrivateKey);
                //    string encryptedBankName = _encryptionService.EncryptText(decryptedBankName, newEncryptionPrivateKey);
                //    string encryptedBic = _encryptionService.EncryptText(decryptedBic, newEncryptionPrivateKey);
                //    string encryptedCountry = _encryptionService.EncryptText(decryptedCountry, newEncryptionPrivateKey);
                //    string encryptedIban = _encryptionService.EncryptText(decryptedIban, newEncryptionPrivateKey);

                //    order.DirectDebitAccountHolder = encryptedAccountHolder;
                //    order.DirectDebitAccountNumber = encryptedAccountNumber;
                //    order.DirectDebitBankCode = encryptedBankCode;
                //    order.DirectDebitBankName = encryptedBankName;
                //    order.DirectDebitBIC = encryptedBic;
                //    order.DirectDebitCountry = encryptedCountry;
                //    order.DirectDebitIban = encryptedIban;

                //    _orderService.UpdateOrder(order);
                //}

                //update user information
                //optimization - load only users with PasswordFormat.Encrypted
                var users = _userService.GetAllUsersByPasswordFormat(PasswordFormat.Encrypted);
                foreach (var user in users)
                {
                    string decryptedPassword = _encryptionService.DecryptText(user.Password, oldEncryptionPrivateKey);
                    string encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    user.Password = encryptedPassword;
                    _userService.UpdateUser(user);
                }

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(securitySettings);
                NotifySuccess(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                NotifyError(exc);
            }
			return RedirectToAction("GeneralCommon", new { selectedTab = "generalsettings-edit-3" });
        }
        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("togglefulltext")]
        public ActionResult ToggleFullText(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

			var commonSettings = _settingService.LoadSetting<CommonSettings>();

            try
            {
                //if (!_fulltextService.IsFullTextSupported())
                //    throw new SpecterException(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.NotSupported"));

                //if (commonSettings.UseFullTextSearch)
                //{
                //    _fulltextService.DisableFullText();

                //    commonSettings.UseFullTextSearch = false;
                //    _settingService.SaveSetting(commonSettings);

                //    NotifySuccess(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disabled"));
                //}
                //else
                //{
                //    _fulltextService.EnableFullText();

                //    commonSettings.UseFullTextSearch = true;
                //    _settingService.SaveSetting(commonSettings);

                //    NotifySuccess(_localizationService.GetResource("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enabled"));
                //}
            }
            catch (Exception exc)
            {
                NotifyError(exc);
            }
			return RedirectToAction("GeneralCommon", new { selectedTab = "generalsettings-edit-9" });
        }
        #endregion GeneralCommon

        #region AllSettings
        public ActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return View();
        }
        [HttpPost]
        public ActionResult AllSettings([DataSourceRequest]DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            var settings = _settingService
                .GetAllSettings()
				.Select(x =>
				{
					var settingModel = new SettingModel()
					{
						Id = x.Id,
						Name = _localizationService.GetResource(x.Name),
						Value = x.Value,
					};
					return settingModel;
				})
                .ToList();

            return Json(settings.ToDataSourceResult(request));
        }

        #region Filters
        public ActionResult FilterMenu_Name()
        {
            return Json(_settingService.GetAllSettings().Select(e => e.Name).Distinct(), JsonRequestBehavior.AllowGet);
        }
        #endregion Filters

        public ActionResult SettingUpdate(SettingModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var setting = _settingService.GetSettingById(model.Id);
			if (setting == null)
				return Content(_localizationService.GetResource("Admin.Configuration.Settings.NoneWithThatId"));

			if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				//setting name or store has been changed
				_settingService.DeleteSetting(setting);
			}

			_settingService.SetSetting(model.Name, model.Value);

            //activity log
            _userActivityService.InsertActivity("EditSettings", _localizationService.GetResource("ActivityLog.EditSettings"));

            return null;
            //return AllSettings(command);
        }

        public ActionResult SettingAdd([Bind(Exclude = "Id")] SettingModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

			_settingService.SetSetting(model.Name, model.Value);

            //activity log
            _userActivityService.InsertActivity("AddNewSetting", _localizationService.GetResource("ActivityLog.AddNewSetting"), model.Name);

            return null;
            //return AllSettings(command);
        }

        public ActionResult SettingDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var setting = _settingService.GetSettingById(id);
            if (setting == null)
                throw new ArgumentException("No setting found with the specified id");
            _settingService.DeleteSetting(setting);

            //activity log
            _userActivityService.InsertActivity("DeleteSetting", _localizationService.GetResource("ActivityLog.DeleteSetting"), setting.Name);

            return null;
            //return AllSettings(command);
        }
        #endregion AllSettings
        #endregion Methods
    }
}
