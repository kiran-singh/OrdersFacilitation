using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OrdersApi.Services
{
    public class RemoveSensitiveDataContractResolver : DefaultContractResolver
    {
        public static readonly List<string> SensitivePropertyNames = new List<string>()
        {
            "password",
            "token",
            "account"
        };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (RemoveSensitiveDataContractResolver.SensitivePropertyNames.Contains(property.PropertyName.ToLower()))
                property.ShouldSerialize = (Predicate<object>)(instance => false);
            return property;
        }

    }
}