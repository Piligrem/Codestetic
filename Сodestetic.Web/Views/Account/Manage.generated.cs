﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 2 "..\..\Views\Account\Manage.cshtml"
    using Microsoft.AspNet.Identity;
    
    #line default
    #line hidden
    using Specter.Web;
    using Specter.Web.Core.Events;
    using Specter.Web.Core.Infrastructure;
    using Specter.Web.Framework;
    using Specter.Web.Framework.Events;
    using Specter.Web.Framework.Mvc;
    using Specter.Web.Framework.UI;
    using Specter.Web.Framework.UI.Captcha;
    
    #line 1 "..\..\Views\Account\Manage.cshtml"
    using Specter.Web.Models;
    
    #line default
    #line hidden
    using Specter.Web.Models.Common;
    using Specter.Web.Models.Device;
    using Specter.Web.Models.Dialogs;
    using Specter.Web.Models.GeoZones;
    using Specter.Web.Models.GPS;
    using Specter.Web.Models.Map;
    using Specter.Web.Models.Notify;
    using Specter.Web.Models.User;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Account/Manage.cshtml")]
    public partial class _Views_Account_Manage_cshtml : Specter.Web.Framework.ViewEngines.Razor.WebViewPage<dynamic>
    {
        public _Views_Account_Manage_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\Account\Manage.cshtml"
  
    ViewBag.Title = "Manage Account";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("\r\n<h2>");

            
            #line 8 "..\..\Views\Account\Manage.cshtml"
Write(ViewBag.Title);

            
            #line default
            #line hidden
WriteLiteral("</h2>\r\n<p");

WriteLiteral(" class=\"text-success\"");

WriteLiteral(">");

            
            #line 9 "..\..\Views\Account\Manage.cshtml"
                   Write(ViewBag.StatusMessage);

            
            #line default
            #line hidden
WriteLiteral("</p>\r\n<div>\r\n    <div>\r\n");

            
            #line 12 "..\..\Views\Account\Manage.cshtml"
        
            
            #line default
            #line hidden
            
            #line 12 "..\..\Views\Account\Manage.cshtml"
         if (ViewBag.HasLocalPassword)
        {
            
            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Account\Manage.cshtml"
       Write(Html.Partial("_ChangePasswordPartial"));

            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Account\Manage.cshtml"
                                                   
        }
        else
        {
            
            
            #line default
            #line hidden
            
            #line 18 "..\..\Views\Account\Manage.cshtml"
       Write(Html.Partial("_SetPasswordPartial"));

            
            #line default
            #line hidden
            
            #line 18 "..\..\Views\Account\Manage.cshtml"
                                                
        }

            
            #line default
            #line hidden
WriteLiteral("\r\n        ");

WriteLiteral("\r\n    </div>\r\n</div>\r\n");

WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
