namespace com.etsoo.Utils.Address
{
    /// <summary>
    /// Country or region
    /// 国家或地区
    /// </summary>
    public partial record AddressRegion
    {
        /// <summary>
        // Id, like CN for China
        // https://www.iban.com/country-codes
        // 国家编号
        /// </summary>
        public string Id { get; }

        /// <summary>
        // 3-code id like CHN for China
        // 三个字母国家编号
        /// </summary>
        public string Id3 { get; }

        /// <summary>
        // Number id, like 156 for China
        // 数字编号
        /// </summary>
        public string Nid { get; }

        /// <summary>
        // Continent
        // 洲
        /// </summary>
        public AddressContinent Continent { get; }

        /// <summary>
        /// Phone exit code for international dial, like 00 in China
        /// 国际拨号的电话退出代码
        /// </summary>
        public string ExitCode { get; }

        /// <summary>
        /// Area code for international dial, like 86 for China
        /// 国际电话区号
        /// </summary>
        public string Idd { get; }

        /// <summary>
        /// Currency, like CNY for China's currency
        /// 币种
        /// </summary>
        public string Currency { get; }

        /// <summary>
        /// Languages
        /// 语言
        /// </summary>
        public IEnumerable<string> Languages { get; }

        /// <summary>
        /// Phone validator
        /// 电话验证器
        /// </summary>
        public AddressRegionPhoneValidatorDel? PhoneValidator { get; set; }

        // Constructor
        // 构造函数
        public AddressRegion(string id, string id3, string nid, AddressContinent continent, string exitCode, string idd, string currency, IEnumerable<string> languages)
        {
            (Id, Id3, Nid, Continent, ExitCode, Idd, Currency, Languages)
                =
            (id, id3, nid, continent, exitCode, idd, currency, languages);
        }
    }
}
