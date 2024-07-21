using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.String;
using System.Text.Json.Serialization;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Common JSON serializer context
    /// 通用 JSON 序列化器上下文
    /// </summary>
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)
    ]
    [JsonSerializable(typeof(StringKeyDictionaryObject))]
    [JsonSerializable(typeof(StringKeyDictionaryString))]
    [JsonSerializable(typeof(Dictionary<string, object>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(IDictionary<string, string>))]
    [JsonSerializable(typeof(IEnumerable<GuidItem>))]
    [JsonSerializable(typeof(IEnumerable<IdItem>))]
    [JsonSerializable(typeof(IEnumerable<IdLabelItem>))]
    [JsonSerializable(typeof(IEnumerable<long>))]
    [JsonSerializable(typeof(IEnumerable<ulong>))]
    [JsonSerializable(typeof(IEnumerable<int>))]
    [JsonSerializable(typeof(IEnumerable<uint>))]
    [JsonSerializable(typeof(IEnumerable<short>))]
    [JsonSerializable(typeof(IEnumerable<ushort>))]
    [JsonSerializable(typeof(IEnumerable<byte>))]
    [JsonSerializable(typeof(IEnumerable<Guid>))]
    [JsonSerializable(typeof(IEnumerable<string>))]
    [JsonSerializable(typeof(IEnumerable<double>))]
    [JsonSerializable(typeof(IEnumerable<decimal>))]
    [JsonSerializable(typeof(IEnumerable<float>))]
    [JsonSerializable(typeof(IActionResult))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(DateTime))]
    [JsonSerializable(typeof(DateTimeOffset))]
    public partial class CommonJsonSerializerContext : JsonSerializerContext
    {
    }
}