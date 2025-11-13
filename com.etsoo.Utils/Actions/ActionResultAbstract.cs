using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Abstract action result
    /// 抽象操作结果
    /// </summary>
    [JsonDerivedType(typeof(ActionResult))]
    [JsonDerivedType(typeof(ActionResult<IdData>))]
    [JsonDerivedType(typeof(ActionResult<IdMsgData>))]
    [JsonDerivedType(typeof(ActionResult<StringIdData>))]
    [JsonDerivedType(typeof(ActionResult<StringIdMsgData>))]
    public abstract record ActionResultAbstract
    {
        /// <summary>
        /// Ok or not
        /// 是否成功
        /// </summary>
        public bool Ok { get; init; }

        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        public string? Type
        {
            get;
            init
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Support type/field format
                    var items = value.Split('/');
                    if (items.Length > 1)
                    {
                        field = items[0];
                        Field = items[1];
                        return;
                    }
                }

                field = value;
            }
        }

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Field
        /// 字段
        /// </summary>
        public string? Field
        {
            get;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    field = value;
            }
        }

        /// <summary>
        /// Status code
        /// 状态码
        /// </summary>
        public int? Status { get; init; }

        /// <summary>
        /// Detail
        /// 细节
        /// </summary>
        public string? Detail { get; set; }

        /// <summary>
        /// Trace id
        /// 跟踪编号
        /// </summary>
        public string? TraceId { get; set; }
    }

    /// <summary>
    /// Generic action result
    /// 操作结果泛型
    /// </summary>
    /// <typeparam name="D">Generic data type</typeparam>
    public record ActionResult<D> : ActionResultAbstract
    {
        /// <summary>
        /// Ok or not
        /// 是否成功
        /// </summary>
        [MemberNotNullWhen(true, nameof(Data))]
        public new bool Ok { get; init; }

        /// <summary>
        /// Data
        /// 数据
        /// </summary>
        public D? Data { get; set; }

        /// <summary>
        /// To action result
        /// 转为操作结果
        /// </summary>
        /// <param name="typeInfo">Json type info for the Data transformation</param>
        /// <returns>Result</returns>
        public async ValueTask<ActionResult> ToActionResultAsync(JsonTypeInfo<D>? typeInfo = null)
        {
            var result = new ActionResult
            {
                Ok = Ok,
                Type = Type,
                Title = Title,
                Field = Field,
                Status = Status,
                Detail = Detail,
                TraceId = TraceId
            };

            if (typeInfo != null && Data != null)
            {
                var data = await SharedUtils.ObjectToDictionaryAsync(Data, typeInfo);
                foreach (var item in data)
                {
                    result.Data[item.Key] = item.Value;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Id action result data
    /// 编号结果数据
    /// </summary>
    public record IdData
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public long Id { get; init; }
    }

    /// <summary>
    /// Id and message action result data
    /// 编号和消息操作结果数据
    /// </summary>
    public record IdMsgData
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public long Id { get; init; }

        /// <summary>
        /// Message
        /// 消息
        /// </summary>
        public required string Msg { get; init; }
    }

    /// <summary>
    /// String action result data
    /// 字符串编号操作结果数据
    /// </summary>
    public record StringIdData
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public required string Id { get; init; }
    }

    /// <summary>
    /// String id and message action result data
    /// 字符串编号和消息操作结果数据
    /// </summary>
    public record StringIdMsgData
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Message
        /// 消息
        /// </summary>
        public required string Msg { get; init; }
    }
}
