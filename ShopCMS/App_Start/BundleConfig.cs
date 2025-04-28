using System.Web;
using System.Web.Optimization;

namespace ahmadi
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/Base/Scripts/jquery.unobtrusive*",
                        "~/Content/Base/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Content/Base/Scripts/jquery-ui-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Admin/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new StyleBundle("~/bundles/Admin/FileManager/Css").Include(
                  "~/Areas/Admin/Content/Stylesheets/Component/MyFileManager.css"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/Admin/FileManager/Js").Include(
                  "~/Areas/Admin/Content/Scripts/Component/MyFileManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/Admin/Scripts").Include(
                  "~/Areas/Admin/Content/Scripts/Base/jQuery-2.1.4.min.js",
                  "~/Areas/Admin/Content/Scripts/Base/bootstrap.min.js",
                   "~/Areas/Admin/Content/Scripts/Component/jquery.simplyCountable.js",
                   "~/Areas/Admin/Content/Scripts/Master.js"));

            bundles.Add(new ScriptBundle("~/bundles/TinyMCE/Js").Include(
            "~/Scripts/tinymce/tinymce.min.js",
            "~/Scripts/tinymce/tiny_mce_load.js"));

            bundles.Add(new ScriptBundle("~/bundles/TinyMCEFixUrl/Js").Include(
                 "~/Scripts/tinymce/tinymce.min.js",
                 "~/Scripts/tinymce/tiny_mce_load_fix_url.js"));

            bundles.Add(new ScriptBundle("~/bundles/TinyMCE/User/Js").Include(
              "~/Scripts/tinymce/tinymce.min.js",
              "~/Scripts/tinymce/tiny_mce_User_load.js"));



            var bootstrapJs = new ScriptBundle("~/bundles/Sitejs");
            bootstrapJs.Include("~/content/Base/scripts/jquery-1.12.4.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/popper.min.js");
            bootstrapJs.Include("~/content/Base/scripts/bootstrap.min.js");
            bootstrapJs.Include("~/content/Base/scripts/owl.carousel.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/magnific-popup.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/waypoints.min.js");
            bootstrapJs.Include("~/content/Base/scripts/parallax.js");
            //bootstrapJs.Include("~/content/Base/scripts/jquery.countdown.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/Hoverparallax.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/jquery.countTo.js");
            //bootstrapJs.Include("~/content/Base/scripts/imagesloaded.pkgd.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/isotope.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/jquery.appear.js");
            //bootstrapJs.Include("~/content/Base/scripts/jquery.parallax-scroll.js");
            //bootstrapJs.Include("~/content/Base/scripts/jquery.dd.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/slick.min.js");
            //bootstrapJs.Include("~/content/Base/scripts/jquery.elevatezoom.js");
            bootstrapJs.Include("~/content/Base/Scripts/sweetalert/sweetalert.min.js");
            bootstrapJs.Include("~/content/Base/Scripts/VideoPlayer/dist/mediaelement-and-player.min.js");
            bootstrapJs.Include("~/content/Base/Scripts/VideoPlayer/dist/jump-forward/jump-forward.js");
            bootstrapJs.Include("~/content/Base/Scripts/VideoPlayer/dist/skip-back/skip-back.js");
            bootstrapJs.Include("~/content/Base/Scripts/VideoPlayer/dist/speed/speed.min.js");
            bootstrapJs.Include("~/content/Base/Scripts/scroll/jquery.trackpad-scroll-emulator.min.js");
            bootstrapJs.Include("~/Content/Base/Scripts/jquery.lazyload.js");
            bootstrapJs.Include("~/content/Base/scripts/scripts.js");
            bootstrapJs.Include("~/content/Base/Scripts/jquery.unobtrusive-ajax.js");
            bootstrapJs.Include("~/content/Base/Scripts/jquery.validate.js");
            bootstrapJs.Include("~/content/Base/Scripts/jquery.validate.unobtrusive.js");
            bootstrapJs.Include("~/Content/Base/Scripts/View/_JoinNewsletter.js");
            bundles.Add(bootstrapJs);

            var bootstrapCSS = new StyleBundle("~/bundles/SiteCss");
            bootstrapCSS.Include("~/Content/Base/Scripts/sweetalert/sweetalert.css");
            bootstrapCSS.Include("~/Content/Base/Css/animate.css");
            bootstrapCSS.Include("~/Content/Base/Css/bootstrap.min.css");
            //bootstrapCSS.Include("~/Content/Base/Css/all.min.css");
           // bootstrapCSS.Include("~/Content/Base/Css/ionicons.min.css");
            //bootstrapCSS.Include("~/Content/Base/Css/themify-icons.css");
           //bootstrapCSS.Include("~/Content/Base/Css/linearicons.css");
            //bootstrapCSS.Include("~/Content/Base/Css/flaticon.css");
            //bootstrapCSS.Include("~/Content/Base/Css/simple-line-icons.css");
            //bootstrapCSS.Include("~/Content/Base/Css/font-awesome.css");
            bootstrapCSS.Include("~/Content/Base/Css/owl.carousel.min.css");
            bootstrapCSS.Include("~/Content/Base/Css/owl.theme.css");
            bootstrapCSS.Include("~/Content/Base/Css/owl.theme.default.min.css");
            //bootstrapCSS.Include("~/Content/Base/Css/magnific-popup.css");
            //bootstrapCSS.Include("~/Content/Base/Css/slick.css");
            //bootstrapCSS.Include("~/Content/Base/Css/slick-theme.css");
            //bootstrapCSS.Include("~/Content/Base/Css/style.css");
            bootstrapCSS.Include("~/Content/Base/Css/responsive.css");
            bootstrapCSS.Include("~/Content/Base/Scripts/scroll/trackpad-scroll-emulator.css");
            bootstrapCSS.Include("~/Content/Base/Scripts/VideoPlayer/dist/mediaelementplayer.css");
            bootstrapCSS.Include("~/Content/Base/Scripts/VideoPlayer/dist/jump-forward/jump-forward.css");
            bootstrapCSS.Include("~/Content/Base/Scripts/VideoPlayer/dist/skip-back/skip-back.css");
            bootstrapCSS.Include("~/Content/Base/Scripts/VideoPlayer/dist/speed/speed.css");
            bootstrapCSS.Include("~/Content/Default/Site.css");
            bootstrapCSS.Transforms.Add(new CssMinify());
            bundles.Add(bootstrapCSS);


            BundleTable.EnableOptimizations = true;

        }
    }
}
