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
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(StringKeyDictionaryObject))]
    [JsonSerializable(typeof(StringKeyDictionaryString))]
    [JsonSerializable(typeof(Dictionary<string, object>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(IEnumerable<GuidItem>))]
    [JsonSerializable(typeof(IEnumerable<IdItem>))]
    [JsonSerializable(typeof(IEnumerable<IdLabelItem>))]
    [JsonSerializable(typeof(ActionResult))]
    public partial class CommonJsonSerializerContext : JsonSerializerContext
    {
    }
}