using System.Collections.Generic;

namespace UCDArch.Core.CommonValidator
{
    /// <summary>
    /// Interface which defines a class to be validatable 
    /// </summary>
    public interface IValidatable
    {
        bool IsValid();
        ICollection<IValidationResult> ValidationResults();
    }
}