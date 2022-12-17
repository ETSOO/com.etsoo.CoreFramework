namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// PII (Personally Identifiable Information) attribute
    /// PII（个人身份信息）属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PIIAttribute : Attribute
    {
    }
}
