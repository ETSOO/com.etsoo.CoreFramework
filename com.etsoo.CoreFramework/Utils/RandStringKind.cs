using System;

namespace com.etsoo.CoreFramework.Utils
{
    /// <summary>
    /// Random string kind
    /// 随机字符串类型
    /// </summary>
    [Flags]
    public enum RandStringKind
    {
        /// <summary>
        /// Digits
        /// 数字
        /// </summary>
        Digit = 1,

        /// <summary>
        /// Lowercase letters
        /// 小写字母
        /// </summary>
        LowerCaseLetter = 2,

        /// <summary>
        /// Upercase letters
        /// 大写字母
        /// </summary>
        UperCaseLetter = 4,

        /// <summary>
        /// Digits and letters
        /// 数字和字母
        /// </summary>
        DigitAndLetter = 7,

        /// <summary>
        /// Symbols
        /// 符号字符
        /// </summary>
        Symbol = 8,

        /// <summary>
        /// All
        /// 全部字符
        /// </summary>
        All = 15
    }
}
