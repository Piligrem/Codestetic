using System.Collections.Generic;

namespace Codestetic.Web.Framework.UI
{
    public interface IHtmlAttributesContainer
    {
        IDictionary<string, object> HtmlAttributes { get; }
    }
}
