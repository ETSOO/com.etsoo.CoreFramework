using com.etsoo.Utils.SpanMemory;
using System.Buffers;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace com.etsoo.Utils.String
{
    /// <summary>
    /// String utils
    /// 字符串工具类
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// String type
        /// 字符串类型
        /// </summary>
        public static readonly Type StringType = typeof(string);

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
                if (target != null)
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
            return new Regex("[^a-zA-Z0-9]").Replace(input, "");
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
            // Null or blank content
            if (string.IsNullOrWhiteSpace(s))
                return null;

            // Default value
            var value = default(T);

            // Switch pattern
            var parser = value switch
            {
                Enum => new TryParseDelegate<T>(Enum.TryParse),
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

        /// <summary>
        /// Try parse object to all possible type, may bear performance lost
        /// 尝试解析对象到所有可能的类型，可能有效率损失
        /// </summary>
        /// <typeparam name="T">Generic target type</typeparam>
        /// <param name="d">Object value</param>
        /// <returns>Parsed result</returns>
        public static T? TryParseObjectAll<T>(object? d)
        {
            if (d == null || d == DBNull.Value)
            {
                return default;
            }

            if (d is T t)
            {
                return t;
            }

            var s = d.ToString();
            if (s == null) return default;

            if (s is T st)
            {
                return st;
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(StringType))
            {
                return (T?)converter.ConvertFromInvariantString(s);
            }

            return default;
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
                Indented = false
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
    }
}
