using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.RegressionTests.SampleMappings;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

//TODO: Maybe remove this unit test?
namespace UCDArch.RegressionTests.Repository
{
    [TestClass]
    public class UnitAssociationRepositoryTests : FluentRepositoryTestBase<UnitAssociationMap>
    {
        private readonly IRepository<UnitAssociation> _unitAssociationRepository = new Repository<UnitAssociation>();
        private readonly IRepository<User> _userRepository = new Repository<User>();
        private readonly IRepository<Unit> _unitRepository = new Repository<Unit>();

        protected override void LoadData()
        {
            CreateUsers();
            CreateUnits();
        }

        [TestMethod]
        public void CanSaveValidUnitAssociationUsingGenericRepository()
        {
            var newUnitAssociation = CreateValidUnitAssociation();
            Assert.AreEqual(true, newUnitAssociation.IsTransient());

            _unitAssociationRepository.EnsurePersistent(newUnitAssociation);

            Assert.AreEqual(false, newUnitAssociation.IsTransient());
        }


        #region helper methods
        /// <summary>
        /// Creates the valid unit association. Is this correct?
        /// </summary>
        /// <returns></returns>
        private UnitAssociation CreateValidUnitAssociation()
        {
            var unitAssociation = new UnitAssociation
                                      {
                                          Inactive = false,
                                          User = _userRepository.GetNullableById(2),
                                          Unit = _unitRepository.GetNullableById(2)
                                      };
            return unitAssociation;
        }

        /// <summary>
        /// Creates 5 units for DB Tests.
        /// </summary>
        private void CreateUnits()
        {
            NHibernateSessionManager.Instance.BeginTransaction();
            for (int i = 1; i <= 5; i++)
            {
                var unit = CreateValidUnit();
                unit.FullName += i.ToString();
                unit.ShortName += i.ToString();

                _unitRepository.EnsurePersistent(unit); //Save
            }
            NHibernateSessionManager.Instance.CommitTransaction();
        }

        /// <summary>
        /// Creates the valid unit.
        /// </summary>
        /// <returns></returns>
        private static Unit CreateValidUnit()
        {
            return new Unit
            {
                FullName = "FullName",
                ShortName = "ShortName",
                FISCode = "1234",
                PPSCode = "123456"
            };
        }

        /// <summary>
        /// Creates the users for DB Tests.
        /// </summary>
        private void CreateUsers()
        {
            string[] names = { "Scott", "John", "James", "Bob", "Larry", "Joe", "Pete", "Adam", "Alan", "Ken" };
            string[] loginIDs = { "postit", "aaaaa", "bbbbb", "ccccc", "ddddd", "eeeee", "fffff", "ggggg", "hhhhh", "iiiii" };

            //using (var ts = new TransactionScope())
            //{
            NHibernateSessionManager.Instance.BeginTransaction();
            for (int i = 0; i < 10; i++)
            {
                var user = new User
                {
                    Email = "fake@ucdavis.edu",
                    EmployeeID = "999999999",
                    FirstName = names[i],
                    LastName = "Last",
                    LoginID = loginIDs[i],
                    UserKey = Guid.NewGuid()
                };

                _userRepository.EnsurePersistent(user); //Save
            }

            NHibernateSessionManager.Instance.CommitTransaction();
        }
        #endregion helper methods
    }
}
