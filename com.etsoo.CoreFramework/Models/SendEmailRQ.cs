using com.etsoo.WebUtils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Send email request data
    /// 发送邮件请求数据
    /// </summary>
    public class SendEmailRQ
    {
        /// <summary>
        /// Recipient
        /// 收件人
        /// </summary>
        [EmailAddress]
        public required string Recipient { get; init; }

        /// <summary>
        /// Template name
        /// 模板名称
        /// </summary>
        public required string Template { get; init; }

        /// <summary>
        /// Token
        /// 令牌
        /// </summary>
        public required string Token { get; init; }

        /// <summary>
        /// Data
        /// 数据
        /// </summary>
        [IsJson]
        public required string Data { get; init; }
    }
}
