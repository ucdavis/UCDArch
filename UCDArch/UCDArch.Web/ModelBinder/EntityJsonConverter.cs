using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UCDArch.Core.DomainModel;

namespace UCDArch.Web.ModelBinder
{

    public class EntityJsonConverter : JsonConverter
    {
        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                var id = token.ToString();
                var entity = GetEntity(objectType, id);
                return entity;
            }
            else if (token.Type == JTokenType.Array)
            {
                var ids = token.ToObject<List<string>>();
                var entities = GetEntityCollection(objectType, ids);
                return entities;
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return ValueBinderHelper.IsEntityType(objectType) || ValueBinderHelper.IsSimpleGenericBindableEntityCollection(objectType);
        }

        private static object GetEntity(Type modelType, string rawId)
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

        private static object GetEntityCollection(Type collectionType, List<string> rawIds)
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