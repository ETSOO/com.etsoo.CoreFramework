namespace com.etsoo.Localization.Country
{
    /// <summary>
    /// Region item
    /// 地区项目
    /// </summary>
    public record RegionItem
    {
        /// <summary>
        /// Id, like CN
        /// 编号，如CN
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Id with 3 characters, like CHN
        /// 3字符编号
        /// </summary>
        public required string Id3 { get; init; }

        /// <summary>
        /// Name, like China
        /// 名称，如中国
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Native name, like 中国
        /// 原生名
        /// </summary>
        public required string NativeName { get; init; }

        /// <summary>
        /// English name, like China
        /// 英文名
        /// </summary>
        public required string EnglishName { get; init; }

        /// <summary>
        /// Currency used
        /// 使用的货币
        /// </summary>
        public required CurrencyItem Currency { get; init; }

        /// <summary>
        /// Cultures supported
        /// 支持的文化
        /// </summary>
        public required IList<CultureItem> Cultures { get; init; }
    }
}
