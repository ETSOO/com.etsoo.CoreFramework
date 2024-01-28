using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// PII sensitive data converter, may also look JsonConverterFactory
    /// PII 敏感数据转换器
    /// </summary>
    public class PIIConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue("***");
        }
    }
}
