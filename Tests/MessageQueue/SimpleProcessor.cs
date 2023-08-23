using com.etsoo.MessageQueue;
using com.etsoo.MessageQueue.QueueProcessors;
using System.Text;

namespace Tests.MessageQueue
{
    internal class StringProcessor : IMessageQueueProcessor
    {
        private readonly Action<MessageReceivedProperties, string?> _messageAction;

        public StringProcessor(Action<MessageReceivedProperties, string?> messageAction)
        {
            _messageAction = messageAction;
        }

        public bool CanDeserialize(MessageReceivedProperties properties)
        {
            return true;
        }

        public async Task ExecuteAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var data = Encoding.UTF8.GetString(body.ToArray());
            _messageAction(properties, data);
        }
    }

    internal class SimpleProcessor : IMessageQueueProcessor
    {
        private readonly Action<MessageReceivedProperties, SimpleData?> _messageAction;

        public SimpleProcessor(Action<MessageReceivedProperties, SimpleData?> messageAction)
        {
            _messageAction = messageAction;
        }

        public bool CanDeserialize(MessageReceivedProperties properties)
        {
            return properties.AppId?.Equals("SmartERPTest") is true;
        }

        public async Task ExecuteAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken)
        {
            var data = await MessageQueueUtils.FromJsonBytesAsync<SimpleData>(body, cancellationToken);
            _messageAction(properties, data);
        }
    }
}
