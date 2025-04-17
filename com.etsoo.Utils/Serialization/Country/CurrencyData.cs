using System.Text.Json.Serialization;

namespace com.etsoo.Utils.Serialization.Country
{
    /// <summary>
    /// Currency data
    /// 币种数据
    /// </summary>
    [JsonDerivedType(typeof(CurrencyItem))]
    [JsonDerivedType(typeof(CurrencyExchangeData))]
    public record CurrencyData
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
