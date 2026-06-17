using com.etsoo.Utils.Serialization.Country;
using TimeZoneConverter;

namespace com.etsoo.Database.Converters
{
    /// <summary>
    /// Time zone utilities
    /// 时区工具
    /// </summary>
    public static class TimeZoneUtils
    {
        /// <summary>
        /// Create a new instance of <see cref="TimeZoneItem"/> from time zone info
        /// 从时区信息创建一个新的<see cref="TimeZoneItem"/>实例
        /// </summary>
        /// <param name="tz">Time zone info</param>
        /// <returns>Result</returns>
        public static TimeZoneItem CreateFrom(TimeZoneInfo tz)
        {
            var id = tz.Id;

            if (!TZConvert.KnownIanaTimeZoneNames.Contains(id, StringComparer.OrdinalIgnoreCase))
            {
                id = TZConvert.WindowsToIana(id) ?? id;
            }

            return new TimeZoneItem
            {
                Id = id,
                DisplayName = tz.DisplayName,
                StandardName = tz.StandardName,
                UtcOffset = tz.BaseUtcOffset
            };
        }

        /// <summary>
        /// Create a new instance of <see cref="TimeZoneItem"/> from time zone id
        /// 通过时区ID创建一个新的<see cref="TimeZoneItem"/>实例
        /// </summary>
        /// <param name="id">Time zone ID</param>
        /// <returns>Result</returns>
        public static TimeZoneItem? CreateFrom(string id)
        {
            var tz = GetTimeZoneBase(id);
            if (tz == null) return null;

            return CreateFrom(tz);
        }

        /// <summary>
        /// Get time zone
        /// 获取时区
        /// </summary>
        /// <param name="timeZone">Time zone</param>
        /// <returns>Time zone</returns>
        public static TimeZoneInfo? GetTimeZoneBase(string? timeZone)
        {
            if (timeZone != null && TZConvert.TryGetTimeZoneInfo(timeZone, out var ts))
            {
                return ts;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get time zone
        /// 获取时区
        /// </summary>
        /// <param name="timeZone">Time zone</param>
        /// <returns>Time zone</returns>
        public static TimeZoneInfo GetTimeZone(string? timeZone)
        {
            return GetTimeZoneBase(timeZone) ?? TimeZoneInfo.Local;
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
