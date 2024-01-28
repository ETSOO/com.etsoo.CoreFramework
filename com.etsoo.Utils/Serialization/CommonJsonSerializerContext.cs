using com.etsoo.Utils.Models;
using com.etsoo.Utils.String;
using System.Text.Json.Serialization;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Common JSON serializer context
    /// 通用 JSON 序列化器上下文
    /// </summary>
    [JsonSerializable(typeof(Dictionary<string, object>))]
    [JsonSerializable(typeof(StringKeyDictionaryObject))]
    [JsonSerializable(typeof(GuidItem))]
    public partial class CommonJsonSerializerContext : JsonSerializerContext
    {
    }
}
