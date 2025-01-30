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
