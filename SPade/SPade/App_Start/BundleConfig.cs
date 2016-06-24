using System.Web;
using System.Web.Optimization;

namespace SPade
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/fonts/font-awesome/css/font-awesome.min.css",
                      "~/Content/animate.min.css",
                      "~/Content/dashboardstyle.css",
                      "~/Content/daterangepicker.css",
                      "~/Content/indexstyle.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/script").Include(
                "~/Scripts/bootstrap.min.js",
                      "~/Script/custom-script.js",
                "~/Scripts/jquery-1.10.2.min.js",
                "~/Scripts/jquery.tablesorter.min.js",
                "~/Scripts/daterangepicker.js",
                "~/Scripts/wow.min.js"));
        }
    }
}
