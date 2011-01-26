using System;
using UCDArch.Core.CommonValidator;

namespace UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter
{
    public class ValidationResult : IValidationResult
    {
        public Type ClassContext { get; set; }

        public string PropertyName { get; set; }

        public string Message { get; set; }
    }
}
