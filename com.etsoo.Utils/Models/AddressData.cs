namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Country or region
    /// 国家或地区
    /// </summary>
    public record AddressRegion
    {
        /// <summary>
        /// CN - China
        /// 中国
        /// </summary>
        public static AddressRegion CN => new("CN", "CHN", "156", AddressContinent.AS, "00", "86", "CNY", new[] { "zh-CN" });

        /// <summary>
        /// HK - HK, China
        /// 中国香港
        /// </summary>
        public static AddressRegion HK => new("HK", "HKG", "344", AddressContinent.AS, "001", "852", "HKD", new[] { "zh-HK" });

        /// <summary>
        /// SG - Singapore
        /// 新加坡
        /// </summary>
        public static AddressRegion SG => new("SG", "SGP", "702", AddressContinent.AS, "000", "65", "SGD", new[] { "zh-SG" });

        /// <summary>
        /// JP - Japan
        /// 日本
        /// </summary>
        public static AddressRegion JP => new("JP", "JPN", "392", AddressContinent.AS, "010", "81", "JPY", new[] { "ja-JP" });

        /// <summary>
        /// US - United States
        /// 美国
        /// </summary>
        public static AddressRegion US => new("US", "USA", "840", AddressContinent.NA, "011", "1", "USD", new[] { "en-US" });

        /// <summary>
        /// CA - Canada
        /// 加拿大
        /// </summary>
        public static AddressRegion CA => new("CA", "CAN", "124", AddressContinent.NA, "011", "1", "USD", new[] { "en-CA", "fr-CA" });

        /// <summary>
        /// AU - Australia
        /// 澳大利亚
        /// </summary>
        public static AddressRegion AU => new("AU", "AUS", "036", AddressContinent.OC, "0011", "61", "AUD", new[] { "en-AU" });

        /// <summary>
        /// NZ - New Zealand
        /// 新西兰
        /// </summary>
        public static AddressRegion NZ => new("NZ", "NZL", "554", AddressContinent.OC, "00", "64", "NZD", new[] { "en-NZ", "mi-NZ" });

        /// <summary>
        /// GB - Great Britain
        /// 英国
        /// </summary>
        public static AddressRegion GB => new("GB", "GBR", "826", AddressContinent.EU, "00", "44", "GBP", new[] { "en-GB" });

        /// <summary>
        /// IE - Ireland
        /// 爱尔兰
        /// </summary>
        public static AddressRegion IE => new("IE", "IRL", "372", AddressContinent.EU, "00", "353", "IEP", new[] { "en-IE" });

        /// <summary>
        /// DE - Germany
        /// 德国
        /// </summary>
        public static AddressRegion DE => new("DE", "DEU", "276", AddressContinent.EU, "00", "49", "EUR", new[] { "de-DE" });

        /// <summary>
        /// FR - France
        /// 法国
        /// </summary>
        public static AddressRegion FR => new("FR", "FRA", "250", AddressContinent.EU, "00", "33", "EUR", new[] { "fr-FR" });

        /// <summary>
        /// All countries
        /// 所有国家
        /// </summary>
        public static IEnumerable<AddressRegion> All => new List<AddressRegion>
        {
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
        };

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

        // Constructor
        // 构造函数
        public AddressRegion(string id, string id3, string nid, AddressContinent continent, string exitCode, string idd, string currency, IEnumerable<string> languages)
        {
            (Id, Id3, Nid, continent, ExitCode, Idd, Currency, Languages)
                =
            (id, id3, nid, Continent, exitCode, idd, currency, languages);
        }
    }

    /// <summary>
    /// Continent
    /// 洲
    /// </summary>
    public enum AddressContinent
    {
        /// <summary>
        /// Africa
        /// 非洲
        /// </summary>
        AF = 1,

        /// <summary>
        /// Antarctica
        /// 南极洲
        /// </summary>
        AN = 2,

        /// <summary>
        /// Asia
        /// 亚洲
        /// </summary>
        AS = 3,

        /// <summary>
        /// Europe
        /// 欧洲
        /// </summary>
        EU = 4,

        /// <summary>
        /// North America
        /// 北美洲
        /// </summary>
        NA = 5,

        /// <summary>
        /// Oceania
        /// 大洋洲
        /// </summary>
        OC = 6,

        /// <summary>
        /// South America
        /// 南美洲
        /// </summary>
        SA = 7
    }
}
