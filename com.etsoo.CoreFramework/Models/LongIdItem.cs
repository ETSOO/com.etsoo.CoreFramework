namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Long ID item
    /// 长整型编号项
    /// </summary>
    public record LongIdItem
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public required long Id { get; init; }

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        public required string Title { get; init; }
    }
}
