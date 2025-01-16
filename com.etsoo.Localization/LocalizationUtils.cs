using com.etsoo.Localization.Country;
using System.Globalization;
using TimeZoneConverter;

namespace com.etsoo.Localization
{
    /// <summary>
    /// Localization utilities
    /// 本地化工具
    /// </summary>
    public static class LocalizationUtils
    {
        /// <summary>
        /// Set culture
        /// 设置文化
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Changed or not</returns>
        public static CultureInfo SetCulture(string language)
        {
            if (CultureInfo.CurrentCulture.Name == language && CultureInfo.CurrentUICulture.Name == language)
                return CultureInfo.CurrentCulture;

            // Set related cultures
            var ci = new CultureInfo(language);
            SetCulture(ci);

            return ci;
        }

        /// <summary>
        /// Set culture
        /// 设置文化
        /// </summary>
        /// <param name="ci">Culture info</param>
        /// <returns>Changed or not</returns>
        public static void SetCulture(CultureInfo ci)
        {
            // Set related cultures
            CultureInfo.CurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        /// <summary>
        /// Get time zone
        /// 获取时区
        /// </summary>
        /// <param name="timeZone">Time zone</param>
        /// <returns>Time zone</returns>
        public static TimeZoneInfo GetTimeZone(string? timeZone)
        {
            if (timeZone != null && TZConvert.TryGetTimeZoneInfo(timeZone, out var ts))
            {
                return ts;
            }

            return TimeZoneInfo.Local;
        }

        /// <summary>
        /// Check the input is time zone string or not
        /// 检查输入是否为时区字符串
        /// </summary>
        /// <param name="timeZone">Time zone</param>
        /// <returns>Result</returns>
        public static bool IsTimeZone(string? timeZone)
        {
            if (!string.IsNullOrEmpty(timeZone) && TZConvert.TryGetTimeZoneInfo(timeZone, out _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get regions by currency (USD, CNY), one currency may be used in multiple countries
        /// 从币种获取区域信息
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static IEnumerable<(RegionInfo Region, CultureInfo Culture)> GetRegionsByCurrency(string currency)
        {
            // RegionInfo.CurrentRegion;

            // Two letter code ISO3166 of country / region
            // new RegionInfo("CN");

            // Cultrue, but new RegionInfo("zh-Hans") will failed because of missing country/region info
            // new RegionInfo("zh-CN"), new RegionInfo("zh-Hans-CN")

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (var culture in cultures)
            {
                var region = new RegionInfo(culture.Name);
                if (region.ISOCurrencySymbol.Equals(currency))
                {
                    yield return (region, culture);
                }
            }
        }

        /// <summary>
        /// Get currency data
        /// 获取币种信息
        /// </summary>
        /// <param name="currency">Curency code</param>
        /// <returns>Result</returns>
        public static (string Symbol, string Name, string EnglishName)? GetCurrencyData(string currency)
        {
            return GetAllRegions().GetCurrencyData(currency);
        }

        /// <summary>
        /// Get currency data
        /// 获取币种信息
        /// </summary>
        /// <param name="regions">Regions returned with "GetRegions"</param>
        /// <param name="currency">Curency code</param>
        /// <returns>Result</returns>
        public static (string Symbol, string Name, string EnglishName)? GetCurrencyData(this Dictionary<string, RegionItem> regions, string currency)
        {
            var item = regions.FirstOrDefault(r => r.Value.Currency.Id.Equals(currency)).Value?.Currency;
            if (item != null)
            {
                return (item.Symbol, item.Name, item.EnglishName);
            }

            return null;
        }

        /// <summary>
        /// Get currencies
        /// 获取币种信息
        /// </summary>
        /// <param name="regions">Regions returned with "GetAllRegions"</param>
        /// <param name="ids">Currency ids to include</param>
        /// <returns>Result</returns>
        public static IEnumerable<CurrencyItem> GetCurrencies(this Dictionary<string, RegionItem> regions, IEnumerable<string>? ids = null)
        {
            var currencies = regions.Select(regions => regions.Value.Currency).DistinctBy(currency => currency.Id);

            if (ids != null)
            {
                // Sort by the occurance in the ids
                var sorts = ids.ToList();
                return currencies.Where(currency => sorts.Contains(currency.Id)).OrderBy(currency => sorts.IndexOf(currency.Id));
            }

            return currencies;
        }

        /// <summary>
        /// Get regions
        /// </summary>
        /// <param name="regions">Regions returned with "GetAllRegions"</param>
        /// <param name="ids">Region ids to include</param>
        /// <returns>Result</returns>
        public static IEnumerable<RegionItem> GetRegions(this Dictionary<string, RegionItem> regions, IEnumerable<string> ids)
        {
            var sort = ids.ToList();
            return regions.Where(region => sort.Contains(region.Key)).Select(region => region.Value).OrderBy(region => sort.IndexOf(region.Id));
        }

        /// <summary>
        /// Get all regions, when caching is needed, take the culture into consideration
        /// 获取所有地区信息，需要缓存时，考虑文化
        /// </summary>
        /// <param name="regionIds">Region ids to include</param>
        /// <returns>Result</returns>
        public static Dictionary<string, RegionItem> GetAllRegions(IEnumerable<string>? regionIds = null)
        {
            // Regions
            var regions = new Dictionary<string, RegionItem>();

            // Get all regions
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (var culture in cultures)
            {
                // Region id, like "CN"
                var regionId = culture.Name.Split('-').Last();
                if (regionId.Length != 2 || (regionIds != null && !regionIds.Contains(regionId)))
                {
                    continue;
                }

                // Region info
                RegionInfo ri;
                try
                {
                    ri = new RegionInfo(regionId);
                }
                catch
                {
                    continue;
                }

                // Id, like "CN"
                var id = ri.TwoLetterISORegionName;

                // Three letter id, like "CHN"
                // When the region is not a country, it will be empty
                var id3 = ri.ThreeLetterISORegionName;
                if (string.IsNullOrEmpty(id3))
                {
                    continue;
                }

                if (regions.TryGetValue(id, out var region))
                {
                    region.Cultures.Add(new CultureItem
                    {
                        Id = culture.Name,
                        Id2 = culture.TwoLetterISOLanguageName,
                        Id3 = culture.ThreeLetterISOLanguageName,
                        Parent = culture.Parent.Name,
                        NativeName = culture.NativeName,
                        EnglishName = culture.EnglishName
                    });
                }
                else
                {
                    regions.Add(id, new RegionItem
                    {
                        Id = id,
                        Id3 = id3,
                        Name = Resources.ResourceManager.GetString(id) ?? ri.CurrencyNativeName,
                        NativeName = ri.NativeName,
                        EnglishName = ri.EnglishName,
                        Currency = new CurrencyItem
                        {
                            Id = ri.ISOCurrencySymbol,
                            Name = Resources.ResourceManager.GetString(ri.ISOCurrencySymbol) ?? ri.CurrencyNativeName,
                            NativeName = ri.CurrencyNativeName,
                            EnglishName = ri.CurrencyEnglishName,
                            Symbol = ri.CurrencySymbol
                        },
                        Cultures = [
                            new CultureItem
                                {
                                    Id = culture.Name,
                                    Id2 = culture.TwoLetterISOLanguageName,
                                    Id3 = culture.ThreeLetterISOLanguageName,
                                    Parent = culture.Parent.Name,
                                    NativeName = culture.NativeName,
                                    EnglishName = culture.EnglishName
                                }
                        ]
                    });
                }
            }

            return regions;
        }

        /// <summary>
        /// Get cultures by country / region code
        /// Two letter code ISO3166, like CN
        /// 从国家编号获取文化信息
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public static IEnumerable<CultureInfo> GetCulturesByCountry(string country)
        {
            var ends = $"-{country}";
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                              .Where(c => c.Name.EndsWith(ends));
        }

        /// <summary>
        /// Utc datetime to local
        /// UTC时间转换为本地时间
        /// </summary>
        /// <param name="input">Input Utc datetime</param>
        /// <param name="timeZone">Target local time zone</param>
        /// <returns>Local datetime</returns>
        public static DateTime UtcToLocal(this DateTime input, string? timeZone)
        {
            return input.UtcToLocal(GetTimeZone(timeZone));
        }

        /// <summary>
        /// Utc datetime to local
        /// UTC时间转换为本地时间
        /// </summary>
        /// <param name="input">Input Utc datetime</param>
        /// <param name="timeZone">Target local time zone</param>
        /// <returns>Local datetime</returns>
        public static DateTime UtcToLocal(this DateTime input, TimeZoneInfo timeZone)
        {
            // Ignore local
            if (input.Kind == DateTimeKind.Local)
                return input;

            // Convert
            return TimeZoneInfo.ConvertTimeFromUtc(input, timeZone);
        }
    }
}
