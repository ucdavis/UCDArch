using System.Collections.Generic;
using System.Linq;
using UCDArch.Core.CommonValidator;

namespace UCDArch.Testing.Extensions
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// Extended the Validation results building a list with the PropertyName and Error.
        /// </summary>
        /// <param name="validationResults">The validation results.</param>
        /// <returns></returns>
        public static List<string> AsMessageList(this IEnumerable<IValidationResult> validationResults)
        {
            return validationResults.Select(result => result.Message).ToList();
        }
    }
}