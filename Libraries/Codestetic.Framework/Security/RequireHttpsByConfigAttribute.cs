using System;
using System.Web.Mvc;
using Codestetic.Web.Core;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Security;
using Codestetic.Web.Core.Infrastructure;

namespace Codestetic.Web.Framework.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsByConfigAttribute : FilterAttribute, IAuthorizationFilter
    {
        public RequireHttpsByConfigAttribute(SslRequirement sslRequirement)
        {
            this.SslRequirement = sslRequirement;
        }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            // don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;
            
            // only redirect for GET requests, 
            // otherwise the browser might not propagate the verb and request body correctly.
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

			if (!DataSettings.DatabaseIsInstalled())
                return;

            var currentConnectionSecured = filterContext.HttpContext.Request.IsSecureConnection();

            var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
            if (securitySettings.ForceSslForAllPages)
            {
                // all pages are forced to be SSL no matter of the specified value
                this.SslRequirement = SslRequirement.Yes;
            }
            else
            {
                this.SslRequirement = SslRequirement.No;
            }

            switch (this.SslRequirement)
            {
                //case SslRequirement.Yes:
                //    {
                //        if (!currentConnectionSecured)
                //        {
                //            var storeContext = EngineContext.Current.Resolve<ISiteContext>();
                //            var currentSite = storeContext.CurrentSite;

                //            if (currentSite != null && currentSite.GetSecurityMode() > HttpSecurityMode.Unsecured)
                //            {
                //                // redirect to HTTPS version of page
                //                // string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                //                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                //                string url = webHelper.GetThisPageUrl(true, true);
                //                filterContext.Result = new RedirectResult(url);
                //            }
                //        }
                //    }
                //    break;
                case SslRequirement.No:
                    {
                        if (currentConnectionSecured)
                        {
                            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

                            // redirect to HTTP version of page
                            // string url = "http://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                            string url = webHelper.GetThisPageUrl(true, false);
                            filterContext.Result = new RedirectResult(url);
                        }
                    }
                    break;
                case SslRequirement.Retain:
                    {
                        //do nothing
                    }
                    break;
                default:
                    throw new SpecterException("Not supported SslProtected parameter");
            }
        }

        public SslRequirement SslRequirement { get; set; }
    }
}
