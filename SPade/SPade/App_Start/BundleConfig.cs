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

            bundles.Add(new StyleBundle("~/bundles/style").Include(
                      "~/Content/bootstrap.css",
                      "~/fonts/font-awesome/css/font-awesome.min.css",
                      "~/Content/animate.min.css",
                      "~/Content/dashboardstyle.css",
                      "~/Content/daterangepicker.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/indexscript").Include(
                      "~/Scripts/wow.min.js",
                      "~/Scripts/indexscript.js"));

            bundles.Add(new StyleBundle("~/bundles/indexstyle").Include(
                      "~/Content/indexstyle.css"));

            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/addassignmentscripts").Include(
                        "~/Scripts/moment.min.js",
                        "~/Scripts/daterangepicker.js",
                        "~/Scripts/addassignmentscript.js",
                        "~/Scripts/jquery.validate.date.js"));

            bundles.Add(new ScriptBundle("~/bundles/tablescript").Include(
                    "~/Scripts/jquery.tablesorter.combined.js",
                    "~/Scripts/initializetablesorter.js"));

            bundles.Add(new ScriptBundle("~/bundles/tablestyle").Include(
                      "~/Content/theme.bootstrap.css" //css for tablesorter
            ));

        }
    }
}
