using com.etsoo.CoreFramework.Business;
using com.etsoo.Database;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Query request data interface
    /// 查询请求数据接口
    /// </summary>
    public interface IQueryRQ
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
    }
}