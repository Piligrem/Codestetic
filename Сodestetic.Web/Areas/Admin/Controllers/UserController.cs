using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;

using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Infrastructure;
using Codestetic.Web.Services.Directory;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Services.Helpers;
using Codestetic.Web.Services.Localization;
using Codestetic.Web.Services.Security;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Core;
using Codestetic.Web.Areas.Admin.Models.Users;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Services.Common;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Areas.Admin.Infrastructure;
using Codestetic.Web.Services.Devices;
using Codestetic.Web.Services.Messages;
using Codestetic.Web.Core.Domain.Messages;
using Codestetic.Web.Areas.Admin.Models.Common;
using Codestetic.Web.Areas.Admin.Models.Devices;
using Codestetic.Web.Core.Domain.Devices;

namespace Codestetic.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UserController : AdminControllerBase
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IUserActivityService _userActivityService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IUserRegistrationService _userRegistrationService;
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
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressService _addressService;
        #endregion Fields

        #region Constructor
        public UserController(IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService, IUserActivityService userActivityService,
            DateTimeSettings dateTimeSettings, IDateTimeHelper dateTimeHelper,
            IDeviceService deviceService, IDeviceReportService deviceReportService,
            ISessionService sessionService, IWorkContext workContext,
            IPermissionService permissionService, IUserRegistrationService userRegistrationService,
            UserSettings userSettings, ICountryService countryService,
            AdminAreaSettings adminAreaSettings, INewsLetterSubscriptionService newsLetterSubscriptionService,
            IQueuedEmailService queuedEmailService, EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            AddressSettings addressSettings, IAddressService addressService,
            IUserReportService userReportService, IUserService userService)
        {
            this._genericAttributeService = genericAttributeService;
            this._userService = userService;
            this._userActivityService = userActivityService;
            this._userRegistrationService = userRegistrationService;
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
            this._adminAreaSettings = adminAreaSettings;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            this._addressSettings = addressSettings;
            this._addressService = addressService;
        }
        #endregion Constructor

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #region Users
        #region List User

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //load registered users by default
            var defaultRoleIds = new[] { _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Id };
            var listModel = new UserListModel()
            {
                UsernamesEnabled = _userSettings.UsernamesEnabled,
                DateOfBirthEnabled = _userSettings.DateOfBirthEnabled,
                CompanyEnabled = _userSettings.CompanyEnabled,
                PhoneEnabled = _userSettings.PhoneEnabled,
                ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled,
                AvailableUserRoles = _userService.GetAllUserRoles(true).Select(cr => cr.ToModel()).ToList(),
                SearchUserRoleIds = defaultRoleIds,
            };
            var users = _userService.GetAllUsers(null, null, defaultRoleIds, null, null, null, null, 0, 0, null, null, null, 0, _adminAreaSettings.GridPageSize);
            //user list
            listModel.Users = new GridModel<UserModel>
            {
                Data = users.Select(PrepareUserModelForList),
                Total = users.TotalCount
            };
            return View(listModel);
        }
        #endregion List User

        #region Create User
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

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
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        public ActionResult Create(UserModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var user2 = _userService.GetUserByEmail(model.Email);
                if (user2 != null)
                    ModelState.AddModelError("", "Email is already registered");
            }
            if (!String.IsNullOrWhiteSpace(model.Username) & _userSettings.UsernamesEnabled)
            {
                var user2 = _userService.GetUserByEmail(model.Username);
                if (user2 != null)
                    ModelState.AddModelError("", "Username is already registered");
            }

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);
            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!String.IsNullOrEmpty(userRolesError))
                ModelState.AddModelError("", userRolesError);
            bool allowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);

            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserGuid = Guid.NewGuid(),
                    Email = model.Email,
                    Username = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    AdminComment = model.AdminComment,
                    Active = model.Active,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                };
                _userService.InsertUser(user);

                //form fields
                if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                if (_userSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                if (_userSettings.DateOfBirthEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_userSettings.CompanyEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Company, model.Company);
                if (_userSettings.StreetAddressEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress, model.StreetAddress);
                if (_userSettings.StreetAddress2Enabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress2, model.StreetAddress2);
                if (_userSettings.ZipPostalCodeEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ZipPostalCode, model.ZipPostalCode);
                if (_userSettings.CityEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.City, model.City);
                if (_userSettings.CountryEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CountryId, model.CountryId);
                if (_userSettings.PhoneEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                if (_userSettings.FaxEnabled)
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);


                //password
                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _userSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            NotifyError(changePassError);
                    }
                }

                //user roles
                if (allowManagingUserRoles)
                {
                    foreach (var userRole in newUserRoles)
                        user.UserRoles.Add(userRole);
                    _userService.UpdateUser(user);
                }

                //activity log
                _userActivityService.InsertActivity("AddNewUser", _localizationService.GetResource("ActivityLog.AddNewUser"), user.Id);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = user.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.DisplayVatNumber = false;
            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            model.AllowManagingUserRoles = allowManagingUserRoles;
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.CountryId) });
                }
            }
            return View(model);
        }
        #endregion Create User

        #region Edit User
        public ActionResult Edit(long id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null || user.Deleted)
                //No user found with the specified id
                return RedirectToAction("List");

            var model = new UserModel();
            model.Id = user.Id;
            model.Email = user.Email;
            model.Username = user.Username;
            model.AdminComment = user.AdminComment;
            model.Active = user.Active;
            model.TimeZoneId = user.GetAttribute<string>(SystemUserAttributeNames.TimeZoneId);
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.VatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc).Value;
            model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc.Value, DateTimeKind.Utc).Value;
            model.LastIpAddress = user.LastIpAddress;
            model.LastVisitedPage = user.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);

            //form fields
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Gender = user.GetAttribute<string>(SystemUserAttributeNames.Gender);
            model.DateOfBirth = user.GetAttribute<DateTime?>(SystemUserAttributeNames.DateOfBirth);
            model.Company = user.GetAttribute<string>(SystemUserAttributeNames.Company);
            model.StreetAddress = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
            model.StreetAddress2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
            model.ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);
            model.City = user.GetAttribute<string>(SystemUserAttributeNames.City);
            model.CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
            model.Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
            model.Fax = user.GetAttribute<string>(SystemUserAttributeNames.Fax);

            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;

            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }
            }

            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            model.SelectedUserRoleIds = user.UserRoles.Select(cr => cr.Id).ToArray();
            model.AllowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);
            //external authentication records
            //model.AssociatedExternalAuthRecords = GetAssociatedExternalAuthRecords(user);

            return View(model);
        }
        [HttpPost, ParameterBasedOnFormNameAttribute("save", "continueEditing")]
        public ActionResult Edit(UserModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null || user.Deleted)
                //No user found with the specified id
                return RedirectToAction("List");

            //validate user roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);
            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!String.IsNullOrEmpty(userRolesError))
                ModelState.AddModelError("", userRolesError);
            bool allowManagingUserRoles = _permissionService.Authorize(StandardPermissionProvider.ManageUserRoles);

            if (ModelState.IsValid)
            {
                try
                {
                    user.AdminComment = model.AdminComment;
                    user.Active = model.Active;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    //email
                    if (!String.IsNullOrWhiteSpace(model.Email))
                    {
                        _userRegistrationService.SetEmail(user, model.Email);
                    }
                    else
                    {
                        user.Email = model.Email;
                    }

                    //username
                    if (_userSettings.UsernamesEnabled && _userSettings.AllowUsersToChangeUsernames)
                    {
                        if (!String.IsNullOrWhiteSpace(model.Username))
                        {
                            _userRegistrationService.SetUsername(user, model.Username);
                        }
                        else
                        {
                            user.Username = model.Username;
                        }
                    }

                    _userService.UpdateUser(user);

                    //form fields
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);
                    if (_userSettings.DateOfBirthEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, model.DateOfBirth);
                    if (_userSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Company, model.Company);
                    if (_userSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress, model.StreetAddress);
                    if (_userSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.StreetAddress2, model.StreetAddress2);
                    if (_userSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    if (_userSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.City, model.City);
                    if (_userSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.CountryId, model.CountryId);
                    if (_userSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Phone, model.Phone);
                    if (_userSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Fax, model.Fax);

                    //user roles
                    if (allowManagingUserRoles)
                    {
                        foreach (var userRole in allUserRoles)
                        {
                            if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                            {
                                //new role
                                if (user.UserRoles.Where(cr => cr.Id == userRole.Id).Count() == 0)
                                    user.UserRoles.Add(userRole);
                            }
                            else
                            {
                                //removed role
                                if (user.UserRoles.Where(cr => cr.Id == userRole.Id).Count() > 0)
                                    user.UserRoles.Remove(userRole);
                            }
                        }
                        _userService.UpdateUser(user);
                    }

                    //activity log
                    _userActivityService.InsertActivity("EditUser", _localizationService.GetResource("ActivityLog.EditUser"), user.Id);

                    NotifySuccess(_localizationService.GetResource("Admin.Users.Updated"));
                    return continueEditing ? RedirectToAction("Edit", user.Id) : RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    NotifyError(ex.Message, false);
                }
            }


            //If we got this far, something failed, redisplay form
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc).Value;
            model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc.Value, DateTimeKind.Utc).Value;
            model.LastIpAddress = model.LastIpAddress;
            model.LastVisitedPage = user.GetAttribute<string>(SystemUserAttributeNames.LastVisitedPage);
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.FaxEnabled = _userSettings.FaxEnabled;
            //countries and states
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }
            }
            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            model.AllowManagingUserRoles = allowManagingUserRoles;
            //external authentication records
            //model.AssociatedExternalAuthRecords = GetAssociatedExternalAuthRecords(user);
            return View(model);
        }
        #endregion Edit User

        #region Delete User
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                _userService.DeleteUser(user);

                //remove newsletter subscription (if exists)
                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(user.Email);
                if (subscription != null)
                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);

                //activity log
                _userActivityService.InsertActivity("DeleteUser", _localizationService.GetResource("ActivityLog.Delete.User"), user.Id);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
                return RedirectToAction("Edit", new { id = user.Id });
            }
        }
        #endregion Delete User
        
        #region Change password
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var changePassRequest = new ChangePasswordRequest(model.Email,
                    false, _userSettings.DefaultPasswordFormat, model.Password);
                var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                if (changePassResult.Success)
                    NotifySuccess(_localizationService.GetResource("Admin.Users.PasswordChanged"));
                else
                    foreach (var error in changePassResult.Errors)
                        NotifyError(error);
            }

            return RedirectToAction("Edit", user.Id);
        }
        #endregion Change password

        #region SendEmail
        public ActionResult SendEmail(UserModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(user.Email))
                    throw new SpecterException("User email is empty");
                if (!user.Email.IsEmail())
                    throw new SpecterException("User email is not valid");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new SpecterException("Email subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new SpecterException("Email body is empty");

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new SpecterException("Email account can't be loaded");

                var email = new QueuedEmail()
                {
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = user.GetFullName(),
                    To = user.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                _queuedEmailService.InsertQueuedEmail(email);
                NotifySuccess(_localizationService.GetResource("Admin.Users.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                NotifyError(exc.Message);
            }

            return RedirectToAction("Edit", new { id = user.Id });
        }
        #endregion SendEmail

        #region Addresses
        public ActionResult AddressesList(long userId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var addresses = user.Addresses.OrderByDescending(a => a.CreatedOnUtc).ThenByDescending(a => a.Id).ToList();
            var gridModel = new GridModel<AddressModel>
            {
                Data = addresses.Select(x =>
                {
                    var model = x.ToModel();
                    var addressHtmlSb = new StringBuilder("<div>");
                    if (_addressSettings.CompanyEnabled && !String.IsNullOrEmpty(model.Company))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Company));
                    if (_addressSettings.StreetAddressEnabled && !String.IsNullOrEmpty(model.Address1))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address1));
                    if (_addressSettings.StreetAddress2Enabled && !String.IsNullOrEmpty(model.Address2))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.Address2));
                    if (_addressSettings.CityEnabled && !String.IsNullOrEmpty(model.City))
                        addressHtmlSb.AppendFormat("{0},", Server.HtmlEncode(model.City));
                    if (_addressSettings.ZipPostalCodeEnabled && !String.IsNullOrEmpty(model.ZipPostalCode))
                        addressHtmlSb.AppendFormat("{0}<br />", Server.HtmlEncode(model.ZipPostalCode));
                    if (_addressSettings.CountryEnabled && !String.IsNullOrEmpty(model.CountryName))
                        addressHtmlSb.AppendFormat("{0}", Server.HtmlEncode(model.CountryName));
                    addressHtmlSb.Append("</div>");
                    return model;
                }),
                Total = addresses.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
            //return Json(gridModel, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddressDelete(long userId, long addressId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);

            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var address = _addressService.GetAddressById(addressId); //user.Addresses.Where(a => a.Id == model.Id).FirstOrDefault();
            user.RemoveAddress(address);
            _userService.UpdateUser(user);
            //now delete the address record
            _addressService.DeleteAddress(address);
            return Json(new { Success = true });
        }
        
        public ActionResult AddressCreate(long userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var model = new UserAddressModel();
            model.Address = new AddressModel();
            model.UserId = userId;
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });

            return View(model);
        }
        [HttpPost]
        public ActionResult AddressCreate(UserAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                user.Addresses.Add(address);
                _userService.UpdateUser(user);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Addresses.Added"));
                return RedirectToAction("Edit", new { Id = model.UserId });
                //return RedirectToAction("AddressEdit", new { addressId = address.Id, userId = model.UserId });
            }

            //If we got this far, something failed, redisplay form
            model.UserId = user.Id;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
            {
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            }
            return View(model);
        }

        public ActionResult AddressEdit(long addressId, long userId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = user.Id });

            var model = new UserAddressModel();
            model.UserId = userId;
            model.Address = address.ToModel();
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            return View(model);
        }
        [HttpPost]
        public ActionResult AddressEdit(UserAddressModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.UserId);
            if (user == null)
                //No user found with the specified id
                return RedirectToAction("List");

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                //No address found with the specified id
                return RedirectToAction("Edit", new { id = user.Id });

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                _addressService.UpdateAddress(address);

                NotifySuccess(_localizationService.GetResource("Admin.Users.Addresses.Updated"));
                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, userId = model.UserId });
            }

            //If we got this far, something failed, redisplay form
            model.UserId = user.Id;
            model.Address = address.ToModel();
            model.Address.CompanyEnabled = _addressSettings.CompanyEnabled;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CountryEnabled = _addressSettings.CountryEnabled;
            model.Address.CityEnabled = _addressSettings.CityEnabled;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.StreetAddressEnabled = _addressSettings.StreetAddressEnabled;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Enabled = _addressSettings.StreetAddress2Enabled;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeEnabled = _addressSettings.ZipPostalCodeEnabled;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneEnabled = _addressSettings.PhoneEnabled;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxEnabled = _addressSettings.FaxEnabled;
            model.Address.FaxRequired = _addressSettings.FaxRequired;
            //countries
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            return View(model);
        }
        #endregion Addresses

        #region Devices
        public ActionResult DevicesList(long userId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            var devices = _deviceService.GetAllDevicesByUser(user);
            if (devices == null)
                throw new ArgumentException("No devices found with the specified user", "userId");

            var devicesList = devices.Select(device => PrepareDeviceModelForList(user, device));

            return new JsonResult
            {
                Data = new GridModel<UserDeviceModel>() { Data = devicesList, Total = devicesList.Count() }
            };
        }

        public ActionResult AttachDetachDevice(long userId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(userId);
            var devices = _deviceService.GetAllDevices();
            //device list
            var listModel = new GridModel<UserDeviceModel>
            {
                Data = devices.Select(device => PrepareDeviceModelForList(user, device)),
                Total = devices.TotalCount
            };
            if (devices == null)
                return RedirectToAction("List");

            ViewBag.userId = userId;
            ViewBag.userFullName = user.GetFullName();
            //ViewBag.deviceType = device.DeviceType.Factory + " " + device.DeviceType.Model;
            return View(listModel);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachDetachDevice(long userId, long deviceId, bool userAttached, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(deviceId);
            var user = _userService.GetUserById(userId);
            var message = string.Empty;

            if (device == null || user == null)
                return RedirectToAction("AttachDetachDevice");

            if (!device.Users.Any(u => u.Id == userId) && userAttached)
            {
                device.Users.Add(user);
                _deviceService.UpdateDevice(device);
            }
            else if (device.Users.Any(u => u.Id == userId) && userAttached)
                message = _localizationService.GetResource("Admin.Users.DeviceAlreadyAttached");
            else if (device.Users.Any(u => u.Id == userId) && !userAttached)
            {
                device.Users.Remove(user);
                _deviceService.UpdateDevice(device);
            }
            else if (!device.Users.Any(u => u.Id == userId) && !userAttached)
                message = _localizationService.GetResource("Admin.Users.DeviceAlreadyDetached");

            return Json(new
            {
                Success = true,
                Message = message
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DetachDevice(long userId, long deviceId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var device = _deviceService.GetDeviceById(deviceId);
            if (device == null)
                throw new ArgumentException("No device found with the specified id", "deviceId");

            var user = device.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            device.RemoveUser(user);
            _deviceService.UpdateDevice(device);
            return Json(new { Success = true });
        }
        #endregion Devices
        #endregion Users

        #region Report
        [ChildActionOnly]
        public ActionResult ReportRegisteredUsers()
        {
            var model = GetReportRegisteredUsersModel();
            return PartialView(model);
        }

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
        protected string GetUserRolesNames(IList<UserRole> userRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < userRoles.Count; i++)
            {
                sb.Append(userRoles[i].Name);
                if (i != userRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
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
        [NonAction]
        protected string ValidateUserRoles(IList<UserRole> userRoles)
        {
            if (userRoles == null)
                throw new ArgumentNullException("userRoles");

            //ensure a user is not added to both 'Guests' and 'Registered' user roles
            //ensure that a user is in at least one required role ('Guests' and 'Registered')
            bool isInGuestsRole = userRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Guests) != null;
            bool isInRegisteredRole = userRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Registered) != null;
            if (isInGuestsRole && isInRegisteredRole)
            {
                //return "The user cannot be in both 'Guests' and 'Registered' user roles";
                return String.Format(_localizationService.GetResource("Admin.Users.MustBeUserOrGuest"),
                    _userService.GetUserRoleBySystemName(SystemUserRoleNames.Guests).Name,
                    _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Name);
            }
            if (!isInGuestsRole && !isInRegisteredRole)
            {
                //return "Add the user to 'Guests' or 'Registered' user role";
                return String.Format(_localizationService.GetResource("Admin.Users.CanOnlyBeUserOrGuest"),
                    _userService.GetUserRoleBySystemName(SystemUserRoleNames.Guests).Name,
                    _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Name);
            }
            //no errors
            return "";
        }
        [NonAction]
        protected UserModel PrepareUserModelForList(User user)
        {
            return new UserModel()
            {
                Id = user.Id,
                Email = !String.IsNullOrEmpty(user.Email) ? user.Email : (user.IsGuest() ? _localizationService.GetResource("Admin.Users.Guest") : "Unknown"),
                Username = user.Username,
                FullName = user.GetFullName(),
                Company = user.GetAttribute<string>(SystemUserAttributeNames.Company),
                Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone),
                ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode),
                UserRoleNames = GetUserRolesNames(user.UserRoles.ToList()),
                Active = user.Active,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc).Value,
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc.Value, DateTimeKind.Utc).Value,
            };
        }
        [NonAction]
        protected UserDeviceModel PrepareDeviceModelForList(User user, Device device)
        {
            var deviceAttached = device.Users.Any(u => u.Id == user.Id);
            var model = new UserDeviceModel()
            {
                Id = device.Id,
                Name = device.Name,
                IMEI = device.IMEI,
                DeviceType = device.DeviceType.Factory + ":" + device.DeviceType.Model,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc).Value,
                LastConnection = _dateTimeHelper.ConvertToUserTime(device.DevicePosition.DateOnUtc),
                UpdateOn = _dateTimeHelper.ConvertToUserTime(device.DeviceSetting.UpdateOnUtc),
                Active = device.Active,
                DeviceAttached = deviceAttached,
                AddButtonStyle = deviceAttached ? "display:none;" : "",
                RemoveButtonStyle = deviceAttached ? "" : "display:none;"
            };
            return model;
        }
        #endregion Utilities
    }
}