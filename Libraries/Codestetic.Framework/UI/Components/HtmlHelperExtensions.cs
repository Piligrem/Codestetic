using System.Web.Mvc;

namespace Codestetic.Web.Framework.UI
{
    public static class HtmlHelperExtensions
    {
        public static ComponentFactory TabSite(this HtmlHelper helper)
        {
            return new ComponentFactory(helper);
        }
    }
}
