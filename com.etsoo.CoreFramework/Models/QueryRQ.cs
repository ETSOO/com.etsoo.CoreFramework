using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Business;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Query request data interface
    /// 查询请求数据接口
    /// </summary>
    public interface IQueryRQ : IModelValidator
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        object? Id { get; }

        /// <summary>
        /// Is disabled or not
        /// 是否禁用
        /// </summary>
        bool? Disabled { get; set; }

        /// <summary>
        /// Filter keyword
        /// 过滤关键字
        /// </summary>
        string? Keyword { get; set; }

        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        QueryPagingData? QueryPaging { get; set; }

        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        EntityStatus? Status { get; set; }
    }

    /// <summary>
    /// Query request data base
    /// 查询请求数据基类
    /// </summary>
    public abstract record QueryRQBase
    {
        /// <summary>
        /// Disabled or not, null for all, true for disabled (> EntityStatus.Approved), false for enabled (<= 100)
        /// 是否禁用
        /// </summary>
        public bool? Disabled { get; set; }

        /// <summary>
        /// Keyword to filter
        /// 用于过滤的关键字
        /// </summary>
        public virtual string? Keyword { get; set; }

        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        public QueryPagingData? QueryPaging { get; set; }

        /// <summary>
        /// Status
        /// 状态
        /// </summary>
        public EntityStatus? Status { get; set; }
    }

    /// <summary>
    /// Query request data
    /// 查询请求数据
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record QueryRQ<T> : QueryRQBase, IQueryRQ where T : struct
    {
        object? IQueryRQ.Id => Id;

        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public T? Id { get; set; }

        /// <summary>
        /// Ids
        /// 编号列表
        /// </summary>
        public virtual IEnumerable<T>? Ids { get; set; }

        /// <summary>
        /// Excluded ids
        /// 排除的编号
        /// </summary>
        public virtual IEnumerable<T>? ExcludedIds { get; set; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (!QueryPaging.IsOrderByValid())
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(QueryPaging));
            }

            if (Keyword != null && Keyword.Length > 128)
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Keyword));
            }

            return null;
        }
    }

    /// <summary>
    /// Search request data with string id
    /// 查询请求数据
    /// </summary>
    public record QueryRQ : QueryRQBase, IQueryRQ
    {
        object? IQueryRQ.Id => Id;

        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Ids
        /// 编号列表
        /// </summary>
        public virtual IEnumerable<string>? Ids { get; set; }

        /// <summary>
        /// Excluded ids
        /// 排除的编号
        /// </summary>
        public virtual IEnumerable<string>? ExcludedIds { get; set; }

        /// <summary>
        /// Is valid id or not
        /// 编号是否有效
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Result</returns>
        protected virtual bool IsValidId(string id)
        {
            return id.Length is (>=1 and <= 256);
        }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (!QueryPaging.IsOrderByValid())
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(QueryPaging));
            }

            if (Keyword != null && Keyword.Length > 128)
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Keyword));
            }

            if (Id != null && !IsValidId(Id))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Id));
            }

            if (Ids != null)
            {
                foreach (var id in Ids)
                {
                    if (!IsValidId(id))
                    {
                        return ApplicationErrors.NoValidData.AsResult(nameof(Ids));
                    }
                }
            }

            if (ExcludedIds != null)
            {
                foreach (var id in ExcludedIds)
                {
                    if (!IsValidId(id))
                    {
                        return ApplicationErrors.NoValidData.AsResult(nameof(ExcludedIds));
                    }
                }
            }

            return null;
        }
    }
}