﻿using System.Globalization;
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