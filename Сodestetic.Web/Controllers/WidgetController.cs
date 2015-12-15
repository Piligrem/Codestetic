using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Framework.UI;

namespace Codestetic.Web.Controllers
{
    public partial class WidgetController : PublicControllerBase
    {
        private readonly IWidgetSelector _widgetSelector;

        public WidgetController(IWidgetSelector widgetSelector)
        {
            this._widgetSelector = widgetSelector;
        }

        [ChildActionOnly]
        public ActionResult WidgetsByZone(IEnumerable<WidgetRouteInfo> widgets)
        {
            return PartialView(widgets);
        }

    }
}
