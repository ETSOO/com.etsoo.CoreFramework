using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub message consumer options
    /// Google PubSub 消息消费者选项
    /// </summary>
    public record GooglePubSubConsumerOptions : GooglePubSubOptions
    {
        /// <summary>
        /// Project id
        /// 项目编号
        /// </summary>
        [Required]
        public string ProjectId { get; set; } = default!;

        /// <summary>
        /// Subscription id
        /// 订阅编号
        /// </summary>
        [Required]
        public string SubscriptionId { get; set; } = default!;
    }

    [OptionsValidator]
    public partial class ValidateGooglePubSubConsumerOptions : IValidateOptions<GooglePubSubConsumerOptions>
    {
    }
}
