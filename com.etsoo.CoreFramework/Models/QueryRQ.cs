namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Search request data
    /// 查询请求数据
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public partial record QueryRQ<T> : QueryData where T : struct
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public T? Id { get; set; }
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