using System.Web.Mvc;
using System.Web.Optimization;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using SampleUCDArchApp.App_Start;
using SampleUCDArchApp.Controllers;
using SampleUCDArchApp.Core.Domain;
using UCDArch.Data.NHibernate;
using UCDArch.Web.IoC;
using UCDArch.Web.ModelBinder;

[assembly: WebActivator.PreApplicationStartMethod(typeof(UCDArchBootstrapper), "PreStart")]
namespace SampleUCDArchApp.App_Start
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
    }
}