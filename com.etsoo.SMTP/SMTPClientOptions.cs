namespace com.etsoo.SMTP
{
    /// <summary>
    /// SMTP client options
    /// SMTP客户端配置
    /// </summary>
    public record SMTPClientOptions(string Host, int Port, bool UseSsl, string? Sender = null, string? UserName = null, string? Password = null);
}
