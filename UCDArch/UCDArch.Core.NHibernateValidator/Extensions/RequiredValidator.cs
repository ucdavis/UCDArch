using System.Collections;
using NHibernate.Validator.Engine;

namespace UCDArch.Core.NHibernateValidator.Extensions
{
    /// <summary>
    /// Required Validator means the value can not be null or empty (or only spaces)
    /// </summary>
    public class RequiredValidator : IValidator
    {
        public bool IsValid(object value, IConstraintValidatorContext constraintValidatorContext)
        {
            if (value == null)
            {
                return false;
            }

            var check = value as string;
            if (check != null)
            {
                return !string.Empty.Equals(check.Trim());
            }

            var ev = value as IEnumerable;
            if (ev != null)
            {
                return ev.GetEnumerator().MoveNext();
            }

            return false;
        }
    }
}