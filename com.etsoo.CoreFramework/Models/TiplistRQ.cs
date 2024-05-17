using com.etsoo.Database;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Tiplist Request data
    /// 动态列表请求数据
    /// </summary>
    public abstract record TiplistRQ
    {
        /// <summary>
        /// Search keyword
        /// 查询关键词
        /// </summary>
        public abstract string Keyword { get; set; }

        /// <summary>
        /// Query paging data
        /// 查询分页数据
        /// </summary>
        public QueryPagingData QueryPaging { get; } = new();
    }
}
