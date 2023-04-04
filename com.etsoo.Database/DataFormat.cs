using com.etsoo.Utils;
using System.Text.Json;

namespace com.etsoo.Database
{
    /// <summary>
    /// Data format
    /// 数据格式
    /// </summary>
    public abstract class DataFormat : Enumeration<byte>
    {
        /// <summary>
        /// XML
        /// https://developer.mozilla.org/en-US/docs/Web/XML/XML_introduction
        /// </summary>
        public static readonly DataFormat Xml = new XmlDataFormat();

        /// <summary>
        /// JSON
        /// https://developer.mozilla.org/en-US/docs/Learn/JavaScript/Objects/JSON
        /// </summary>
        public static readonly DataFormat Json = new JsonDataFormat();

        /// <summary>
        /// Blank value
        /// 空值
        /// </summary>
        public abstract string BlankValue { get; }

        /// <summary>
        /// Root start content
        /// 根节点开始内容
        /// </summary>
        public abstract string RootStart { get; }

        /// <summary>
        /// Root end content
        /// 根节点结束内容
        /// </summary>
        public abstract string RootEnd { get; }

        protected DataFormat(byte id, string name) : base(id, name)
        {
        }

        /// <summary>
        /// Create element start content
        /// 创建元素开始内容
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public abstract string CreateElementStart(string name, bool firstChild);

        /// <summary>
        /// Create element end content
        /// 创建元素结束内容
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Result</returns>
        public abstract string CreateElementEnd(string name);

        /// <summary>
        /// Write JSON
        /// 写入JSON
        /// </summary>
        /// <param name="writer">Writer</param>
        public override void WriteJson(Utf8JsonWriter writer)
        {
            writer.WriteNumberValue(Value);
        }

        // XML format
        private class XmlDataFormat : DataFormat
        {
            public override string BlankValue => "";

            public override string RootStart => "<root>";

            public override string RootEnd => "</root>";

            public XmlDataFormat() : base(0, "XML") { }

            public override string CreateElementStart(string name, bool firstChild)
            {
                return $"<{name}>";
            }

            public override string CreateElementEnd(string name)
            {
                return $"</{name}>";
            }
        }

        // JSON format
        private class JsonDataFormat : DataFormat
        {
            public override string BlankValue => "null";

            public override string RootStart => "{";

            public override string RootEnd => "}";

            public JsonDataFormat() : base(1, "JSON") { }

            public override string CreateElementStart(string name, bool firstChild)
            {
                return $"{(firstChild ? "" : ", ")}\"{name}\":";
            }

            public override string CreateElementEnd(string name)
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Data format JSON converter
    /// 数据格式 JSON 转化器
    /// </summary>
    public class DataFormatConverter : EnumerationConverter<DataFormat, byte> { }
}
