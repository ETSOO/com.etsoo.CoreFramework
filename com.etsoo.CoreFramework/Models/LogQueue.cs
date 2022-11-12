namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Log queue, see table SysLogQueue
    /// 日志队列，见表 SysLogQueue
    /// </summary>
    public record LogQueue
    {
        /// <summary>
        /// Unique id
        /// 唯一编号
        /// </summary>
        public long Id { get; init; }

        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        public int UserId { get; init; }

        /// <summary>
        /// Current organization
        /// 当前机构
        /// </summary>
        public int? OrganizationId { get; init; }

        /// <summary>
        /// Main type
        /// 主要类型
        /// </summary>
        public short Type { get; init; }

        /// <summary>
        /// Sub type
        /// 子类型
        /// </summary>
        public short SubType { get; init; }

        /// <summary>
        /// JSON data passed
        /// 传递的JSON数据
        /// </summary>
        public string? Data { get; init; }

        /// <summary>
        /// Creation UTC
        /// 登记时间UTC
        /// </summary>
        public DateTime Creation { get; init; }
    }
}
