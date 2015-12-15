using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestetic.Web.Framework.UI
{
    public class VisualEffect
    {
        public VisualEffect()
        {
            this.Delay = 500;
        }
        public EnumEffect Effect { get; set; }
        public int Delay { get; set; }
    }
}
