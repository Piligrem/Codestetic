using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Codestetic.Web.Framework.Localization;
using Codestetic.Web.Framework.Mvc.Routes;

namespace Codestetic.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes, bool databaseInstalled)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute(".db/{*virtualpath}");

            if (databaseInstalled)
            {
                // register custom routes (plugins, etc)
                var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
                routePublisher.RegisterRoutes(routes);
            }

            #region Login/Register
            //routes.MapRoute("Login",
            //                "user/login",
            //                new { controller = "User", action = "Login" },
            //                new[] { "Specter.Web.Controllers" });
            //routes.MapRoute("LoginCheckoutAsGuest",
            //                "login/checkoutasguest",
            //                new { controller = "User", action = "Login", checkoutAsGuest = true },
            //                new[] { "Specter.Web.Controllers" });
            //routes.MapRoute("Register",
            //                "register/{id}",
            //                new { controller = "User", action = "Register", area = "", id = UrlParameter.Optional },
            //                new[] { "Specter.Web.Controllers" });
            //routes.MapRoute("Logout",
            //                "user/logout",
            //                new { controller = "User", action = "Logout" },
            //                new[] { "Specter.Web.Controllers" });
            //routes.MapRoute("RegisterResult",
            //                "registerresult/{resultId}",
            //                new { controller = "User", action = "RegisterResult" },
            //                new { resultId = @"\d+" },
            //                new[] { "Specter.Web.Controllers" });
            //routes.MapRoute("CheckUsernameAvailability",
            //                "user/checkusernameavailability",
            //                new { controller = "User", action = "CheckUsernameAvailability" },
            //                new[] { "Specter.Web.Controllers" });
            //#endregion Login/Register
            //#region Password recovery
            //routes.MapRoute("PasswordRecovery",
            //    "passwordrecovery",
            //    new { controller = "User", action = "PasswordRecovery" },
            //    new[] { "Specter.Web.Controllers" });
            //routes.MapRoute("PasswordRecoveryConfirm",
            //    "passwordrecovery/confirm",
            //    new { controller = "User", action = "PasswordRecoveryConfirm" },
            //    new[] { "Specter.Web.Controllers" });
            //#endregion Password recovery
            //#region User
            //routes.MapLocalizedRoute("MyAccount",
            //    "user/info",
            //    new { controller = "User", action = "Info" },
            //    new[] { "Specter.Web.Controllers" });
            //routes.MapLocalizedRoute("UserInfo",
            //    "UserInfo",
            //    new { controller = "User", action = "Info" },
            //    new[] { "Specter.Web.Controllers" });
            //routes.MapLocalizedRoute("UserAddresses",
            //    "User/Addresses",
            //    new { controller = "User", action = "Addresses" },
            //    new[] { "Specter.Web.Controllers" });
            //routes.MapLocalizedRoute("AccountActivation",
            //    "user/activation",
            //    new { controller = "User", action = "AccountActivation" },
            //    new[] { "Specter.Web.Controllers" });
            //routes.MapLocalizedRoute("UserProfile",
            //    "profile/{id}",
            //    new { controller = "Profile", action = "Index" },
            //    new { id = @"\d+" },
            //    new[] { "Specter.Web.Controllers" });
            //routes.MapLocalizedRoute("UserProfilePaged",
            //    "profile/{id}/page/{page}",
            //    new { controller = "Profile", action = "Index" },
            //    new { id = @"\d+", page = @"\d+" },
            //    new[] { "Specter.Web.Controllers" });
            //#endregion User
            //#region Order
            //routes.MapLocalizedRoute("UserOrders",
            //    "user/orders",
            //    new { controller = "User", action = "Orders" },
            //    new[] { "Specter.Web.Controllers" });
            //#endregion Order
            //#region Password
            //routes.MapLocalizedRoute("UserChangePassword",
            //    "user/changepassword",
            //    new { controller = "User", action = "ChangePassword" },
            //    new[] { "Specter.Web.Controllers" });
            //#endregion Password
            //#region Addresses
            //routes.MapRoute("UserAddressDelete",
            //    "AddressDelete/{id}",
            //    new { controller = "User", action = "AddressDelete", id = UrlParameter.Optional },
            //    //new { addressId = @"\d+" },
            //    new[] { "Specter.Web.Controllers" });
            ////routes.MapLocalizedRoute("UserAddressEdit",
            ////    "AddressEdit/{addressId}",
            ////    new { controller = "User", action = "AddressEdit", addressId = UrlParameter.Optional },
            ////    new { addressId = @"\d+" },
            ////    new[] { "Specter.Web.Controllers" });
            //routes.MapRoute("UserAddressAdd",
            //    "AddressAdd",
            //    new { controller = "User", action = "AddressAdd" },
            //    new[] { "Specter.Web.Controllers" });
            //#endregion Addresses
            //#region Page not found
            //routes.MapLocalizedRoute("PageNotFound",
            //    "404",
            //    new { controller = "Common", action = "PageNotFound" },
            //    new[] { "Specter.Web.Controllers" });
            #endregion Page not found

            routes.MapRoute(
                name: "Default",    // Route name
                url: "{controller}/{action}/{id}",  // URL with parameters
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Specter.Web.Controllers" }     // Namespace
            );

            routes.MapLocalizedRoute("MapView",
                "Map/Map",
                new { controller = "Map", action = "Map" },
                new[] { "Specter.Web.Controllers" });
        }
    }
}
