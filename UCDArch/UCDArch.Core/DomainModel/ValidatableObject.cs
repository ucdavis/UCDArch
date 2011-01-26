using System;
using System.Collections.Generic;
using UCDArch.Core.CommonValidator;

namespace UCDArch.Core.DomainModel
{
    /// <summary>
    /// Provides an implementation of IValidatable which provides a validatable base object
    /// </summary>
    [Serializable]
    public abstract class ValidatableObject : BaseObject, IValidatable
    {
        public virtual bool IsValid()
        {
            return Validator.IsValid(this);
        }

        public virtual ICollection<IValidationResult> ValidationResults()
        {
            return Validator.ValidationResultsFor(this);
        }

        private static IValidator Validator
        {
            get
            {
                return SmartServiceLocator<IValidator>.GetService();
            }
        }
    }
}