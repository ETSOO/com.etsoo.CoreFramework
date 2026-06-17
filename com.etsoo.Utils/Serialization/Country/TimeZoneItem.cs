namespace com.etsoo.Utils.Serialization.Country
{
    /// <summary>
    /// Time zone item
    /// 时区项目
    /// </summary>
    public record TimeZoneItem
    {
        /// <summary>
        /// IANA Id
        /// IANA 编号
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Display name
        /// 显示名称
        /// </summary>
        public required string DisplayName { get; set; }

        /// <summary>
        /// Standard name
        /// 标准名称
        /// </summary>
        public required string StandardName { get; init; }

        /// <summary>
        /// Utc offset
        /// UTC偏移
        /// </summary>
        public TimeSpan UtcOffset { get; init; }
    }
}
