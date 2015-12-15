using System;
using System.Web;
using System.Web.Mvc;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Core.Domain.Common;

namespace Codestetic.Web.Framework.Controllers
{
    public class SiteClosedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            string controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

			if (!DataSettings.DatabaseIsInstalled())
                return;

            var siteInformationSettings = EngineContext.Current.Resolve<SiteInformationSettings>();
            if (!siteInformationSettings.SiteClosed)
                return;

            if (//ensure it's not the Login page
                !(controllerName.Equals("Specter.Web.Controllers.UserController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Login", StringComparison.InvariantCultureIgnoreCase)) &&
                //ensure it's not the Logout page
                !(controllerName.Equals("Specter.Web.Controllers.UserController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("Logout", StringComparison.InvariantCultureIgnoreCase)) &&
                //ensure it's not the store closed page
                !(controllerName.Equals("Specter.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("SiteClosed", StringComparison.InvariantCultureIgnoreCase)) &&
				//ensure it's not the change language page (request)
                !(controllerName.Equals("Specter.Web.Controllers.CommonController", StringComparison.InvariantCultureIgnoreCase) && actionName.Equals("SetLanguage", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (siteInformationSettings.SiteClosedAllowForAdmins && EngineContext.Current.Resolve<IWorkContext>().CurrentUser.IsAdmin())
                {
                    //do nothing - allow admin access
                }
                else
                {
                    var siteClosedUrl = new UrlHelper(filterContext.RequestContext).RouteUrl("SiteClosed");
                    filterContext.Result = new RedirectResult(siteClosedUrl);
                }
            }
        }
    }
}
