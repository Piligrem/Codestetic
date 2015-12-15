using System.Web;
using System.Web.Optimization;

namespace Codestetic.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Core
            bundles.Add(new ScriptBundle("~/bundles/loaders")
                .Include("~/Scripts/specter.loader.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-2.1.1.js"));

            bundles.Add(new ScriptBundle("~/bundles/language")
                .Include("~/Scripts/specter.language.js"));

            bundles.Add(new ScriptBundle("~/bundles/specter.eventbroker")
                .Include("~/Scripts/specter.eventbroker.js")
                .Include("~/Scripts/underscore.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js"));
            #endregion Core

            #region By View
            #region ControlPanel
            bundles.Add(new ScriptBundle("~/bundles/specter.core")
                .Include("~/Scripts/specter.utils.js")
                .Include("~/Scripts/specter.ui.control.js")
                .Include("~/Scripts/bootstrap-modal.js"));

            bundles.Add(new ScriptBundle("~/bundles/specter.signalr")
                .Include("~/Scripts/jquery.signalR-{version}.js")
                .Include("~/Scripts/specter.signalr.hub.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment")
                .Include("~/Scripts/moment.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery.datetimepicker")
                .Include("~/Scripts/jquery.datetimepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/specter.controlpanel.notify.map")
                .Include("~/Scripts/specter.notify.js")
                .Include("~/Scripts/yandex.map.controls.js"));
            #endregion ControlPanel

            #region Leaflet map
            bundles.Add(new ScriptBundle("~/bundles/leaflet")
                .Include("~/Scripts/leaflet.js")
                .Include("~/Scripts/plugins/leaflet/leaflet.extension.js")
                .Include("~/Scripts/layers/bing.js")
                .Include("~/Scripts/layers/google.js")
                .Include("~/Scripts/layers/yandex.js")
                .Include("~/Scripts/layers/dgis.js")
                /*.Include("~/Scripts/plugins/leaflet/leaflet.draw.js")*/);

            bundles.Add(new ScriptBundle("~/bundles/specter.controls")
                .Include("~/Scripts/specter.panels.js")
                .Include("~/Scripts/specter.forms.js")
                .Include("~/Scripts/specter.simple.control.js")
                .Include("~/Scripts/plugins/string.extensions.js"));

            bundles.Add(new ScriptBundle("~/bundles/leaflet.core")
                .Include("~/Scripts/specter.utils.js")
                .Include("~/Scripts/leaflet.ui.control.js")
                .Include("~/Scripts/bootstrap-modal.js"));

            bundles.Add(new ScriptBundle("~/bundles/leaflet.controlpanel.notify.map")
                .Include("~/Scripts/specter.notify.js")
                .Include("~/Scripts/leaflet.map.controls.js"));
            #endregion Leaflet map

            bundles.Add(new ScriptBundle("~/bundles/specter.geozone")
                .Include("~/Scripts/spectrum-1.5.1.js")
                .Include("~/Scripts/specter.geozone.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/specter.settings")
                .Include("~/Scripts/spectrum-1.5.1.js"));
            #endregion By View

            #region jQuery plugins
            bundles.Add(new ScriptBundle("~/bundles/jquery-ui")
                .Include("~/Scripts/jquery-ui-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-nicescroll")
                .Include("~/Scripts/jquery.nicescroll.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-notify")
                .Include("~/Scripts/jquery.pnotify.js"));
            #endregion jQuery plugins

            bundles.Add(new StyleBundle("~/Content/css")
                .Include(
                      //"~/Content/bootstrap.css",
                      "~/Content/site.css")); //,
                      //"~/Content/jquery-ui.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            // ??? Temporary for debug
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
