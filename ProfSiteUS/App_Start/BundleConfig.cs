using System.Web;
using System.Web.Optimization;

namespace ProfSiteUS
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


            bundles.Add(
             new ScriptBundle("~/bundles/angular").Include(
                 "~/Scripts/angular.min.js",
                 "~/Scripts/json3.min.js",
                 "~/Scripts/es5-shim.min.js",
                 "~/Scripts/angular-resource.min.js",
                 "~/Scripts/angular-sanitize.min.js",
                 "~/Scripts/angular-animate.min.js",
                 "~/Scripts/angular-route.min.js"));

            bundles.Add(
              new ScriptBundle("~/bundles/charts").Include(
                  "~/Scripts/app.js",
                  "~/Scripts/directives/chart.js",
                  "~/Scripts/directives/mouse-capture.js",
                  "~/Scripts/services/chart.js",
                  "~/Scripts/services/dragging-service.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/detectizr.js",
                        "~/Scripts/modernizr*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/resetcss").Include(
              "~/Content/reset.css"));


            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                  "~/Content/bootstrap.css"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/main.css"));
        }
    }
}
