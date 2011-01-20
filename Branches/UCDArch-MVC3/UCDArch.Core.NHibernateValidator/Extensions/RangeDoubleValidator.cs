using System;
using NHibernate.Validator.Engine;

namespace UCDArch.Core.NHibernateValidator.Extensions
{
    [Serializable]
    public class RangeDoubleValidator : IInitializableValidator<RangeDoubleAttribute>
    {
        private double _max;
        private double _min;

        #region IInitializableValidator<RangeAttribute> Members

        public void Initialize(RangeDoubleAttribute parameters)
        {
            _max = parameters.Max;
            _min = parameters.Min;
        }

        public bool IsValid(object value, IConstraintValidatorContext validatorContext)
        {
            if (value == null)
            {
                return true;
            }

            try
            {
                double cvalue = Convert.ToDouble(value);
                return cvalue >= _min && cvalue <= _max;
            }
            catch (InvalidCastException)
            {
                if (value is char)
                {
                    int i = Convert.ToInt32(value);
                    return i >= _min && i <= _max;
                }
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        #endregion
    }

}