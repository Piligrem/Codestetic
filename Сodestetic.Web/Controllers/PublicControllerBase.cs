using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Framework.Security;

namespace Codestetic.Web.Controllers
{

    [UserLastActivity]
    [SiteIpAddress]
    [SiteLastVisitedPage]
    [SiteClosedAttribute]
    [PublicSiteAllowNavigation]
    [LanguageSeoCodeAttribute]
    [RequireHttpsByConfigAttribute(SslRequirement.Retain)]
    public abstract partial class PublicControllerBase : SpecterController { }
}
