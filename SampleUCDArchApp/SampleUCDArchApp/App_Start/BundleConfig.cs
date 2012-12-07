﻿using System.Web;
using System.Web.Optimization;

namespace SampleUCDArchApp
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use Google CDN in production
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr")
                            .Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));

            // Note: Including bootstrap responsive-- comment it out if you don't need responsive css
            bundles.Add(new StyleBundle("~/Content/css")
                            .Include("~/Content/bootstrap.css")
                            .Include("~/Content/bootstrap-responsive.css")
                            .Include("~/Content/site.css"));
        }
    }
}