namespace com.etsoo.Utils.Serialization.Country
{
    /// <summary>
    /// Currency item
    /// 币种项目
    /// </summary>
    public record CurrencyItem : CurrencyData
    {
        /// <summary>
        /// Native name, like 人民币
        /// 原生名
        /// </summary>
        public required string NativeName { get; init; }
    }
}
