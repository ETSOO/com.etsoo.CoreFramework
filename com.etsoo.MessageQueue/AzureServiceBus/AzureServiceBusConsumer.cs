using Azure.Messaging.ServiceBus;
using com.etsoo.MessageQueue.QueueProcessors;
using Microsoft.Extensions.Logging;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus message consumer
    /// Azure 服务总线消息消费者
    /// </summary>
    public class AzureServiceBusConsumer : MessageQueueConsumer
    {
        private readonly ServiceBusProcessor _processor;

        public AzureServiceBusConsumer(ServiceBusProcessor processor, IEnumerable<IMessageQueueProcessor> processors, ILogger logger)
            : base(processors, logger)
        {
            _processor = processor;
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var properties = new MessageReceivedProperties();
            try
            {
                properties = AzureServiceBusUtils.CreatePropertiesFromMessage(message);
                await ProcessAsync(message.Body.ToMemory(), properties, args.CancellationToken);

                if (!_processor.AutoCompleteMessages)
                {
                    // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
                    // Set AutoCompleteMessages to false to [settle messages](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
                    // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
                    await args.CompleteMessageAsync(args.Message);
                }
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

        public override async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            if (_processor.IsProcessing) return;

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);

            // Keep running
            while (!cancellationToken.IsCancellationRequested)
            {
            }

            await _processor.StopProcessingAsync(CancellationToken.None);

            _processor.ProcessMessageAsync -= MessageHandler;
            _processor.ProcessErrorAsync -= ErrorHandler;
        }
    }
}
