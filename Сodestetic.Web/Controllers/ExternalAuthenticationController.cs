using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Codestetic.Web.Services.Authentication.External;
using Codestetic.Web.Models.User;
using Codestetic.Web.Framework.Controllers;
using Codestetic.Web.Core;

namespace Codestetic.Web.Controllers
{

    public partial class ExternalAuthenticationController : PublicControllerBase
    {
		#region Fields
        private readonly IOpenAuthenticationService _openAuthenticationService;
        #endregion Fields

        #region Constructors

        public ExternalAuthenticationController(IOpenAuthenticationService openAuthenticationService)
        {
            this._openAuthenticationService = openAuthenticationService;
        }
        #endregion Constructors

        #region Methods
        public RedirectResult RemoveParameterAssociation(string returnUrl)
        {
            ExternalAuthorizerHelper.RemoveParameters();
            return Redirect(returnUrl);
        }

        [ChildActionOnly]
        public ActionResult ExternalMethods()
        {
            //model
            var model = new List<ExternalAuthenticationMethodModel>();

			foreach (var eam in _openAuthenticationService.LoadActiveExternalAuthenticationMethods())
            {
                var eamModel = new ExternalAuthenticationMethodModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                eam.GetPublicInfoRoute(out actionName, out controllerName, out routeValues);
                eamModel.ActionName = actionName;
                eamModel.ControllerName = controllerName;
                eamModel.RouteValues = routeValues;

                model.Add(eamModel);
            }

            return PartialView(model);
        }
        #endregion Methods
    }
}
