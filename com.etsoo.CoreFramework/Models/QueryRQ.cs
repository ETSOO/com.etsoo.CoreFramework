using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Business;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Query request data interface
    /// 查询请求数据接口
    /// </summary>
    public interface IQueryRQ : IQueryRQBase
    {
        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        EntityStatus? Status { get; set; }
    }

    /// <summary>
    /// Query request data
    /// 查询请求数据
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record QueryRQ<T> : QueryRQBase<T>, IQueryRQ where T : struct
    {
        protected override object? GetId() => Id;

        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        public EntityStatus? Status { get; set; }

        protected override IActionResult? GenerateResult(string field)
        {
            return ApplicationErrors.NoValidData.AsResult(field);
        }
    }

    /// <summary>
    /// Search request data with string id
    /// 查询请求数据
    /// </summary>
    public record QueryRQ : QueryRQBase, IQueryRQ
    {
        protected override object? GetId() => Id;

        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        public EntityStatus? Status { get; set; }

        protected override IActionResult? GenerateResult(string field)
        {
            return ApplicationErrors.NoValidData.AsResult(field);
        }
    }
}