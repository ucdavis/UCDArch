using System.Collections.Generic;
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
            var resultsList = new List<string>();

            foreach (var result in validationResults)
            {
                resultsList.Add(string.Format("{0}: {1}", result.PropertyName, result.Message));
            }

            return resultsList;
        }
    }
}