using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub message producer options
    /// Google PubSub 消息生产者选项
    /// </summary>
    public record GooglePubSubProducerOptions : GooglePubSubOptions
    {
        /// <summary>
        /// Project id
        /// 项目编号
        /// </summary>
        [Required]
        public string ProjectId { get; init; } = default!;

        /// <summary>
        /// Topic id
        /// 主题编号
        /// </summary>
        [Required]
        public string TopicId { get; init; } = default!;
    }

    [OptionsValidator]
    public partial class ValidateGooglePubSubProducerOptions : IValidateOptions<GooglePubSubProducerOptions>
    {
    }
}
