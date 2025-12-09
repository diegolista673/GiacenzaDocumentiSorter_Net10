using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace GiacenzaSorterRm.Helpers
{
    public static class TempDataHelper
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T? Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out object? o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static T? Peek<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object? o = tempData.Peek(key);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static void Put<T>(this TempDataDictionary tempData, T value) where T : class
        {
            string? fullName = typeof(T).FullName;
            if (fullName != null)
            {
                tempData[fullName] = value;
            }
        }

        public static void Put<T>(this TempDataDictionary tempData, string key, T value) where T : class
        {
            string? fullName = typeof(T).FullName;
            if (fullName != null)
            {
                tempData[fullName + key] = value;
            }
        }

        public static T? Get<T>(this TempDataDictionary tempData) where T : class
        {
            string? fullName = typeof(T).FullName;
            if (fullName != null && tempData.TryGetValue(fullName, out object? o))
            {
                return o as T;
            }
            return null;
        }

        public static T? Get<T>(this TempDataDictionary tempData, string key) where T : class
        {
            string? fullName = typeof(T).FullName;
            if (fullName != null && tempData.TryGetValue(fullName + key, out object? o))
            {
                return o as T;
            }
            return null;
        }
    }
}
