using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chen.Commons.JsonConverters
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        private readonly string _dateFormatString;
        public DateTimeJsonConverter()
        {
            _dateFormatString = "yyyy-MM-dd HH:mm:ss";
        }
        public DateTimeJsonConverter(string dateFormatString)
        {
            _dateFormatString = dateFormatString;
        }
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? str = reader.GetString();
            if (str == null)
            {
                return default;
            }
            else
            {
                return DateTime.Parse(str);
            }
        }   

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // 固定使用服务器所在时区，前端如果想适应用户时区自行调整。
            writer.WriteStringValue(value.ToString(_dateFormatString));
        }
    }
}
