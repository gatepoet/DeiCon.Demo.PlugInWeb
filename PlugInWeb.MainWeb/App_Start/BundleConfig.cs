using System.Web;
using System.Web.Optimization;

namespace PlugInWeb.MainWeb
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/foundation/css").Include(
           "~/Content/foundation/foundation.css",
           "~/Content/foundation/foundation.mvc.css",
           "~/Content/foundation/app.css"));

            bundles.Add(new ScriptBundle("~/bundles/foundation").Include(
                      "~/Scripts/jquery-1.7.1.js",
                      "~/Scripts/foundation/jquery.*",
                      "~/Scripts/foundation/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                      "~/Scripts/knockout-2.1.0.js",
                      "~/Scripts/knockout.mapping-latest.js"));

        }
    }
}