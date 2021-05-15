using MimeKit;
using System.Threading;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Net.SMTP
{
    /// <summary>
    /// SMTP client interface
    /// SMTP客户端接口
    /// </summary>
    public interface ISMTPClient
    {
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
