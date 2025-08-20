using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UCDArch.Core.DomainModel;

namespace UCDArch.Web.ModelBinder
{

    public class EntityJsonContractResolver : DefaultContractResolver
    {

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            if (member is PropertyInfo property)
            {
                if (ValueBinderHelper.IsEntityType(property.PropertyType) || ValueBinderHelper.IsSimpleGenericBindableEntityCollection(property.PropertyType))
                {
                    jsonProperty.Converter = new EntityJsonConverter();
                }
            }

            return jsonProperty;
        }



    }
}