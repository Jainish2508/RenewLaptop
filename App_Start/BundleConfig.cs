using System.Web.Optimization;

namespace Renew_Laptop
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));



            bundles.Add(new StyleBundle("~/css/maincss").Include(
                    "~/Content/css/main.min.css",
                    "~/Content/revolution/css/settings.css",        //revolution slider css
                    "~/Content/revolution/css/layers.css",
                    "~/Content/revolution/css/navigation.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/mainjs").Include(
                    //"~/Content/js/jquery.min.js",
                    "~/Content/js/bootstrap.min.js",
                    "~/Content/js/menumaker.js",                                //menu js
                    "~/Content/js/wow.js",                                      //wow animation
                    "~/Content/js/custom.js",                                   //custom js
                    "~/Content/revolution/js/jquery.themepunch.tools.min.js",   //revolution js files
                    "~/Content/revolution/js/jquery.themepunch.revolution.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.actions.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.carousel.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.kenburn.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.layeranimation.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.migration.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.navigation.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.parallax.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.slideanims.min.js",
                    "~/Content/revolution/js/extensions/revolution.extension.video.min.js"
                ));
            BundleTable.EnableOptimizations = true;
            bundles.IgnoreList.Clear();
        }
    }
}