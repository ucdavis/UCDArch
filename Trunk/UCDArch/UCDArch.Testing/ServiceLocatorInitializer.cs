using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using Rhino.Mocks;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Web.IoC;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Castle.MicroKernel.Registration;

namespace UCDArch.Testing
{
    public class ServiceLocatorInitializer
    {
        public static IWindsorContainer Init()
        {
            IWindsorContainer container = new WindsorContainer();

            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            container.Register(Component.For<IDbContext>().ImplementedBy<DbContext>().Named("DbContext"));

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }

        public static IWindsorContainer InitWithFakeDBContext()
        {
            IWindsorContainer container = new WindsorContainer();

            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));

            var dbContext = MockRepository.GenerateMock<IDbContext>();

            container.Register(Component.For<IDbContext>().Instance(dbContext));
            
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }
    }
}