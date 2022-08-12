using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Search request data
    /// 查询请求数据
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record QueryRQ<T> where T : struct
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public T? Id { get; init; }

        /// <summary>
        /// Current page
        /// 当前页码
        /// </summary>
        [Required]
        public int CurrentPage { get; init; }

        /// <summary>
        /// Batch size
        /// 批量请求数量
        /// </summary>
        [Required]
        [Range(1, 1000)]
        public int BatchSize { get; init; }

        /// <summary>
        /// Order by field
        /// 排序字段
        /// </summary>
        public string? OrderBy { get; init; }

        /// <summary>
        /// Order ascending or descending
        /// 升序或降序排列
        /// </summary>
        public bool? OrderByAsc { get; init; }
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
        public string? Sid { get; init; }
    }
}