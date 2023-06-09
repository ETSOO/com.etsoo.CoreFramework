using com.etsoo.MessageQueue;

namespace Tests.MessageQueue
{
    internal class SimpleProcessor : IMessageQueueProcessor
    {
        private readonly Action<MessageReceivedProperties, SimpleData?> _messageAction;

        public SimpleProcessor(Action<MessageReceivedProperties, SimpleData?> messageAction)
        {
            _messageAction = messageAction;
        }

        public bool CanDeserialize(MessageReceivedProperties properties)
        {
            return true;
        }

        public async Task ExecuteAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken)
        {
            var data = await MessageQueueUtils.FromJsonBytesAsync<SimpleData>(body, cancellationToken);
            _messageAction(properties, data);
        }
    }
}
