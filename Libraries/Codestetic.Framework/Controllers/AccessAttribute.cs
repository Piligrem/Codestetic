using System.Web.Mvc;

namespace Codestetic.Web.Framework.Controllers
{
    public class AccessAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}
