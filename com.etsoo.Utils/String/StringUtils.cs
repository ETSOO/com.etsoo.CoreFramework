﻿using com.etsoo.Utils.SpanMemory;
using System.Buffers;
using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace com.etsoo.Utils.String
{
    /// <summary>
    /// String utils
    /// 字符串工具类
    /// </summary>
    public static partial class StringUtils
    {
        /// <summary>
        /// Format file size
        /// 格式化文件大小
        /// </summary>
        /// <param name="size">File size</param>
        /// <param name="fractionDigits">Fraction digits</param>
        /// <returns>Result</returns>
        public static string FormatFileSize(long size, int fractionDigits = 2)
        {
            int i = size == 0 ? 0 : (int)Math.Floor(Math.Log(size) / Math.Log(1024));
            return $"{(size / Math.Pow(1024, i)).ToString($"F{fractionDigits}")} {new string[] { "B", "KB", "MB", "GB", "TB", "PB" }[i]}";
        }

        /// <summary>
        /// Get longest common substring (dynamic programming instead of recursion)
        /// 获取最长公共子串 (动态编程而不是递归)
        /// </summary>
        /// <param name="s1">First string</param>
        /// <param name="s2">Second string</param>
        /// <returns>Result</returns>
        public static ReadOnlySpan<char> GetLCS(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2)
        {
            var table = new int[s1.Length + 1, s2.Length + 1];
            var maxLength = 0;
            var endIndexS1 = 0;

            for (var i = 1; i <= s1.Length; i++)
            {
                for (var j = 1; j <= s2.Length; j++)
                {
                    if (s1[i - 1] == s2[j - 1])
                    {
                        var newLength = table[i - 1, j - 1] + 1;
                        table[i, j] = newLength;

                        if (newLength > maxLength)
                        {
                            maxLength = newLength;
                            endIndexS1 = i;
                        }
                    }
                    else
                    {
                        table[i, j] = 0;
                    }
                }
            }

            return maxLength > 0 ? s1.Slice(endIndexS1 - maxLength, maxLength) : [];
        }

        /// <summary>
        /// Get same parts (dynamic programming instead of recursion)
        /// 获取相同部分 (动态编程而不是递归)
        /// </summary>
        /// <param name="s1">First string</param>
        /// <param name="s2">Second string</param>
        /// <param name="minChars">Minimum characters to consider a part</param>
        /// <returns>Result</returns>
        public static string[] GetSameParts(ReadOnlySpan<char> s1, ReadOnlySpan<char> s2, int minChars = 1)
        {
            var result = new Dictionary<int, int>();

            var table = new int[s1.Length + 1, s2.Length + 1];

            for (var i = 1; i <= s1.Length; i++)
            {
                for (var j = 1; j <= s2.Length; j++)
                {
                    if (s1[i - 1] == s2[j - 1])
                    {
                        var newLength = table[i - 1, j - 1] + 1;
                        table[i, j] = newLength;

                        result[i - newLength] = newLength;
                    }
                    else
                    {
                        table[i, j] = 0;
                    }
                }
            }

            var s = s1.ToArray();

            return [.. result.Where(kv => kv.Value >= minChars && !result.Any(r => kv.Key > r.Key && kv.Key + kv.Value <= r.Key + r.Value))
                .OrderByDescending(kv => kv.Value)
                .Select(result => s.AsSpan(result.Key, result.Value).ToString())];
        }

        /// <summary>
        /// Hide normal data
        /// 隐藏一般信息
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>Result</returns>
        public static string HideData(string data, char? endChar = null)
        {
            if (data == string.Empty) return data;

            if (endChar != null)
            {
                var index = data.IndexOf(endChar.Value);
                if (index == -1)
                {
                    return HideData(data, null);
                }

                return HideData(data[..index], null) + data[index..];
            }

            var len = data.Length;
            if (len < 4)
                return data[..1] + "***";
            if (len < 6)
                return data[..2] + "***";
            if (len < 8)
                return data[..2] + "***" + data[^2..];
            if (len < 12)
                return data[..3] + "***" + data[^3..];

            return data[..4] + "***" + data[^4..];
        }

        /// <summary>
        /// Hide email address
        /// 隐藏电子邮箱地址
        /// </summary>
        /// <param name="email">Input email</param>
        /// <returns>Result</returns>
        public static string HideEmail(string email)
        {
            return HideData(email, '@');
        }

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
                return [];

            return input.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Base64 chars to number
        /// Base64字符转换为数字
        /// </summary>
        /// <param name="base64Chars">Base64 chars</param>
        /// <returns>Result</returns>
        public static long CharsToNumber(string base64Chars)
        {
            return Convert.FromBase64String(base64Chars).Select((item, index) => item * (long)Math.Pow(128, index)).Sum();
        }

        /// <summary>
        /// Get system primitive type value, same list in CommonJsonSerializerContext
        /// 获取系统基本类型值
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>Result</returns>
        public static object? GetPrimitiveValue(object? value)
        {
            return value switch
            {
                null or string or bool
                or byte or short or ushort or int or uint or long or ulong
                or float or double or decimal or DateTime or DateTimeOffset
                or Guid or char => value,
                _ => value.ToString()
            };
        }

        /// <summary>
        /// Number to base64 chars
        /// 数字转换为Base64字符
        /// </summary>
        /// <param name="input">Input number</param>
        /// <returns>Result</returns>
        public static string NumberToChars(long input)
        {
            var bytes = new List<byte>();
            while (input > 0)
            {
                var code = input % 128;
                bytes.Add((byte)code);
                input = (input - code) / 128;
            }
            return Convert.ToBase64String(bytes.ToArray());
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

        /// <summary>
        /// Remove all non letters
        /// 移除所有非字母数字符号
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        public static string RemoveNonLetters(string input)
        {
            return MyRegex().Replace(input, "");
        }

        private delegate bool TryParseDelegate<T>(string s, out T input);

        private static TryParseDelegate<bool> GetBoolParser(ref string s)
        {
            if (s is "on" or "1") s = "true";
            else if (s is "off" or "0") s = "false";

            return new TryParseDelegate<bool>(bool.TryParse);
        }

        /// <summary>
        /// Split string to int and guid parts
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        public static (int? id, Guid? guid) SplitIntGuid(string? input, char splitter = '|')
        {
            if (string.IsNullOrEmpty(input)) return (null, null);

            var parts = input.Split(splitter);
            if (parts.Length != 2) return (null, null);

            if (int.TryParse(parts[0], out var id))
            {
                if (Guid.TryParse(parts[1], out var uid))
                {
                    return (id, uid);
                }
            }

            return (null, null);
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
            var (value, isNull) = TryParseBase<T>(s);
            return isNull ? null : value;
        }

        private static (T?, bool) TryParseBase<T>(string? s)
        {
            // Default value
            var value = default(T);

            // Null or blank content
            if (string.IsNullOrWhiteSpace(s))
                return (value, true);

            // Switch pattern
            var parser = value switch
            {
                Enum => Enum.TryParse(typeof(T), s, out var e) ? (string s, out T result) =>
                {
                    result = (T)e;
                    return result != null;
                }
                : null,
                bool => GetBoolParser(ref s) as TryParseDelegate<T>,
                int => new TryParseDelegate<int>(int.TryParse) as TryParseDelegate<T>,
                long => new TryParseDelegate<long>(long.TryParse) as TryParseDelegate<T>,
                DateTime => new TryParseDelegate<DateTime>(DateTime.TryParse) as TryParseDelegate<T>,
                decimal => new TryParseDelegate<decimal>(decimal.TryParse) as TryParseDelegate<T>,
                double => new TryParseDelegate<double>(double.TryParse) as TryParseDelegate<T>,
                float => new TryParseDelegate<double>(double.TryParse) as TryParseDelegate<T>,
                byte => new TryParseDelegate<byte>(byte.TryParse) as TryParseDelegate<T>,
                Guid => new TryParseDelegate<Guid>(Guid.TryParse) as TryParseDelegate<T>,
                char => new TryParseDelegate<char>(char.TryParse) as TryParseDelegate<T>,
                short => new TryParseDelegate<short>(short.TryParse) as TryParseDelegate<T>,
                ushort => new TryParseDelegate<ushort>(ushort.TryParse) as TryParseDelegate<T>,
                uint => new TryParseDelegate<uint>(uint.TryParse) as TryParseDelegate<T>,
                ulong => new TryParseDelegate<ulong>(ulong.TryParse) as TryParseDelegate<T>,
                _ => null
            };

            // Another way to do with IParsable<TSelf> but don't work with Enum
            // https://learn.microsoft.com/en-us/dotnet/api/system.iparsable-1?view=net-8.0
            if (parser != null && parser(s, out T newValue))
            {
                return (newValue, false);
            }
            else
            {
                return (value, true);
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

        /// <summary>
        /// Try parse object to all possible type
        /// 尝试解析对象到所有可能的类型
        /// </summary>
        /// <typeparam name="T">Generic target type</typeparam>
        /// <param name="d">Object value</param>
        /// <returns>Parsed result</returns>
        public static T? TryParseObjectAll<T>(object? d) where T : notnull
        {
            var dv = default(T?);
            if (d == null || d == DBNull.Value)
            {
                return dv;
            }

            if (d is T t)
            {
                return t;
            }

            var s = d.ToString();
            if (s == null) return dv;

            if (s is T st)
            {
                return st;
            }

            var (value, isNull) = TryParseBase<T>(s);
            return isNull ? dv : value;
        }

        /// <summary>
        /// Write Json bytes
        /// 写Json字节
        /// </summary>
        /// <param name="fun">Callback function</param>
        /// <param name="options">Options</param>
        /// <returns>Json string</returns>
        public static ReadOnlySpan<byte> WriteJsonBytes(Action<Utf8JsonWriter> fun, JsonWriterOptions? options = null)
        {
            options ??= new JsonWriterOptions
            {
                Indented = false,
                Encoder = SharedUtils.JsonDefaultSerializerOptions.Encoder
            };

            var container = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(container, options.Value);

            writer.WriteStartObject();
            fun(writer);
            writer.WriteEndObject();
            writer.Flush();

            return container.WrittenSpan;
        }

        /// <summary>
        /// Write Json string
        /// 写Json字符串
        /// </summary>
        /// <param name="fun">Callback function</param>
        /// <param name="options">Options</param>
        /// <returns>Json string</returns>
        public static string WriteJson(Action<Utf8JsonWriter> fun, JsonWriterOptions? options = null)
        {
            return Encoding.UTF8.GetString(WriteJsonBytes(fun, options));
        }

        [GeneratedRegex("[^a-zA-Z0-9]")]
        private static partial Regex MyRegex();
    }
}
