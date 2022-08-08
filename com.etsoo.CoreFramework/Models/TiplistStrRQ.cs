namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Tiplist Request data with string id
    /// 动态列表请求数据
    /// </summary>
    public record TiplistStrRQ : TiplistRQ<int>
    {
        /// <summary>
        /// String id
        /// 字符串编号
        /// </summary>
        public string? Sid { get; init; }
    }
}
