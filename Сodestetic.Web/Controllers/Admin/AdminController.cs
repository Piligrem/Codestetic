using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;

using Specter.Web.Data;
using Specter.Web.Models;
using Specter.Web.Core.Data;
using Specter.Web.Core.Domain.Configuration;
using Specter.Web.Services.Configuration;
using Specter.Web.Core.Caching;

using Specter.Web.Services.Logging;
using Specter.Web.Core.Logging;
using Specter.Web.Core.Events;
using Specter.Web.Core;
using Specter.Web.Services.Users;
using Specter.Web.Services.GPS;
using Specter.Web.Core.Domain.Media;
using Specter.Web.Services.Media;
using Specter.Web.Core.Domain.Common;
using Specter.Web.Core.Domain.Users;
using Specter.Web.Core.Domain.Localization;
using Specter.Web.Services.Security;
using Specter.Web.Services.Common;
using Specter.Web.Services.Localization;
using Specter.Web.Core.Localization;
using Specter.Web.Infrastructure.Cache;

namespace Specter.Web.Admin.Controllers
{
    [Authorize]
    public class AdminController : AdminControllerBase
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

        private readonly ISettingService _settingService;

        private readonly IDeviceIndicatorService deviceIndicatorService;
        #endregion Fields

        #region Constructors
        public AdminController(
            ILanguageService languageService,
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
            ISettingService settingService)
        {
            this._languageService = languageService;
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

            this.deviceIndicatorService = deviceIndicatorService;
        }
        #endregion Constructors

        #region Properties
        public Localizer T { get; set; }
        #endregion Properties

        public ActionResult Index()
        {
            _cacheManager.Clear();
            return View();
        }

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
                    long logoPictureId = siteSettings.LogoPictureId;

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
        public ActionResult Menu()
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
    }
}