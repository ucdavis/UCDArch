using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.CommonValidator;
using Validator = UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter.Validator;

namespace UCDArch.Tests.UCDArch.Core.DataAnnotationsValidator.CommonValidationAdapter
{
    [TestClass]
    public class ValidatorTests
    {
        [TestMethod]
        public void CanValidateObject()
        {
            var validator = new Validator();

            var invalidObject = new SomeObject();
            Assert.IsFalse(validator.IsValid(invalidObject));

            var validObject = new SomeObject
            {
                Name = "ValidName"
            };
            Assert.IsTrue(validator.IsValid(validObject));
        }

        [TestMethod]
        public void CanRetriveValiationResults()
        {
            var validator = new Validator();

            var invalidObject = new SomeObject();
            ICollection<IValidationResult> results = validator.ValidationResultsFor(invalidObject);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Name", results.First().PropertyName);
            Assert.AreEqual(typeof(SomeObject), results.First().ClassContext);
            Assert.AreEqual("Dude...the name please!!", results.First().Message);
        }

        #region CompleteObject Tests

        /// <summary>
        /// Determines whether this instance [can validate complete object].
        /// </summary>
        [TestMethod]
        public void CanValidateCompleteObject()
        {
            var validator = new Validator();

            var invalidObject = CreateValidCompleteObject();
            invalidObject.RequiredValidatorString = null;

            Assert.IsFalse(validator.IsValid(invalidObject));

            var validObject = CreateValidCompleteObject();

            var validationErrors = validator.ValidationResultsFor(validObject);

            Assert.IsTrue(validator.IsValid(validObject));
        }

        #region Required Attribute tests

