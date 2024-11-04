namespace com.etsoo.Localization.Country
{
    /// <summary>
    /// Currency item
    /// 币种项目
    /// </summary>
    public record CurrencyItem
    {
        /// <summary>
        /// Id, like CNY
        /// 编号，如CNY
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Name, like Chinese Yuan
        /// 名称，如人民币
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Native name, like 人民币
        /// 原生名
        /// </summary>
        public required string NativeName { get; init; }

        /// <summary>
        /// English name, like Chinese Yuan
        /// 英文名
        /// </summary>
        public required string EnglishName { get; init; }

        /// <summary>
        /// Symbol, like ¥
        /// 货币符号
        /// </summary>
        public required string Symbol { get; init; }
    }
}
