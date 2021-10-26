namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Action result base
    /// 操作结果基类
    /// </summary>
    public record ActionResultBase
    {
        /// <summary>
        /// Is auto set datetime to Utc kind
        /// 是否设置日期时间为Utc类型
        /// </summary>
        public static bool UtcDateTime { get; set; }

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
                        Field = items[1];
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

        /// <summary>
        /// Field
        /// 字段
        /// </summary>
        public string? Field { get; init; }

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
}
