using System;
using System.Web;
using System.Web.Optimization;

namespace ProfSite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var t = new string['2', '2'];

            bundles.Add(new StyleBundle("~/content/fileupload")
                .Include("~/content/fileupload/jquery.fileupload-ui.css")
                );


            bundles.Add(
                new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/jquery.common.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr.js", "~/Scripts/detectizr.js"));

            bundles.Add(
                new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/bootstrap-timepicker.js",
                "~/Scripts/respond.js",
                "~/Scripts/tooltip.js"));

            bundles.Add(
                new ScriptBundle("~/bundles/jquery-plugins").Include(
                // "~/Scripts/jquery.socialcalendar.js",
                    "~/Scripts/twitter-text.js",
                    "~/Scripts/jquery-social-post-dialog.js",
                    "~/Scripts/jquery.social.js",
                    "~/Scripts/trendingtopics.js",
                // "~/Scripts/calendar.js",
                    "~/Scripts/read-more-modal.js"));

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

            bundles.Add(
                new StyleBundle("~/Content/css").Include(
                "~/Content/main.css",
                "~/Content/main2.css",
                "~/Content/modal_bullet_format.css"));

            bundles.Add(
                new StyleBundle("~/Content/bravectocss").Include(
                "~/Content/bravectohome.css"));

            bundles.Add(
                new StyleBundle("~/Content/bravopetscss").Include(
                "~/Content/bravectohome.css",
                "~/Content/bravopets.css"
                ));

            bundles.Add(
             new StyleBundle("~/Admin/Content/css").Include(
            "~/Content/admin.css"
             ));

            bundles.Add(
           new StyleBundle("~/bundles/admin").Include(
           "~/Scripts/tinymce.js","~/Scripts/jquery.admin.js"));

            bundles.Add(
                new ScriptBundle("~/bundles/calendar").Include(
                //"~/Scripts/calendar/bootstrap-datepicker.js",
                //"~/Scripts/calendar/bootstrap-timepicker.js",
                    "~/Scripts/calendar/carousel.js",
                    "~/Scripts/calendar/collapse.js",
                    "~/Scripts/calendar/jstz.js",
                    "~/Scripts/calendar/modal.js",
                    "~/Scripts/calendar/scrollspy.js",
                    "~/Scripts/calendar/tab.js",
                    //"~/Scripts/calendar/tooltip.js",
                    "~/Scripts/calendar/transition.js",
                    "~/Scripts/calendar/underscore.js",
                // "~/Scripts/calendar/site.js",
                    "~/Scripts/calendar.js",
                    "~/components/js/socialcalendar.js"
                    ));

            bundles.Add(
                new ScriptBundle("~/bundles/locales").Include(
                    "~/Scripts/locales/bootstrap-datepicker.de.js",
                    "~/Scripts/locales/bootstrap-datepicker.es.js",
                    "~/Scripts/locales/bootstrap-datepicker.fr.js",
                    "~/Scripts/locales/bootstrap-datepicker.it.js",
                    "~/Scripts/locales/bootstrap-datepicker.nl.js",
                    "~/Scripts/locales/de-DE.js",
                    "~/Scripts/locales/es-ES.js",
                    "~/Scripts/locales/fr-FR.js",
                    "~/Scripts/locales/it-IT.js",
                    "~/Scripts/locales/pt-PT.js",
                    "~/Scripts/locales/ru-RU.js"));


            bundles.Add(new ScriptBundle("~/bundles/fileupload")
              .Include("~/scripts/FileUpload/vendor/jquery.ui.widget.js")
              .Include("~/scripts/FileUpload/tmpl.js")
              .Include("~/scripts/FileUpload/load-image.js")
              .Include("~/scripts/FileUpload/canvas-to-blob.js")

              .Include("~/scripts/FileUpload/jquery.iframe-transport.js")
              .Include("~/scripts/FileUpload/jquery.fileupload.js")
              .Include("~/scripts/FileUpload/jquery.fileupload-fp.js")
              .Include("~/scripts/FileUpload/jquery.fileupload-ui.js")
                //.Include("~/scripts/FileUpload/locale.js")
                // .Include("~/scripts/FileUpload/main.js")
              );

        }
    }
}
/*
<script src="scripts/app.js"></script>
<script src="scripts/directives/mouse-capture.js"></script>
<script src="scripts/directives/chart.js"></script>
<script src="scripts/services/dragging-service.js"></script>
<script src="scripts/services/chart.js"></script>
*/