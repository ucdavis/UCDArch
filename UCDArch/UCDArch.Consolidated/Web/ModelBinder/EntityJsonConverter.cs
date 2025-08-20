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
                var entity = ValueBinderHelper.GetEntity(objectType, id);
                return entity;
            }
            else if (token.Type == JTokenType.Array)
            {
                var ids = token.ToObject<List<string>>();
                var entities = ValueBinderHelper.GetEntityCollection(objectType, ids);
                return entities;
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return ValueBinderHelper.IsEntityType(objectType) || ValueBinderHelper.IsSimpleGenericBindableEntityCollection(objectType);
        }

    }
}