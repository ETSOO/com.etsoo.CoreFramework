using Dapper;
using System.Text;
using System.Web;

namespace com.etsoo.Utils.String
{
    /// <summary>
    /// String extension
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Is the string only ansii characters
        /// 字符串是唯一的ASCII字符吗
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        public static bool IsAnsi(this string input)
        {
            return !input.Any(c => c > 127);
        }

        /// <summary>
        /// Join as string, ended with itemSplitter
        /// 链接成字符串，以 itemSplitter 结尾
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Items</param>
        /// <param name="valueSplitter">Name / value splitter</param>
        /// <param name="itemSplitter">Item splitter</param>
        /// <returns></returns>
        public static string JoinAsString<T>(this IEnumerable<KeyValuePair<string, T>> items, string valueSplitter = "=", string itemSplitter = "&")
        {
            return items.Aggregate(new StringBuilder(), (s, x) => s.Append(x.Key + valueSplitter + x.Value + itemSplitter), s => s.ToString());
        }

        /// <summary>
        /// Join as web query, ended with itemSplitter
        /// 链接成网页查询字符串，以 itemSplitter 结尾
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Items</param>
        /// <param name="valueSplitter">Name / value splitter</param>
        /// <param name="itemSplitter">Item splitter</param>
        /// <returns>Result</returns>
        public static string JoinAsQuery<T>(this IEnumerable<KeyValuePair<string, T>> items, string valueSplitter = "=", string itemSplitter = "&")
        {
            return items.Aggregate(new StringBuilder(), (s, x) => s.Append(HttpUtility.UrlEncode(x.Key) + valueSplitter + (x.Value == null ? string.Empty : HttpUtility.UrlEncode(x.Value.ToString())) + itemSplitter), s => s.ToString());
        }

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

            return StringUtils.PascalCaseToLinuxStyle(input);
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

            return StringUtils.LinuxStyleToPascalCase(input);
        }

        /// <summary>
        /// To Pascal word
        /// 转换为首字母大写单词
        /// </summary>
        /// <param name="input">Input word</param>
        /// <returns>Pascal word</returns>
        public static ReadOnlySpan<char> ToPascalWord(this ReadOnlySpan<char> input)
        {
            Span<char> span = new char[input.Length];

            // Change the first letter to upper case
            span[0] = char.ToUpper(input[0]);

            // Left letters to lower case
            input[1..].ToLowerInvariant(span[1..]);

            return span;
        }

        /// <summary>
        /// To Json true or false, avoid True/False
        /// 转换为Json的true或者false，避免出现True/False
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>true or false</returns>
        public static string ToJson(this bool input)
        {
            return input ? "true" : "false";
        }

        /// <summary>
        /// To Json null, true or false, avoid True/False
        /// 转换为Json的null, true或者false，避免出现True/False
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>true or false</returns>
        public static string ToJson(this bool? input)
        {
            if (!input.HasValue)
                return "null";

            return input.Value.ToJson();
        }

        /// <summary>
        /// To Dapper DbString
        /// 转化为 Dapper DbString
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="isAnsi">Is non-unicode</param>
        /// <param name="length">Length</param>
        /// <param name="fixedLength">Fixed length</param>
        /// <returns>DbString</returns>
        public static DbString? ToDbString(this string input, bool? isAnsi = null, int? length = null, bool? fixedLength = null)
        {
            // Remove unnecessary empty
            input = input.Trim();
            if (input == string.Empty) return null;

            return input.ToDbStringSafe(isAnsi, length, fixedLength);
        }

        /// <summary>
        /// To Dapper DbString without empty by any chance.
        /// 确保没有任何空白的转化为 Dapper DbString
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="isAnsi">Is non-unicode</param>
        /// <param name="length">Length</param>
        /// <param name="fixedLength">Fixed length</param>
        /// <returns>DbString</returns>
        public static DbString ToDbStringSafe(this string input, bool? isAnsi = null, int? length = null, bool? fixedLength = null)
        {
            return new DbString { IsAnsi = isAnsi ?? input.IsAnsi(), IsFixedLength = fixedLength.GetValueOrDefault(), Value = input, Length = length ?? input.Length };
        }
    }
}
