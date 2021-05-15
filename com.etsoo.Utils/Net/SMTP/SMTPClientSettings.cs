namespace com.etsoo.Utils.Net.SMTP
{
    /// <summary>
    /// SMTP client settings
    /// SMTP客户端配置
    /// </summary>
    public record SMTPClientSettings(string Host, int Port, bool UseSsl, string? UserName, string? Password);
}
