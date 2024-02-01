using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Model common JSON serializer context
    /// 通用模型 JSON 序列化器上下文
    /// </summary>
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(ChangePasswordDto))]
    [JsonSerializable(typeof(ChangePasswordRQ))]
    [JsonSerializable(typeof(ExchangeTokenRQ))]
    [JsonSerializable(typeof(IdNameItem))]
    [JsonSerializable(typeof(IdTitleItem))]
    [JsonSerializable(typeof(InitCallRQ))]
    [JsonSerializable(typeof(LoginIdRQ))]
    [JsonSerializable(typeof(LoginRQ))]
    [JsonSerializable(typeof(MergeRQ))]
    [JsonSerializable(typeof(QueryRQ))]
    [JsonSerializable(typeof(QueryRQ<long>), TypeInfoPropertyName = "QueryLongRQ")]
    [JsonSerializable(typeof(QueryRQ<Guid>), TypeInfoPropertyName = "QueryGuidRQ")]
    [JsonSerializable(typeof(RefreshTokenRQ))]
    [JsonSerializable(typeof(SignoutRQ))]
    [JsonSerializable(typeof(SortRQ))]
    [JsonSerializable(typeof(TiplistRQ))]
    [JsonSerializable(typeof(TiplistRQ<long>), TypeInfoPropertyName = "TiplistLongRQ")]
    [JsonSerializable(typeof(TiplistRQ<Guid>), TypeInfoPropertyName = "TiplistGuidRQ")]
    [JsonSerializable(typeof(UpdateStatusRQ<int>), TypeInfoPropertyName = "UpdateStatusRQ")]
    [JsonSerializable(typeof(UpdateStatusRQ<long>), TypeInfoPropertyName = "UpdateStatusLongRQ")]
    [JsonSerializable(typeof(UpdateStatusRQ<Guid>), TypeInfoPropertyName = "UpdateStatusGuidRQ")]
    public partial class ModelJsonSerializerContext : JsonSerializerContext
    {
    }
}