using System;
using System.Text.Json;

namespace Andy.X.Client.Extensions
{
    public static class Json
    {
        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(object), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = true,
            });
        }
        public static string ObjectToJson<T>(this object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(T), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = true,
            });
        }

        public static string TryObjectToJson<T>(this object obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, typeof(T), new JsonSerializerOptions()
                {

                    PropertyNameCaseInsensitive = true,
                    IgnoreReadOnlyProperties = true,
                });
            }
            catch (Exception)
            {
                return "{}";
            }

        }

        public static T JsonToObject<T>(this string jsonMessage)
        {
            return (T)(JsonSerializer.Deserialize(jsonMessage, typeof(T), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = true,
            }));
        }

        public static dynamic JsonToDynamic(this string jsonMessage, Type type)
        {
            return (JsonSerializer.Deserialize(jsonMessage, type, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = true,
            })) as dynamic;
        }

        public static T TryJsonToObject<T>(this string jsonMessage)
        {
            try
            {
                return (T)JsonSerializer.Deserialize(jsonMessage, typeof(T), new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    IgnoreReadOnlyProperties = true,
                });
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
