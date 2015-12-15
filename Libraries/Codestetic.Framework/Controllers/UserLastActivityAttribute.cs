using System;
using System.Web.Mvc;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Services.Users;

namespace Codestetic.Web.Framework.Controllers
{
    public class UserLastActivityAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!DataSettings.DatabaseIsInstalled())
                return;

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var user = workContext.CurrentUser;

            //update last activity date
            if (user.LastActivityDateUtc.Value.AddMinutes(1.0) < DateTime.UtcNow)
            {
                var userService = EngineContext.Current.Resolve<IUserService>();
                user.LastActivityDateUtc = DateTime.UtcNow;
                userService.UpdateUser(user);
            }
        }
    }
}
