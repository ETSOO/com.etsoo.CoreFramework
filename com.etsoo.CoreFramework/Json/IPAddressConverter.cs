using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Json
{
    /// <summary>
    /// IP Address to JSON converter
    /// IP地址到JSON转换器
    /// </summary>
    public class IPAddressConverter : JsonConverter<IPAddress>
    {
        public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // https://github.com/dotnet/runtime/issues/44006
            return IPAddress.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
