using com.etsoo.CoreFramework.Business;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Update status request data
    /// 更新状态请求数据
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record UpdateStatusRQ<T> where T : struct
    {
        /// <summary>
        /// id
        /// 编号
        /// </summary>
        public T Id { get; init; }

        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        public EntityStatus Status { get; init; }
    }
}
