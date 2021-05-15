namespace com.etsoo.Utils.Net.SMS
{
    /// <summary>
    /// SMS resource
    /// 短信资源
    /// </summary>
    public record SMSResource (string? EndPoint = null, string? Country = null, string? Language = null, string? Template = null, string? Signature = null, bool Default = false);
}
