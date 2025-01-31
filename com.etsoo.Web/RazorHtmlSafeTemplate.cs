using RazorEngineCore;

namespace com.etsoo.Web
{
    /// <summary>
    /// Razor HTML safe template
    /// https://github.com/adoconnection/RazorEngineCore/wiki/@Raw
    /// Razor HTML安全模板
    /// </summary>
    public class RazorHtmlSafeTemplate : RazorEngineTemplateBase
    {
        class RawContent(object value)
        {
            public object Value { get; set; } = value;
        }

        public object Raw(object value)
        {
            return new RawContent(value);
        }

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
