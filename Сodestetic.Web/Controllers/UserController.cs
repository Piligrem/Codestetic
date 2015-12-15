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

namespace Codestetic.Web.Controllers
{
    public class UserController : PublicControllerBase
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

        private readonly ICurrencyService _currencyService;
        //private readonly IPriceFormatter _priceFormatter;
        //private readonly IOrderProcessingService _orderProcessingService;
        //private readonly IOrderService _orderService;

        //private readonly IOpenAuthenticationService _openAuthenticationService;

        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly MediaSettings _mediaSettings;
        #endregion Fields

        public UserController(IAuthenticationService authenticationService,
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
            IWorkflowMessageService workflowMessageService,
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

            //this._orderProcessingService = orderProcessingService;
            //this._orderTotalCalculationService = orderTotalCalculationService;
            //this._orderService = orderService;
            //this._priceFormatter = priceFormatter;
            
            this._currencyService = currencyService;
            
            this._captchaSettings = captchaSettings;

            this._workflowMessageService = workflowMessageService;
            this._mediaSettings = mediaSettings;
        }

        #region Login/Logout/Register
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Login(bool? checkoutAsGuest)
        {
            var model = new LoginModel();
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.CheckoutAsGuest = checkoutAsGuest.HasValue ? checkoutAsGuest.Value : false;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;

            if (_userSettings.PrefillLoginUsername.HasValue())
            {
                if (model.UsernamesEnabled)
                    model.Username = _userSettings.PrefillLoginUsername;
                else
                    model.Email = _userSettings.PrefillLoginUsername;

            }
            if (_userSettings.PrefillLoginPwd.HasValue())
            {
                model.Password = _userSettings.PrefillLoginPwd;
            }

            return View(model);
        }

        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [CaptchaValidator]
        public ActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {
                if (_userSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                if (_userRegistrationService.ValidateUser(_userSettings.UsernamesEnabled ? model.Username : model.Email, model.Password))
                {
                    var user = _userSettings.UsernamesEnabled ? _userService.GetUserByUsername(model.Username) : _userService.GetUserByEmail(model.Email);

                    //migrate shopping cart
                    //_shoppingCartService.MigrateShoppingCart(_workContext.CurrentUser, user);

                    //sign in new user
                    _authenticationService.SignIn(user, model.RememberMe);

                    //activity log
                    _userActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), user);

                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        if (user.IsAdmin() || user.IsSuperAdmin())
                            return RedirectToAction("", "Dashboard", new { area = "Admin" });

                    return RedirectToRoute("HomePage");
                }
                else
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                }
            }

            //If we got this far, something failed, redisplay form
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return View(model);
        }

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Register()
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new RegisterModel();
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultSiteTimeZone.Id) });
            //model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem() { Text = c.GetLocalized(x => x.Name), Value = c.Id.ToString() });
                }
            }

            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl, bool captchaValid)
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_workContext.CurrentUser.IsRegistered())
            {
                //Already registered user. 
                _authenticationService.SignOut();

                //Save a new record
                _workContext.CurrentUser = _userService.InsertGuestUser();
            }
            var user = _workContext.CurrentUser;

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            if (ModelState.IsValid)
            {
                if (_userSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                bool isApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new UserRegistrationRequest(user, model.Email,
                    _userSettings.UsernamesEnabled ? model.Username : model.Email, model.Password, _userSettings.DefaultPasswordFormat, 
                    model.FirstName, model.LastName, isApproved);
                var registrationResult = _userRegistrationService.RegisterUser(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    ////VAT number
                    //if (_taxSettings.EuVatEnabled)
                    //{
                    //    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.VatNumber, model.VatNumber);

                    //    string vatName = "";
                    //    string vatAddress = "";
                    //    var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                    //    _genericAttributeService.SaveAttribute(user,
                    //        SystemUserAttributeNames.VatNumberStatusId,
                    //        (int)vatNumberStatus);
                    //    //send VAT number admin notification
                    //    if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                    //        _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(user, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    //}

                    //form fields
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);

                    if (_userSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = null;
                        try
                        {
                            dateOfBirth = new DateTime(model.DateOfBirthYear.Value, model.DateOfBirthMonth.Value, model.DateOfBirthDay.Value);
                        }
                        catch { }
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, dateOfBirth);
                    }
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

                    //newsletter
                    if (_userSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(model.Email);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription()
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    Active = true,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    //login user now
                    if (isApproved)
                        _authenticationService.SignIn(user, true);

                    //associated with external account (if possible)
                    //TryAssociateAccountWithExternalAccount(user);

                    //insert default address (if possible)
                    var defaultAddress = new Address()
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Company = user.GetAttribute<string>(SystemUserAttributeNames.Company),
                        CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId) > 0 ?
                            (int?)user.GetAttribute<int>(SystemUserAttributeNames.CountryId) : null,
                        City = user.GetAttribute<string>(SystemUserAttributeNames.City),
                        Address1 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress),
                        Address2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2),
                        ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode),
                        PhoneNumber = user.GetAttribute<string>(SystemUserAttributeNames.Phone),
                        FaxNumber = user.GetAttribute<string>(SystemUserAttributeNames.Fax),
                        CreatedOnUtc = user.CreatedOnUtc
                    };
                    if (this._addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        //set default address
                        user.Addresses.Add(defaultAddress);
                        user.BillingAddress = defaultAddress;
                        _userService.UpdateUser(user);
                    }

                    //notifications
                    if (_userSettings.NotifyNewUserRegistration)
                        _workflowMessageService.SendUserRegisteredNotificationMessage(user, _localizationSettings.DefaultAdminLanguageId);

                    switch (_userSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                                _workflowMessageService.SendUserEmailValidationMessage(user, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send user welcome message
                                _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

                                var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                if (!String.IsNullOrEmpty(returnUrl))
                                    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("HomePage");
                            }
                    }
                }
                else
                {
                    foreach (var error in registrationResult.Errors)
                        ModelState.AddModelError("", error);
                }
            }

            //If we got this far, something failed, redisplay form
            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultSiteTimeZone.Id) });
            //model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem() { Text = c.GetLocalized(x => x.Name), Value = c.Id.ToString(), Selected = (c.Id == model.CountryId) });
                }
            }

            return View(model);
        }

        public ActionResult RegisterResult(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel()
            {
                Result = resultText
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.NotAvailable");

            if (_userSettings.UsernamesEnabled && username != null)
            {
                username = username.Trim();

                if (UsernameIsValid(username))
                {
                    if (_workContext.CurrentUser != null &&
                        _workContext.CurrentUser.Username != null &&
                        _workContext.CurrentUser.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                    {
                        statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.CurrentUsername");
                    }
                    else
                    {
                        var user = _userService.GetUserByUsername(username);
                        if (user == null)
                        {
                            statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.Available");
                            usernameAvailable = true;
                        }
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            //external authentication
            //ExternalAuthorizerHelper.RemoveParameters();

            if (_workContext.OriginalUserIfImpersonated != null)
            {
                //logout impersonated user
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalUserIfImpersonated,
                    SystemUserAttributeNames.ImpersonatedUserId, null);
                //redirect back to user details page (admin area)
                return this.RedirectToAction("Edit", "User", new { id = _workContext.CurrentUser.Id, area = "Admin" });
            }
            else
            {
                //standard logout 

                //activity log
                _userActivityService.InsertActivity("PublicStore.Logout", _localizationService.GetResource("ActivityLog.PublicStore.Logout"));

                _authenticationService.SignOut();
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult AccountActivation(string token, string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            var cToken = user.GetAttribute<string>(SystemUserAttributeNames.AccountActivationToken);
            if (String.IsNullOrEmpty(cToken))
                return RedirectToRoute("HomePage");

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            //activate user account
            user.Active = true;
            _userService.UpdateUser(user);
            _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AccountActivationToken, "");
            //send welcome message
            _workflowMessageService.SendUserWelcomeMessage(user, _workContext.WorkingLanguage.Id);

            var model = new AccountActivationModel();
            model.Result = _localizationService.GetResource("Account.AccountActivation.Activated");
            return View(model);
        }
        #endregion Login/Logout/Register

        #region Password recovery
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        public ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null && user.Active && !user.Deleted)
                {
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                    _workflowMessageService.SendUserPasswordRecoveryMessage(user, _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }


        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            var cPrt = user.GetAttribute<string>(SystemUserAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return RedirectToRoute("HomePage");

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            var model = new PasswordRecoveryConfirmModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        public ActionResult PasswordRecoveryConfirmPOST(string token, string email, PasswordRecoveryConfirmModel model)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
                return RedirectToRoute("HomePage");

            var cPrt = user.GetAttribute<string>(SystemUserAttributeNames.PasswordRecoveryToken);
            if (String.IsNullOrEmpty(cPrt))
                return RedirectToRoute("HomePage");

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            if (ModelState.IsValid)
            {
                var response = _userRegistrationService.ChangePassword(new ChangePasswordRequest(email,
                    false, _userSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.PasswordRecoveryToken, "");

                    model.SuccessfullyChanged = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion Password recovery

        #region MyAccount
        [Authorize]
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult MyAccount()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = GetUserNavigationModel(user);
            return View(model);
        }

        #region Info
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Info()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = new UserInfoModel();
            PrepareUserInfoModel(model, user, false);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Info(UserInfoModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            //email
            if (String.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("", "Email is not provided.");
            //username 
            if (_userSettings.UsernamesEnabled &&
                this._userSettings.AllowUsersToChangeUsernames)
            {
                if (String.IsNullOrEmpty(model.Username))
                    ModelState.AddModelError("", "Username is not provided.");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_userSettings.UsernamesEnabled &&
                        this._userSettings.AllowUsersToChangeUsernames)
                    {
                        if (!user.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _userRegistrationService.SetUsername(user, model.Username.Trim());
                            //re-authenticate
                            _authenticationService.SignIn(user, true);
                        }
                    }
                    //email
                    if (!user.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        _userRegistrationService.SetEmail(user, model.Email.Trim());
                        //re-authenticate (if user names are disabled)
                        if (!_userSettings.UsernamesEnabled)
                        {
                            _authenticationService.SignIn(user, true);
                        }
                    }
                    // FirstName and LastName
                    _userRegistrationService.SetFirstLastNames(user, model.FirstName, model.LastName);

                    //properties
                    if (_dateTimeSettings.AllowUsersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.TimeZoneId, model.TimeZoneId);
                    }
                    //VAT number
                    //if (_taxSettings.EuVatEnabled)
                    //{
                    //    var prevVatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);

                    //    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.VatNumber, model.VatNumber);
                    //    if (prevVatNumber != model.VatNumber)
                    //    {
                    //        string vatName = "";
                    //        string vatAddress = "";
                    //        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out vatName, out vatAddress);
                    //        _genericAttributeService.SaveAttribute(user,
                    //                SystemUserAttributeNames.VatNumberStatusId,
                    //                (int)vatNumberStatus);
                    //        //send VAT number admin notification
                    //        if (!String.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                    //            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(user, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    //    }
                    //}

                    //form fields
                    if (_userSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Gender, model.Gender);

                    if (_userSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = null;
                        try
                        {
                            dateOfBirth = new DateTime(model.DateOfBirthYear.Value, model.DateOfBirthMonth.Value, model.DateOfBirthDay.Value);
                        }
                        catch { }
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.DateOfBirth, dateOfBirth);
                    }
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

                    //newsletter
                    if (_userSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(user.Email);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            else
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription()
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = user.Email,
                                    Active = true,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.Signature, model.Signature);

                    return RedirectToRoute("UserInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }


            //If we got this far, something failed, redisplay form
            PrepareUserInfoModel(model, user, true);
            return View(model);
        }
        #endregion Info

        #region Addresses
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Addresses()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = new UserAddressListModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            foreach (var address in user.Addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModel(address, false, _addressSettings, _localizationService, () => _countryService.GetAllCountries());
                model.Addresses.Add(addressModel);
            }
            return View(model);
        }

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult AddressDelete(int addressId)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (address != null)
            {
                user.RemoveAddress(address);
                _userService.UpdateUser(user);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            return RedirectToRoute("UserAddresses");
        }

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult AddressAdd()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = new UserAddressEditModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            model.Address.PrepareModel(null, false, _addressSettings, _localizationService, () => _countryService.GetAllCountries());

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressAdd(UserAddressEditModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;


            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                user.Addresses.Add(address);
                _userService.UpdateUser(user);

                return RedirectToRoute("UserAddresses");
            }


            //If we got this far, something failed, redisplay form
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            model.Address.PrepareModel(null, true, _addressSettings, _localizationService, () => _countryService.GetAllCountries());

            return View(model);
        }

        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult AddressEdit(long addressId)
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;
            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (address == null)
                //address is not found
                return RedirectToRoute("UserAddresses");

            var model = new UserAddressEditModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            model.Address.PrepareModel(address, false, _addressSettings, _localizationService, () => _countryService.GetAllCountries());

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(UserAddressEditModel model, long addressId)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;
            //find address (ensure that it belongs to the current user)
            var address = user.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (address == null)
                //address is not found
                return RedirectToRoute("UserAddresses");

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                _addressService.UpdateAddress(address);

                return RedirectToRoute("UserAddresses");
            }

            //If we got this far, something failed, redisplay form
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Addresses;
            model.Address.PrepareModel(address, true, _addressSettings, _localizationService, () => _countryService.GetAllCountries());
            return View(model);
        }
        #endregion Addresses

        #region Orders
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Orders()
        {
            //if (!IsCurrentUserRegistered())
            //    return new HttpUnauthorizedResult();

            //var user = _workContext.CurrentUser;
            //var model = PrepareUserOrderListModel(user);
            //return View(model);
            return View("_NotSupported");
        }

        //[HttpPost, ActionName("Orders")]
        //[FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
        //public ActionResult CancelRecurringPayment(FormCollection form)
        //{
        //    if (!IsCurrentUserRegistered())
        //        return new HttpUnauthorizedResult();

        //    //get recurring payment identifier
        //    int recurringPaymentId = 0;
        //    foreach (var formValue in form.AllKeys)
        //        if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
        //            recurringPaymentId = Convert.ToInt32(formValue.Substring("cancelRecurringPayment".Length));

        //    var recurringPayment = _orderService.GetRecurringPaymentById(recurringPaymentId);
        //    if (recurringPayment == null)
        //    {
        //        return RedirectToRoute("UserOrders");
        //    }

        //    var user = _workContext.CurrentUser;
        //    if (_orderProcessingService.CanCancelRecurringPayment(user, recurringPayment))
        //    {
        //        var errors = _orderProcessingService.CancelRecurringPayment(recurringPayment);

        //        var model = PrepareUserOrderListModel(user);
        //        model.CancelRecurringPaymentErrors = errors;

        //        return View(model);
        //    }
        //    else
        //    {
        //        return RedirectToRoute("UserOrders");
        //    }
        //}
        #endregion Orders

        #region Change password
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult ChangePassword()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            var model = new ChangePasswordModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.ChangePassword;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var user = _workContext.CurrentUser;

            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.ChangePassword;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(user.Email,
                    true, _userSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _userRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model);
                }
                else
                {
                    foreach (var error in changePasswordResult.Errors)
                        ModelState.AddModelError("", error);
                }
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion Change password

        #region Avatar
        [RequireHttpsByConfigAttribute(SslRequirement.Yes)]
        public ActionResult Avatar()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            var model = new UserAvatarModel();
            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Avatar;
            model.AvatarUrl = _pictureService.GetPictureUrl(
                user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("upload-avatar")]
        public ActionResult UploadAvatar(UserAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Avatar;


            if (ModelState.IsValid)
            {
                try
                {
                    var userAvatar = _pictureService.GetPictureById(user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId));
                    if ((uploadedFile != null) && (!String.IsNullOrEmpty(uploadedFile.FileName)))
                    {
                        int avatarMaxSize = _userSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.ContentLength > avatarMaxSize)
                            throw new SpecterException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        byte[] userPictureBinary = uploadedFile.GetPictureBits();
                        if (userAvatar != null)
                            userAvatar = _pictureService.UpdatePicture(userAvatar.Id, userPictureBinary, uploadedFile.ContentType, null, true);
                        else
                            userAvatar = _pictureService.InsertPicture(userPictureBinary, uploadedFile.ContentType, null, true);
                    }

                    long userAvatarId = 0;
                    if (userAvatar != null)
                        userAvatarId = userAvatar.Id;

                    _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AvatarPictureId, userAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                        _mediaSettings.AvatarPictureSize,
                        false);
                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }


            //If we got this far, something failed, redisplay form
            model.AvatarUrl = _pictureService.GetPictureUrl(
                user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("remove-avatar")]
        public ActionResult RemoveAvatar(UserAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (!_userSettings.AllowUsersToUploadAvatars)
                return RedirectToRoute("UserInfo");

            var user = _workContext.CurrentUser;

            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Avatar;

            var userAvatar = _pictureService.GetPictureById(user.GetAttribute<int>(SystemUserAttributeNames.AvatarPictureId));
            if (userAvatar != null)
                _pictureService.DeletePicture(userAvatar);
            _genericAttributeService.SaveAttribute(user, SystemUserAttributeNames.AvatarPictureId, 0);

            return RedirectToRoute("UserAvatar");
        }
        #endregion Avatar
        #endregion MyAccount

        #region Utilities
        [NonAction]
        protected bool IsCurrentUserRegistered()
        {
            return _workContext.CurrentUser.IsRegistered();
        }
        [NonAction]
        protected UserNavigationModel GetUserNavigationModel(User user)
        {
            var model = new UserNavigationModel();
            model.HideAvatar = !_userSettings.AllowUsersToUploadAvatars;
            model.HideForumSubscriptions = !_forumSettings.ForumsEnabled || !_forumSettings.AllowUsersToManageSubscriptions;
            return model;
        }
        [NonAction]
        protected void PrepareUserInfoModel(UserInfoModel model, User user, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (user == null)
                throw new ArgumentNullException("user");

            model.AllowUsersToSetTimeZone = _dateTimeSettings.AllowUsersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            if (!excludeProperties)
            {
                model.VatNumber = user.GetAttribute<string>(SystemUserAttributeNames.VatNumber);
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Gender = user.GetAttribute<string>(SystemUserAttributeNames.Gender);
                var dateOfBirth = user.GetAttribute<DateTime?>(SystemUserAttributeNames.DateOfBirth);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
                model.Company = user.GetAttribute<string>(SystemUserAttributeNames.Company);
                model.StreetAddress = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress);
                model.StreetAddress2 = user.GetAttribute<string>(SystemUserAttributeNames.StreetAddress2);
                model.ZipPostalCode = user.GetAttribute<string>(SystemUserAttributeNames.ZipPostalCode);
                model.City = user.GetAttribute<string>(SystemUserAttributeNames.City);
                model.CountryId = user.GetAttribute<int>(SystemUserAttributeNames.CountryId);
                model.Phone = user.GetAttribute<string>(SystemUserAttributeNames.Phone);
                model.Fax = user.GetAttribute<string>(SystemUserAttributeNames.Fax);

                //newsletter
                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmail(user.Email);
                model.Newsletter = newsletter != null && newsletter.Active;

                model.Signature = user.GetAttribute<string>(SystemUserAttributeNames.Signature);

                model.Email = user.Email;
                model.Username = user.Username;
            }
            else
            {
                if (_userSettings.UsernamesEnabled && !_userSettings.AllowUsersToChangeUsernames)
                    model.Username = user.Username;
            }

            //countries
            if (_userSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem()
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }
            }
            //model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //model.VatNumberStatusNote = ((VatNumberStatus)user.GetAttribute<int>(SystemUserAttributeNames.VatNumberStatusId))
            //     .GetLocalizedEnum(_localizationService, _workContext);
            model.GenderEnabled = _userSettings.GenderEnabled;
            model.DateOfBirthEnabled = _userSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _userSettings.CompanyEnabled;
            model.CompanyRequired = _userSettings.CompanyRequired;
            model.StreetAddressEnabled = _userSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _userSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _userSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _userSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _userSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _userSettings.ZipPostalCodeRequired;
            model.CityEnabled = _userSettings.CityEnabled;
            model.CityRequired = _userSettings.CityRequired;
            model.CountryEnabled = _userSettings.CountryEnabled;
            model.PhoneEnabled = _userSettings.PhoneEnabled;
            model.PhoneRequired = _userSettings.PhoneRequired;
            model.FaxEnabled = _userSettings.FaxEnabled;
            model.FaxRequired = _userSettings.FaxRequired;
            model.NewsletterEnabled = _userSettings.NewsletterEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _userSettings.CheckUsernameAvailabilityEnabled;
            model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

            //external authentication
            //foreach (var ear in _openAuthenticationService.GetExternalIdentifiersFor(user))
            //{
            //    var authMethod = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(ear.ProviderSystemName);
            //    if (authMethod == null || !authMethod.IsMethodActive(_externalAuthenticationSettings))
            //        continue;

            //    model.AssociatedExternalAuthRecords.Add(new UserInfoModel.AssociatedExternalAuthModel()
            //    {
            //        Id = ear.Id,
            //        Email = ear.Email,
            //        ExternalIdentifier = ear.ExternalIdentifier,
            //        AuthMethodName = authMethod.GetLocalizedValue(_localizationService, "FriendlyName", _workContext.WorkingLanguage.Id)
            //    });
            //}

            model.NavigationModel = GetUserNavigationModel(user);
            model.NavigationModel.SelectedTab = UserNavigationEnum.Info;
        }
        //[NonAction]
        //protected UserOrderListModel PrepareUserOrderListModel(User user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException("user");

        //    var model = new UserOrderListModel();
        //    model.NavigationModel = GetUserNavigationModel(user);
        //    model.NavigationModel.SelectedTab = UserNavigationEnum.Orders;
        //    var orders = _orderService.SearchOrders(_siteContext.CurrentStore.Id, user.Id,
        //        null, null, null, null, null, null, null, null, 0, int.MaxValue);
        //    foreach (var order in orders)
        //    {
        //        var orderModel = new UserOrderListModel.OrderDetailsModel()
        //        {
        //            Id = order.Id,
        //            OrderNumber = order.GetOrderNumber(),
        //            CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
        //            OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
        //            IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order)
        //        };
        //        var orderTotalInUserCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
        //        orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInUserCurrency, true, order.UserCurrencyCode, false, _workContext.WorkingLanguage);

        //        model.Orders.Add(orderModel);
        //    }

        //    var recurringPayments = _orderService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
        //        user.Id, 0, null);
        //    foreach (var recurringPayment in recurringPayments)
        //    {
        //        var recurringPaymentModel = new UserOrderListModel.RecurringOrderModel()
        //        {
        //            Id = recurringPayment.Id,
        //            StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
        //            CycleInfo = string.Format("{0} {1}", recurringPayment.CycleLength, recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext)),
        //            NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
        //            TotalCycles = recurringPayment.TotalCycles,
        //            CyclesRemaining = recurringPayment.CyclesRemaining,
        //            InitialOrderId = recurringPayment.InitialOrder.Id,
        //            CanCancel = _orderProcessingService.CanCancelRecurringPayment(user, recurringPayment),
        //        };

        //        model.RecurringOrders.Add(recurringPaymentModel);
        //    }

        //    return model;
        //}
        [NonAction]
        protected bool UsernameIsValid(string username)
        {
            var result = true;

            if (String.IsNullOrEmpty(username))
            {
                return false;
            }

            // other validation 

            return result;
        }
        #endregion Utilities
    }
}