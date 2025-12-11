using com.etsoo.MessageQueue;
using com.etsoo.MessageQueue.LocalRabbitMQ;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace Tests.MessageQueue
{
    [TestClass]
    public class LocalRabbitMQTests
    {
        [TestMethod]
        public async Task ProducerSendAsyncTest()
        {
            var producer = new LocalRabbitMQProducer(new LocalRabbitMQProducerOptions { QueueName = "Etsoo-Test" });
            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true }, new MessageProperties { AppId = "SmartERPTest", UserId = "GUID" });
            await producer.DisposeAsync();
            Assert.IsNotNull(messageId);
        }

        [TestMethod]
        public async Task ProducerReceiveAsyncTest()
        {
            var producer = new LocalRabbitMQProducer(new LocalRabbitMQProducerOptions { QueueName = "Etsoo-Test" });
            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true }, new MessageProperties { AppId = "SmartERPTest", UserId = "GUID" });
            await producer.DisposeAsync();

            var messages = new List<MessageReceivedProperties>();
            var action = (MessageReceivedProperties properties, SimpleData? data) =>
            {
                messages.Add(properties);
            };

            var consumer = new LocalRabbitMQConsumer(
                new LocalRabbitMQConsumerOptions { QueueName = "Etsoo-Test" },
                new[] { new SimpleProcessor(action) },
                Mock.Of<ILogger>()
               );
            await consumer.StartAsync(default);
            await Task.Delay(1000);
            await consumer.StopAsync(default);

            Assert.IsTrue(messages.Any());
            Assert.IsTrue(messages.Any(m => "GUID".Equals(m.UserId)));
            Assert.IsTrue(messages.Any(m => messageId.Equals(m.MessageId)));
        }

        [TestMethod]
        public async Task ProducerReceiveAsyncStringTest()
        {
            var producer = new LocalRabbitMQProducer(new LocalRabbitMQProducerOptions { QueueName = "Etsoo-Hub-Test" });
            var messageId = await producer.SendAsync(Encoding.UTF8.GetBytes("Hello"), new MessageProperties { AppId = "SmartERPTest" });
            await producer.DisposeAsync();

            var source = new CancellationTokenSource(2000);

            var messages = new List<(MessageReceivedProperties, string?)>();
            var action = (MessageReceivedProperties properties, string? data) =>
            {
                messages.Add((properties, data));
            };

            var consumer = new LocalRabbitMQConsumer(
                new LocalRabbitMQConsumerOptions { QueueName = "Etsoo-Hub-Test" },
                new[] { new StringProcessor(action) },
                Mock.Of<ILogger>()
               );
            await consumer.StartAsync(source.Token);
            await Task.Delay(1000);
            await consumer.StopAsync(source.Token);

            Assert.IsTrue(messages.Any());
            Assert.IsTrue(messages.Any(m => "Hello".Equals(m.Item2)));
            Assert.IsTrue(messages.Any(m => messageId.Equals(m.Item1.MessageId)));
        }
    }
}
