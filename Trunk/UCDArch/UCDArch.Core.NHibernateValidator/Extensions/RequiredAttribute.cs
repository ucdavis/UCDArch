using System;
using NHibernate.Validator.Engine;

namespace UCDArch.Core.NHibernateValidator.Extensions
{
    /// <summary>
    /// For string and string IEnumerable.
    /// Valid strings may not be null, empty, or only spaces.
    /// Valid lists must not be empty.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [ValidatorClass(typeof(RequiredValidator))]
    public class RequiredAttribute : Attribute, IRuleArgs
    {
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private string _message = "{validator.notNullNotEmpty}"; //Use the not empty language by default
    }
}