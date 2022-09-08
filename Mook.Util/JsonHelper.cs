using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;

namespace Mook.Util
{
    /// <summary>
    /// JSON数据帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// JSON格式字符转换为T类型的对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <summary>
        /// JSON格式字符转换为Object对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        public static object ToObject(string jsonStr)
        {
            return JsonConvert.DeserializeObject(jsonStr);
        }

        /// <summary>
        /// Object对象转换为JSON格式数据
        /// </summary>
        /// <param name="obj">object对象</param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy/MM/dd HH:mm:ss",
                ContractResolver = new CamelCasePropertyNamesContractResolver()               
            };
            settings.Converters.Add(new LongJsonConverter());
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// JSON格式字符转换为JObject对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        public static JObject ToJObject(string jsonStr)
        {
            return JsonConvert.DeserializeObject<JObject>(jsonStr);
        }

        /// <summary>
        /// JSON格式字符转换为键值对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(string jsonStr)
        {
            return JsonConvert.DeserializeObject<IDictionary<string, string>>(jsonStr);
        }

        /// <summary>
        /// JSON格式字符转换为T类型列表
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        public static List<T> ToList<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<List<T>>(jsonStr);
        }

        /// <summary>
        /// List集合转换为DataTable对象
        /// </summary>
        /// <param name="list">List集合</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> list)
        {
            return JsonConvert.DeserializeObject<DataTable>(ToJson(list));
        }
    }

    public class LongJsonConverter : JsonConverter<long>
    {
        public override long ReadJson(JsonReader reader, Type objectType, long existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var flag = long.TryParse(reader.Value.ToString(), out long num);
            return flag == true ? num : 0;
        }

        public override void WriteJson(JsonWriter writer, long value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}