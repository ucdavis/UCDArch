using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;
using UCDArch.Core.NHibernateValidator.Extensions;
using xVal.RuleProviders;
using xVal.Rules;

namespace UCDArch.Web.Validator
{
    public class ValidatorRulesProvider : CachingRulesProvider
    {
        private readonly RuleEmitterList<IRuleArgs> _ruleEmitters;

        public ValidatorRulesProvider()
        {
            _ruleEmitters = new RuleEmitterList<IRuleArgs>();

            _ruleEmitters.AddSingle<LengthAttribute>(x => new StringLengthRule(x.Min, x.Max));
            _ruleEmitters.AddSingle<MinAttribute>(x => new RangeRule(x.Value, null));
            _ruleEmitters.AddSingle<MaxAttribute>(x => new RangeRule(null, x.Value));
            _ruleEmitters.AddSingle<RangeAttribute>(x => new RangeRule(x.Min, x.Max));
            _ruleEmitters.AddSingle<RangeDoubleAttribute>(x => new RangeRule(x.MinAsDecimal, x.MaxAsDecimal));
            _ruleEmitters.AddSingle<NotEmptyAttribute>(x => new RequiredRule());
            _ruleEmitters.AddSingle<NotNullNotEmptyAttribute>(x => new RequiredRule());
            _ruleEmitters.AddSingle<RequiredAttribute>(x => new RequiredRule());
            _ruleEmitters.AddSingle<NotNullAttribute>(x => new RequiredRule());
            _ruleEmitters.AddSingle<PatternAttribute>(x => new RegularExpressionRule(x.Regex, x.Flags));
            _ruleEmitters.AddSingle<EmailAttribute>(x => new DataTypeRule(DataTypeRule.DataType.EmailAddress));
            _ruleEmitters.AddSingle<DigitsAttribute>(MakeDigitsRule);
        }

        protected override RuleSet GetRulesFromTypeCore(Type type)
        {
            var classMapping = new ValidatorEngine().GetClassValidator(type);

            var rules = from member in type.GetMembers()
                        where member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property
                        from constraint in classMapping.GetMemberConstraints(member.Name).OfType<IRuleArgs>()
                        // All NHibernate Validation validators attributes must implement this interface
                        from rule in ConvertToXValRules(constraint)
                        where rule != null
                        select new { MemberName = member.Name, Rule = rule };

            return new RuleSet(rules.ToLookup(x => x.MemberName, x => x.Rule));
        }

        private IEnumerable<Rule> ConvertToXValRules(IRuleArgs ruleArgs)
        {
            foreach (var rule in _ruleEmitters.EmitRules(ruleArgs))
            {
                if (rule != null)
                {
                    rule.ErrorMessage = MessageIfSpecified(ruleArgs.Message);
                    yield return rule;
                }
            }
        }

        private static RegularExpressionRule MakeDigitsRule(DigitsAttribute att)
        {
            if (att == null) throw new ArgumentNullException("att");
            string pattern;
            if (att.FractionalDigits < 1)
                pattern = string.Format(@"\d{{0,{0}}}", att.IntegerDigits);
            else
                pattern = string.Format(@"\d{{0,{0}}}(\.\d{{1,{1}}})?", att.IntegerDigits, att.FractionalDigits);
            return new RegularExpressionRule(pattern);
        }

        private static string MessageIfSpecified(string message)
        {
            // We don't want to display the default {validator.*} messages
            if ((message != null) && !message.StartsWith("{validator."))
                return message;
            return null;
        }
    }
}