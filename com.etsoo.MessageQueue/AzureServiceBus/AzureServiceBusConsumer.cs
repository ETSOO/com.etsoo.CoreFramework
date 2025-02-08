using Azure.Messaging.ServiceBus;
using com.etsoo.MessageQueue.QueueProcessors;
using Microsoft.Extensions.Logging;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus message consumer
    /// Azure 服务总线消息消费者
    /// </summary>
    public class AzureServiceBusConsumer(ServiceBusProcessor processor, IEnumerable<IMessageQueueProcessor> processors, ILogger logger) : MessageQueueConsumer(processors, logger)
    {
        private readonly ServiceBusProcessor _processor = processor;

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
                    Logger.LogError("No Processor for Message: {message}, Properties: {properties}", message.Body.ToString(), properties);
                    await args.DeadLetterMessageAsync(args.Message, "NoProcessor", cancellationToken: args.CancellationToken);
                    return;
                }
                else if (count > 1)
                {
                    Logger.LogWarning("More Than One Processor for Message: {message}, Properties: {properties}", message.Body.ToString(), properties);
                }

                if (!_processor.AutoCompleteMessages)
                {
                    // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
                    // Set AutoCompleteMessages to false to [settle messages](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
                    // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
                    await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                }

                // Log success
                Logger.LogInformation("Message {id} Processed {count}", properties.MessageId, count);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Message: {message}, Properties: {properties}", message.Body.ToString(), properties);

                await args.RenewMessageLockAsync(args.Message, args.CancellationToken);

                if (_processor.AutoCompleteMessages) throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Logger.LogError("ErrorHandler: {args}", args);
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
