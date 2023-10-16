using com.etsoo.MessageQueue;
using com.etsoo.MessageQueue.GooglePubSub;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.MessageQueue
{
    [TestFixture]
    internal class GooglePubSubTests
    {
        [Test]
        public async Task ProducerSendAsyncTest()
        {
            var client = await GooglePubSubUtils.CreatePublisherClientAsync(new GooglePubSubProducerOptions
            {
                ProjectId = "pelagic-pod-350823",
                TopicId = "SmartERP",
                CredentialsPath = "C:\\api\\pelagic-pod-350823-9274363e3d6b.json"
            });
            var producer = new GooglePubSubProducer(client);

            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true }, new MessageProperties { AppId = "SmartERPTest" });

            await producer.DisposeAsync();
            await client.DisposeAsync();

            Assert.IsNotNull(messageId);
        }

        [Test]
        public async Task ProducerReceiveAsyncTest()
        {
            var client = await GooglePubSubUtils.CreatePublisherClientAsync(new GooglePubSubProducerOptions
            {
                ProjectId = "pelagic-pod-350823",
                TopicId = "SmartERP",
                CredentialsPath = "C:\\api\\pelagic-pod-350823-9274363e3d6b.json"
            });
            var producer = new GooglePubSubProducer(client);

            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true }, new MessageProperties { AppId = "SmartERPTest" });

            await producer.DisposeAsync();

            var messages = new List<MessageReceivedProperties>();
            var action = (MessageReceivedProperties properties, SimpleData? data) =>
            {
                messages.Add(properties);
            };

            var source = new CancellationTokenSource();

            var subscriber = await GooglePubSubUtils.CreateSubscriberClientAsync(new GooglePubSubConsumerOptions
            {
                ProjectId = "pelagic-pod-350823",
                SubscriptionId = "SmartERP-sub",
                CredentialsPath = "C:\\api\\pelagic-pod-350823-9274363e3d6b.json"
            });
            var consumer = new GooglePubSubConsumer(
                subscriber,
                new[] { new SimpleProcessor(action) },
                Mock.Of<ILogger>()
               );

            // Fire and go, will keep running
            consumer.StartAsync(source.Token);

            await Task.Delay(2000);
            await consumer.StopAsync(source.Token);

            await subscriber.DisposeAsync();
            await client.DisposeAsync();

            Assert.IsTrue(messages.Any());
            Assert.IsTrue(messages.Any(m => messageId.Equals(m.MessageId)));
        }
    }
}
