using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UCDArch.Core.DomainModel;

namespace UCDArch.Web.ModelBinder
{
    internal static class ValueBinderHelper
    {
        internal static object GetEntityFor(Type collectionEntityType, object typedId, Type idType)
        {
            object entityRepository = GenericRepositoryFactory.CreateEntityRepositoryFor(collectionEntityType, idType);

            return entityRepository.GetType()
                .InvokeMember(RepositoryGetMethodName, BindingFlags.InvokeMethod, null, entityRepository, new[] { typedId });
        }

        private const string RepositoryGetMethodName = "GetById";


        internal static bool IsEntityType(Type propertyType)
        {
            bool isEntityType = propertyType.GetInterfaces()
                .Any(type => type.IsGenericType &&
                             type.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

            return isEntityType;
        }
        
        internal static bool IsSimpleGenericBindableEntityCollection(Type propertyType)
        {
            bool isSimpleGenericBindableCollection =
                propertyType.IsGenericType &&
                (propertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(ISet<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            bool isSimpleGenericBindableEntityCollection =
                isSimpleGenericBindableCollection && IsEntityType(propertyType.GetGenericArguments().First());

            return isSimpleGenericBindableEntityCollection;
        }        

        internal static object GetEntity(Type modelType, string rawId)
        {
            Type entityInterfaceType = modelType.GetInterfaces()
                .First(interfaceType => interfaceType.IsGenericType
                                        && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

            Type idType = entityInterfaceType.GetGenericArguments().First();

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
            }

            return null;
        }

        internal static object GetEntityCollection(Type collectionType, IEnumerable<string> rawIds)
        {
            Type collectionEntityType = collectionType.GetGenericArguments().First();

            int countOfEntityIds = rawIds.Count();
            Array entities = Array.CreateInstance(collectionEntityType, countOfEntityIds);

            Type entityInterfaceType = collectionEntityType.GetInterfaces()
                .First(interfaceType => interfaceType.IsGenericType
                                        && interfaceType.GetGenericTypeDefinition() == typeof(IDomainObjectWithTypedId<>));

            Type idType = entityInterfaceType.GetGenericArguments().First();

            var i = 0;
            foreach (var rawId in rawIds)
            {
                if (string.IsNullOrEmpty(rawId))
                {
                    return null;
                }

                object typedId =
                    (idType == typeof(Guid))
                        ? new Guid(rawId)
                        : Convert.ChangeType(rawId, idType);

                object entity = ValueBinderHelper.GetEntityFor(collectionEntityType, typedId, idType);
                entities.SetValue(entity, i);
                i++;
            }

            return entities;
        }

    }
    
}