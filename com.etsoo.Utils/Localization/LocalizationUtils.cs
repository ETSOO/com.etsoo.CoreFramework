﻿using System.Globalization;
using TimeZoneConverter;

namespace com.etsoo.Utils.Localization
{
    public static class LocalizationUtils
    {
        private static DateTime JsBaseDateTime => new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
        /// Set datatime's Utc kind
        /// 设置日期时间的类型为Utc
        /// </summary>
        /// <param name="input">Input datetime</param>
        /// <returns>Utc datetime</returns>
        public static DateTime? SetUtcKind(DateTime? input)
        {
            if (input == null)
                return input;
            return SetUtcKind(input.Value);
        }

        /// <summary>
        /// Set datatime's Utc kind
        /// 设置日期时间的类型为Utc
        /// </summary>
        /// <param name="input">Input datetime</param>
        /// <returns>Utc datetime</returns>
        public static DateTime SetUtcKind(DateTime input)
        {
            return input.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(input, DateTimeKind.Utc) : input;
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
        /// JS date miliseconds to C# DateTime UTC
        /// JS 日期毫秒到 C# DateTime UTC
        /// </summary>
        /// <param name="miliseconds">Miliseconds</param>
        /// <returns>DateTime UTC</returns>
        public static DateTime JsMilisecondsToUTC(long miliseconds)
        {
            return JsBaseDateTime.AddMilliseconds(miliseconds);
        }

        /// <summary>
        /// DateTime UTC to JS date miliseconds
        /// C# DateTime UTC 到JS 日期毫秒
        /// </summary>
        /// <param name="dt">UTC DateTime</param>
        /// <returns>Miliseconds</returns>
        public static long UTCToJsMiliseconds(DateTime? dt = null)
        {
            return (long)(dt.GetValueOrDefault(DateTime.UtcNow) - JsBaseDateTime).TotalMilliseconds;
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
