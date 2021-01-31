using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Buildersoft.Andy.X.Client.Extensions
{
    public static class JsonSerialization
    {

        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(object), new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
            });
        }

        public static string ObjectToJson<TClass>(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(TClass), new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
            });
        }

        public static string TryObjectToJson<TClass>(this object obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, typeof(TClass), new JsonSerializerOptions()
                {
                    IgnoreNullValues = true,
                    IgnoreReadOnlyProperties = true,
                });
            }
            catch (Exception)
            {
                return "{}";
            }
        }

        public static TClass JsonToObject<TClass>(this string jsonMessage)
        {
            return (TClass)(JsonSerializer.Deserialize(jsonMessage, typeof(TClass), new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
            }));
        }

        public static dynamic JsonToDynamic(this string jsonMessage, Type type)
        {
            return (dynamic)(JsonSerializer.Deserialize(jsonMessage, type, new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true,
            }));
        }

        public static TClass TryJsonToObject<TClass>(this string jsonMessage)
        {
            try
            {
                return (TClass)JsonSerializer.Deserialize(jsonMessage, typeof(TClass), new JsonSerializerOptions()
                {
                    IgnoreNullValues = true,
                    IgnoreReadOnlyProperties = true,
                });
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
