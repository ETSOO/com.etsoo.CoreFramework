using System.Text.Json.Serialization;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// PII (Personally Identifiable Information) attribute
    /// PII（个人身份信息）属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct, Inherited = false)]
    public class PIIAttribute : JsonConverterAttribute
    {
        public override JsonConverter? CreateConverter(Type typeToConvert)
        {
            return new PIIConverter();
        }
    }
}