using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using UCDArch.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;
using UCDArch.Web.Attributes;
using UCDArch.Tests;

namespace UCDArch.Tests.UCDArch.Web.Controllers
{
    [TestClass]
    public class ControllerTransactionTests
    {
        private IDbContext _dbContext;
        private SampleController _controller;
        private static int _beginTransactionCount;
        private static int _commitTransactionCount;
        private static int _closeSessionCount;


        [TestInitialize]
        public void Setup()
        {
            _controller = new SampleController();
            _controller.ControllerContext = new ControllerContext();

            ServiceLocatorInitializer.InitWithFakeDBContext();

            _dbContext = SmartServiceLocator<IDbContext>.GetService();

            _dbContext.Stub(x => x.IsActive).Repeat.Any().Return(true);
            _dbContext.Stub(x => x.BeginTransaction()).Repeat.Any().WhenCalled(x => _beginTransactionCount++);
            _dbContext.Stub(x => x.CommitTransaction()).Repeat.Any().WhenCalled(x => _commitTransactionCount++);
            _dbContext.Stub(x => x.CloseSession()).Repeat.Any().WhenCalled(x=> _closeSessionCount++);
        }

        [TestCleanup]
        public void TearDown()
        {
            _beginTransactionCount = 0;
            _commitTransactionCount = 0;
            _closeSessionCount = 0;
        }

        /// <summary>
        /// Controller closes session when calling method without manual transaction attribute.
        /// </summary>
        [TestMethod]
        public void ControllerCloseSessionWhenCallingMethodWithoutManualTransactionAttribute()
        {
            _controller.MethodWithoutManualTransactionAttribute();

            Assert.AreEqual(1, _closeSessionCount);
            //_dbContext.AssertWasCalled(x => x.CloseSession(), x => x.Repeat.Once());
        }

        /// <summary>
        /// Controller DOESN'T close session when calling method with manual transaction attribute.
        /// </summary>
        [TestMethod]
        public void ControllerDoesNotCloseSessionWhenCallingMethodWithManualTransactionAttribute()
        {
            _controller.MethodWithManualTransactionAttribute();

            Assert.AreEqual(0, _closeSessionCount);
            //_dbContext.AssertWasNotCalled(a => a.CloseSession());
        }

        /// <summary>
        /// Controller begins the transaction when calling method without manual transaction attribute.
        /// </summary>
        [TestMethod]
        public void ControllerBeginsTransactionWhenCallingMethodWithoutManualTransactionAttribute()
        {
            _controller.MethodWithoutManualTransactionAttribute();

            Assert.AreEqual(1, _beginTransactionCount);
            //_dbContext.AssertWasCalled(a=>a.BeginTransaction(), a=>a.Repeat.Once());
        }

        /// <summary>
        /// Controller commits the transaction when calling method without manual transaction attribute.
        /// </summary>
        [TestMethod]
        public void ControllerCommitsTransactionWhenCallingMethodWithoutManualTransactionAttribute()
        {
            //Assume the transaction has been opened correctly
            _dbContext.Stub(a => a.IsActive).Return(true);

            _controller.MethodWithoutManualTransactionAttribute();

            Assert.AreEqual(1, _commitTransactionCount);
            //_dbContext.AssertWasCalled(a => a.CommitTransaction(), a => a.Repeat.Once());
        }

        /// <summary>
        /// Controller does not begin the transaction when calling method with manual transaction attribute.
        /// This is a case where the begin and commit/rollback would be handeled manually.
        /// </summary>
        [TestMethod]
        public void ControllerDoesNotBeginTransactionWhenCallingMethodWithManualTransactionAttribute()
        {
            _controller.MethodWithManualTransactionAttribute();

            Assert.AreEqual(0, _beginTransactionCount);
            //_dbContext.AssertWasNotCalled(a => a.BeginTransaction());
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

            _controller.MethodWithManualTransactionAttribute();

            Assert.AreEqual(0, _commitTransactionCount);
            //_dbContext.AssertWasNotCalled(a=>a.CommitTransaction());
        }

        /// <summary>
        /// Controller calls begin transaction only once when calling method with transaction attribute.
        /// This has the [Transaction] Attribute, but we still want the begin/commit to only happen once.
        /// </summary>
        [TestMethod]
        public void ControllerCallsBeginTransactionOnlyOnceWhenCallingMethodWithTransactionAttribute()
        {
            _controller.MethodWithTransactionAttribute();

            Assert.AreEqual(1, _beginTransactionCount);
            //_dbContext.AssertWasCalled(a => a.BeginTransaction(), a=>a.Repeat.Once());
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

            _controller.MethodWithTransactionAttribute();

            Assert.AreEqual(1, _commitTransactionCount);
            //_dbContext.AssertWasCalled(a => a.CommitTransaction(), a=>a.Repeat.Once());
        }

        /// <summary>
        /// Controller calls begin transaction only once when calling method with manual transaction attribute and transaction scope.
        /// </summary>
        [TestMethod]
        public void ControllerCallsBeginTransactionOnlyOnceWhenCallingMethodWithManualTransactionAttributeAndTransactionScope()
        {
            _controller.MethodWithManualTransactionAttributeAndTransactionScope();

            Assert.AreEqual(1, _beginTransactionCount);
            //_dbContext.AssertWasCalled(a => a.BeginTransaction(), a => a.Repeat.Once());
        }

        /// <summary>
        /// Controller calls commit transaction only once when calling method with manual transaction attribute and transaction scope.
        /// </summary>
        [TestMethod]
        public void ControllerCallsCommitTransactionOnlyOnceWhenCallingMethodWithManualTransactionAttributeAndTransactionScope()
        {
            _controller.MethodWithManualTransactionAttributeAndTransactionScope();

            Assert.AreEqual(1, _commitTransactionCount);
            //_dbContext.AssertWasCalled(a => a.CommitTransaction(), a => a.Repeat.Once());
        }


        /// <summary>
        /// Controller calls begin transaction twice when calling method without manual transaction attribute and transaction scope.
        /// Assuming that this is the correct behaviour.
        /// </summary>
        [TestMethod]
        public void ControllerCallsBeginTransactionTwiceWhenCallingMethodWithoutManualTransactionAttributeAndTransactionScope()
        {
            _controller.MethodWithoutManualTransactionAttributeAndTransactionScope();

            Assert.AreEqual(2, _beginTransactionCount);
            //_dbContext.AssertWasCalled(a => a.BeginTransaction(), a => a.Repeat.Twice());
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