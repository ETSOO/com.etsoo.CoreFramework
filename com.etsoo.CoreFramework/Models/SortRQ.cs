namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Sort with category request data
    /// 分类数据排序请求数据
    /// </summary>
    public record SortRQ
    {
        /// <summary>
        /// Category
        /// 分类编号
        /// </summary>
        public required byte Category { get; init; }

        /// <summary>
        /// Sort data
        /// 排序数据
        /// </summary>
        public required Dictionary<int, short> Data { get; init; }
    }
}
