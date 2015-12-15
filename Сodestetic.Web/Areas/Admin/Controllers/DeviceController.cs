using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Spatial;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.IO;

using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Codestetic.Web.Areas.Admin.Models.Devices;
using Codestetic.Web.Services.Devices;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Infrastructure;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Core.Domain.Devices;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Areas.Admin.Infrastructure;
using Codestetic.Web.Areas.Admin.Models.Users;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Services.Common;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Data;

namespace Codestetic.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DeviceController : AdminControllerBase
    {
        #region Constants
        private const string colorPattern = @"^rgba\(\s*([0-9]+)\s*,\s*([0-9]+)\s*,\s*([0-9]+)\s*,\s*([0-9.]+)\s*\)";
        #endregion Constants

        #region Fields
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DeviceDefaultSettings _deviceDefaultSettings;
        private readonly IDbContext _context;
        #endregion Fields

        #region Constructors
        public DeviceController(IDeviceService deviceService, IUserService userService,
            DateTimeSettings dateTimeSettings, IDateTimeHelper dateTimeHelper,
            AdminAreaSettings adminAreaSettings, IUserActivityService userActivityService,
            DeviceDefaultSettings deviceDefaultSettings, IDbContext context,
            IPermissionService permissionService, ILocalizationService localizationService)
        {
            this._deviceService = deviceService;
            this._userService = userService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
            this._userActivityService = userActivityService;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._deviceDefaultSettings = deviceDefaultSettings;
            this._context = context;
        }
        #endregion Constructors

        #region Methods

        #region Index/List
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            return View();
        }
        [HttpPost]
        public ActionResult List([DataSourceRequest]DataSourceRequest request)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var devices = _deviceService.GetAllDevices(0, _adminAreaSettings.GridPageSize);
            var model = devices.Select(PrepareDeviceModelForList); // devices.Select(PrepareDeviceModelForList).ToList();
            return Json(model.ToDataSourceResult(request));
            //return View(listModel);
        }
        #endregion Index/List

        #region Create Device
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = new DeviceModel();

            //Device types
            var availableDeviceTypes = _deviceService
                .GetAllDeviceTypes()
                .Select(dt => dt.ToModel());
            model.AvailableDeviceTypes = availableDeviceTypes.Select(dt =>
                new SelectListItem()
                {
                    Text = dt.Factory + " " + dt.DeviceModel,
                    Value = dt.Id.ToString()
                }).ToList();

            //default value
            model.Active = true;

            return View(model);
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        public ActionResult Create(DeviceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (String.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Devices.ErrorName"));
            if (String.IsNullOrEmpty(model.IMEI))
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Devices.ErrorIMEI"));
            if (model.DeviceTypeId == 0)
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Devices.ErrorDeviceType"));
            if (!String.IsNullOrEmpty(model.IMEI))
            {
                var device2 = _deviceService.GetDeviceByIMEI(model.IMEI);
                if (device2 != null)
                    ModelState.AddModelError("", "IMEI is already registered");
            }

            if (ModelState.IsValid)
            {
                var device = new Device()
                {
                    IMEI = model.IMEI,
                    Name = model.Name,
                    Active = model.Active,
                    DeviceTypeId = model.DeviceTypeId,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                _deviceService.InsertDevice(device);
                device = _deviceService.GetDeviceByIMEI(model.IMEI);

                var position = new DevicePosition()
                {
                    Id = device.Id, // DeviceId
                    TimestampOnUtc = DateTime.UtcNow,
                    DateOnUtc = DateTime.UtcNow,
                    Valid = true,
                    Position = Geo.ToGeoPoint(0, 0),
                    Altitude = 0,
                    Angle = 0,
                    Speed = 0
                };
                var settings = new DeviceSetting()
                {
                    Id = device.Id, // DeviceId
                    CreatedOnUtc = DateTime.UtcNow,
                    PictureId = 0,
                    MarkerId = 0,
                    
                    ToleranceEnable = _deviceDefaultSettings.ToleranceEnable,
                    ToleranceRadius = _deviceDefaultSettings.ToleranceRadius,
                    ToleranceStrokeColor = _deviceDefaultSettings.ToleranceStrokeColor,
                    ToleranceStrokeWidth = _deviceDefaultSettings.ToleranceStrokeWidth,
                    ToleranceFillColor = _deviceDefaultSettings.ToleranceFillColor,

                    TrackEnable = _deviceDefaultSettings.TrackEnable,
                    TrackStrokeColor = _deviceDefaultSettings.TrackStrokeColor,
                    TrackStrokeWidth = _deviceDefaultSettings.TrackStrokeWidth,

                    IntervalUpdateDevice = _deviceDefaultSettings.IntervalUpdateDevice,

                    ControlSatellites = _deviceDefaultSettings.ControlSatellites,
                    ControlBattery = _deviceDefaultSettings.ControlBattery,
                    MinBatteryLevel = _deviceDefaultSettings.MinBatteryLevel,
                    ControlGSM = _deviceDefaultSettings.ControlGSM,
                    ControlButtonSos = _deviceDefaultSettings.ControlButtonSos,
                    ControlInGeoZone = _deviceDefaultSettings.ControlInGeoZone,
                    ControlOutGeoZone = _deviceDefaultSettings.ControlOutGeoZone,
                };
                device.DevicePosition = position;
                device.DeviceSetting = settings;
                _deviceService.InsertDevicePosition(position);
                _deviceService.InsertDeviceSetting(settings);

                //activity log
                _userActivityService.InsertActivity("AddNewDevice", _localizationService.GetResource("ActivityLog.AddNewDevice"), device.Id);

                NotifySuccess(_localizationService.GetResource("Admin.Devices.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = device.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            //Device types
            var availableDeviceTypes = _deviceService
                .GetAllDeviceTypes()
                .Select(dt => dt.ToModel());
            model.AvailableDeviceTypes = availableDeviceTypes.Select(dt =>
                new SelectListItem()
                {
                    Text = dt.Factory + " " + dt.DeviceModel,
                    Value = dt.Id.ToString()
                }).ToList();
            //form fields
            model.Active = true;

            return View(model);
        }
        #endregion Create User

        #region Edit Device
        public ActionResult Edit(long id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(id);
            if (device == null)
                return RedirectToAction("List");

            var model = PrepareDeviceModelForList(device);

            //Device types
            var availableDeviceTypes = _deviceService
                .GetAllDeviceTypes()
                .Select(dt => dt.ToModel());
            model.AvailableDeviceTypes = availableDeviceTypes.Select(dt =>
                new SelectListItem()
                {
                    Text = dt.Factory + " " + dt.DeviceModel,
                    Value = dt.Id.ToString()
                }).ToList();

            return View(model);
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(DeviceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(model.Id);
            if (device == null)
                return RedirectToAction("List");

            if (String.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Devices.ErrorName"));
            if (String.IsNullOrEmpty(model.IMEI))
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Devices.ErrorIMEI"));
            if (model.DeviceTypeId == 0)
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Devices.ErrorDeviceType"));
            if (!String.IsNullOrEmpty(model.IMEI))
            {
                var device2 = _deviceService.GetDeviceByIMEI(model.IMEI);
                if (device2 != null && device2.Id != device.Id)
                    ModelState.AddModelError("", "IMEI is already registered");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    device.Name = model.Name;
                    device.IMEI = model.IMEI;
                    device.Active = model.Active;

                    device.DeviceSetting.PictureId = model.PictureId;
                    device.DeviceSetting.MarkerId = model.MarkerId;

                    device.DeviceSetting.ToleranceEnable = model.ToleranceEnable;
                    device.DeviceSetting.ToleranceRadius = model.Tolerance;
                    device.DeviceSetting.ToleranceStrokeColor = model.StrokeColor.RgbaToHex();
                    device.DeviceSetting.ToleranceStrokeWidth = model.StrokeWidth;
                    device.DeviceSetting.ToleranceFillColor = model.FillColor.RgbaToHex();

                    device.DeviceSetting.TrackEnable = model.TrackEnable;
                    device.DeviceSetting.TrackStrokeColor = model.TrackStrokeColor.RgbaToHex();
                    device.DeviceSetting.TrackStrokeWidth = model.TrackStrokeWidth;
                    
                    //device.DeviceSetting.IntervalUpdateDevice

                    device.DeviceSetting.ControlSatellites = model.ControlSatellites;
                    device.DeviceSetting.ControlBattery = model.ControlBattery;
                    device.DeviceSetting.MinBatteryLevel = model.MinBatteryLevel;
                    device.DeviceSetting.ControlGSM = model.ControlGSM;
                    device.DeviceSetting.ControlButtonSos = model.ControlButtonSos;
                    device.DeviceSetting.ControlInGeoZone = model.ControlInGeoZone;
                    device.DeviceSetting.ControlOutGeoZone = model.ControlOutGeoZone;

                    device.DeviceSetting.UpdateOnUtc = DateTime.UtcNow;

                    _deviceService.UpdateDevice(device);

                    //activity log
                    _userActivityService.InsertActivity("EditDevice", _localizationService.GetResource("ActivityLog.EditDevice"), device.Id);

                    NotifySuccess(_localizationService.GetResource("Admin.Devices.Updated"));
                    return continueEditing ? RedirectToAction("Edit", device.Id) : RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    NotifyError(ex.Message, false);
                }
            }

            //If we got this far, something failed, redisplay form
            //Device types
            var availableDeviceTypes = _deviceService
                .GetAllDeviceTypes()
                .Select(dt => dt.ToModel());
            model.AvailableDeviceTypes = availableDeviceTypes.Select(dt =>
                new SelectListItem()
                {
                    Text = dt.Factory + " " + dt.DeviceModel,
                    Value = dt.Id.ToString()
                }).ToList();
            
            //model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            //model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            //model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            //foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            //    model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            //model.CreatedOn = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc, DateTimeKind.Utc).Value;
            //model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(device.LastActivityDateUtc.Value, DateTimeKind.Utc).Value;
            //model.LastIpAddress = model.LastIpAddress;
            //model.LastVisitedPage = device.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);

            return View(model);
        }
        #endregion Edit Device

        #region Delete Device
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(id);
            if (device == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                var position = device.DevicePosition;
                var settings = device.DeviceSetting;

                _deviceService.DeleteDevicePosition(position);
                _deviceService.DeleteDeviceSetting(settings);

                _deviceService.DeleteDevice(device);

                //activity log
                _userActivityService.InsertActivity("DeleteDevice", _localizationService.GetResource("ActivityLog.Delete.Device"), device.Id);

                NotifySuccess(_localizationService.GetResource("Admin.Devices.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
                return RedirectToAction("Edit", new { id = device.Id });
            }
        }
        #endregion Delete Device

        #region Users
        public ActionResult UsersList(long deviceId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(deviceId);
            if (device == null)
                throw new ArgumentException("No device found with the specified id", "deviceId");

            var users = device.Users.ToList();
            var gridModel = new GridModel<UserModel>
            {
                Data = users.Select(x =>
                {
                    var model = x.ToModel();
                    return model;
                }),
                Total = users.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
            //return Json(gridModel, JsonRequestBehavior.AllowGet);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachDetachUser(long deviceId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(deviceId);
            //load registered users by default
            var defaultRoleIds = new[] { _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Id };
            var users = _userService.GetAllUsers(null, null, defaultRoleIds, null, null, null, null, 0, 0, null, null, null, 0, _adminAreaSettings.GridPageSize);
            //user list
            var listModel = new GridModel<DeviceUserModel>
            {
                Data = users.Select(user => PrepareUserModelForList(user, device)),
                Total = users.TotalCount
            };
            if (device == null)
                return RedirectToAction("List");

            ViewBag.deviceId = deviceId;
            ViewBag.deviceName = device.Name;
            ViewBag.deviceType = device.DeviceType.Factory + " " + device.DeviceType.Model;
            return View(listModel);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachDetachUser(long deviceId, long userId, bool userAttached, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(deviceId);
            var user = _userService.GetUserById(userId);
            var message = string.Empty;

            if (device == null || user == null)
                return RedirectToAction("AttachDetachUser");

            if (!device.Users.Any(u => u.Id == userId) && userAttached)
            {
                device.Users.Add(user);
                _deviceService.UpdateDevice(device);
            }
            else if (device.Users.Any(u => u.Id == userId) && userAttached)
                message = _localizationService.GetResource("Admin.Devices.UserAlreadyAttached");
            else if (device.Users.Any(u => u.Id == userId) && !userAttached)
            {
                device.Users.Remove(user);
                _deviceService.UpdateDevice(device);
            }
            else if (!device.Users.Any(u => u.Id == userId) && !userAttached)
                message = _localizationService.GetResource("Admin.Devices.UserAlreadyDetached");

            return Json(new 
            { 
                Success = true,
                Message = message
            });
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult RemoveUser(long deviceId, long userId, GridCommand command)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
        //        return AccessDeniedView();

        //    var device = _deviceService.GetDeviceById(deviceId);
        //    if (device == null)
        //        throw new ArgumentException("No device found with the specified id", "deviceId");

        //    var user = device.Users.Where(u => u.Id == userId).FirstOrDefault();
        //    if (user == null)
        //        throw new ArgumentException("No user found with the specified id", "userId");

        //    device.RemoveUser(user);
        //    _deviceService.UpdateDevice(device);
        //    return Json(new { Success = true });
        //}
        #endregion Users

        #endregion Methods

        #region Utilities
        [NonAction]
        protected DeviceModel PrepareDeviceModelForList(Device device)
        {
            if (device.DevicePosition == null) return new DeviceModel(); 
            return new DeviceModel()
            {
                Id = device.Id,
                Name = device.Name,
                IMEI = device.IMEI,
                DeviceType = device.DeviceType.Factory + ":" + device.DeviceType.Model,
                DeviceTypeId = device.DeviceTypeId,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc).Value,
                PictureId = device.DeviceSetting.PictureId,
                MarkerId = device.DeviceSetting.MarkerId,
                ToleranceEnable = device.DeviceSetting.ToleranceEnable,
                Tolerance = device.DeviceSetting.ToleranceRadius,
                StrokeColor = device.DeviceSetting.ToleranceStrokeColor,
                StrokeWidth = device.DeviceSetting.ToleranceStrokeWidth,
                FillColor = device.DeviceSetting.ToleranceFillColor,
                TrackEnable = device.DeviceSetting.TrackEnable,
                TrackStrokeColor = device.DeviceSetting.TrackStrokeColor,
                TrackStrokeWidth = device.DeviceSetting.TrackStrokeWidth,
                IntervalUpdateDevice = device.DeviceSetting.IntervalUpdateDevice,
                ControlSatellites = device.DeviceSetting.ControlSatellites,
                ControlBattery = device.DeviceSetting.ControlBattery,
                MinBatteryLevel = Convert.ToInt16(device.DeviceSetting.MinBatteryLevel),
                ControlGSM = device.DeviceSetting.ControlGSM,
                ControlButtonSos = device.DeviceSetting.ControlButtonSos,
                ControlInGeoZone = device.DeviceSetting.ControlInGeoZone,
                ControlOutGeoZone = device.DeviceSetting.ControlOutGeoZone,
                UpdateOn = _dateTimeHelper.ConvertToUserTime(device.DeviceSetting.UpdateOnUtc),
                LastConnection = device.DevicePosition != null ? _dateTimeHelper.ConvertToUserTime(device.DevicePosition.DateOnUtc) : DateTime.MinValue,
                Active = device.Active,
            };
        }
        [NonAction]
        protected DeviceUserModel PrepareUserModelForList(User user, Device device)
        {
            var deviceAttached = device.Users.Any(u => u.Id == user.Id);
            var model = new DeviceUserModel()
            {
                Id = user.Id,
                Email = !String.IsNullOrEmpty(user.Email) ? user.Email : (user.IsGuest() ? _localizationService.GetResource("Admin.Users.Guest") : "Unknown"),
                Username = user.Username,
                FullName = user.GetFullName(),
                Company = user.GetAttribute<string>(SystemUserAttributeNames.Company),
                Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone),
                Active = user.Active,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc).Value,
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc.Value, DateTimeKind.Utc).Value,
                DeviceAttached = deviceAttached,
                AddButtonStyle = deviceAttached ? "display:none;" : "",
                RemoveButtonStyle = deviceAttached ? "" : "display:none;"
            };
            return model;
        }
        #endregion Utilities
    }
}
