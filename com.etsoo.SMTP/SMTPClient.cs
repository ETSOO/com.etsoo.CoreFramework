using MailKit.Net.Smtp;
using MimeKit;

namespace com.etsoo.SMTP
{
    /// <summary>
    /// SMTP client
    /// SMTP客户端
    /// </summary>
    public class SMTPClient : ISMTPClient
    {
        /// <summary>
        /// Options
        /// 配置
        /// </summary>
        public SMTPClientOptions Options { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="options">Options</param>
        public SMTPClient(SMTPClientOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Format message
        /// 格式化信息
        /// </summary>
        /// <param name="message">Message</param>
        protected virtual void FormatMessage(MimeMessage message)
        {
            // Set the sender
            message.Sender ??= MailboxAddress.Parse(Options.Sender ?? Options.UserName);

            if (message.From.Count == 0)
            {
                // Set one from
                message.From.Add(message.Sender);
            }

            // Default recipients
            message.To.AddRange(Options.To);
            message.Cc.AddRange(Options.Cc);
            message.Bcc.AddRange(Options.Bcc);

            if (message.To.Count == 0)
            {
                message.From.Add(message.Sender);
            }
        }

        /// <summary>
        /// Send email
        /// 发送电子邮件
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual async Task SendAsync(MimeMessage message, CancellationToken token = default)
        {
            // Format the message
            FormatMessage(message);

            // Client
            using var client = new SmtpClient();

            // Connection
            await client.ConnectAsync(Options.Host, Options.Port, Options.UseSsl, token);

            // Auth
            if (!string.IsNullOrEmpty(Options.UserName) && !string.IsNullOrEmpty(Options.Password))
            {
                await client.AuthenticateAsync(Options.UserName, Options.Password, token);
            }

            // Send
            await client.SendAsync(message, token);

            // Disconnect
            await client.DisconnectAsync(true, token);
        }
    }
}