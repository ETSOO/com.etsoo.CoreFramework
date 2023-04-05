using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
        /// Current page
        /// 当前页码
        /// </summary>
        public uint CurrentPage { get; set; }

        /// <summary>
        /// Batch size
        /// 批量请求数量
        /// </summary>
        [Range(1, 1000)]
        public ushort BatchSize { get; set; }

        /// <summary>
        /// Order by field
        /// 排序字段
        /// </summary>
        public string? OrderBy { get; set; }

        /// <summary>
        /// Order ascending or descending
        /// 升序或降序排列
        /// </summary>
        public bool? OrderByAsc { get; set; }

        /// <summary>
        /// Get order command
        /// 获取排序命令
        /// </summary>
        /// <returns>Command</returns>
        public string? GetOrderCommand()
        {
            if (string.IsNullOrEmpty(OrderBy)) return null;
            if (MyRegex().IsMatch(OrderBy))
            {
                var byText = OrderByAsc.GetValueOrDefault(true) ? "ASC" : "DESC";
                return $"ORDER BY {OrderBy} {byText}";
            }
            return null;
        }

        [GeneratedRegex("^[0-9a-zA-Z_]+$")]
        private static partial Regex MyRegex();
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