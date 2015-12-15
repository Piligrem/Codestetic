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
    using Specter.Web;
    using Specter.Web.Core.Events;
    using Specter.Web.Core.Infrastructure;
    using Specter.Web.Framework;
    using Specter.Web.Framework.Events;
    using Specter.Web.Framework.Mvc;
    using Specter.Web.Framework.UI;
    using Specter.Web.Framework.UI.Captcha;
    using Specter.Web.Models;
    using Specter.Web.Models.Common;
    using Specter.Web.Models.Device;
    using Specter.Web.Models.Dialogs;
    using Specter.Web.Models.GeoZones;
    using Specter.Web.Models.GPS;
    using Specter.Web.Models.Map;
    using Specter.Web.Models.Notify;
    using Specter.Web.Models.User;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Account/ExternalLoginConfirmation.cshtml")]
    public partial class _Views_Account_ExternalLoginConfirmation_cshtml : Specter.Web.Framework.ViewEngines.Razor.WebViewPage<Specter.Web.Models.ExternalLoginConfirmationViewModel>
    {
        public _Views_Account_ExternalLoginConfirmation_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 2 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
  
    ViewBag.Title = "Register";

            
            #line default
            #line hidden
WriteLiteral("\r\n<h2>");

            
            #line 5 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
Write(ViewBag.Title);

            
            #line default
            #line hidden
WriteLiteral(".</h2>\r\n<h3>Associate your ");

            
            #line 6 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
              Write(ViewBag.LoginProvider);

            
            #line default
            #line hidden
WriteLiteral(" account.</h3>\r\n\r\n");

            
            #line 8 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
 using (Html.BeginForm("ExternalLoginConfirmation", "Account", new { ReturnUrl = ViewBag.ReturnUrl }, FormMethod.Post, new { @class = "form-horizontal", role = "form" }))
{
    
            
            #line default
            #line hidden
            
            #line 10 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
Write(Html.AntiForgeryToken());

            
            #line default
            #line hidden
            
            #line 10 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
                            


            
            #line default
            #line hidden
WriteLiteral("    <h4>Association Form</h4>\r\n");

WriteLiteral("    <hr />\r\n");

            
            #line 14 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
    
            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
Write(Html.ValidationSummary(true, "", new { @class = "text-danger" }));

            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
                                                                     

            
            #line default
            #line hidden
WriteLiteral("    <p");

WriteLiteral(" class=\"text-info\"");

WriteLiteral(">\r\n        You\'ve successfully authenticated with <strong>");

            
            #line 16 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
                                                  Write(ViewBag.LoginProvider);

            
            #line default
            #line hidden
WriteLiteral("</strong>.\r\n            Please enter a user name for this site below and click th" +
"e Register button to finish\r\n            logging in.\r\n    </p>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 21 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
   Write(Html.LabelFor(m => m.Email, new { @class = "col-md-2 control-label" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"col-md-10\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 23 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
       Write(Html.TextBoxFor(m => m.Email, new { @class = "form-control" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 24 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
       Write(Html.ValidationMessageFor(m => m.Email, "", new { @class = "text-danger" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"form-group\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"col-md-offset-2 col-md-10\"");

WriteLiteral(">\r\n            <input");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"btn btn-default\"");

WriteLiteral(" value=\"Register\"");

WriteLiteral(" />\r\n        </div>\r\n    </div>\r\n");

            
            #line 32 "..\..\Views\Account\ExternalLoginConfirmation.cshtml"
}

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
