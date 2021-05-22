using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Net.SMTP
{
    /// <summary>
    /// SMTP client
    /// SMTP客户端
    /// </summary>
    public class SMTPClient : ISMTPClient
    {
        private static SMTPClientSettings Parse(IConfigurationSection section)
        {
            return new SMTPClientSettings(
                section.GetValue<string>("Host"),
                section.GetValue("Port", 0),
                section.GetValue("UseSsl", false),
                section.GetValue<string?>("Sender"),
                section.GetValue<string?>("UserName"),
                section.GetValue<string?>("Password")
            );
        }

        /// <summary>
        /// Settings
        /// 配置
        /// </summary>
        protected SMTPClientSettings Settings { get; private set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="settings">Settings</param>
        public SMTPClient(SMTPClientSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        public SMTPClient(IConfigurationSection section) : this(Parse(section))
        {

        }

        /// <summary>
        /// Format message
        /// 格式化信息
        /// </summary>
        /// <param name="message">Message</param>
        protected virtual void FormatMessage(MimeMessage message)
        {
            if(message.Sender == null)
            {
                // Set the sender
                message.Sender = MailboxAddress.Parse(Settings.Sender ?? Settings.UserName);
            }

            if (message.From.Count == 0)
            {
                // Set one from
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
            await client.ConnectAsync(Settings.Host, Settings.Port, Settings.UseSsl, token);

            // Auth
            if (!string.IsNullOrEmpty(Settings.UserName))
            {
                await client.AuthenticateAsync(Settings.UserName, Settings.Password, token);
            }

            // Send
            await client.SendAsync(message, token);

            // Disconnect
            await client.DisconnectAsync(true, token);
        }
    }
}
