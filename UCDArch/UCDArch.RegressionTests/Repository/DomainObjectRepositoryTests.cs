using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.RegressionTests.SampleMappings;
using UCDArch.Testing;

namespace UCDArch.RegressionTests.Repository
{
    [TestClass]
    public class DomainObjectRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<User> _userRepository = new Repository<User>();

        protected override void LoadData()
        {
            DomainObjectDataHelper.LoadDomainDataUsers(_userRepository);
        }

        /// <summary>
        /// Determines whether this instance [can save new user].
        /// </summary>
        [TestMethod]
        public void CanSaveNewUser()
        {
            User user = DomainObjectDataHelper.CreateValidUser();
            user.LoginID = "GBH756H4D3";

            Assert.IsTrue(user.IsTransient());
            _userRepository.DbContext.BeginTransaction();
            _userRepository.EnsurePersistent(user);
            _userRepository.DbContext.CommitTransaction();

            Assert.IsFalse(user.IsTransient()); //Make sure the user is saved

            NHibernateSessionManager.Instance.GetSession().Evict(user); //get the user out of the local cache

            //Now get the user back out
            int userId = user.Id;

            User retrievedUser = _userRepository.GetNullableById(userId);

            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual("GBH756H4D3", retrievedUser.LoginID); //Make sure it is the correct user
            Assert.AreEqual(11, _userRepository.GetAll().Count);
        }

        [TestMethod]
        public void CanModifyUser()
        {
            User firstUser = _userRepository.GetNullableById(1); //Just get the first user

            Assert.AreEqual("Scott", firstUser.FirstName); //First user is scott

            _userRepository.DbContext.BeginTransaction();
            firstUser.FirstName = "Tiny";
            _userRepository.EnsurePersistent(firstUser);
            _userRepository.DbContext.CommitTransaction();

            NHibernateSessionManager.Instance.GetSession().Evict(firstUser);
                //Evict the user from the cache so we can retrieve it

            firstUser = _userRepository.GetNullableById(1); //Get the user back out

            Assert.AreEqual("Tiny", firstUser.FirstName); //the name should now be tiny
        }

        [TestMethod]
        public void CanRemoveUser()
        {
            User firstUser = _userRepository.GetNullableById(1); //Just get the first user

            Assert.AreEqual("Scott", firstUser.FirstName); //First user is scott

            _userRepository.DbContext.BeginTransaction();
            _userRepository.Remove(firstUser);
            _userRepository.DbContext.CommitTransaction();

            Assert.AreEqual(9, _userRepository.GetAll().Count); //There should be 9 users now

            User shouldNotExistUser = _userRepository.GetNullableById(1);

            Assert.IsNull(shouldNotExistUser);
        }

        /// <summary>
        /// Determines whether this instance [can roll back added user].
        /// </summary>
        [TestMethod]
        public void CanRollBackAddedUser()
        {
            User user = DomainObjectDataHelper.CreateValidUser();
            user.LoginID = "JF63KD834K";

            Assert.IsTrue(user.IsTransient());
            _userRepository.DbContext.BeginTransaction();
            _userRepository.EnsurePersistent(user);
            int userId = user.Id;
            _userRepository.DbContext.RollbackTransaction();

            Assert.IsFalse(user.IsTransient()); //Make sure the user is saved

            NHibernateSessionManager.Instance.GetSession().Evict(user); //get the user out of the local cache
            
            User retrievedUser = _userRepository.GetNullableById(userId);

            Assert.IsNull(retrievedUser);
            Assert.AreEqual(10, _userRepository.GetAll().Count);
        }

        /// <summary>
        /// Without the force save generates exception.
        /// </summary>
        [TestMethod]
        public void WithoutForceSaveGeneratesException()
        {
            User user = DomainObjectDataHelper.CreateValidUser();
            user.LoginID = "JF63KD834K";

            Assert.IsTrue(user.IsTransient());
            _userRepository.DbContext.BeginTransaction();
            _userRepository.EnsurePersistent(user);
            _userRepository.DbContext.RollbackTransaction();

            Assert.IsFalse(user.IsTransient()); //Make sure the user is saved

            NHibernateSessionManager.Instance.GetSession().Evict(user); //get the user out of the local cache
            
            try
            {
                _userRepository.DbContext.BeginTransaction();
                _userRepository.EnsurePersistent(user); //Don't force the save, will generate an exception
                Assert.Fail("Expected Exception of type StaleObjectStateException");
            }
            catch (StaleObjectStateException)
            {
                
            }
        }

        /// <summary>
        /// Determines whether this instance [can force save without exception].
        /// </summary>
        [TestMethod]
        public void CanForceSaveWithoutException()
        {
            User user = DomainObjectDataHelper.CreateValidUser();
            user.LoginID = "JF63KD834K";

            Assert.IsTrue(user.IsTransient());
            _userRepository.DbContext.BeginTransaction();
            _userRepository.EnsurePersistent(user);
            _userRepository.DbContext.RollbackTransaction();

            Assert.IsFalse(user.IsTransient()); //Make sure the user is saved

            NHibernateSessionManager.Instance.GetSession().Evict(user); //get the user out of the local cache


            _userRepository.DbContext.BeginTransaction();
            _userRepository.EnsurePersistent(user, true); //Force the save
            _userRepository.DbContext.CommitTransaction();

            Assert.AreEqual(11, _userRepository.GetAll().Count);
        }
    }
}