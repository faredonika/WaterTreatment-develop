using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace WaterTreatment.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap-multiselect.css",
                "~/Content/bootstrap-datepicker.css",
                "~/Content/titatoggle-dist.css",
                "~/Content/font-awesome.css",
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/pdf")
                .Include("~/Content/font-awesome.css", "~/Content/ReportPDF.css")
            );

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/require.js",
                "~/Scripts/App/main.js"));

            bundles.Add(new StyleBundle("~/Content/jsTree/themes/default/bundle").Include(
                "~/Content/jsTree/themes/default/style.css", new CssRewriteUrlTransform()));
        }
    }
}