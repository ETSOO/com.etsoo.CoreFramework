using com.etsoo.Utils.String;
using System.Xml;

namespace com.etsoo.Utils
{
    /// <summary>
    /// Xml utils
    /// Xml工具
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        /// Get list value items <item><field1>...</field1><field2>...</field2></item><item><field1>...</field1><field2>...</field2></item>
        /// 获取列表值的子对象
        /// </summary>
        /// <param name="value">List value</param>
        /// <returns>List items</returns>
        public static IEnumerable<Dictionary<string, string>> GetList(string? value)
        {
            if (string.IsNullOrEmpty(value)) yield break;

            using var reader = XmlReader.Create(SharedUtils.GetStream(value), new XmlReaderSettings { Async = false, ConformanceLevel = ConformanceLevel.Fragment });
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Depth == 0)
                {
                    yield return ParseReader(reader.ReadSubtree(), 1);
                }
            }
        }

        /// <summary>
        /// Get field value
        /// 获取字段值
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="field">Field</param>
        /// <returns>Value</returns>
        public static string? GetValue(Dictionary<string, string> dic, string field)
        {
            if (dic.TryGetValue(field, out var value))
            {
                return value;
            }
            return default;
        }

        /// <summary>
        /// Get field value
        /// 获取字段值
        /// </summary>
        /// <typeparam name="T">Generic value type</typeparam>
        /// <param name="dic">Dictionary</param>
        /// <param name="field">Field</param>
        /// <returns>Value</returns>
        public static T? GetValue<T>(Dictionary<string, string> dic, string field) where T : struct
        {
            if (dic.TryGetValue(field, out var value))
            {
                return StringUtils.TryParse<T>(value);
            }
            return default;
        }

        /// <summary>
        /// Parse XML reader depth data
        /// 解析XML读写器特定层数据
        /// </summary>
        /// <param name="reader">XML reader</param>
        /// <param name="depth">Depth</param>
        /// <returns>Result</returns>
        public static Dictionary<string, string> ParseReader(XmlReader reader, int depth = 1)
        {
            var dic = new Dictionary<string, string>();

            var nextDepth = depth + 1;

            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Depth == depth)
                {
                    var name = reader.Name;
                    var values = new List<string>();
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement) break;

                        if (reader.Depth == nextDepth)
                        {
                            if (reader.HasValue)
                            {
                                values.Add(reader.Value);
                            }
                            else
                            {
                                values.Add(reader.ReadOuterXml());
                            }
                        }
                    }

                    dic[name] = string.Join(string.Empty, values);
                }
            }

            return dic;
        }

        /// <summary>
        /// Parse XML specific depth data
        /// 解析XML特定层数据
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <param name="depth">Depth</param>
        /// <returns>Result</returns>
        public static Dictionary<string, string> ParseXml(Stream input, int depth = 1)
        {
            using var reader = XmlReader.Create(input, new XmlReaderSettings { Async = false });
            return ParseReader(reader, depth);
        }

        /// <summary>
        /// Async parse XML reader depth data
        /// 异步解析XML读写器特定层数据
        /// </summary>
        /// <param name="reader">XML reader</param>
        /// <param name="depth">Depth</param>
        /// <returns>Result</returns>
        public static async Task<Dictionary<string, string>> ParseReaderAsync(XmlReader reader, int depth = 1)
        {
            var dic = new Dictionary<string, string>();

            var nextDepth = depth + 1;

            while (await reader.ReadAsync())
            {
                if (reader.IsStartElement() && reader.Depth == depth)
                {
                    var name = reader.Name;
                    var values = new List<string>();
                    while (await reader.ReadAsync())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement) break;

                        if (reader.Depth == nextDepth)
                        {
                            if (reader.HasValue)
                            {
                                values.Add(reader.Value);
                            }
                            else
                            {
                                values.Add(await reader.ReadOuterXmlAsync());
                            }
                        }
                    }

                    dic[name] = string.Join(string.Empty, values);
                }
            }

            return dic;
        }

        /// <summary>
        /// Async parse XML specific depth data
        /// 异步解析XML特定层数据
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <param name="depth">Depth</param>
        /// <returns>Result</returns>
        public static async Task<Dictionary<string, string>> ParseXmlAsync(Stream input, int depth = 1)
        {
            using var reader = XmlReader.Create(input, new XmlReaderSettings { Async = true });
            return await ParseReaderAsync(reader, depth);
        }

        /// <summary>
        /// Write CData element
        /// 写入 CData 对象
        /// </summary>
        /// <param name="writer">Xml writer</param>
        /// <param name="localName">Element local name</param>
        /// <param name="text">CData text</param>
        /// <returns>Task</returns>
        public static async Task WriteCDataAsync(XmlWriter writer, string localName, string text)
        {
            await writer.WriteStartElementAsync(null, localName, null);
            await writer.WriteCDataAsync(text);
            await writer.WriteEndElementAsync();
        }
    }
}
