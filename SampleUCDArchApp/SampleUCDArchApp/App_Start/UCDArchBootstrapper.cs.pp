using System.Web.Mvc;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using $rootnamespace$.App_Start;
using $rootnamespace$.Controllers;
using UCDArch.Data.NHibernate;
using UCDArch.Web.IoC;
using UCDArch.Web.ModelBinder;

[assembly: WebActivator.PreApplicationStartMethod(typeof(UCDArchBootstrapper), "PreStart")]
namespace $rootnamespace$.App_Start
{
    public class UCDArchBootstrapper
    {
        /// <summary>
        /// PreStart for the UCDArch Application configures the model binding, db, and IoC container
        /// </summary>
        public static void PreStart()
        {
            ModelBinders.Binders.DefaultBinder = new UCDArchModelBinder();

            NHibernateSessionConfiguration.Mappings.UseFluentMappings(typeof(Customer).Assembly);

            ConfigureAdditonalBundles();

            IWindsorContainer container = InitializeServiceLocator();
        }

        private static void ConfigureAdditonalBundles()
        {
            const string dataTablesVersion = "1.9.4";

            var bundles = BundleTable.Bundles;
            
            bundles.Add(new ScriptBundle("~/bundles/datatables")
                            .Include(string.Format("~/Scripts/DataTables-{0}/media/js/jquery.dataTables.js", dataTablesVersion))
                            .Include("~/Scripts/datatables-bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.js"));

            // Note: Including bootstrap responsive-- comment it out if you don't need responsive css
            bundles.Add(new StyleBundle("~/Content/css")
                            .Include("~/Content/bootstrap.css")
                            .Include("~/Content/bootstrap-custom.css")
                            .Include("~/Content/bootstrap-responsive.css")
                            .Include("~/Content/site.css"));

            // Note: Including datatables helper css for bootstrap (http://datatables.net/blog/Twitter_Bootstrap_2)
            bundles.Add(new StyleBundle(string.Format("~/Content/DataTables-{0}/media/css/dataTables", dataTablesVersion))
                            .Include(string.Format("~/Content/DataTables-{0}/media/css/jquery.dataTables.css", dataTablesVersion))
                            .Include(string.Format("~/Content/DataTables-{0}/media/css/datatables-bootstrap.css", dataTablesVersion)));
        }

        private static IWindsorContainer InitializeServiceLocator()
        {
            IWindsorContainer container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }
    }
}