using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Security;

using Codestetic.Web.Models;
using Codestetic.Web.Data;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Framework.Users;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Core;
using Codestetic.Web.Services.Authentication;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Services.Common;
using Codestetic.Web.Framework.Security;
using Codestetic.Web.Core.Domain.Messages;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Models.User;
using Codestetic.Web.Core.Domain.Forums;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Services.Directory;
using Codestetic.Web.Areas.Admin.Models.Common;
using Codestetic.Web.Services.Messages;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Services.Media;
using Codestetic.Web.Core.Domain.Media;
using Codestetic.Web.Framework.UI.Captcha;
using Codestetic.Web.Core.Domain.Localization;
using System.IO;
using Codestetic.Web.Models.Device;
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Services.Devices;

namespace Codestetic.Web.Controllers
{
    public class DeviceController : PublicControllerBase
    {
        #region Fields
        //private ApplicationUserManager _userManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IUserService _userService;
        private readonly IUserActivityService _userActivityService;
        private readonly IWorkContext _workContext;
        private readonly UserSettings _userSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly ForumSettings _forumSettings;
        private readonly IPictureService _pictureService;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWebHelper _webHelper;
        private readonly CaptchaSettings _captchaSettings;
        private readonly IDeviceService _deviceService;

        private readonly ICurrencyService _currencyService;
        //private readonly IPriceFormatter _priceFormatter;
        //private readonly IOrderProcessingService _orderProcessingService;
        //private readonly IOrderService _orderService;

        //private readonly IOpenAuthenticationService _openAuthenticationService;

        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly MediaSettings _mediaSettings;
        #endregion Fields

        public DeviceController(IAuthenticationService authenticationService,
            IDateTimeHelper dateTimeHelper, DateTimeSettings dateTimeSettings,
            IPictureService pictureService, MediaSettings mediaSettings,
            IWorkContext workContext, IUserService userService,
            //IOrderTotalCalculationService orderTotalCalculationService, IOrderService orderService,
            //IOrderProcessingService orderProcessingService, IPriceFormatter priceFormatter,
            ICurrencyService currencyService, IWebHelper webHelper,
            //IOpenAuthenticationService openAuthenticationService,
            UserSettings userSettings, ForumSettings forumSettings, AddressSettings addressSettings,
            IUserRegistrationService userRegistrationService, ICountryService countryService,
            ILocalizationService localizationService, LocalizationSettings localizationSettings, 
            IGenericAttributeService genericAttributeService, INewsLetterSubscriptionService newsLetterSubscriptionService,
            CaptchaSettings captchaSettings, IAddressService addressService,
            IWorkflowMessageService workflowMessageService, IDeviceService deviceService,
            IUserActivityService userActivityService)
        {
            this._authenticationService = authenticationService;
            this._dateTimeHelper = dateTimeHelper;
            this._dateTimeSettings = dateTimeSettings;
            this._userRegistrationService = userRegistrationService;
            this._userService = userService;
            this._workContext = workContext;
            this._userActivityService = userActivityService;
            this._userSettings = userSettings;
            this._pictureService = pictureService;
            this._forumSettings = forumSettings;
            this._addressSettings = addressSettings;
            this._countryService = countryService;
            this._addressService = addressService;
            this._localizationSettings = localizationSettings;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            //this._openAuthenticationService = openAuthenticationService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;

            this._deviceService = deviceService;

            //this._orderProcessingService = orderProcessingService;
            //this._orderTotalCalculationService = orderTotalCalculationService;
            //this._orderService = orderService;
            //this._priceFormatter = priceFormatter;
            
            this._currencyService = currencyService;
            
            this._captchaSettings = captchaSettings;

            this._workflowMessageService = workflowMessageService;
            this._mediaSettings = mediaSettings;
        }

        #region Info
        public ActionResult Info()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();
            
            var user = _workContext.CurrentUser;

            var model = new DeviceUserListModel();
            model.NavigationModel = GetDeviceNavigationModel();
            model.NavigationModel.SelectedTab = DeviceNavigationEnum.Info;
            foreach (var device in user.Devices)
            {
                var deviceModel = new DeviceUserInfoModel();
                PrepareDeviceInfoModel(deviceModel, device, false);
                model.Devices.Add(deviceModel);
            }
            return View(model);
        }
        #endregion Info

