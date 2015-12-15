using System.Web;
using System.Web.Mvc;
using Codestetic.Web.Framework.Users;
using Codestetic.Web.Framework.Controllers;

namespace Codestetic.Web
{
    [SiteIpAddress]
    [SiteLastVisitedPageAttribute]
    public class BaseController : SpecterController
    {
        //protected virtual new CustomPrincipal User
        //{
        //    get { return HttpContext.User as CustomPrincipal; }
        //}
    }
}