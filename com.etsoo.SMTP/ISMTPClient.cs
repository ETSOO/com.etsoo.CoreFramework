using MimeKit;

namespace com.etsoo.SMTP
{
    /// <summary>
    /// SMTP client interface
    /// SMTP客户端接口
    /// </summary>
    public interface ISMTPClient
    {
        /// <summary>
        /// Options
        /// 配置
        /// </summary>
        SMTPClientOptions Options { get; }

        /// <summary>
        /// Send email
        /// 发送电子邮件
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Task</returns>
        Task SendAsync(MimeMessage message, CancellationToken token = default);
    }
}
