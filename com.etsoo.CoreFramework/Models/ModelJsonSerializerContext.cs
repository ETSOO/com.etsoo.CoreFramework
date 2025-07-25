﻿using com.etsoo.CoreFramework.Business;
using com.etsoo.CoreFramework.Services;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using Json.Schema;
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

    [JsonSerializable(typeof(AntiforgeryRequestToken))]
    [JsonSerializable(typeof(AssetUnit))]
    [JsonSerializable(typeof(DataPrivacy))]
    [JsonSerializable(typeof(EntityStatus))]
    [JsonSerializable(typeof(IdentityType))]
    [JsonSerializable(typeof(IdentityTypeFlags))]
    [JsonSerializable(typeof(ProductUnit))]
    [JsonSerializable(typeof(RepeatOption))]

    [JsonSerializable(typeof(ApiTokenRQ))]
    [JsonSerializable(typeof(ChangePasswordDto))]
    [JsonSerializable(typeof(ChangePasswordRQ))]
    [JsonSerializable(typeof(DateTime))]
    [JsonSerializable(typeof(DateTimeOffset))]
    [JsonSerializable(typeof(EmailTemplateDto))]
    [JsonSerializable(typeof(ExchangeTokenRQ))]
    [JsonSerializable(typeof(ErrorLogData))]
    [JsonSerializable(typeof(GetAuthRequestRQ))]
    [JsonSerializable(typeof(IEnumerable<IdNameItem>))]
    [JsonSerializable(typeof(IEnumerable<IdTitleItem>))]
    [JsonSerializable(typeof(InitCallRQ))]
    [JsonSerializable(typeof(LoginIdRQ))]
    [JsonSerializable(typeof(LongIdItem))]
    [JsonSerializable(typeof(MergeRQ))]
    [JsonSerializable(typeof(PublicServiceUserData))]
    [JsonSerializable(typeof(PublicUserData))]
    [JsonSerializable(typeof(QueryRQ))]
    [JsonSerializable(typeof(QueryRQ<int>), TypeInfoPropertyName = "QueryIntRQ")]
    [JsonSerializable(typeof(QueryRQ<long>), TypeInfoPropertyName = "QueryLongRQ")]
    [JsonSerializable(typeof(QueryRQ<Guid>), TypeInfoPropertyName = "QueryGuidRQ")]
    [JsonSerializable(typeof(RefreshTokenData))]
    [JsonSerializable(typeof(RefreshTokenRQ))]
    [JsonSerializable(typeof(SendEmailRQ))]
    [JsonSerializable(typeof(SignModel))]
    [JsonSerializable(typeof(SignoutRQ))]
    [JsonSerializable(typeof(SortRQ))]
    [JsonSerializable(typeof(SwitchOrgRQ))]
    [JsonSerializable(typeof(UpdateStatusRQ<int>), TypeInfoPropertyName = "UpdateStatusRQ")]
    [JsonSerializable(typeof(UpdateStatusRQ<long>), TypeInfoPropertyName = "UpdateStatusLongRQ")]
    [JsonSerializable(typeof(UpdateStatusRQ<Guid>), TypeInfoPropertyName = "UpdateStatusGuidRQ")]

    [JsonSerializable(typeof(UpdateResultData))]
    [JsonSerializable(typeof(UpdateResultData<long>), TypeInfoPropertyName = "UpdateResultLongData")]
    [JsonSerializable(typeof(UpdateResultData<Guid>), TypeInfoPropertyName = "UpdateResultGuidData")]
    [JsonSerializable(typeof(UpdateResultData<int>), TypeInfoPropertyName = "UpdateResultIntData")]

    [JsonSerializable(typeof(ApiTokenData))]
    [JsonSerializable(typeof(AppTokenData))]
    [JsonSerializable(typeof(IUserToken))]

    // https://docs.json-everything.net/schema/basics/
    [JsonSerializable(typeof(JsonSchema))]
    [JsonSerializable(typeof(EvaluationResults))]

    [JsonSerializable(typeof(IEnumerable<EntityChangedProperty>))]
    public partial class ModelJsonSerializerContext : JsonSerializerContext
    {
    }
}