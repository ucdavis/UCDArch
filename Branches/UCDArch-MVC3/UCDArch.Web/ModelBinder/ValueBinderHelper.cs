using System;
using System.Reflection;

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
    }
}