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
    }
    
}