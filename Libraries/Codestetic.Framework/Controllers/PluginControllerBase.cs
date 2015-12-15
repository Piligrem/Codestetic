using System.Web.Mvc;
using System.Web.Routing;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Infrastructure;

namespace Codestetic.Web.Framework.Controllers
{
    [AdminAuthorize]
    public abstract partial class PluginControllerBase : SpecterController
    {
        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="requestContext">Request context</param>
        protected override void Initialize(RequestContext requestContext)
        {
            //set work context to admin mode
            EngineContext.Current.Resolve<IWorkContext>().IsAdmin = true;

            base.Initialize(requestContext);
        }

        /// <summary>
        /// Renders default access denied view as a partial
        /// </summary>
        protected ActionResult AccessDeniedPartialView()
        {
            return PartialView("~/Administration/Views/Security/AccessDenied.cshtml");
        }
    }
}
