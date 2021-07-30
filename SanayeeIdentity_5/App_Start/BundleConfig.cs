//using System.Web;
//using System.Web.Optimization;

//namespace SanayeeIdentity_5
//{
//    public class BundleConfig
//    {
//        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
//        public static void RegisterBundles(BundleCollection bundles)
//        {
//            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
//                        "~/Scripts/jquery-{version}.js"));

//            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
//                        "~/Scripts/jquery.validate*"));

//            // Use the development version of Modernizr to develop with and learn from. Then, when you're
//            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
//            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
//                        "~/Scripts/modernizr-*"));

//            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
//                      "~/Scripts/bootstrap.js",
//                      "~/Scripts/respond.js"));

//            bundles.Add(new StyleBundle("~/Content/css").Include(
//                      "~/Content/bootstrap.css",
//                      "~/Content/site.css"));
//        }
//    }
//}

using System.Web;
using System.Web.Optimization;

namespace SanayeeIdentity_5
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Mohamed Fathallah Bundles for his Styles
            bundles.Add(new ScriptBundle("~/bundles/FetoScript").Include("~/Scripts/bxslider.js", "~/Scripts/jquery.appear.js"
                , "~/Scripts/jquery.fancybox.pack.js", "~/Scripts/script.js",
                "~/Scripts/validate.js", "~/Scripts/revolution.min.js", "~/Scripts/rev-slider.js", "~/Scripts/knob.js"
                , "~/Scripts/jquery.mixitup.min.js", "~/Scripts/jquery.js", "~/Scripts/bootstrap.min.js"
                       ));
            bundles.Add(new StyleBundle("~/Content/FetoCss").Include(
                     "~/Content/animate.css",
                     "~/Content/flaticon.css",
                     "~/Content/font-awesome.min.css",
                     "~/Content/jquery.fancybox.css",
                     "~/Content/responsive.css",
                     "~/Content/rev-settings.css",
                     "~/Content/style.css"));

            //End Of Bundles of Mohamed Fathallah

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-ui-1.12.1.js", "~/Scripts/jquery.unobtrusive-ajax"));

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
                      "~/Content/site.css"));
        }
    }
}

