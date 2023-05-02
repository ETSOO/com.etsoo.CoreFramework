using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, (string, string, string)> cachedCurrencies = new();

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
            CultureInfo.CurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            return ci;
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
        public static (string Symbol, string NativeName, string EnglishName)? GetCurrencyData(string currency)
        {
            if (cachedCurrencies.TryGetValue(currency, out var data))
            {
                return data;
            }

            var items = GetRegionsByCurrency(currency);
            if (items.Any())
            {
                var region = items.First().Region;

                data = (region.CurrencySymbol, region.CurrencyNativeName, region.CurrencyEnglishName);
                cachedCurrencies.TryAdd(currency, data);

                return data;
            }

            return null;
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
