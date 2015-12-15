using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

using Specter.Web.Core.Domain.Common;
using Specter.Web.Core.Domain.Users;
using Specter.Web.Infrastructure;
using Specter.Web.Services.Directory;
using Specter.Web.Services.GPS;
using Specter.Web.Services.Helpers;
using Specter.Web.Services.Localization;
using Specter.Web.Services.Security;
using Specter.Web.Services.Users;
using Specter.Web.Core;
using Specter.Web.Infrastructure.Models.Users;

namespace Specter.Web.Admin.Controllers
{
    public class UserController : AdminControllerBase
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IUserReportService _userReportService;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDeviceService _deviceService;
        private readonly IDeviceReportService _deviceReportService;
        private readonly ISessionService _sessionService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly UserSettings _userSettings;
        private readonly ICountryService _countryService;
        #endregion Fields

        #region Constructor
        public UserController(
            ILocalizationService localizationService, 
            DateTimeSettings dateTimeSettings, IDateTimeHelper dateTimeHelper,
            IDeviceService deviceService, IDeviceReportService deviceReportService,
            ISessionService sessionService, IWorkContext workContext,
            IPermissionService permissionService,
            UserSettings userSettings, ICountryService countryService,
            IUserReportService userReportService, IUserService userService)
        {
            this._userService = userService;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._userReportService = userReportService;
            this._deviceService = deviceService;
            this._deviceReportService = deviceReportService;
            this._sessionService = sessionService;
            this._userSettings = userSettings;
            this._countryService = countryService;
            this._workContext = workContext;
            this._permissionService = permissionService;
        }
        #endregion Constructor

        #region Users
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            ViewBag.Layout = "~/Views/Shared/_LayoutAdmin.cshtml";

            var model = new UserModel();
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultSiteTimeZone.Id) });
            
            model.DisplayVatNumber = false;

            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            model.SelectedUserRoleIds = new long[0];
            model.AllowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);

            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.StateProvinceEnabled = _userSettings.StateProvinceEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;

            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
                }
            }

            //default value
            model.Active = true;

            return View(model);
        }

        #endregion Users

        #region Report
        [ChildActionOnly]
        public ActionResult ReportRegisteredUsers()
        {
            var model = GetReportRegisteredUsersModel();
            return PartialView(model);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ReportRegisteredUsersList(GridCommand command)
        {
            var model = GetReportRegisteredUsersModel();
            var gridModel = new GridModel<RegisteredUserReportLineModel>
            {
                Data = model,
                Total = model.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [ChildActionOnly]
        public ActionResult TodayReport()
        {
            var model = GetTodayReportModel();
            return PartialView(model);
        }
        #endregion Report

        #region Utilities
        [NonAction]
        protected IList<RegisteredUserReportLineModel> GetReportRegisteredUsersModel()
        {
            var report = new List<RegisteredUserReportLineModel>();
            report.Add(new RegisteredUserReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.7days"),
                Users = _userReportService.GetRegisteredUsersReport(7)
            });

            report.Add(new RegisteredUserReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.14days"),
                Users = _userReportService.GetRegisteredUsersReport(14)
            });
            report.Add(new RegisteredUserReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.month"),
                Users = _userReportService.GetRegisteredUsersReport(30)
            });
            report.Add(new RegisteredUserReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Users.Reports.RegisteredUsers.Fields.Period.year"),
                Users = _userReportService.GetRegisteredUsersReport(365)
            });

            return report;
        }
        [NonAction]
        protected TodayReportModel GetTodayReportModel()
        {
            var report = new TodayReportModel();
            report.NowConnectedDevices = _deviceReportService.NowConnectedDevices();
            report.TodayActiveDevices = _deviceReportService.TodayActiveDevices();
            report.TodayConnected = _deviceReportService.TodayConnected();
            report.TodayDeactivatedDevices = _deviceReportService.TodayDeactivatedDevices();
            report.UsersOnline = _sessionService.GetCountSession();

            return report;
        }
        #endregion Utilities
    }
}