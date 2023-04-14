namespace com.etsoo.CoreFramework.Business
{
    /// <summary>
    /// Standard entity status enum
    /// 标准实体状态枚举
    /// </summary>
    public enum EntityStatus : byte
    {
        /// <summary>
        /// Normal
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Flaged
        /// 标记的
        /// </summary>
        Flaged = 9,

        /// <summary>
        /// Approved
        /// 已批准
        /// </summary>
        Approved = 100,

        /// <summary>
        /// Doing
        /// 处理中
        /// </summary>
        Doing = 110,

        /// <summary>
        /// Audited
        /// 已审核
        /// </summary>
        Audited = 120,

        /// <summary>
        /// Inactivated
        /// 已停用
        /// </summary>
        Inactivated = 200,

        /// <summary>
        /// Completed
        /// 已完成
        /// </summary>
        Completed = 250,

        /// <summary>
        /// Archived
        /// 已归档
        /// </summary>
        Archived = 254,

        /// <summary>
        /// Deleted
        /// 标记删除
        /// </summary>
        Deleted = 255
    }
}
