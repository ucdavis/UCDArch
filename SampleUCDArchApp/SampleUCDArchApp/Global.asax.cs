using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using MvcMiniProfiler;
using SampleUCDArchApp.Controllers;
using UCDArch.Data.NHibernate;
using UCDArch.Web.IoC;
using UCDArch.Web.ModelBinder;
using SampleUCDArchApp.Core.Domain;
using SampleUCDArchApp.Helpers;

namespace SampleUCDArchApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            #if DEBUG
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            #endif

            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.DefaultBinder = new UCDArchModelBinder();

            AutomapperConfig.Configure();

            NHibernateSessionConfiguration.Mappings.UseFluentMappings(typeof(Customer).Assembly);

            InitProfilerSettings();

            IWindsorContainer container = InitializeServiceLocator();
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

        private static void InitProfilerSettings()
        {
            //Don't profile any resource files 
            MiniProfiler.Settings.IgnoredPaths = new[] { "/mini-profiler-", "/css/", "/scripts/", "/images/", "/favicon.ico" };

            //Clean up the nhibernate stack trace
            MiniProfiler.Settings.ExcludeAssembly("mscorlib");
            MiniProfiler.Settings.ExcludeAssembly("NHibernate");
            MiniProfiler.Settings.ExcludeAssembly("System.Web.Extensions");
            MiniProfiler.Settings.ExcludeType("DbCommandProxy");

            MiniProfiler.Settings.SqlFormatter = new MvcMiniProfiler.SqlFormatters.InlineFormatter();
        }

        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)
            {
                MiniProfiler.Start();
            }
        }

        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}