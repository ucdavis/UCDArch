using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using Rhino.Mocks;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Web.IoC;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace UCDArch.Testing
{
    public class ServiceLocatorInitializer
    {
        public static IWindsorContainer Init()
        {
            IWindsorContainer container = new WindsorContainer();
            
            container.AddComponent("validator",
                typeof(IValidator), typeof(Validator));
            container.AddComponent("DbContext", 
                typeof (IDbContext), typeof (DbContext));

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }

        public static IWindsorContainer InitWithFakeDBContext()
        {
            IWindsorContainer container = new WindsorContainer();

            container.AddComponent("validator",
                typeof(IValidator), typeof(Validator));

            var dbContext = MockRepository.GenerateMock<IDbContext>();

            container.Kernel.AddComponentInstance<IDbContext>(dbContext);
            
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            return container;
        }
    }
}