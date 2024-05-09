using com.etsoo.Database;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Search request data interface
    /// 查询请求数据接口
    /// </summary>
    public interface IQueryRQ
    {
        object? Id { get; }

        QueryPagingData? QueryPaging { get; set; }
    }

    /// <summary>
    /// Search request data
    /// 查询请求数据
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public partial record QueryRQ<T> : IQueryRQ where T : struct
    {
        object? IQueryRQ.Id => Id;

        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public T? Id { get; set; }

        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        public QueryPagingData? QueryPaging { get; set; }
    }

    /// <summary>
    /// Search request data with string id
    /// 查询请求数据
    /// </summary>
    public record QueryRQ : IQueryRQ
    {
        object? IQueryRQ.Id => Id;

        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        public QueryPagingData? QueryPaging { get; set; }
    }
}