using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UCDArch.Core.CommonValidator;

namespace UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter
{
    public class Validator : IValidator
    {
        public bool IsValid(object value)
        {
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var validationContext = new ValidationContext(value, null, null);
            return System.ComponentModel.DataAnnotations.Validator.TryValidateObject(value, validationContext, results, true);
        }

        public ICollection<IValidationResult> ValidationResultsFor(object value)
        {
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var validationContext = new ValidationContext(value, null, null);
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(value, validationContext, results, true);

            var classContextType = value.GetType();
            return results.Select(validationResult => new ValidationResult
                                                          {
                                                              ClassContext = classContextType, 
                                                              Message = validationResult.ErrorMessage, 
                                                              PropertyName = validationResult.MemberNames.FirstOrDefault() ?? "UnknownField"
                                                          }).Cast<IValidationResult>().ToList();
        }
    }
}
