namespace com.etsoo.Address
{
    /// <summary>
    /// Countries or regions
    /// 国家或地区
    /// </summary>
    public partial record AddressRegion
    {
        /// <summary>
        /// CN - China
        /// 中国
        /// </summary>
        public static AddressRegion CN => new("CN", "CHN", "156", AddressContinent.AS, "00", "86", "CNY", ["zh-CN"]) { PhoneValidator = AddressRegionPhoneValidator.CN };

        /// <summary>
        /// HK - HK, China
        /// 中国香港
        /// </summary>
        public static AddressRegion HK => new("HK", "HKG", "344", AddressContinent.AS, "001", "852", "HKD", ["zh-HK"]);

        /// <summary>
        /// SG - Singapore
        /// 新加坡
        /// </summary>
        public static AddressRegion SG => new("SG", "SGP", "702", AddressContinent.AS, "000", "65", "SGD", ["zh-SG"]);

        /// <summary>
        /// JP - Japan
        /// 日本
        /// </summary>
        public static AddressRegion JP => new("JP", "JPN", "392", AddressContinent.AS, "010", "81", "JPY", ["ja-JP"]);

        /// <summary>
        /// US - United States
        /// 美国
        /// </summary>
        public static AddressRegion US => new("US", "USA", "840", AddressContinent.NA, "011", "1", "USD", ["en-US"]);

        /// <summary>
        /// CA - Canada
        /// 加拿大
        /// </summary>
        public static AddressRegion CA => new("CA", "CAN", "124", AddressContinent.NA, "011", "1", "USD", ["en-CA", "fr-CA"]);

        /// <summary>
        /// AU - Australia
        /// 澳大利亚
        /// </summary>
        public static AddressRegion AU => new("AU", "AUS", "036", AddressContinent.OC, "0011", "61", "AUD", ["en-AU"]);

        /// <summary>
        /// NZ - New Zealand
        /// 新西兰
        /// </summary>
        public static AddressRegion NZ => new("NZ", "NZL", "554", AddressContinent.OC, "00", "64", "NZD", ["en-NZ", "mi-NZ"]) { PhoneValidator = AddressRegionPhoneValidator.NZ };

        /// <summary>
        /// GB - Great Britain
        /// 英国
        /// </summary>
        public static AddressRegion GB => new("GB", "GBR", "826", AddressContinent.EU, "00", "44", "GBP", ["en-GB"]);

        /// <summary>
        /// IE - Ireland
        /// 爱尔兰
        /// </summary>
        public static AddressRegion IE => new("IE", "IRL", "372", AddressContinent.EU, "00", "353", "IEP", ["en-IE"]);

        /// <summary>
        /// DE - Germany
        /// 德国
        /// </summary>
        public static AddressRegion DE => new("DE", "DEU", "276", AddressContinent.EU, "00", "49", "EUR", ["de-DE"]);

        /// <summary>
        /// FR - France
        /// 法国
        /// </summary>
        public static AddressRegion FR => new("FR", "FRA", "250", AddressContinent.EU, "00", "33", "EUR", ["fr-FR"]);

        /// <summary>
        /// All countries
        /// 所有国家
        /// </summary>
        public static IEnumerable<AddressRegion> All =>
        [
            CN,
            HK,
            SG,
            JP,
            US,
            CA,
            AU,
            NZ,
            GB,
            IE,
            DE,
            FR
        ];

        /// <summary>
        /// Get country by id
        /// 从编号获取国家
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Result</returns>
        public static AddressRegion? GetById(string id)
        {
            return id switch
            {
                nameof(CN) => CN,
                nameof(HK) => HK,
                nameof(SG) => SG,
                nameof(JP) => JP,
                nameof(US) => US,
                nameof(CA) => CA,
                nameof(AU) => AU,
                nameof(NZ) => NZ,
                nameof(GB) => GB,
                nameof(IE) => IE,
                nameof(DE) => DE,
                nameof(FR) => FR,
                _ => null
            };
        }

        /// <summary>
        /// Get country by IDD
        /// 从国家拨号获取国家
        /// </summary>
        /// <param name="idd">IDD</param>
        /// <returns>Result</returns>
        public static AddressRegion? GetByIdd(string idd)
        {
            return All.FirstOrDefault(c => c.Idd == idd);
        }
    }
}
