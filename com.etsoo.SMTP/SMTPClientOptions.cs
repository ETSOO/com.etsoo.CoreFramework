namespace com.etsoo.SMTP
{
    /// <summary>
    /// SMTP client options
    /// SMTP客户端配置
    /// </summary>
    public record SMTPClientOptions(
        string Host,
        int Port,
        bool UseSsl,
        string? Sender = null,
        string? UserName = null,
        string? Password = null,
        IEnumerable<string>? To = null,
        IEnumerable<string>? Cc = null,
        IEnumerable<string>? Bcc = null
    )
    {
        /// <summary>
        /// Configuration section name
        /// 配置区名称
        /// </summary>
        public const string SectionName = "EtsooProxy:SMTP";
    }
}
