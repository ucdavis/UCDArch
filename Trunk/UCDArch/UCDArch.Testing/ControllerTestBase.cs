using Castle.Windsor;
using Rhino.Mocks;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Controller;

namespace UCDArch.Testing
{
    public abstract class ControllerTestBase<CT> where CT : SuperController
    {
        protected CT Controller { get; set; }

        protected ControllerTestBase()
        {
            RegisterRoutes();

            InitServiceLocator();

            SetupController();

            Controller.Repository = MockRepository.GenerateStub<IRepository>();
        }

        /// <summary>
        /// Register your routes in here if you want to do any route-based testing
        /// </summary>
        protected virtual void RegisterRoutes()
        {
            //Register your routes
        }

        protected virtual void InitServiceLocator()
        {
            var container = ServiceLocatorInitializer.Init();

            RegisterAdditionalServices(container);
        }

        /// <summary>
        /// Instead of overriding InitServiceLocator, you can jump in here to register additional services for testing
        /// </summary>
        protected virtual void RegisterAdditionalServices(IWindsorContainer container)
        {

        }

        /// <summary>
        /// Setup your controller using something like Controller = new YourController(params);
        /// </summary>
        protected abstract void SetupController();

        protected IRepository<T> FakeRepository<T>() where T : ValidatableObject
        {
            return MockRepository.GenerateStub<IRepository<T>>();
        }
    }
}