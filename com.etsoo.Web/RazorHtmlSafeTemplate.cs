using RazorEngineCore;

namespace com.etsoo.Web
{
    class RawContent(object value)
    {
        public object Value { get; set; } = value;
    }

    /// <summary>
    /// Razor HTML safe template HTML utilities
    /// Razor HTML安全模板HTML工具
    /// </summary>
    public class RazorHtmlSafeTemplateHtml
    {
        /// <summary>
        /// Raw content
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>Result</returns>
        public object Raw(object value)
        {
            return new RawContent(value);
        }
    }

    /// <summary>
    /// Razor HTML safe template
    /// https://github.com/adoconnection/RazorEngineCore/wiki/@Raw
    /// Razor HTML安全模板
    /// </summary>
    public class RazorHtmlSafeTemplate : RazorEngineTemplateBase
    {
        /// <summary>
        /// Simulate the Html object in Razor
        /// 模仿Razor中的Html对象
        /// </summary>
        public RazorHtmlSafeTemplateHtml Html { get; } = new();

        public override void Write(object? obj = null)
        {
            var value = obj is RawContent rawContent
                ? rawContent.Value
                : System.Web.HttpUtility.HtmlEncode(obj);

            base.Write(value);
        }

        public override void WriteAttributeValue(string prefix, int prefixOffset, object? value, int valueOffset, int valueLength, bool isLiteral)
        {
            value = value is RawContent rawContent
                ? rawContent.Value
                : System.Web.HttpUtility.HtmlAttributeEncode(value?.ToString());

            base.WriteAttributeValue(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral);
        }
    }
}
