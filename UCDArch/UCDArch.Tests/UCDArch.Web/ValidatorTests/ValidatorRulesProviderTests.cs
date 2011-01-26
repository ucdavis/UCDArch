using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;
using UCDArch.Testing;
using UCDArch.Web.Validator;

namespace UCDArch.Tests.UCDArch.Web.ValidatorTests
{
    [TestClass]
    public class ValidationRulesProviderTests
    {
        public ValidationRulesProviderTests()
        {
            ServiceLocatorInitializer.InitWithFakeDBContext();
        }

        [TestMethod]
        public void RulesProviderReturnsFourValidatedProperties()
        {
            var rulesProvider = new ValidatorRulesProvider();

            var rules = rulesProvider.GetRulesFromType(typeof(ValidatableClass));

            var numRules = rules.Keys.Count();

            Assert.AreEqual(4, numRules);
        }

        [TestMethod]
        public void RulesProviderReturnsOnlyStringLengthRuleForFirstName()
        {
            var rulesProvider = new ValidatorRulesProvider();

            var allRules = rulesProvider.GetRulesFromType(typeof(ValidatableClass));
            
            var firstNameRules = allRules["FirstName"];

            var firstNameRulesList = firstNameRules.ToList();
            
            Assert.AreEqual(1, firstNameRulesList.Count);
            Assert.AreEqual("StringLength", firstNameRulesList[0].RuleName);
        }

        [TestMethod]
        public void RulesProviderReturnsTwoRulesForLastName()
        {
            var rulesProvider = new ValidatorRulesProvider();

            var allRules = rulesProvider.GetRulesFromType(typeof(ValidatableClass));

            var lastNameRules = allRules["LastName"];

            var lastNameRulesList = lastNameRules.OrderBy(x=>x.RuleName).ToList();

            Assert.AreEqual(2, lastNameRulesList.Count);

            Assert.AreEqual("Required", lastNameRulesList[0].RuleName);
            Assert.AreEqual("StringLength", lastNameRulesList[1].RuleName);
        }

        [TestMethod]
        public void RulesProviderReturnsCorrectMessageForAge()
        {
            var rulesProvider = new ValidatorRulesProvider();

            var allRules = rulesProvider.GetRulesFromType(typeof(ValidatableClass));

            var ageRules = allRules["Age"];

            var ageRulesList = ageRules.OrderBy(x => x.RuleName).ToList();

            Assert.AreEqual(1, ageRulesList.Count);

            Assert.AreEqual("You can't be that old or young!", ageRulesList[0].ErrorMessage);
        }

        internal class ValidatableClass
        {
            [Length(20)]
            public string FirstName { get; set; }

            [Length(50)]
            [Required]
            public string LastName { get; set; }

            [Range(1, 150, "You can't be that old or young!")]
            public int Age { get; set; }

            [NotNullNotEmpty]
            public IList<string> List { get; set; }

            public string DummyProperty { get; set; }
        }

    }
}