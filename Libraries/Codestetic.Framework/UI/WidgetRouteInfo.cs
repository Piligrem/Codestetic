using System.Web.Routing;
using Codestetic.Web.Framework.Mvc;

namespace Codestetic.Web.Framework.UI
{
    public partial class WidgetRouteInfo : ModelBase
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}