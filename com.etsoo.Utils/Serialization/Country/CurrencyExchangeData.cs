namespace com.etsoo.Utils.Serialization.Country
{
    /// <summary>
    /// Exchange rate data
    /// 汇率数据
    /// </summary>
    public record CurrencyExchangeData : CurrencyData
    {
        /// <summary>
        /// Exchange rate, 100 foreign currency exchanged to the local currency amount
        /// 汇率，100外币兑换得本币金额
        /// </summary>
        public required decimal ExchangeRate { get; init; }

        /// <summary>
        /// Updated time
        /// 更新时间
        /// </summary>
        public required DateTime UpdateTime { get; init; }
    }
}