        /// <summary>
        /// Validates the required attribute null test.
        /// </summary>
        [TestMethod]
        public void ValidateRequiredAttributeNullTest()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            objectTotest.RequiredValidatorString = null;
            ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("RequiredValidatorString", results.First().PropertyName);
            Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext);
            Assert.AreEqual("The RequiredValidatorString field is required.", results.First().Message);
        }

        /// <summary>
        /// Validates the required attribute empty test.
        /// </summary>
        [TestMethod]
        public void ValidateRequiredAttributeEmptyTest()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            objectTotest.RequiredValidatorString = string.Empty;
            ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("RequiredValidatorString", results.First().PropertyName);
            Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext);
            Assert.AreEqual("The RequiredValidatorString field is required.", results.First().Message);
        }

        /// <summary>
        /// Validates the required attribute spaces only test.
        /// </summary>
        [TestMethod]
        public void ValidateRequiredAttributeSpacesOnlyTest()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            objectTotest.RequiredValidatorString = "    ";
            ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("RequiredValidatorString", results.First().PropertyName);
            Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext);
            Assert.AreEqual("The RequiredValidatorString field is required.", results.First().Message);
        }

        /// <summary>
        /// Validates the required attribute overide message test.
        /// </summary>
        [TestMethod]
        public void ValidateRequiredAttributeOverideMessageTest()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            objectTotest.RequiredValidatorSpecialMessage = "    ";
            ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("RequiredValidatorSpecialMessage", results.First().PropertyName);
            Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext);
            Assert.AreEqual("Special Error message here!", results.First().Message);
        }

        /// <summary>
        /// Validates the required attribute null list.
        /// </summary>
        [TestMethod]
        public void ValidateRequiredAttributeNullList()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            objectTotest.RequiredListString = null;
            ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("RequiredListString", results.First().PropertyName);
            Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext);
            Assert.AreEqual("The RequiredListString field is required.", results.First().Message);
        }

        /// <summary>
        /// Validates the required attribute empty list.
        /// </summary>
        [TestMethod, Ignore] //Ignore vecause data annotations does not check list length
        public void ValidateRequiredAttributeEmptyList()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            objectTotest.RequiredListString = new List<string>();
            ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("RequiredListString", results.First().PropertyName);
            Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext);
            Assert.AreEqual("may not be null or empty", results.First().Message);
        }

        #endregion Required Attribute tests

        #region RangeDouble Attribute tests
        /// <summary>
        /// Validates the range double is valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<double> { 1.01, 1.010001, 1.1, 1.999, 2, 2.0001, 2.4999999, 2.5 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidator = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }

        }

        /// <summary>
        /// Validates the range double is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<double> { -1.1, -1, 0, 0.0001, 1, 1.0000001, 2.5000001, 3, 4, 4.1 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidator = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidator", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidator must be between 1.01 and 2.5.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double min only is valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleMinOnlyIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<double> { 3, 3.000001, 3.1, 4, 20000000000.0, 3.0E300 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorMinOnly = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double min only is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleMinOnlyIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<double> { -1.0E200, -1.1, -1, 0, 0.0001, 1, 1.0000001, 2.5000001, 2.9999999 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorMinOnly = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidatorMinOnly", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidatorMinOnly must be between 3 and 1.79769313486232E+308.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double max only is valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleMaxOnlyIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<double> { 3, 2.99999, 2, 1, 0, -3.0E300 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorMaxOnly = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double max only is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleMaxOnlyIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<double> { 3.000001, 4, 2.0E200 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorMaxOnly = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidatorMaxOnly", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidatorMaxOnly must be between -1.79769313486232E+308 and 3.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double nullable is valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleNullableIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<double?> { null, 3, 3.5, 4 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorNullable = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double nullable is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleNullableIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<double?> { -3, -2, 0, 2.999, 4.0001, 5 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorNullable = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidatorNullable", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidatorNullable must be between 3 and 4.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double special message is used when not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleSpecialMessageIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();

            objectTotest.RangeDoubleValidatorSpecialMessage = 2;
            ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("RangeDoubleValidatorSpecialMessage", results.First().PropertyName);
            Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext);
            Assert.AreEqual("Special Error message here!", results.First().Message);
        }

        /// <summary>
        /// Validates the range double string is valid tests.
        /// </summary>
        [TestMethod, Ignore] //DataAnnotations does not validate strings as doubles
        public void ValidateRangeDoubleStringIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<string> { null, "3", "3.5", "4", "3.0E2" };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorString = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double string is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleStringIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<string> { "-3", "-2", "0", "2.999", "1001", "ABC", "2.99E5" };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorString = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidatorString", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidatorString must be between 3 and 1000.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double int is valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleIntIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<int> { 3, 4, 5, 6, 7, 8, 9, 10 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorInt = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double int is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleIntIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<int> { -3, -1, 0, 1, 2, 11, 12 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorInt = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidatorInt", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidatorInt must be between 3 and 10.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double decimal is valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleDecimalIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<decimal> { (decimal)3.01, (decimal)3.5, 4, (decimal)9.999, 10 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorDecimal = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double Decimal is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleDecimalIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<decimal> { (decimal)-3.01, -1, 0, 1, 3, (decimal)10.9999, 12 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorDecimal = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidatorDecimal", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidatorDecimal must be between 3.01 and 10.999.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double float is valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleFloatIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<float> { 3, (float)3.01, (float)3.5, 4, (float)9.999, 10 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorFloat = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double float is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleFloatIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<float> { (float)-3.01, -3, -1, 0, 1, (float)2.999999, (float)10.9999, 12 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorFloat = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidatorFloat", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidatorFloat must be between 3 and 10.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double long is valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleLongIsValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<long> { 3, 4, 5, 6, 7, 8, 9, 10 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorLong = list;
                Assert.IsTrue(validator.IsValid(objectTotest), "Failed on: \"" + list + "\"");
            }
        }

        /// <summary>
        /// Validates the range double long is not valid tests.
        /// </summary>
        [TestMethod]
        public void ValidateRangeDoubleLongIsNotValidTests()
        {
            var validator = new Validator();
            var objectTotest = CreateValidCompleteObject();
            var doubleList = new List<long> { -3, -1, 0, 1, 2, 11, 12 };
            foreach (var list in doubleList)
            {
                objectTotest.RangeDoubleValidatorLong = list;
                ICollection<IValidationResult> results = validator.ValidationResultsFor(objectTotest);
                Assert.IsNotNull(results, "Failed on: \"" + list + "\"");
                Assert.AreEqual(1, results.Count, "Failed on: \"" + list + "\"");
                Assert.AreEqual("RangeDoubleValidatorLong", results.First().PropertyName, "Failed on: \"" + list + "\"");
                Assert.AreEqual(typeof(CompleteObject), results.First().ClassContext, "Failed on: \"" + list + "\"");
                Assert.AreEqual("The field RangeDoubleValidatorLong must be between 3 and 10.", results.First().Message, "Failed on: \"" + list + "\"");
            }
        }

        #endregion RangeDouble Attribute tests

        #endregion CompleteObject Tests



        #region Nested type: SomeObject

        private class SomeObject
        {
            [Required(ErrorMessage = "Dude...the name please!!")]
            public string Name { get; set; }
        }

        #endregion


        #region CompleteObject

        /// <summary>
        /// Creates the valid complete object.
        /// </summary>
        /// <returns></returns>
        private static CompleteObject CreateValidCompleteObject()
        {
            return new CompleteObject
            {
                RequiredValidatorString = "string",
                RequiredValidatorSpecialMessage = "string",
                RequiredListString = new List<string> { "test1", "test2" },
                RangeDoubleValidator = 2,
                RangeDoubleValidatorMinOnly = 3,
                RangeDoubleValidatorMaxOnly = 0,
                RangeDoubleValidatorNullable = null,
                RangeDoubleValidatorSpecialMessage = 3,
                RangeDoubleValidatorString = "7",
                RangeDoubleValidatorInt = 4,
                RangeDoubleValidatorDecimal = (decimal)4.001,
                RangeDoubleValidatorFloat = (float)4.001,
                RangeDoubleValidatorLong = 4
            };
        }

        /// <summary>
        /// An object to test the validator against. For now this is only testing the extensions, 
        /// but it could be expanded to include all the validator attributes.
        /// </summary>
        private class CompleteObject
        {
            [Required]
            public string RequiredValidatorString { get; set; }
            [Required(ErrorMessage = "Special Error message here!")]
            public string RequiredValidatorSpecialMessage { get; set; }
            [Required]
            public List<string> RequiredListString { get; set; }
           
            [Range(1.01, 2.5)]
            public double RangeDoubleValidator { get; set; }
            [Range(3, double.MaxValue)]
            public double RangeDoubleValidatorMinOnly { get; set; }
            [Range(double.MinValue, 3)]
            public double RangeDoubleValidatorMaxOnly { get; set; }
            [Range(3.0, 4.0)]
            public double? RangeDoubleValidatorNullable { get; set; }
            [Range(3, double.MaxValue, ErrorMessage = "Special Error message here!")]
            public double RangeDoubleValidatorSpecialMessage { get; set; }
            [Range(3, 1000)]
            public string RangeDoubleValidatorString { get; set; }
            [Range(3, 10)]
            public int RangeDoubleValidatorInt { get; set; }
            [Range(3.01, 10.999)]
            public decimal RangeDoubleValidatorDecimal { get; set; }
            [Range(3f, 10f)]
            public float RangeDoubleValidatorFloat { get; set; }
            [Range(3, 10)]
            public long RangeDoubleValidatorLong { get; set; }

            //TODO: Other Validation attributes. As these are added, add them to the CreatevalidCompleteObject()
        }
        #endregion CompleteObject
    }
}