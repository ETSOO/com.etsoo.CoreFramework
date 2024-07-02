using System.Text.Json.Serialization;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Model common JSON serializer context
    /// 通用模型 JSON 序列化器上下文
    /// </summary>
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    )]
    [JsonSerializable(typeof(MessageProperties))]
    [JsonSerializable(typeof(MessageReceivedProperties))]
    public partial class MessageQueueJsonSerializerContext : JsonSerializerContext
    {
    }
}
