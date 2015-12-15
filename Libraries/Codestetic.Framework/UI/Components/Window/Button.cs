using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestetic.Web.Framework.UI
{
    public enum EnumActions
    {
        Unknow,
        Close,
        Submit,
        Function
    }

    public class Button
    {
        public Button() { }

        public string Title { get; set; }
        public object Action { get; set; }
    }
}
