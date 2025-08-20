using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UCDArch.Core;
using UCDArch.Core.CommonValidator;
using UCDArch.Core.Utils;

namespace UCDArch.Web.Validator
{
    public class MvcValidationAdapter
    {
        public static ModelStateDictionary TransferValidationMessagesTo(
            ModelStateDictionary modelStateDictionary,
            IEnumerable<IValidationResult> validationResults)
        {

            return TransferValidationMessagesTo(null, modelStateDictionary, validationResults);
        }

        /// <summary>
        /// This acts as a more "manual" alternative to moving validation errors to the 
        /// <see cref="ModelStateDictionary" /> if you use the default ctor of <see cref="UCDArchModelBinder" />.
        /// If you use the autoValidate overload of the <see cref="UCDArchModelBinder"/> then you usually wouldn't
        /// need to move these values.
        /// </summary>
        /// <param name="keyBase">If supplied, will be used as the model state prefix
        /// instead of the class name</param>
        /// <param name="modelStateDictionary"></param>
        /// <param name="validationResults">Collection of validation results using the IValidationResult interface</param>
        public static ModelStateDictionary TransferValidationMessagesTo(
            string keyBase, ModelStateDictionary modelStateDictionary,
            IEnumerable<IValidationResult> validationResults)
        {

            Check.Require(modelStateDictionary != null, "modelStateDictionary may not be null");
            Check.Require(validationResults != null, "invalidValues may not be null");

            foreach (IValidationResult validationResult in validationResults)
            {
                Check.Require(validationResult.ClassContext != null,
                    "validationResult.ClassContext may not be null");

                string key = (keyBase ?? validationResult.ClassContext.Name) +
                    (!string.IsNullOrEmpty(validationResult.PropertyName)
                        ? "." + validationResult.PropertyName
                        : "");

                modelStateDictionary.AddModelError(key, validationResult.Message);
                modelStateDictionary.SetModelValue(key, new ValueProviderResult());
            }

            return modelStateDictionary;
        }

        public static ICollection<IValidationResult> GetValidationResultsFor(object obj)
        {
            var validator = SmartServiceLocator<IValidator>.GetService();

            return validator.ValidationResultsFor(obj);
        }
    }
}