using Dapper;
using System;

namespace com.etsoo.Utils.String
{
    /// <summary>
    /// String extension
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Pascal to snake case transformation
        /// Pascal 转化为 Snake 命名
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        public static ReadOnlySpan<char> ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return StringUtil.PascalCaseToLinuxStyle(input);
        }

        /// <summary>
        /// Snake to pascal case transformation
        /// Snake 转化为 pascal 命名
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        public static ReadOnlySpan<char> ToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return StringUtil.LinuxStyleToPascalCase(input);
        }

        /// <summary>
        /// To Dapper DbString
        /// 转化为 Dapper DbString
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="isAnsi">Is non-unicode</param>
        /// <returns>DbString</returns>
        public static DbString ToDbString(this string input, bool isAnsi = false)
        {
            return new DbString { IsAnsi = isAnsi, IsFixedLength = false, Value = input, Length = input.Length };
        }
    }
}
