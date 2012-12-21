using System.Web.Optimization;
using $rootnamespace$.App_Start;

[assembly: WebActivator.PostApplicationStartMethod(typeof(BundleBootstrapper), "PostStart")]
namespace $rootnamespace$.App_Start
{
    /// <summary>
    /// Configures additional bundles, like bootstrap and datatables
    /// </summary>
    public class BundleBootstrapper
    {
        private static void PostStart()
        {
            const string dataTablesVersion = "1.9.4";

            var bundles = BundleTable.Bundles;

            bundles.Add(new ScriptBundle("~/bundles/datatables")
                            .Include(string.Format("~/Scripts/DataTables-{0}/media/js/jquery.dataTables.js", dataTablesVersion))
                            .Include("~/Scripts/datatables-bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js"));

            // Note: Including bootstrap responsive-- comment it out if you don't need responsive css
            bundles.Add(new StyleBundle("~/Content/styles")
                            .Include("~/Content/bootstrap.css")
                            .Include("~/Content/bootstrap-custom.css")
                            .Include("~/Content/bootstrap-responsive.css")
                            .Include("~/Content/custom.css"));

            // Note: Including datatables helper css for bootstrap (http://datatables.net/blog/Twitter_Bootstrap_2)
            bundles.Add(new StyleBundle(string.Format("~/Content/DataTables-{0}/media/css/dataTables", dataTablesVersion))
                            .Include(string.Format("~/Content/DataTables-{0}/media/css/jquery.dataTables.css", dataTablesVersion))
                            .Include(string.Format("~/Content/DataTables-{0}/media/css/datatables-bootstrap.css", dataTablesVersion)));
        }
    }
}