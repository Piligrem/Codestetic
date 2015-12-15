using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Localization;
using Codestetic.Web.Core.Infrastructure;
using Codestetic.Web.Framework.Localization;

namespace Codestetic.Web.Framework.Controllers
{
    public class SetWorkingCultureAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            // don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataSettings.DatabaseIsInstalled())
                return;

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var workingLanguage = workContext.WorkingLanguage;

            CultureInfo culture;
            if (workContext.CurrentUser != null && workingLanguage != null)
            {
                culture = new CultureInfo(workingLanguage.LanguageCulture);
            }
            else
            {
                culture = new CultureInfo("en-US");
            }
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
