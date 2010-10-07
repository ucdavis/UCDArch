using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;

namespace UCDArch.Tests.UCDArch.Core.NHibernateValidator.CommonValidationAdapter
{
    [TestClass]
    public class CompositeKeyValidationTests
    {
        [TestInitialize]
        public void Setup()
        {
            ServiceLocatorInitializer.Init();
        }

        [TestMethod]
        public void ValidCompositeKeyReturnValid()
        {
            var sample = GetValidSampleClass();

            sample.SetAssignedIdTo(GetValidCompositeKey());

            Assert.AreEqual(true, sample.IsValid());
        }

        [TestMethod]
        public void InvalidCompositeKeyReturnsInvalid()
        {
            var sample = GetValidSampleClass();

            sample.SetAssignedIdTo(GetInvalidCompositeKey());

            Assert.AreEqual(false, sample.IsValid());
        }

        [TestMethod]
        public void InvalidCompositeKeyReturnsProperErrors()
        {
            var sample = GetValidSampleClass();

            sample.SetAssignedIdTo(GetInvalidCompositeKey());

            Assert.AreEqual(false, sample.IsValid());

            sample.ValidationResults().AsMessageList().AssertErrorsAre(
                "Month: must be between 1 and 12",
                "Year: must be greater than or equal to 2000");
        }

        [TestMethod]
        public void InvalidCompositeKeyAndClassReturnsProperErrors()
        {
            var sample = GetInvalidSampleClass();

            sample.SetAssignedIdTo(GetInvalidCompositeKey());

            Assert.AreEqual(false, sample.IsValid());

            sample.ValidationResults().AsMessageList().AssertErrorsAre(
                "Name: may not be null",
                "Month: must be between 1 and 12",
                "Year: must be greater than or equal to 2000");
        }

        private static SampleClass GetValidSampleClass()
        {
            return new SampleClass {Name = "ValidName"};
        }

        private static SampleClass GetInvalidSampleClass()
        {
            return new SampleClass { Name = null };
        }

        private static CompositeKey GetValidCompositeKey()
        {
            return new CompositeKey {Month = 6, Year = 2009};
        }

        private static CompositeKey GetInvalidCompositeKey()
        {
            return new CompositeKey {Month = 50, Year = 1};
        }

        public class SampleClass : DomainObjectWithTypedId<CompositeKey>, IHasAssignedId<CompositeKey>
        {
            [NotNull]
            public string Name { get; set; }

            [Valid]
            public override CompositeKey Id
            {
                get
                {
                    return base.Id;
                }
                protected set
                {
                    base.Id = value;
                }
            }

            public void SetAssignedIdTo(CompositeKey assignedId)
            {
                Id = assignedId;
            }
        }

        public class CompositeKey
        {
            [Range(1,12)]
            public int Month { get; set; }

            [Min(2000)]
            public int Year { get; set; }
        }
    }
}