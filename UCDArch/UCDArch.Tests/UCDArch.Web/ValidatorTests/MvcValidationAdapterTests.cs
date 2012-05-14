using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing;
using UCDArch.Web.Validator;
using Validator = UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter.Validator;

namespace UCDArch.Tests.UCDArch.Web.ValidatorTests
{
    [TestClass]
    public class MvcValidationAdapterTests
    {
        [TestInitialize]
        public void Setup()
        {
            ServiceLocatorInitializer.Init();
        }

        /// <summary>
        /// Determines whether this instance [can validate object].
        /// </summary>
        [TestMethod]
        public void CanValidateObject()
        {
            var validator = new Validator();

            var invalidObject = new SomeObject
            {
                LastName = "",
                FirstName = "ThisFirstNameIsTooLong",
                Street = null
            };
            Assert.IsFalse(validator.IsValid(invalidObject));

            var validObject = new SomeObject
            {
                LastName = "Last",
                FirstName = "First",
                Street = "SomeStreet"
            };
            Assert.IsTrue(validator.IsValid(validObject));
        }

        /// <summary>
        /// Determines whether this instance [can transfer validation results to model state].
        /// </summary>
        [TestMethod]
        public void CanTransferValidationResultsToModelState()
        {
            var validator = new Validator();

            var invalidObject = new SomeObject
            {
                LastName = null,
                FirstName = "ThisFirstNameIsTooLong",
                Street = " ",
                MiddleName = "valid"
            };
            Assert.IsFalse(validator.IsValid(invalidObject));

            var results = validator.ValidationResultsFor(invalidObject);
            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Count, "Wrong number of validation messages encountered.");

            ModelStateDictionary modelState = new ModelStateDictionary();
            Assert.IsNotNull(modelState);
            Assert.AreEqual(0, modelState.Values.Count);
            MvcValidationAdapter.TransferValidationMessagesTo(modelState, validator.ValidationResultsFor(invalidObject));

            Assert.AreEqual(3, modelState.Values.Count);

            var resultsList = new List<string>();
            foreach (var result in modelState.Values)
            {
                foreach (var errs in result.Errors)
                {
                    resultsList.Add(errs.ErrorMessage);
                }
            }
            var errors = new[]
                             {
                                 "Dude...the name please!!",
                                 "The Street field is required.",
                                 "The field FirstName must be a string with a maximum length of 10."
                             };

            Assert.AreEqual(resultsList.Count, errors.Length, "Number of error messages do not match");
            foreach (var error in errors)
            {
                Assert.IsTrue(resultsList.Contains(error), "Expected error \"" + error + "\" not found");
            }


        }


        #region Nested type: SomeObject

        /// <summary>
        /// A class to validate against
        /// </summary>
        private class SomeObject
        {
            [Required(ErrorMessage = "Dude...the name please!!")]
            public string LastName { get; set; }

            [Required]
            public string Street { get; set; }

            [StringLength(10)]
            public string FirstName { get; set; }

            [StringLength(10)]
            public string MiddleName { get; set; }
        }

        #endregion
    }
}