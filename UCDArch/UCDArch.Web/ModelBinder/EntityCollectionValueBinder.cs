using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UCDArch.Core.DomainModel;

namespace UCDArch.Web.ModelBinder
{
    class EntityCollectionValueBinder : IModelBinder
    {
        #region Implementation of IModelBinder

        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound value.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param><param name="bindingContext">The binding context.</param>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Type collectionType = bindingContext.ModelType;
            Type collectionEntityType = collectionType.GetGenericArguments().First();

            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != null)
            {
                int countOfEntityIds = valueProviderResult.Count();
                Array entities = Array.CreateInstance(collectionEntityType, countOfEntityIds);

                Type entityInterfaceType = collectionEntityType.GetInterfaces()
                    .First(interfaceType => interfaceType.IsGenericType
                                            && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

                Type idType = entityInterfaceType.GetGenericArguments().First();

                var i = 0;
                foreach (var rawId in valueProviderResult)
                {
                    if (string.IsNullOrEmpty(rawId))
                    {
                        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, ValueProviderResult.None);
                        return Task.CompletedTask;
                    }

                    object typedId =
                        (idType == typeof(Guid))
                            ? new Guid(rawId)
                            : Convert.ChangeType(rawId, idType);

                    object entity = ValueBinderHelper.GetEntityFor(collectionEntityType, typedId, idType);
                    entities.SetValue(entity, i);
                    i++;
                }

                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            }
                return Task.CompletedTask;
        }

        #endregion
    }
}