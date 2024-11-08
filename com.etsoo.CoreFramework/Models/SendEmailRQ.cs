using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.String;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Send email request data
    /// 发送邮件请求数据
    /// </summary>
    public class SendEmailRQ : IModelValidator
    {
        /// <summary>
        /// Recipient
        /// 收件人
        /// </summary>
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
        public required string Data { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (!new EmailAddressAttribute().IsValid(Recipient))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Recipient));
            }

            if (Template.Length is not (>= 16 and <= 128))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Template));
            }

            if (Token.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Token));
            }

            if (!Data.IsJson())
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Data));
            }

            return null;
        }
    }
}
