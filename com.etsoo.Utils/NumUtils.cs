namespace com.etsoo.Utils
{
    /// <summary>
    /// Number utilities
    /// 数字工具
    /// </summary>
    public static class NumUtils
    {
        /// <summary>
        /// Get month period number by date
        /// 从日期获取月周期数字
        /// </summary>
        /// <param name="date">Input date</param>
        /// <returns>Month period number</returns>
        public static int GetMonthPeriod(DateTimeOffset date)
        {
            return GetMonthPeriod(date.Year, date.Month);
        }

        /// <summary>
        /// Get month period number by year and month
        /// 从年和月获取月周期数字
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month, start from 1</param>
        /// <returns>Month period number</returns>
        public static int GetMonthPeriod(int year, int month)
        {
            return year * 100 + month;
        }

        /// <summary>
        /// Get month period range by year
        /// 从年获取月周期范围
        /// </summary>
        /// <param name="year">Year</param>
        /// <returns>Result</returns>
        public static (int MonthStart, int MonthEnd) GetMonthPeriodRange(int year)
        {
            return (GetMonthPeriod(year, 1), GetMonthPeriod(year, 12));
        }

        /// <summary>
        /// Get quarter period number by year and month
        /// 从年和月获取季度周期数字
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month, start from 1</param>
        /// <returns>Quarter period number</returns>
        public static int GetQuarterPeriod(int year, int month)
        {
            return year * 10 + (month - 1) / 3 + 1;
        }

        /// <summary>
        /// Get quarter period range by year
        /// 从年获取季度周期范围
        /// </summary>
        /// <param name="year">Year</param>
        /// <returns>Result</returns>
        public static (int QuarterStart, int QuarterEnd) GetQuarterPeriodRange(int year)
        {
            return (GetQuarterPeriod(year, 1), GetQuarterPeriod(year, 12));
        }
    }
}