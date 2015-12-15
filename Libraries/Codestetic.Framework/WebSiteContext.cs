using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codestetic.Web.Core.Domain.Common;
using Codestetic.Web.Core;

namespace Codestetic.Web.Framework
{
    public partial class WebSiteContext : ISiteContext
    {
        private readonly IWebHelper _webHelper;
        private readonly SiteSettings _siteSettings;

        private SiteSettings _cachedSite;

        public WebSiteContext(IWebHelper webHelper, SiteSettings siteSettings)
        {
            this._siteSettings = siteSettings;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Gets or sets the current site
        /// </summary>
        public SiteSettings CurrentSite
        {
            get
            {
                if (_cachedSite != null)
                    return _cachedSite;

                //ty to determine the current site by HTTP_HOST
                var host = _webHelper.ServerVariables("HTTP_HOST");

                if (_siteSettings == null)
                    throw new Exception("No site could be loaded");

                _cachedSite = _siteSettings;
                return _cachedSite;
            }
        }
    }
}
