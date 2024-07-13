using Chen.Commons.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace System
{
    public static class JsonExtentions
    {
        //如果不设置这个，那么"雅思真题"就会保存为"\u96C5\u601D\u771F\u9898"
        public readonly static JavaScriptEncoder Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        public static JsonSerializerOptions CreateJsonSerializerOptions(bool camelCase = false)
        {
            JsonSerializerOptions opt = new JsonSerializerOptions { Encoder = Encoder };
            if (camelCase)
            {
                opt.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }
            opt.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            return opt;
        }

        public static string ToJsonString(this object value,bool camelCase = false)
        {
            var opt = CreateJsonSerializerOptions(camelCase);
            return JsonSerializer.Serialize(value,value.GetType(), opt);    
        }

        public static T? ParseJson<T>(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T?);
            }
            var opt = CreateJsonSerializerOptions(true);
            return JsonSerializer.Deserialize<T>(value,opt);
        }


        public static string ToUpperOfJsonKey(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            // 将 JSON 字符串解析为字典
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(value);

            // 创建一个新的字典，用于储存转换后的键值对。
            var convertedDictionary = new Dictionary<string, object>();

            // 输出字典的所有键
            foreach (var keyValuePair in dictionary)
            {
                convertedDictionary.Add(UpperCaseFirstLetter(keyValuePair.Key), keyValuePair.Value) ;
            }

            // 将新字典序列化为字符串 
            return JsonSerializer.Serialize(convertedDictionary);
        }

        private static string UpperCaseFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char[] chars = input.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            return new string(chars);
        }
    }
}
