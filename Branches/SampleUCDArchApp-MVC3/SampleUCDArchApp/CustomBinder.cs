using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;

namespace SampleUCDArchApp
{
    public class CustomBinder : DefaultModelBinder
    {
        /// <summary>
        /// Called when the model is updating. We handle updating the Id property here because it gets filtered out
        /// of the normal MVC2 property binding.
        /// </summary>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param>
        /// <param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param>
        /// <returns>
        /// true if the model is updating; otherwise, false.
        /// </returns>
        protected override bool OnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (IsEntityType(bindingContext.ModelType))
            {
                //handle the Id property
                PropertyDescriptor idProperty =
                    (from PropertyDescriptor property in TypeDescriptor.GetProperties(bindingContext.ModelType)
                     where property.Name == ID_PROPERTY_NAME
                     select property).SingleOrDefault();

                BindProperty(controllerContext, bindingContext, idProperty);

            }
            return base.OnModelUpdating(controllerContext, bindingContext);
        }

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            Type propertyType = propertyDescriptor.PropertyType;

            if (IsEntityType(propertyType))
            {
                //use the EntityValueBinder
                return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, new EntityValueBinder());
            }

            if (IsSimpleGenericBindableEntityCollection(propertyType))
            {
                //use the EntityValueCollectionBinder
                return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, new EntityCollectionValueBinder());
            }

            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }

        protected override void SetProperty(ControllerContext controllerContext,
            ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            if (propertyDescriptor.Name == ID_PROPERTY_NAME)
            {
                SetIdProperty(bindingContext, propertyDescriptor, value);
            }
            else if (value as IEnumerable != null && IsSimpleGenericBindableEntityCollection(propertyDescriptor.PropertyType))
            {
                SetEntityCollectionProperty(bindingContext, propertyDescriptor, value);
            }
            else
            {
                base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
            }

        }

        private static bool IsEntityType(Type propertyType)
        {
            bool isEntityType = propertyType.GetInterfaces()
                .Any(type => type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

            return isEntityType;
        }

        private static bool IsSimpleGenericBindableEntityCollection(Type propertyType)
        {
            bool isSimpleGenericBindableCollection =
                propertyType.IsGenericType &&
                (propertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(Iesi.Collections.Generic.ISet<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.ISet<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            bool isSimpleGenericBindableEntityCollection =
                isSimpleGenericBindableCollection && IsEntityType(propertyType.GetGenericArguments().First());

            return isSimpleGenericBindableEntityCollection;
        }

        /// <summary>
        /// If the property being bound is an Id property, then use reflection to get past the 
        /// protected visibility of the Id property, accordingly.
        /// </summary>
        private static void SetIdProperty(ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, object value)
        {
            Type idType = propertyDescriptor.PropertyType;
            object typedId = Convert.ChangeType(value, idType);

            // First, look to see if there's an Id property declared on the entity itself; 
            // e.g., using the new keyword
            PropertyInfo idProperty = bindingContext.ModelType
                .GetProperty(propertyDescriptor.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            // If an Id property wasn't found on the entity, then grab the Id property from
            // the entity base class
            if (idProperty == null)
            {
                idProperty = bindingContext.ModelType
                    .GetProperty(propertyDescriptor.Name,
                        BindingFlags.Public | BindingFlags.Instance);
            }

            // Set the value of the protected Id property
            idProperty.SetValue(bindingContext.Model, typedId, null);
        }


        /// <summary>
        /// If the property being bound is a simple, generic collection of entiy objects, then use 
        /// reflection to get past the protected visibility of the collection property, if necessary.
        /// </summary>
        private static void SetEntityCollectionProperty(ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, object value)
        {
            object entityCollection = propertyDescriptor.GetValue(bindingContext.Model);
            if (entityCollection != value)
            {
                Type entityCollectionType = entityCollection.GetType();

                foreach (object entity in (value as IEnumerable))
                {
                    entityCollectionType.InvokeMember("Add",
                                                      BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, entityCollection,
                                                      new object[] { entity });
                }
            }
        }

        #region Overridable (but not yet overridden) Methods

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return base.BindModel(controllerContext, bindingContext);
        }

        protected override object CreateModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext, Type modelType)
        {

            return base.CreateModel(controllerContext, bindingContext, modelType);
        }

        protected override void BindProperty(ControllerContext controllerContext,
    ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }

        protected override void OnPropertyValidated(ControllerContext controllerContext,
            ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {
            base.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, value);
        }

        protected override bool OnPropertyValidating(ControllerContext controllerContext,
            ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
        {

            return base.OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, value);
        }

        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            base.OnModelUpdated(controllerContext, bindingContext);
        }
        #endregion

        private const string ID_PROPERTY_NAME = "Id";
    }

    class EntityValueBinder : CustomBinder
    {
        #region Implementation of IModelBinder

        /// <summary>
        /// Binds the model value to an entity by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound value.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param><param name="bindingContext">The binding context.</param>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type modelType = bindingContext.ModelType;

            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != null)
            {
                Type entityInterfaceType = modelType.GetInterfaces()
                    .First(interfaceType => interfaceType.IsGenericType
                                            && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

                Type idType = entityInterfaceType.GetGenericArguments().First();
                string rawId = (valueProviderResult.RawValue as string[]).First();

                if (string.IsNullOrEmpty(rawId))
                    return null;

                try
                {
                    object typedId =
                        (idType == typeof(Guid))
                            ? new Guid(rawId)
                            : Convert.ChangeType(rawId, idType);

                    return ValueBinderHelper.GetEntityFor(modelType, typedId, idType);
                }
                // If the Id conversion failed for any reason, just return null
                catch (Exception)
                {
                    return null;
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }

        #endregion
    }

    class EntityCollectionValueBinder : DefaultModelBinder
    {
        #region Implementation of IModelBinder

        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound value.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param><param name="bindingContext">The binding context.</param>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type collectionType = bindingContext.ModelType;
            Type collectionEntityType = collectionType.GetGenericArguments().First();

            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != null)
            {
                int countOfEntityIds = (valueProviderResult.RawValue as string[]).Length;
                Array entities = Array.CreateInstance(collectionEntityType, countOfEntityIds);

                Type entityInterfaceType = collectionEntityType.GetInterfaces()
                    .First(interfaceType => interfaceType.IsGenericType
                                            && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

                Type idType = entityInterfaceType.GetGenericArguments().First();

                for (int i = 0; i < countOfEntityIds; i++)
                {
                    string rawId = (valueProviderResult.RawValue as string[])[i];

                    if (string.IsNullOrEmpty(rawId))
                        return null;

                    object typedId =
                        (idType == typeof(Guid))
                            ? new Guid(rawId)
                            : Convert.ChangeType(rawId, idType);

                    object entity = ValueBinderHelper.GetEntityFor(collectionEntityType, typedId, idType);
                    entities.SetValue(entity, i);
                }

                //base.BindModel(controllerContext, bindingContext);
                return entities;
            }
            return base.BindModel(controllerContext, bindingContext);
        }

        #endregion
    }

    internal static class ValueBinderHelper
    {
        internal static object GetEntityFor(Type collectionEntityType, object typedId, Type idType)
        {
            object entityRepository = GenericRepositoryFactory.CreateEntityRepositoryFor(collectionEntityType, idType);

            return entityRepository.GetType()
                .InvokeMember("GetById", BindingFlags.InvokeMethod, null, entityRepository, new[] { typedId });
        }
    }

    internal class GenericRepositoryFactory
    {
        public static object CreateEntityRepositoryFor(Type entityType, Type idType)
        {
            Type genericRepositoryType = typeof(IRepositoryWithTypedId<,>);
            Type concreteRepositoryType =
                genericRepositoryType.MakeGenericType(new Type[] { entityType, idType });

            object repository;

            try
            {
                repository = ServiceLocator.Current.GetService(concreteRepositoryType);
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("ServiceLocator has not been initialized; " +
                    "I was trying to retrieve " + concreteRepositoryType.ToString());
            }
            catch (ActivationException)
            {
                throw new ActivationException("The needed dependency of type " + concreteRepositoryType.Name +
                    " could not be located with the ServiceLocator. You'll need to register it with " +
                    "the Common Service Locator (CSL) via your IoC's CSL adapter.");
            }

            return repository;
        }
    }
}