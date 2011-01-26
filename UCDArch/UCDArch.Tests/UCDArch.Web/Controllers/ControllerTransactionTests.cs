using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;


namespace UCDArch.Tests.UCDArch.Web.Controllers
{
    [TestClass]
    public class ControllerTransactionTests
    {
        private IDbContext _dbContext;
        private TestControllerBuilder _builder;
        private SampleController _controller;
        
        [TestInitialize]
        public void Setup()
        {
            _builder = new TestControllerBuilder();

            _controller = _builder.CreateController<SampleController>();

            ServiceLocatorInitializer.InitWithFakeDBContext();

            _dbContext = SmartServiceLocator<IDbContext>.GetService();
        }

        /// <summary>
        /// Controller begins the transaction when calling method without manual transaction attribute.
        /// </summary>
        [TestMethod]
        public void ControllerBeginsTransactionWhenCallingMethodWithoutManualTransactionAttribute()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithoutManualTransactionAttribute");

            _dbContext.AssertWasCalled(a=>a.BeginTransaction(), a=>a.Repeat.Once());
        }

        /// <summary>
        /// Controller commits the transaction when calling method without manual transaction attribute.
        /// </summary>
        [TestMethod]
        public void ControllerCommitsTransactionWhenCallingMethodWithoutManualTransactionAttribute()
        {
            //Assume the transaction has been opened correctly
            _dbContext.Expect(a => a.IsActive).Return(true);

            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithoutManualTransactionAttribute");

            _dbContext.AssertWasCalled(a => a.CommitTransaction(), a => a.Repeat.Once());
        }

        /// <summary>
        /// Controller does not begin the transaction when calling method with manual transaction attribute.
        /// This is a case where the begin and commit/rollback would be handeled manually.
        /// </summary>
        [TestMethod]
        public void ControllerDoesNotBeginTransactionWhenCallingMethodWithManualTransactionAttribute()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttribute");

            _dbContext.AssertWasNotCalled(a => a.BeginTransaction());
        }

        /// <summary>
        /// Controller does not commit the transaction when calling method with manual transaction attribute.
        /// This is a case where the begin and commit/rollback would be handeled manually.
        /// </summary>
        [TestMethod]
        public void ControllerDoesNotCommitTransactionWhenCallingMethodWithManualTransactionAttribute()
        {
            //Assume the transaction has been opened correctly
            _dbContext.Expect(a => a.IsActive).Return(true);

            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttribute");

            _dbContext.AssertWasNotCalled(a=>a.CommitTransaction());
        }

        /// <summary>
        /// Controller calls begin transaction only once when calling method with transaction attribute.
        /// This has the [Transaction] Attribute, but we still want the begin/commit to only happen once.
        /// </summary>
        [TestMethod]
        public void ControllerCallsBeginTransactionOnlyOnceWhenCallingMethodWithTransactionAttribute()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithTransactionAttribute");

            _dbContext.AssertWasCalled(a => a.BeginTransaction(), a=>a.Repeat.Once());
        }

        /// <summary>
        /// Controller calls commit transaction only once when calling method with transaction attribute.
        /// This has the [Transaction] Attribute, but we still want the begin/commit to only happen once.
        /// </summary>
        [TestMethod]
        public void ControllerCallsCommitTransactionOnlyOnceWhenCallingMethodWithTransactionAttribute()
        {
            //Assume the transaction has been opened correctly
            _dbContext.Expect(a => a.IsActive).Return(true);

            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithTransactionAttribute");

            _dbContext.AssertWasCalled(a => a.CommitTransaction(), a=>a.Repeat.Once());
        }

        /// <summary>
        /// Controller calls begin transaction only once when calling method with manual transaction attribute and transaction scope.
        /// </summary>
        [TestMethod]
        public void ControllerCallsBeginTransactionOnlyOnceWhenCallingMethodWithManualTransactionAttributeAndTransactionScope()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttributeAndTransactionScope");

            _dbContext.AssertWasCalled(a => a.BeginTransaction(), a => a.Repeat.Once());
        }

        /// <summary>
        /// Controller calls commit transaction only once when calling method with manual transaction attribute and transaction scope.
        /// </summary>
        [TestMethod]
        public void ControllerCallsCommitTransactionOnlyOnceWhenCallingMethodWithManualTransactionAttributeAndTransactionScope()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithManualTransactionAttributeAndTransactionScope");

            _dbContext.AssertWasCalled(a => a.CommitTransaction(), a => a.Repeat.Once());
        }


        /// <summary>
        /// Controller calls begin transaction twice when calling method without manual transaction attribute and transaction scope.
        /// Assuming that this is the correct behaviour.
        /// </summary>
        [TestMethod]
        public void ControllerCallsBeginTransactionTwiceWhenCallingMethodWithoutManualTransactionAttributeAndTransactionScope()
        {
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithoutManualTransactionAttributeAndTransactionScope");

            _dbContext.AssertWasCalled(a => a.BeginTransaction(), a => a.Repeat.Twice());
        }


        /// <summary>
        /// Controller calls commit transaction twice when calling method without manual transaction attribute and transaction scope.
        /// Assuming that this is the correct behaviour.
        /// </summary>
        [TestMethod]
        public void ControllerCallsCommitTransactionTwiceWhenCallingMethodWithoutManualTransactionAttributeAndTransactionScope()
        {
            _dbContext.Expect(a => a.IsActive).Return(true);
            _controller.ActionInvoker.InvokeAction(_controller.ControllerContext,
                                                   "MethodWithoutManualTransactionAttributeAndTransactionScope");

            _dbContext.AssertWasCalled(a => a.CommitTransaction(), a => a.Repeat.Twice());
        }
   
        internal class SampleController : TestSuperController
        {

            public ActionResult MethodWithoutManualTransactionAttribute()
            {
                return Content("String");
            }

            [HandleTransactionsManually]
            public ActionResult MethodWithManualTransactionAttribute()
            {
                return Content("String");
            }

            [Transaction]
            public ActionResult MethodWithTransactionAttribute()
            {
                return Content("String");
            }

            [HandleTransactionsManually]
            public ActionResult MethodWithManualTransactionAttributeAndTransactionScope()
            {                
                using (var ts = new TransactionScope())
                {                    
                    ts.CommitTransaction();
                }
                return Content("String");
            }


            public ActionResult MethodWithoutManualTransactionAttributeAndTransactionScope()
            {
                using (var ts = new TransactionScope())
                {
                    ts.CommitTransaction();
                }
                return Content("String");
            }
        }

        [UseTransactionsByDefault]
        internal class TestSuperController : Controller
        {
            
        }
    }
}