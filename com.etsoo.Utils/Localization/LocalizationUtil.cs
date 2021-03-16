using System.Globalization;
using System.Threading;

namespace com.etsoo.Utils.Localization
{
    public static class LocalizationUtil
    {
        /// <summary>
        /// Set culture
        /// 设置文化
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Changed or not</returns>
        public static bool SetCulture(string language)
        {
            if (CultureInfo.CurrentCulture.Name == language)
                return false;

            // Set related cultures
            var ci = new CultureInfo(language);
            CultureInfo.CurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            return true;
        }
    }
}
