using System.ComponentModel.DataAnnotations;

namespace com.etsoo.Database
{
    /// <summary>
    /// Query paging order
    /// 查询分页排序
    /// </summary>
    public record QueryPagingOrder
    {
        /// <summary>
        /// Field name
        /// </summary>
        public required string Field { get; init; }

        /// <summary>
        /// Descending
        /// </summary>
        public bool Desc { get; set; }

        /// <summary>
        /// Is unique value
        /// </summary>
        public bool Unique { get; set; }
    }


    /// <summary>
    /// Query paging data, Offset pagination vs Keyset pagination
    /// https://learn.microsoft.com/en-us/ef/core/querying/pagination
    /// 查询分页数据，偏移分页与键集分页
    /// </summary>
    public partial record QueryPagingData
    {
        /// <summary>
        /// Current page
        /// 当前页码
        /// </summary>
        public uint? CurrentPage { get; set; }

        /// <summary>
        /// Keyset array, same order as the order by fields
        /// 键值数组，与排序字段顺序一致
        /// </summary>
        public IEnumerable<object>? Keysets { get; set; }

        /// <summary>
        /// Batch size
        /// 批量请求数量
        /// </summary>
        [Range(1, 1000)]
        public ushort BatchSize { get; set; } = 10;

        /// <summary>
        /// Order by fields
        /// 排序字段
        /// </summary>
        public IEnumerable<QueryPagingOrder>? OrderBy { get; set; }
    }
}
