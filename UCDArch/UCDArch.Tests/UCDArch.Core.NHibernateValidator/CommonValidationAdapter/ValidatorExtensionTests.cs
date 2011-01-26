using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Validator.Constraints;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.NHibernateValidator.CommonValidatorAdapter;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace UCDArch.Tests.UCDArch.Core.NHibernateValidator.CommonValidationAdapter
{
    [TestClass]
    public class ValidatorExtensionTests
    {
        /// <summary>
        /// Determines whether this instance [can validate object].
        /// </summary>
        [TestMethod]
        public void CanValidateObject()
        {
            var validator = new Validator();

            var invalidObject = new RequiredObjectWithSpecificMessage
                                    {
                                        Name = " "
                                    };
            Assert.IsFalse(validator.IsValid(invalidObject));

            var validObject = new RequiredObjectWithSpecificMessage
                                  {
                                      Name = "x"
                                  };
            Assert.IsTrue(validator.IsValid(validObject));
        }

        /// <summary>
        /// Determines whether this instance [can retrive specific valiation results].
        /// </summary>
        [TestMethod]
        public void CanRetriveSpecificValiationResults()
        {
            var validator = new Validator();

            var invalidObject = new RequiredObjectWithSpecificMessage();
            ICollection<IValidationResult> results = validator.ValidationResultsFor(invalidObject);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Name", results.First().PropertyName);
            Assert.AreEqual(typeof(RequiredObjectWithSpecificMessage), results.First().ClassContext);
            Assert.AreEqual("Dude...the name please!!", results.First().Message);
        }

        /// <summary>
        /// Determines whether this instance [can retrive generic valiation results].
        /// </summary>
        [TestMethod]
        public void CanRetriveGenericValiationResults()
        {
            var validator = new Validator();

            var invalidObject = new RequiredObjectWithGenericMessage();
            ICollection<IValidationResult> results = validator.ValidationResultsFor(invalidObject);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Name", results.First().PropertyName);
            Assert.AreEqual(typeof(RequiredObjectWithGenericMessage), results.First().ClassContext);
            Assert.AreEqual("may not be null or empty", results.First().Message);
        }

        /// <summary>
        /// Invalidates an invalid object when null.
        /// </summary>
        [TestMethod]
        public void InvalidObjectWhenNull()
        {
            var validator = new Validator();

            var invalidObject = new RequiredObjectWithGenericMessage()
                                    {
                                        Name = null
                                    };
            Assert.IsFalse(validator.IsValid(invalidObject));
        }

        /// <summary>
        /// Invalidates an invalid object when empty.
        /// </summary>
        [TestMethod]
        public void InvalidObjectWhenEmpty()
        {
            var validator = new Validator();

            var invalidObject = new RequiredObjectWithGenericMessage()
            {
                Name = string.Empty
            };
            Assert.IsFalse(validator.IsValid(invalidObject));
        }

        /// <summary>
        /// Invalidates an invalid object when spaces only.
        /// </summary>
        [TestMethod]
        public void InvalidObjectWhenSpacesOnly()
        {
            var validator = new Validator();

            var invalidObject = new RequiredObjectWithGenericMessage()
            {
                Name = "   "
            };
            Assert.IsFalse(validator.IsValid(invalidObject));
        }

        #region Nested type: RequiredObjectWithSpecificMessage and RequiredObjectWithGenericMessage

        /// <summary>
        /// Required Object With Specific Message
        /// </summary>
        private class RequiredObjectWithSpecificMessage
        {
            [Required(Message = "Dude...the name please!!")]
            public string Name { get; set; }
        }

        /// <summary>
        /// Required Object With Generic Message
        /// </summary>
        private class RequiredObjectWithGenericMessage
        {
            [Required]
            public string Name { get; set; }
        }

        #endregion
    }
}
