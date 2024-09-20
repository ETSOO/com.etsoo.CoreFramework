using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Json
{
    /// <summary>
    /// Culture info to JSON converter
    /// 文化信息到JSON转换器
    /// </summary>
    public class CultureInfoConverter : JsonConverter<CultureInfo>
    {
        public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new CultureInfo(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}
