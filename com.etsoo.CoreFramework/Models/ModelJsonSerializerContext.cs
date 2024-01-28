using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Model common JSON serializer context
    /// 通用模型 JSON 序列化器上下文
    /// </summary>
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
    [JsonSerializable(typeof(QueryRQ<long>))]
    [JsonSerializable(typeof(RefreshTokenRQ))]
    [JsonSerializable(typeof(SignoutRQ))]
    [JsonSerializable(typeof(SortRQ))]
    [JsonSerializable(typeof(TiplistRQ))]
    [JsonSerializable(typeof(TiplistRQ<long>))]
    [JsonSerializable(typeof(UpdateStatusRQ<int>))]
    [JsonSerializable(typeof(UpdateStatusRQ<long>))]
    public partial class ModelJsonSerializerContext : JsonSerializerContext
    {
    }
}
