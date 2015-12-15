using System.Web.Routing;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Models.User
{
    public partial class ExternalAuthenticationMethodModel : ModelBase
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}