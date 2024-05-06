using com.etsoo.Database;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Search request data
    /// 查询请求数据
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public partial record QueryRQ<T> where T : struct
    {
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
    public record QueryRQ : QueryRQ<int>
    {
        /// <summary>
        /// String id
        /// 字符串编号
        /// </summary>
        public string? Sid { get; set; }
    }
}