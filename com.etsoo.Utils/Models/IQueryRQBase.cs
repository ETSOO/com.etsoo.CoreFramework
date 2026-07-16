namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Query request data base interface
    /// 查询请求数据基类接口
    /// </summary>
    public interface IQueryRQBase<T>
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public T? Id { get; set; }

        /// <summary>
        /// Ids
        /// 编号列表
        /// </summary>
        public IEnumerable<T>? Ids { get; set; }

        /// <summary>
        /// Excluded ids
        /// 排除的编号
        /// </summary>
        public IEnumerable<T>? ExcludedIds { get; set; }

        /// <summary>
        /// Is enabled or not
        /// 是否启用
        /// </summary>
        bool? Enabled { get; set; }

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
    }
}
