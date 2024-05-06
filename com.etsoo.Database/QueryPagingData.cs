using System.ComponentModel.DataAnnotations;

namespace com.etsoo.Database
{
    /// <summary>
    /// Query paging data
    /// 查询分页数据
    /// </summary>
    public partial record QueryPagingData
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
    }
}
