using com.etsoo.Utils.SpanMemory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace com.etsoo.Utils.String
{
    /// <summary>
    /// String utils
    /// 字符串工具类
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// IEnumerable to string
        /// 链接 IEnumerable 为字符串
        /// </summary>
        /// <param name="items">IEnumerable list</param>
        /// <param name="splitter">Splitter for items</param>
        /// <returns>Result</returns>
        public static string IEnumerableToString(IEnumerable items, char splitter = ',')
        {
            var ss = new StringBuilder();
            var enumerator = items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null)
                    continue;
                ss.Append(splitter);
                ss.Append(enumerator.Current.ToString());
            }
            return ss.Remove(0, 1).ToString();
        }

        /// <summary>
        /// Dictionary to string
        /// 字典对象转化为字符串
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="itemSplitter">Item splitter</param>
        /// <param name="keyValueSplitter">Key value splitter</param>
        /// <returns>Result</returns>
        public static string DictionaryToString(IDictionary dic, char itemSplitter = '&', char keyValueSplitter = '=')
        {
            var ss = new StringBuilder();

            var enumerator = dic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key == null || enumerator.Value == null)
                    continue;

                ss.Append(itemSplitter);
                ss.Append(enumerator.Key);
                ss.Append(keyValueSplitter);
                ss.Append(enumerator.Value);
            }

            return ss.Remove(0, 1).ToString();
        }

        /// <summary>
        /// String as enumerable list
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="input">Input string</param>
        /// <param name="splitter">Splitter</param>
        /// <returns>List</returns>
        public static IEnumerable<T> AsEnumerable<T>(string? input, char splitter = ',') where T : struct
        {
            if (string.IsNullOrEmpty(input))
                yield break;

            var items = input.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in items)
            {
                var target = TryParse<T>(item);
                if (target.HasValue)
                    yield return target.Value;
            }
        }

        /// <summary>
        /// String as enumerable list
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="input">Input string</param>
        /// <param name="splitter">Splitter</param>
        /// <returns>List</returns>
        public static IEnumerable<string> AsEnumerable(string? input, char splitter = ',')
        {
            if (string.IsNullOrEmpty(input))
                return Array.Empty<string>();

            return input.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Transform Pascal name to Linux style (Snake) name, like "HelloWorld" to "hello_world"
        /// Previous version: return Regex.Replace(name, "([A-Z])", m => "_" + char.ToLower(m.Value[0]), RegexOptions.Compiled).TrimStart('_');
        /// 把Pascal命名的字符串改成Linux风格，如 HelloWorld 改成 hello_world
        /// </summary>
        /// <param name="name">Pascal name</param>
        /// <returns>Linux style name</returns>
        public static ReadOnlySpan<char> PascalCaseToLinuxStyle(ReadOnlySpan<char> name)
        {
            // If empty, just return
            if (name.IsEmpty)
                return name;

            // Char count
            var len = name.Length;

            // Contiguous stack memory, 2 times maximum
            var builder = new SpanBuilder<char>(2 * len);

            // Last copy index
            var index = 0;

            for (var i = 1; i < len; i++)
            {
                var c = name[i];

                if (char.IsUpper(c))
                {
                    // Length to copy
                    var cLen = i - index;

                    // Copy previous items
                    builder.Append(name.Slice(index, cLen), char.ToLower);

                    // Add an underscore
                    builder.Append('_');

                    // Next turn
                    index = i;
                }
            }

            var leftLen = len - index;

            // Copy
            builder.Append(name.Slice(index, leftLen), char.ToLower);

            // Return
            return builder.AsSpan();
        }

        /// <summary>
        /// Transform Linux style (Snake) name to Pascal name, like "hello_world" to "HelloWorld"
        /// Previous: return name.Substring(0, 1).ToUpper() + Regex.Replace(name[1..], "(_[a-z])", m => char.ToUpper(m.Value[1]).ToString(), RegexOptions.Compiled);
        /// 把Linux风格的字符串改成Pascal命名，如 hello_world 改成 HelloWorld
        /// </summary>
        /// <param name="name">Linux style name</param>
        /// <returns>Pascal name</returns>
        public static ReadOnlySpan<char> LinuxStyleToPascalCase(ReadOnlySpan<char> name)
        {
            // If empty, just return
            if (name.IsEmpty)
                return name;

            // Char count
            var len = name.Length;

            // Builder
            var builder = new SpanBuilder<char>(len);

            // Last copy index
            var index = 0;

            for (var i = 1; i < len; i++)
            {
                var c = name[i];

                if (c.Equals('_'))
                {
                    // Length to copy
                    var cLen = i - index;

                    // Copy
                    builder.Append(name.Slice(index, cLen), char.ToUpper);

                    // Next turn
                    index = i + 1;
                }
            }

            var leftLen = len - index;

            // Copy
            builder.Append(name.Slice(index, leftLen), char.ToUpper);

            // Return
            return builder.AsSpan();
        }

        private delegate bool TryParseDelegate<T>(string s, out T input);

        private static TryParseDelegate<bool> GetBoolParser(ref string s)
        {
            if (s is "on" or "1") s = "true";
            else if (s is "off" or "0") s = "false";

            return new TryParseDelegate<bool>(bool.TryParse);
        }

        /// <summary>
        /// Try parse string to target type
        /// 尝试解析字符串到目标类型
        /// </summary>
        /// <typeparam name="T">Generic target type</typeparam>
        /// <param name="s">Input string</param>
        /// <returns>Parsed result</returns>
        public static T? TryParse<T>(string? s) where T : struct
        {
            // Null or blank content
            if (string.IsNullOrWhiteSpace(s))
                return null;

            // Default value
            T value = default;

            // Switch pattern
            var parser = value switch
            {
                Enum       => new TryParseDelegate<T>(Enum.TryParse),
                bool       => GetBoolParser(ref s) as TryParseDelegate<T>,
                int        => new TryParseDelegate<int>(int.TryParse) as TryParseDelegate<T>,
                long       => new TryParseDelegate<long>(long.TryParse) as TryParseDelegate<T>,
                DateTime   => new TryParseDelegate<DateTime>(DateTime.TryParse) as TryParseDelegate<T>,
                decimal    => new TryParseDelegate<decimal>(decimal.TryParse) as TryParseDelegate<T>,
                double     => new TryParseDelegate<double>(double.TryParse) as TryParseDelegate<T>,
                float      => new TryParseDelegate<double>(double.TryParse) as TryParseDelegate<T>,
                byte       => new TryParseDelegate<byte>(byte.TryParse) as TryParseDelegate<T>,
                Guid       => new TryParseDelegate<Guid>(Guid.TryParse) as TryParseDelegate<T>,
                char       => new TryParseDelegate<char>(char.TryParse) as TryParseDelegate<T>,
                short      => new TryParseDelegate<short>(short.TryParse) as TryParseDelegate<T>,
                ushort     => new TryParseDelegate<ushort>(ushort.TryParse) as TryParseDelegate<T>,
                uint       => new TryParseDelegate<uint>(uint.TryParse) as TryParseDelegate<T>,
                ulong      => new TryParseDelegate<ulong>(ulong.TryParse) as TryParseDelegate<T>,
                _          => null
            };

            if (parser == null)
            {
                return null;
            }

            if (parser(s, out T newValue))
            {
                return newValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Try parse object to specific type
        /// 尝试解析对象到特定类型
        /// </summary>
        /// <typeparam name="T">Generic target type</typeparam>
        /// <param name="d">Object value</param>
        /// <returns>Parsed result</returns>
        public static T? TryParseObject<T>(object? d) where T : struct
        {
            if (d == null || d == DBNull.Value)
            {
                return null;
            }

            if (d is T t)
            {
                return t;
            }

            return TryParse<T>(d.ToString());
        }
    }
}
