using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

using Codestetic.Web.Areas.Admin.Models.Devices;
using Codestetic.Web.Services.Devices;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Infrastructure;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Core.Domain.Devices;

namespace Codestetic.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DeviceTypeController : AdminControllerBase
    {
        #region Fields
        private readonly IDeviceService _deviceService;
        private readonly IPermissionService _permissionService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        #endregion Fields

        #region Constructors
        public DeviceTypeController(IDeviceService deviceService,
            AdminAreaSettings adminAreaSettings, IUserActivityService userActivityService,
            IPermissionService permissionService, ILocalizationService localizationService)
        {
            this._deviceService = deviceService;
            this._permissionService = permissionService;
            this._adminAreaSettings = adminAreaSettings;
            this._userActivityService = userActivityService;
            this._localizationService = localizationService;
        }
        #endregion Constructors

        #region Methods
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var deviceTypes = _deviceService.GetAllDeviceTypes(0, _adminAreaSettings.GridPageSize);
            var model = deviceTypes.Select(x => x.ToModel()).ToList();
            return View(model);
        }

        #region Create DeviceType
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = new DeviceTypeModel()
            {
                Factory = string.Empty,
                DeviceModel = string.Empty
            };
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        public ActionResult Create(DeviceTypeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (String.IsNullOrEmpty(model.Factory))
                ModelState.AddModelError("", _localizationService.GetResource("Admin.DeviceType.ErrorFactory"));
            if (String.IsNullOrEmpty(model.DeviceModel))
                ModelState.AddModelError("", _localizationService.GetResource("Admin.DeviceType.ErrorDeviceModel"));
            if (!String.IsNullOrEmpty(model.Factory) && !String.IsNullOrEmpty(model.DeviceModel))
            {
                var deviceType2 = _deviceService.GetDeviceType(model.Factory, model.DeviceModel);
                if (deviceType2 != null)
                    ModelState.AddModelError("", "Device type is already registered");
            }

            if (ModelState.IsValid)
            {
                var deviceType = new DeviceType()
                {
                    Factory = model.Factory,
                    Model = model.DeviceModel
                };
                _deviceService.InsertDeviceType(deviceType);

                //activity log
                _userActivityService.InsertActivity("AddNewDeviceType", _localizationService.GetResource("ActivityLog.AddNewDeviceType"), deviceType.Id);

                NotifySuccess(_localizationService.GetResource("Admin.Device.Type.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = deviceType.Id }) : RedirectToAction("List");
            }
            return View(model);
        }
        #endregion Create DeviceType

        #region Edit DeviceType
        public ActionResult Edit(long id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var deviceType = _deviceService.GetDeviceTypeById(id);
            if (deviceType == null)
                return RedirectToAction("List");

            var model = new DeviceTypeModel()
            {
                Id = deviceType.Id,
                Factory = deviceType.Factory,
                DeviceModel = deviceType.Model
            };
            return View(model);
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        public ActionResult Edit(DeviceTypeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var deviceType = _deviceService.GetDeviceTypeById(model.Id);
            if (deviceType == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                try
                {
                    _deviceService.UpdateDeviceType(deviceType);

                    //activity log
                    _userActivityService.InsertActivity("EditUser", _localizationService.GetResource("ActivityLog.EditUser"), deviceType.Id);

                    NotifySuccess(_localizationService.GetResource("Admin.Device.Type.Updated"));
                    return continueEditing ? RedirectToAction("Edit", deviceType.Id) : RedirectToAction("List");
                }
                catch (Exception exc)
                {
                    NotifyError(exc.Message, false);
                }
            }

            return View(model);
        }
        #endregion Edit DeviceType

        #region Delete DeviceType
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var deviceType = _deviceService.GetDeviceTypeById(id);
            if (deviceType == null)
                return RedirectToAction("List");

            try
            {
                _deviceService.DeleteDeviceType(deviceType);

                //activity log
                _userActivityService.InsertActivity("DeleteDeviceType", _localizationService.GetResource("ActivityLog.Delete.DeviceType"), deviceType.Id);

                NotifySuccess(_localizationService.GetResource("Admin.DeviceType.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
                return RedirectToAction("Edit", new { id = deviceType.Id });
            }
        }
        #endregion Delete DeviceType
        #endregion Methods
    }
}
