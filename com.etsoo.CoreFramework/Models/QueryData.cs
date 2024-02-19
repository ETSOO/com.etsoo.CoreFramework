using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Query data
    /// 查询数据
    /// </summary>
    public partial record QueryData
    {
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
                if (OrderBy.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase) || OrderBy.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
                {
                    return $"ORDER BY {OrderBy}";
                }
                else
                {
                    return $"ORDER BY {OrderBy} {byText}";
                }
            }
            return null;
        }

        [GeneratedRegex("^[0-9a-zA-Z_\\s,]+$")]
        private static partial Regex MyRegex();
    }
}
