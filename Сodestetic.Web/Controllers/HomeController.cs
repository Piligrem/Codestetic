using Codestetic.Web.Core;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Configuration;
using Codestetic.Web.Core.Domain.GPS;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Framework.Users;
using Codestetic.Web.Models;
using Codestetic.Web.Services;
using Codestetic.Web.Services.Authentication;
using Codestetic.Web.Services.Configuration;
using Codestetic.Web.Services.Logging;
using Codestetic.Web.Services.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Codestetic.Web.Controllers
{
    public class HomeController : PublicControllerBase
    {
        #region Fields
        private readonly ISettingService _settings;
        private readonly IWebHelper _webHelper;
        private System.Diagnostics.Process process;
        private readonly IWorkContext workContext;
        #endregion Fields

        public HomeController(
            ISettingService settings,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            this._settings = settings;
            this._webHelper = webHelper;
            this.workContext = workContext;
        }

        public ActionResult Index()
        {
            var user = workContext.CurrentUser;

            var model = new LoginViewModel()
            {
                Username = user.GetFullName(),
            };
            return View(model);
        }

        [Authorize]
        public ActionResult Map()
        {
            #region For testing
            //var _authenticationService = EngineContext.Current.Resolve<IAuthenticationService>();
            //var _userRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            //var _userActivityService = EngineContext.Current.Resolve<IUserActivityService>();
            //var _userService = EngineContext.Current.Resolve<IUserService>();
            //var _userSettings = EngineContext.Current.Resolve<UserSettings>();
            //var username = "user1";
            //string email = null;
            //var password = "123qwe!@#";

            //if (_userRegistrationService.ValidateUser(_userSettings.UsernamesEnabled ? username : email, password))
            //{
            //    var user = _userSettings.UsernamesEnabled ? _userService.GetUserByUsername(username) : _userService.GetUserByEmail(email);

            //    //sign in new user
            //    _authenticationService.SignIn(user, false);

            //    //activity log
            //    // _userActivityService.InsertActivity("Login", _localizationService.GetResource("ActivityLog.Login"), user);
            //    _userActivityService.InsertActivity("Login", "Login", user);
            //}
            #endregion For testing

            ViewBag.Message = "Your application description page.";
            var result = new MapModel()
            {
                TimeoutRefreshPosition = _settings.GetSettingByKey<int>("Timeout.Refresh.Position") * 1000,
                ZonePositionEnable = _settings.GetSettingByKey<bool>("ZonePosition.Enable"),
                ZonePositionRadius = _settings.GetSettingByKey<int>("ZonePosition.Radius"),
                VisualizeOnlyValid = _settings.GetSettingByKey<bool>("Visualize.OnlyValid")
            };
            return View(result);
        }

        //[CustomAuthorizeAttribute]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        //[CustomAuthorizeAttribute]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Emul()
        {
            try
            {
                process = new System.Diagnostics.Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.EnableRaisingEvents = true;
                process.StartInfo.FileName = _webHelper.MapPath("~/utils/TeltonikaEmul.exe");
                process.StartInfo.Arguments = " config";

                process.Start();
                process.OutputDataReceived += (s, a) =>
                {
                    var output = a.Data;
                    Debug.WriteLine(output);
                };
                process.ErrorDataReceived += (s, a) =>
                {
                    var error = a.Data;
                    Debug.WriteLine(error);
                };
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                if (process != null)
                {
                    process.Dispose();
                }
            }

            return RedirectToAction("Index");
        }
    }
}