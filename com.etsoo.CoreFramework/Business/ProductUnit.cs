namespace com.etsoo.CoreFramework.Business
{
    /// <summary>
    /// Product units
    /// 产品单位
    /// </summary>
    public enum ProductUnit : byte
    {
        /// <summary>
        /// Picese
        /// 件
        /// </summary>
        PC = 1,

        /// <summary>
        /// Set
        /// 套
        /// </summary>
        SET = 2,

        /**
         * Merge from RepeatOption
         * **/
        /// <summary>
        /// Hour
        /// 小时
        /// </summary>
        HOUR = 10,

        /// <summary>
        /// Day
        /// 天
        /// </summary>
        DAY = 11,

        /// <summary>
        /// Year
        /// 年
        /// </summary>
        YEAR = 12,

        /// <summary>
        /// Week
        /// 周
        /// </summary>
        WEEK = 21,

        /// <summary>
        /// Two weeks
        /// 两周
        /// </summary>
        FORTNIGHT = 22,

        /// <summary>
        /// Four weeks
        /// 四周
        /// </summary>
        FOURWEEK = 24,

        /// <summary>
        /// Month
        /// 月
        /// </summary>
        MONTH = 31,

        /// <summary>
        /// Two months
        /// 两月
        /// </summary>
        BIMONTH = 32,

        /// <summary>
        /// Quater
        /// 季度
        /// </summary>
        QUATER = 33,

        /// <summary>
        /// Half year
        /// 半年
        /// </summary>
        HALFYEAR = 36,

        /// <summary>
        /// Time
        /// 次
        /// </summary>
        TIME = 99,

        /// <summary>
        /// Money
        /// 储值
        /// </summary>
        MONEY = 100,

        /// <summary>
        /// Gram
        /// 克
        /// </summary>
        GRAM = 40,

        /// <summary>
        /// 500G
        /// 斤
        /// </summary>
        JIN = 41,

        /// <summary>
        /// Kilogram
        /// 千克
        /// </summary>
        KILOGRAM = 42,

        /// <summary>
        /// Ton
        /// 吨
        /// </summary>
        TON = 49
    }
}
