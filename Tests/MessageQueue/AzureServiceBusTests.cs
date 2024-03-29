﻿using com.etsoo.MessageQueue;
using com.etsoo.MessageQueue.AzureServiceBus;
using com.etsoo.MessageQueue.GooglePubSub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.MessageQueue
{
    [TestFixture]
    internal class AzureServiceBusTests
    {
        private IConfigurationRoot _configuration;

        public AzureServiceBusTests()
        {
            _configuration = new ConfigurationBuilder()
               .AddUserSecrets<AzureServiceBusTests>()
               .Build();
        }

        [Test]
        public async Task ProducerSendAsyncTest()
        {
            var client = AzureServiceBusUtils.CreateServiceBusSender(new AzureServiceBusProducerOptions
            {
                ConnectionString = _configuration["AzureServiceBusConnectionString"] ?? string.Empty,
                QueueOrTopicName = "smarterpqueue"
            });
            var producer = new AzureServiceBusProducer(client);

            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true }, new MessageProperties { AppId = "SmartERPTest" });

            await producer.DisposeAsync();
            await client.DisposeAsync();

            Assert.That(messageId, Is.Not.Null);
        }

        [Test]
        public async Task ProducerReceiveAsyncTest()
        {
            var sender = AzureServiceBusUtils.CreateServiceBusSender(new AzureServiceBusProducerOptions
            {
                ConnectionString = _configuration["AzureServiceBusConnectionString"] ?? string.Empty,
                QueueOrTopicName = "smarterpqueue"
            });
            var producer = new AzureServiceBusProducer(sender);

            var messageId = await producer.SendJsonAsync(new SimpleData { Num = 1, Bool = true }, new MessageProperties { AppId = "SmartERPTest" });

            await producer.DisposeAsync();
            await sender.DisposeAsync();

            var messages = new List<MessageReceivedProperties>();
            var action = (MessageReceivedProperties properties, SimpleData? data) =>
            {
                messages.Add(properties);
            };

            var subscriber = AzureServiceBusUtils.CreateServiceBusProcessor(new AzureServiceBusConsumerOptions
            {
                ConnectionString = _configuration["AzureServiceBusConnectionString"] ?? string.Empty,
                QueueName = "smarterpqueue"
            });
            var consumer = new AzureServiceBusConsumer(
                subscriber,
                new[] { new SimpleProcessor(action) },
                Mock.Of<ILogger>()
               );
            await consumer.StartAsync(default);
            await Task.Delay(1000);
            await consumer.StopAsync(default);

            await subscriber.DisposeAsync();

            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.Any(m => messageId.Equals(m.MessageId)), Is.True);
        }
    }
}
