using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace UCDArch.Web.ModelBinder
{
    public class EntityModelBinderProvider : IModelBinderProvider
    {
        private readonly ComplexObjectModelBinderProvider _fallbackBinderProvider;

        public EntityModelBinderProvider()
        {
            _fallbackBinderProvider = new ComplexObjectModelBinderProvider();
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (ValueBinderHelper.IsEntityType(context.Metadata.ModelType))
            {
                // A fallback is necessary because no further IModelBinderProviders are considered once a binder is found
                return new EntityModelBinder(context.Metadata.ModelType, _fallbackBinderProvider.GetBinder(context));
            }

            return null;
        }
    }
}
