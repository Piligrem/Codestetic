using System.Web.Mvc;
using System.Web.Routing;
using Codestetic.Web.Framework.Localization;
using Codestetic.Web.Framework.Mvc.Routes;

namespace Codestetic.Web.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            #region Home
            routes.MapLocalizedRoute("HomePage",
                "",
                new { controller = "Home", action = "Index" },
                new[] { "Specter.Web.Controllers" });

            routes.MapLocalizedRoute("AboutPage",
                "about",
                new { controller = "Home", action = "About" },
                new[] { "Specter.Web.Controllers" });

            routes.MapLocalizedRoute("ContactPage",
                "contact",
                new { controller = "Home", action = "Contact" },
                new[] { "Specter.Web.Controllers" });
            #endregion Home

            #region Map
            routes.MapLocalizedRoute("Map",
                "map",
                new { controller = "Home", action = "Map" },
                new[] { "Specter.Web.Controllers" });
            #endregion Map

            #region Login/Register
            routes.MapLocalizedRoute("Login",
                "login",
                new { controller = "User", action = "Login" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("LoginCheckoutAsGuest",
                "login/checkoutasguest",
                new { controller = "User", action = "Login", checkoutAsGuest = true },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("Register",
                "register/{id}",
                new { controller = "User", action = "Register", area = "", id = UrlParameter.Optional },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("Logout",
                "logout",
                new { controller = "User", action = "Logout" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("RegisterResult",
                "registerresult/{resultId}",
                new { controller = "User", action = "RegisterResult" },
                new { resultId = @"\d+" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("CheckUsernameAvailability",
                "user/checkusernameavailability",
                new { controller = "User", action = "CheckUsernameAvailability" },
                new[] { "Specter.Web.Controllers" });
            #endregion Login/Register

            #region Site closed
            routes.MapLocalizedRoute("SiteClosed",
                "storeclosed",
                new { controller = "Common", action = "SiteClosed" },
                new[] { "Specter.Web.Controllers" });
            #endregion Site closed

            #region Password recovery
            routes.MapLocalizedRoute("PasswordRecovery",
                "passwordrecovery",
                new { controller = "User", action = "PasswordRecovery" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("PasswordRecoveryConfirm",
                "passwordrecovery/confirm",
                new { controller = "User", action = "PasswordRecoveryConfirm" },
                new[] { "Specter.Web.Controllers" });
            #endregion Password recovery

            #region User
            routes.MapLocalizedRoute("MyAccount",
                "myaccount",
                new { controller = "User", action = "Info" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("UserInfo",
                "userInfo",
                new { controller = "User", action = "Info" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("UserAddresses",
                "addresses",
                new { controller = "User", action = "Addresses" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("AccountActivation",
                "user/activation",
                new { controller = "User", action = "AccountActivation" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("UserProfile",
                "profile/{id}",
                new { controller = "Profile", action = "Index" },
                new { id = @"\d+" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("UserProfilePaged",
                "profile/{id}/page/{page}",
                new { controller = "Profile", action = "Index" },
                new { id = @"\d+", page = @"\d+" },
                new[] { "Specter.Web.Controllers" });
            #endregion User

            #region Order
            routes.MapLocalizedRoute("UserOrders",
                "orders",
                new { controller = "User", action = "Orders" },
                new[] { "Specter.Web.Controllers" });
            #endregion Order

            routes.MapLocalizedRoute("UserRewardPoints",
                "user/rewardpoints",
                new { controller = "User", action = "RewardPoints" },
                new[] { "Specter.Web.Controllers" });

            #region Password
            routes.MapLocalizedRoute("UserChangePassword",
                "changepassword",
                new { controller = "User", action = "ChangePassword" },
                new[] { "Specter.Web.Controllers" });
            #endregion Password

            //#region Avatar
            //routes.MapLocalizedRoute("UserAvatar",
            //    "user/avatar",
            //    new { controller = "User", action = "Avatar" },
            //    new[] { "Specter.Web.Controllers" });
            //#endregion Avatar

            #region Forum
            //routes.MapLocalizedRoute("UserForumSubscriptions",
            //                "user/forumsubscriptions",
            //                new { controller = "User", action = "ForumSubscriptions" },
            //                new[] { "Specter.Web.Controllers" });
            //routes.MapLocalizedRoute("UserForumSubscriptionsPaged",
            //                "user/forumsubscriptions/{page}",
            //                new { controller = "User", action = "ForumSubscriptions", page = UrlParameter.Optional },
            //                new { page = @"\d+" },
            //                new[] { "Specter.Web.Controllers" });
            //routes.MapLocalizedRoute("DeleteForumSubscription",
            //                "user/forumsubscriptions/delete/{subscriptionId}",
            //                new { controller = "User", action = "DeleteForumSubscription" },
            //                new { subscriptionId = @"\d+" },
            //                new[] { "Specter.Web.Controllers" });
            #endregion Forum

            #region Addresses
            routes.MapLocalizedRoute("UserAddressDelete",
                "addressdelete/{addressId}",
                new { controller = "User", action = "AddressDelete", addressId = UrlParameter.Optional },
                new { addressId = @"\d+" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("UserAddressEdit",
                "addressedit/{addressId}",
                new { controller = "User", action = "AddressEdit", addressId = UrlParameter.Optional },
                new { addressId = @"\d+" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("UserAddressAdd",
                "addressadd",
                new { controller = "User", action = "AddressAdd" },
                new[] { "Specter.Web.Controllers" });
            #endregion Addresses

            #region Device
            routes.MapLocalizedRoute("DevicesInfo",
                "devices/info",
                new { controller = "Device", action = "Info" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("AddDevice",
                "device/add",
                new { controller = "Device", action = "AddDevice" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("DeleteDevice",
                "device/delete",
                new { controller = "Device", action = "Delete" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("SubscriptionPayment",
                "devices/payment",
                new { controller = "Device", action = "SubscriptionPayment" },
                new[] { "Specter.Web.Controllers" });
            routes.MapLocalizedRoute("ReportDevice",
                "device/report",
                new { controller = "Device", action = "ReportDevice" },
                new[] { "Specter.Web.Controllers" });
            #endregion Device

            #region Page not found
            routes.MapLocalizedRoute("PageNotFound",
                "404",
                new { controller = "Common", action = "PageNotFound" },
                new[] { "Specter.Web.Controllers" });
            #endregion Page not found
        }

        public int Priority { get { return 0; } }
    }
}