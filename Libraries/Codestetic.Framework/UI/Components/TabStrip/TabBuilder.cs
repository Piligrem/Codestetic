using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;
using Codestetic.Web.Utilities;

namespace Codestetic.Web.Framework.UI
{

    public class TabBuilder : NavigationItemtWithContentBuilder<Tab, TabBuilder>
    {

        public TabBuilder(Tab item)
            : base(item)
        {
        }

        public TabBuilder Name(string value)
        {
            this.Item.Name = value;
            return this;
        }

        public TabBuilder Pull(TabPull value)
        {
            this.Item.Pull = value;
            return this;
        }
    }
}
