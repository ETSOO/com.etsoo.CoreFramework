﻿namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Abstract action result
    /// 抽象操作结果
    /// </summary>
    public abstract record ActionResultAbstract
    {
        /// <summary>
        /// Ok or not
        /// 是否成功
        /// </summary>
        public bool Ok { get; init; }

        string? type;
        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        public string? Type
        {
            get { return type; }
            init
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Support type/field format
                    var items = value.Split('/');
                    if (items.Length > 1)
                    {
                        type = items[0];
                        field = items[1];
                        return;
                    }
                }

                type = value;
            }
        }

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        string? field;
        /// <summary>
        /// Field
        /// 字段
        /// </summary>
        public string? Field
        {
            get { return field; }
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
        /// Data
        /// 数据
        /// </summary>
        public required D Data { get; init; }
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
        public long? Id { get; init; }

        /// <summary>
        /// Message
        /// 消息
        /// </summary>
        public string? Msg { get; init; }
    }
}
