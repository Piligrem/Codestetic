using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codestetic.Web.Framework.UI
{
    public interface IWidgetSelector
    {
        IEnumerable<WidgetRouteInfo> GetWidgets(string widgetZone);
    }
}
