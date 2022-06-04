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
        /// Parse XML specific depth data
        /// 解析XML特定层数据
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <param name="depth">Depth</param>
        /// <returns>Result</returns>
        public static async Task<Dictionary<string, string>> ParseXmlAsync(Stream input, int depth = 1)
        {
            var dic = new Dictionary<string, string>();

            var nextDepth = depth + 1;

            using var reader = XmlReader.Create(input, new XmlReaderSettings { Async = true });
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
