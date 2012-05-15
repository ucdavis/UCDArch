using System.Collections.Generic;
using UCDArch.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.RegressionTests.SampleMappings;
using UCDArch.Data.NHibernate;

namespace UCDArch.RegressionTests.Repository
{
    /// <summary>
    /// Tests to check object flushing/dirty checking behavior
    /// </summary>
    [TestClass]
    public class ObjectFlushTests : FluentRepositoryTestBase<UnitMap>
    {
        protected override void LoadData()
        {
            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            //Let's create a unit
            var unit = new Unit
            {
                FullName = "FullName",
                ShortName = "ShortName",
                FISCode = "1234",
                PPSCode = "123456"
            };

            unitRepository.EnsurePersistent(unit);

            unitRepository.DbContext.CommitTransaction();

            NHibernateSessionManager.Instance.GetSession().Clear(); 
            //Have to clear the session to get objects out of cache

            base.LoadData();
        }

        [TestMethod]
        public void ChangingLazyObjectDoesNotAutomaticallyFlushChanges()
        {
            var stats = NHibernateSessionManager.Instance.FactoryStatistics;
            stats.Clear();

            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            var existingUnit = unitRepository.GetById(1); //Lazy load

            Assert.IsFalse(existingUnit.IsTransient(), "The unit with id 1 should already exist");

            Assert.AreEqual("FullName", existingUnit.FullName);

            existingUnit.FullName = "ChangedName";

            unitRepository.DbContext.CommitTransaction();

            Assert.AreEqual(0, stats.EntityUpdateCount, "No entities should have been updated without call to EnsurePersistent");
        }

        [TestMethod]
        public void ChangingObjectDoesNotAutomaticallyFlushChanges()
        {
            var stats = NHibernateSessionManager.Instance.FactoryStatistics;
            stats.Clear();

            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            var existingUnit = unitRepository.GetNullableById(1);

            Assert.IsFalse(existingUnit.IsTransient(), "The unit with id 1 should already exist");

            Assert.AreEqual("FullName", existingUnit.FullName);

            existingUnit.FullName = "ChangedName";

            unitRepository.DbContext.CommitTransaction();
            
            Assert.AreEqual(0, stats.EntityUpdateCount, "No entities should have been updated without call to EnsurePersistent");
        }


        [TestMethod]
        public void EnsurePersistSavesChangesWithLazyObject()
        {
            var stats = NHibernateSessionManager.Instance.FactoryStatistics;
            stats.Clear();

            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            var existingUnit = unitRepository.GetById(1); //Lazy load

            Assert.IsFalse(existingUnit.IsTransient(), "The unit with id 1 should already exist");

            Assert.AreEqual("FullName", existingUnit.FullName);

            existingUnit.FullName = "ChangedName";

            unitRepository.EnsurePersistent(existingUnit); //Explicit Save
            
            unitRepository.DbContext.CommitTransaction();

            Assert.AreEqual(1, stats.EntityUpdateCount, "The existing unit should have been updated with the call to EnsurePersistent");
        }

        [TestMethod]
        public void EnsurePersistSavesChanges()
        {
            var stats = NHibernateSessionManager.Instance.FactoryStatistics;
            stats.Clear();

            var unitRepository = Repository.OfType<Unit>();

            unitRepository.DbContext.BeginTransaction();

            var existingUnit = unitRepository.GetNullableById(1);

            Assert.IsFalse(existingUnit.IsTransient(), "The unit with id 1 should already exist");

            Assert.AreEqual("FullName", existingUnit.FullName);

            existingUnit.FullName = "ChangedName";

            unitRepository.EnsurePersistent(existingUnit); //Explicit Save
            
            unitRepository.DbContext.CommitTransaction();

            Assert.AreEqual(1, stats.EntityUpdateCount, "The existing unit should have been updated with the call to EnsurePersistent");
        }
    }
}