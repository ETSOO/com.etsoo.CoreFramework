namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Search request data with string id
    /// 查询请求数据
    /// </summary>
    public record QueryStrRQ : QueryRQ<int>
    {
        /// <summary>
        /// String id
        /// 字符串编号
        /// </summary>
        public string? Sid { get; init; }
    }
}