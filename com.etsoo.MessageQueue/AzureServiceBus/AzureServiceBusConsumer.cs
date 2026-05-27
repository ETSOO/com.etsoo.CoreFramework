using Azure.Messaging.ServiceBus;
using com.etsoo.MessageQueue.QueueProcessors;
using com.etsoo.Utils.String;
using Microsoft.Extensions.Logging;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus message consumer
    /// Azure 服务总线消息消费者
    /// </summary>
    public class AzureServiceBusConsumer((ServiceBusProcessor Processor, ServiceBusSender Sender) sb, IEnumerable<IMessageQueueProcessor> processors, ILogger logger) : MessageQueueConsumer(processors, logger)
    {
        const string RetryCountField = "RetryCount";

        private readonly ServiceBusProcessor _processor = sb.Processor;
        private readonly ServiceBusSender _sender = sb.Sender;

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            //using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(args.CancellationToken);

            var message = args.Message;
            var properties = new MessageReceivedProperties();
            try
            {
                properties = AzureServiceBusUtils.CreatePropertiesFromMessage(message);
                var count = await ProcessAsync(message.Body.ToMemory(), properties, args.CancellationToken);

                if (count == 0)
                {
                    if (Logger.IsEnabled(LogLevel.Error))
                    {
                        Logger.LogError("No Processor for Message: {message}, Properties: {properties}", message.Body.ToString(), properties);
                    }

                    await args.DeadLetterMessageAsync(args.Message, "NoProcessor", cancellationToken: args.CancellationToken);

                    return;
                }
                else if (count > 1)
                {
                    if (Logger.IsEnabled(LogLevel.Information))
                    {
                        Logger.LogInformation("More Than One Processor for Message: {message}, Properties: {properties}", message.Body.ToString(), properties);
                    }
                }

                if (!_processor.AutoCompleteMessages)
                {
                    // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
                    // Set AutoCompleteMessages to false to [settle messages](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
                    // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
                    await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                }

                if (Logger.IsEnabled(LogLevel.Information))
                {
                    // Log success
                    Logger.LogInformation("Message {id} Processed {count}", properties.MessageId, count);
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsEnabled(LogLevel.Error))
                {
                    Logger.LogError(ex, "Message: {message}, Properties: {properties}", message.Body.ToString(), properties);
                }

                // await args.AbandonMessageAsync(args.Message, cancellationToken: args.CancellationToken);
                // await args.RenewMessageLockAsync(args.Message, args.CancellationToken);

                // Retry later
                message.ApplicationProperties.TryGetValue(RetryCountField, out var retryCountObj);
                var retryCount = StringUtils.TryParseObject<int>(retryCountObj).GetValueOrDefault() + 1;

                var retryMessage = new ServiceBusMessage(message);
                retryMessage.ApplicationProperties[RetryCountField] = retryCount;

                // Send the retry message with a delay
                var delay = DateTimeOffset.UtcNow.AddSeconds(GetRetryDelay(retryCount));
                await _sender.ScheduleMessageAsync(retryMessage, delay, args.CancellationToken);

                // Complete the message to remove it from the queue, since we have scheduled a retry message
                await args.CompleteMessageAsync(args.Message, args.CancellationToken);

                if (_processor.AutoCompleteMessages) throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            if (Logger.IsEnabled(LogLevel.Error))
            {
                Logger.LogError("ErrorHandler: {args}", args);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service
        /// 当程序准备好启动服务时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown
        /// 当程序执行正常关闭时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(CancellationToken.None);

            _processor.ProcessMessageAsync -= MessageHandler;
            _processor.ProcessErrorAsync -= ErrorHandler;
        }

        /// <summary>
        /// Dispose of resources
        /// 处置资源
        /// </summary>
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
