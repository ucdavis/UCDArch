using System;
using NHibernate.Validator.Engine;

namespace UCDArch.Core.NHibernateValidator.Extensions
{
    /// <summary>
    /// The annotated elemnt has to be in the appropriate range. Apply on numeric values or string
    /// representation of the numeric value.
    /// A null value is valid
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [ValidatorClass(typeof(RangeDoubleValidator))]
    public class RangeDoubleAttribute : Attribute, IRuleArgs
    {
        private double _max = double.MaxValue;
        private string _message = "{validator.range}";
        private double _min = double.MinValue;

        public RangeDoubleAttribute(double min, double max)
        {
            _min = min;
            _max = max;
        }

        public RangeDoubleAttribute(double min, double max, string message)
        {
            _min = min;
            _max = max;
            _message = message;
        }

        public RangeDoubleAttribute() { }

        public double Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public double Max
        {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// Returns either the specified min double as a decimal or the min value of the decimal type
        /// </summary>
        public decimal MinAsDecimal
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(Min); 
                }
                catch(OverflowException)
                {
                    return decimal.MinValue;
                }
            }
        }

        /// <summary>
        /// Returns either the specified max double as a decimal or the max value of the decimal type
        /// </summary>
        public decimal MaxAsDecimal
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(Max);
                }
                catch (OverflowException)
                {
                    return decimal.MaxValue;
                }
            }
        }

        #region IRuleArgs Members

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        #endregion
    }
}