using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace UCDArch.Web.ModelBinder
{
    public class EntityModelBinder : IModelBinder
    {
        private readonly Type _entityType;
        private readonly IModelBinder _fallbackBinder;

        public EntityModelBinder(Type entityType, IModelBinder fallbackBinder)
        {
            _entityType = entityType;
            _fallbackBinder = fallbackBinder;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (!string.IsNullOrWhiteSpace(valueProviderResult.FirstValue) && Regex.IsMatch(valueProviderResult.FirstValue, @"^[a-zA-Z0-9_\-]+$"))
            {
                var entity = ValueBinderHelper.GetEntity(_entityType, valueProviderResult.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(entity);
            }
            else
            {
                await _fallbackBinder.BindModelAsync(bindingContext);
            }
        }
    }
}
