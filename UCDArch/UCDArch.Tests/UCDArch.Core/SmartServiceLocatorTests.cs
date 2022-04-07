using Castle.Windsor;
using CommonServiceLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UCDArch.Core;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter;
using UCDArch.Web.IoC;
using Castle.MicroKernel.Registration;

namespace UCDArch.Tests.UCDArch.Core
{
    [TestClass]
    public class SmartServiceLocatorTests
    {
        [TestInitialize]
        public void Setup()
        {
            ServiceLocator.SetLocatorProvider(null);
        }

        [TestMethod]
        public void WillBeInformedIfServiceLocatorNotInitialized()
        {
            bool exceptionThrown = false;

            try
            {
                SmartServiceLocator<IValidator>.GetService();
            }
            catch (NullReferenceException e)
            {
                exceptionThrown = true;
                Assert.IsTrue(e.Message.Contains("ServiceLocator has not been initialized"));
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void WillBeInformedIfServiceNotRegistered()
        {
            bool exceptionThrown = false;

            IWindsorContainer container = new WindsorContainer();
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            try
            {
                SmartServiceLocator<IValidator>.GetService();
            }
            catch (ActivationException e)
            {
                exceptionThrown = true;
                Assert.IsTrue(e.Message.Contains("IValidator could not be located"));
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void CanReturnServiceIfInitializedAndRegistered()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Register(Component.For<IValidator>().ImplementedBy<Validator>().Named("validator"));
            
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));

            IValidator validatorService = SmartServiceLocator<IValidator>.GetService();

            Assert.IsNotNull(validatorService);
        }
    }
}