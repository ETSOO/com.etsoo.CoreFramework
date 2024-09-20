using com.etsoo.CoreFramework.Services;
using com.etsoo.CoreFramework.User;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
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

    [JsonSerializable(typeof(AuthCreateTokenRQ))]
    [JsonSerializable(typeof(AuthRefreshTokenRQ))]
    [JsonSerializable(typeof(AuthRequest))]
    [JsonSerializable(typeof(ChangePasswordDto))]
    [JsonSerializable(typeof(ChangePasswordRQ))]
    [JsonSerializable(typeof(DateTime))]
    [JsonSerializable(typeof(DateTimeOffset))]
    [JsonSerializable(typeof(EmailTemplateDto))]
    [JsonSerializable(typeof(ExchangeTokenRQ))]
    [JsonSerializable(typeof(ErrorLogData))]
    [JsonSerializable(typeof(IEnumerable<IdNameItem>))]
    [JsonSerializable(typeof(IEnumerable<IdTitleItem>))]
    [JsonSerializable(typeof(InitCallRQ))]
    [JsonSerializable(typeof(LoginIdRQ))]
    [JsonSerializable(typeof(LoginRQ))]
    [JsonSerializable(typeof(MergeRQ))]
    [JsonSerializable(typeof(QueryRQ))]
    [JsonSerializable(typeof(QueryRQ<int>), TypeInfoPropertyName = "QueryIntRQ")]
    [JsonSerializable(typeof(QueryRQ<long>), TypeInfoPropertyName = "QueryLongRQ")]
    [JsonSerializable(typeof(QueryRQ<Guid>), TypeInfoPropertyName = "QueryGuidRQ")]
    [JsonSerializable(typeof(SendEmailRQ))]
    [JsonSerializable(typeof(SignoutRQ))]
    [JsonSerializable(typeof(SortRQ))]
    [JsonSerializable(typeof(UpdateStatusRQ<int>), TypeInfoPropertyName = "UpdateStatusRQ")]
    [JsonSerializable(typeof(UpdateStatusRQ<long>), TypeInfoPropertyName = "UpdateStatusLongRQ")]
    [JsonSerializable(typeof(UpdateStatusRQ<Guid>), TypeInfoPropertyName = "UpdateStatusGuidRQ")]

    [JsonSerializable(typeof(UpdateResultData))]
    [JsonSerializable(typeof(UpdateResultData<long>), TypeInfoPropertyName = "UpdateResultLongData")]
    [JsonSerializable(typeof(UpdateResultData<Guid>), TypeInfoPropertyName = "UpdateResultGuidData")]
    [JsonSerializable(typeof(UpdateResultData<int>), TypeInfoPropertyName = "UpdateResultIntData")]

    [JsonSerializable(typeof(AppTokenData))]
    [JsonSerializable(typeof(IUserToken))]
    public partial class ModelJsonSerializerContext : JsonSerializerContext
    {
    }
}