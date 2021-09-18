namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Guid item
    /// Guid项目
    /// </summary>
    public record GuidItem
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Item
        /// 项目
        /// </summary>
        public string Item { get; set; } = null!;
    }
}