        #region Add device
        public ActionResult AddDevice()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;
            var model = PrepareDeviceAddModel();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDevice(DeviceModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;
            var availableDeviceTypes = _deviceService.GetAllDeviceTypes();

            if (String.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("", "Name is not provided.");
            if (String.IsNullOrEmpty(model.IMEI))
            {
                ModelState.AddModelError("", "IMEI is not provided.");
            }
            if (!String.IsNullOrEmpty(model.IMEI))
            {
                var deviceRegistred = _deviceService.GetDeviceByIMEI(model.IMEI);
                if (deviceRegistred != null)
                    ModelState.AddModelError("", "A device with so IMEI is already registered.");
            }
            if (model.DeviceTypeId == 0)
            {
                ModelState.AddModelError("", "Device type is not provided.");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _deviceService.AddDevice(user, model.Name, model.IMEI, model.DeviceTypeId);
                }
                return RedirectToRoute("Devices");
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }
            
            model = PrepareDeviceAddModel();

            return View(model);
        }
        #endregion Add device

        #region Edit
        #endregion Edit

        #region Delete
        public ActionResult Delete()
        {
            //if (!IsCurrentUserRegistered())
            //    return new HttpUnauthorizedResult();

            //var user = _workContext.CurrentUser;
            //var model = PrepareUserOrderListModel(user);
            //return View(model);
            return View("_NotSupported");
        }
        #endregion Delete

        #region Payment
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult SubscriptionPayment()
        {
            //if (!IsCurrentUserRegistered())
            //    return new HttpUnauthorizedResult();

            //var user = _workContext.CurrentUser;
            //var model = PrepareUserOrderListModel(user);
            //return View(model);
            return View("_NotSupported");
        }
        #endregion Payment

        #region Report device
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult ReportDevice()
        {
            //if (!IsCurrentUserRegistered())
            //    return new HttpUnauthorizedResult();

            //var user = _workContext.CurrentUser;
            //var model = PrepareUserOrderListModel(user);
            //return View(model);
            return View("_NotSupported");
        }
        #endregion Report device

        #region Utilities
        [NonAction]
        protected bool IsCurrentUserRegistered()
        {
            return _workContext.CurrentUser.IsRegistered();
        }
        [NonAction]
        protected DeviceUserNavigationModel GetDeviceNavigationModel()
        {
            var model = new DeviceUserNavigationModel();
            return model;
        }
        [NonAction]
        protected void PrepareDeviceInfoModel(DeviceUserInfoModel model, Device device, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (device == null)
                throw new ArgumentNullException("device");

            if (!excludeProperties)
            {
                model.Name = device.Name;
                model.IMEI = device.IMEI;
                model.PictureId = device.DeviceSetting.PictureId;
                model.DeviceType = device.DeviceType.Factory + ":" + device.DeviceType.Model;
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc).Value;
                model.LastConnection = _dateTimeHelper.ConvertToUserTime(device.DevicePosition.DateOnUtc);
                model.Active = device.Active;
                model.ControlSatellites = device.DeviceSetting.ControlSatellites;
                model.ControlGSM = device.DeviceSetting.ControlGSM;
                model.ControlBattery = device.DeviceSetting.ControlBattery;
                model.ControlInGeoZone = device.DeviceSetting.ControlInGeoZone;
                model.ControlOutGeoZone = device.DeviceSetting.ControlOutGeoZone;
                model.ControlButtonSos = device.DeviceSetting.ControlButtonSos;
            }
            else
            {
            }

        }
        [NonAction]
        protected DeviceModel PrepareDeviceAddModel()
        {
            var model = new DeviceModel();

            model.AvailableDeviceTypes.Add(new SelectListItem() { Text = _localizationService.GetResource("Device.SelectDeviceType"), Value = "0", Selected = true });
            foreach (var c in _deviceService.GetAllDeviceTypes())
            {
                model.AvailableDeviceTypes.Add(new SelectListItem()
                {
                    Text = c.Factory + ":" + c.Model,
                    Value = c.Id.ToString(),
                });
            }

            model.NavigationModel = GetDeviceNavigationModel();
            model.NavigationModel.SelectedTab = DeviceNavigationEnum.Add;

            return model;
        }
        #endregion Utilities
    }
}