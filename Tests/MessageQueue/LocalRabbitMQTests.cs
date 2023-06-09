using com.etsoo.MessageQueue;
using com.etsoo.MessageQueue.LocalRabbitMQ;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.MessageQueue
{
    [TestFixture]
    internal class LocalRabbitMQTests
    {
        [Test]
        public async Task ProducerSendAsyncTest()
        {
            var producer = new LocalRabbitMQProducer(new LocalRabbitMQProducerOptions { QueueName = "SmartERP" });
            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true });
            await producer.DisposeAsync();
            Assert.IsNotNull(messageId);
        }

        [Test]
        public async Task ProducerReceiveAsyncTest()
        {
            var producer = new LocalRabbitMQProducer(new LocalRabbitMQProducerOptions { QueueName = "SmartERP" });
            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true });
            await producer.DisposeAsync();

            var source = new CancellationTokenSource(1000);

            var messages = new List<MessageReceivedProperties>();
            var action = (MessageReceivedProperties properties, SimpleData? data) =>
            {
                messages.Add(properties);
            };

            var consumer = new LocalRabbitMQConsumer(
                new LocalRabbitMQConsumerOptions { QueueName = "SmartERP" },
                new[] { new SimpleProcessor(action) },
                Mock.Of<ILogger>()
               );
            await consumer.ReceiveAsync(source.Token);

            Assert.IsTrue(messages.Any());
            Assert.IsTrue(messages.Any(m => messageId.Equals(m.MessageId)));
        }
    }
}
