using com.etsoo.Utils.String;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace com.etsoo.CoreFramework.MessageQueue
{
    /// <summary>
    /// RabbitMQ implementation
    /// https://www.rabbitmq.com/
    /// </summary>
    public class RabbitMQEx : IMessageQueue
    {
        /// <summary>
        /// Create connection factory
        /// 创建连接工厂
        /// </summary>
        /// <param name="section">Configuration section</param>
        /// <returns>Connection factory</returns>
        public static ConnectionFactory CreateFactory(IConfigurationSection section)
        {
            var factory = new ConnectionFactory
            {
                HostName = section.GetValue<string>("HostName"),
                UserName = section.GetValue<string>("UserName"),
                Password = section.GetValue<string>("Password"),
                ClientProvidedName = section.GetValue<string>("ClientProvidedName"),
                AutomaticRecoveryEnabled = section.GetValue("AutomaticRecoveryEnabled", true),
                DispatchConsumersAsync = section.GetValue("DispatchConsumersAsync", false),
                UseBackgroundThreadsForIO = section.GetValue("UseBackgroundThreadsForIO", false),
                ConsumerDispatchConcurrency = section.GetValue("ConsumerDispatchConcurrency", Environment.ProcessorCount - 1)
            };

            // VirtualHost
            var virtualHost = section.GetValue<string>("VirtualHost");
            if (!string.IsNullOrEmpty(virtualHost))
                factory.VirtualHost = virtualHost;

            // Port
            var port = section.GetValue<int?>("Port");
            if (port.HasValue)
                factory.Port = port.Value;

            // SSL
            var ssl = section.GetSection("Ssl");
            if (ssl.Exists())
            {
                factory.Ssl = ssl.Get<SslOption>();
            }

            return factory;
        }

        /// <summary>
        /// Connection
        /// 链接对象
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Channel
        /// 频道对象
        /// </summary>
        public IModel Channel { get; }

        // Queues
        readonly List<string> queues = new ();

        // Exchanges
        readonly List<string> exchanges = new ();

        // RPC client properties
        Tuple<string, string>? RPCClientProperties;

        // PRC client result queue
        readonly ConcurrentDictionary<int, BufferBlock<ReadOnlyMemory<byte>>> RPCClientQueue = new ();

        // New connection created
        readonly bool newConnection;

        /// <summary>
        /// Constructor with current connection
        /// 使用当前连接的构造函数
        /// </summary>
        /// <param name="connection">Connection</param>
        public RabbitMQEx(IConnection connection)
        {
            Connection = connection;
            Channel = Connection.CreateModel();
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="factory">Factory</param>
        /// <param name="clientName">Client name</param>
        public RabbitMQEx(ConnectionFactory factory, string? clientName = null) : this(factory.CreateConnection(clientName))
        {
            newConnection = true;
        }

        /// <summary>
        /// Constructor with configuration
        /// 使用配置的构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        public RabbitMQEx(IConfigurationSection section) : this(CreateFactory(section), section.GetValue<string>("ClientName"))
        {

        }

        /// <summary>
        /// Prepare for producing
        /// 准备生产
        /// </summary>
        /// <param name="exchange">Exchange</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="durable">Durable</param>
        /// <param name="exclusive">Exclusive</param>
        /// <param name="autoDelete">Auto delete</param>
        /// <param name="autoAck">Auto ackownledge</param>
        /// <param name="arguments">Arguments</param>
        /// <returns>Is exchange model</returns>
        public bool PrepareProduce(string exchange, string? routingKey = null, bool durable = false, bool exclusive = false, bool autoDelete = false, IDictionary<string, object>? arguments = null)
        {
            if (exchange == string.Empty)
            {
                // Queue mode
                routingKey ??= "default_queue";
                if (!queues.Contains(routingKey))
                {
                    Channel.QueueDeclare(queue: routingKey,
                                         durable: durable,
                                         exclusive: exclusive,
                                         autoDelete: autoDelete,
                                         arguments: arguments);

                    queues.Add(routingKey);
                }

                return false;
            }
            else
            {
                // Exchange mode
                if (!exchanges.Contains(exchange))
                {
                    Channel.ExchangeDeclare(exchange: exchange,
                                            type: "topic",
                                            durable: durable,
                                            autoDelete: autoDelete,
                                            arguments: arguments);

                    exchanges.Add(exchange);
                }

                return true;
            }
        }

        private IBasicProperties? createProperties(IDictionary<string, object>? arguments)
        {
            if (arguments == null)
                return null;

            var p = Channel.CreateBasicProperties();

            foreach (var item in arguments)
            {
                if (item.Value == null)
                    continue;

                switch (item.Key)
                {
                    case "AppId":
                        p.AppId = item.Value.ToString();
                        break;
                    case "ContentEncoding":
                        p.ContentEncoding = item.Value.ToString();
                        break;
                    case "ContentType":
                        p.ContentType = item.Value.ToString();
                        break;
                    case "CorrelationId":
                        p.CorrelationId = item.Value.ToString();
                        break;
                    case "Expiration":
                        p.Expiration = item.Value.ToString();
                        break;
                    case "MessageId":
                        p.MessageId = item.Value.ToString();
                        break;
                    case "Persistent":
                        p.Persistent = StringUtil.TryParseObject<bool>(item.Value).GetValueOrDefault();
                        break;
                    case "Priority":
                        p.Priority = StringUtil.TryParseObject<byte>(item.Value).GetValueOrDefault();
                        break;
                }
            }

            return p;
        }

        /// <summary>
        /// Produce
        /// 生产
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="exchange">Exchange</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="arguments">Arguments</param>
        public void Produce(ReadOnlyMemory<byte> body, string exchange, string routingKey, IDictionary<string, object>? arguments = null)
        {
            // Properties
            var properties = createProperties(arguments);

            // Publish the message
            Channel.BasicPublish(exchange,
                                 routingKey,
                                 mandatory: true,  // The message must be routable
                                 basicProperties: properties,
                                 body);
        }

        /// <summary>
        /// Produce
        /// 生产
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="exchange">Exchange</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="persistent">Persistent</param>
        public void Produce(ReadOnlyMemory<byte> body, string exchange, string routingKey, bool persistent)
        {
            // Properties
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = persistent;

            // Publish the message
            Channel.BasicPublish(exchange,
                                 routingKey,
                                 mandatory: true,  // The message must be routable
                                 basicProperties: properties,
                                 body);
        }

        /// <summary>
        /// Remote procedure call
        /// 远程过程调用
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="queue">Queue name</param>
        public ReadOnlyMemory<byte> PRCCall(ReadOnlyMemory<byte> body, string queue)
        {
            if (RPCClientProperties == null)
                throw new ArgumentNullException(nameof(RPCClientProperties));

            // Thread id based collection
            var id = Thread.CurrentThread.ManagedThreadId;
            var collection = RPCClientQueue.GetOrAdd(id, new BufferBlock<ReadOnlyMemory<byte>>());

            var properties = Channel.CreateBasicProperties();
            properties.CorrelationId = RPCClientProperties.Item1;
            properties.ReplyTo = RPCClientProperties.Item2;
            properties.AppId = id.ToString();

            // Publish the message
            Channel.BasicPublish(exchange: string.Empty,
                                 routingKey: queue,
                                 basicProperties: properties,
                                 body);

            return collection.Receive();
        }

        /// <summary>
        /// Async remote procedure call
        /// 异步远程过程调用
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="queue">Queue name</param>
        public async Task<ReadOnlyMemory<byte>> PRCCallAsync(ReadOnlyMemory<byte> body, string queue)
        {
            if (RPCClientProperties == null)
                throw new ArgumentNullException(nameof(RPCClientProperties));

            // Thread id based collection
            var id = Thread.CurrentThread.ManagedThreadId;
            var collection = RPCClientQueue.GetOrAdd(id, new BufferBlock<ReadOnlyMemory<byte>>());

            var properties = Channel.CreateBasicProperties();
            properties.CorrelationId = RPCClientProperties.Item1;
            properties.ReplyTo = RPCClientProperties.Item2;
            properties.AppId = id.ToString();

            // Publish the message
            Channel.BasicPublish(exchange: string.Empty,
                                 routingKey: queue,
                                 basicProperties: properties,
                                 body);

            return await collection.ReceiveAsync();
        }

        /// <summary>
        /// Produce confirm
        /// 生产确认
        /// </summary>
        /// <param name="callback">Callback</param>
        public void ProduceConfirm(AckownledgeDelegate callback)
        {
            // Enable publisher ackownledgements
            Channel.ConfirmSelect();

            // Successful
            Channel.BasicAcks += async (sender, ea) =>
            {
                await callback(ea.DeliveryTag, ea.Multiple, true);
            };

            // Failed
            Channel.BasicNacks += async (sender, ea) =>
            {
                await callback(ea.DeliveryTag, ea.Multiple, false);
            };
        }

        /// <summary>
        /// Prepare for consuming
        /// 准备消费
        /// </summary>
        /// <param name="callback">Callback</param>
        /// <param name="exchange">Exchange</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="durable">Durable</param>
        /// <param name="exclusive">Exclusive</param>
        /// <param name="autoDelete">Auto delete</param>
        /// <param name="autoAck">Auto ackownledge</param>
        /// <param name="arguments">Arguments</param>
        public void PrepareConsume(ConsumeDelegate callback, string exchange, string routingKey, bool durable = false, bool exclusive = false, bool autoDelete = false, bool autoAck = true, IDictionary<string, object>? arguments = null)
        {
            var isExchange = PrepareProduce(exchange: exchange,
                                            routingKey: routingKey,
                                            durable: durable,
                                            exclusive: exclusive,
                                            autoDelete: autoDelete,
                                            arguments: arguments);


            // Qos = Quality of service
            Channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            // Consumer callback
            var consumer = new AsyncEventingBasicConsumer(Channel);

            consumer.Received += async (sender, ea) => {
                // Data
                if (await callback(ea.Body, ea.RoutingKey, ea.Redelivered))
                {
                    // Manually acknowledge when not auto acknowledgement
                    if (!autoAck)
                        Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            if (isExchange)
            {
                // Unique queue name
                // Multiple queues could be attached to the exchange
                var queueName = Channel.QueueDeclare().QueueName;
                Channel.QueueBind(queue: queueName,
                                  exchange: exchange,
                                  routingKey: routingKey,
                                  arguments: arguments);

                Channel.BasicConsume(queue: queueName,
                                     autoAck: autoAck,
                                     consumer: consumer);
            }
            else
            {
                Channel.BasicConsume(queue: routingKey,
                                     autoAck: autoAck,
                                     consumer: consumer);
            }
        }

        /// <summary>
        /// Prepare for PRC client
        /// 准备远程调用客户端
        /// </summary>
        public void PreparePRCClient()
        {
            // Client unique id
            var queueName = Channel.QueueDeclare().QueueName;

            // Identity and ReplyTo
            var correlationId = Guid.NewGuid().ToString();
            RPCClientProperties = new Tuple<string, string>(correlationId, queueName);

            // Consumer to get result
            var consumer = new AsyncEventingBasicConsumer(Channel);
            consumer.Received += async (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    // Thread id based collection through AppId
                    var id = StringUtil.TryParse<int>(ea.BasicProperties.AppId).GetValueOrDefault(0);
                    var collection = RPCClientQueue.GetOrAdd(id, new BufferBlock<ReadOnlyMemory<byte>>());
                    await collection.SendAsync(ea.Body);
                }
            };

            // Start the consumer
            Channel.BasicConsume(consumer: consumer,
                                 queue: queueName,
                                 autoAck: true);

        }

        /// <summary>
        /// Prepare for PRC server
        /// 准备远程调用服务器
        /// </summary>
        /// <param name="callback">Callback</param>
        /// <param name="queue">Queue name</param>
        public void PreparePRCServer(ConsumeDataDelegate callback, string queue)
        {
            // Prepare producing
            PrepareProduce(exchange: string.Empty,
                           routingKey: queue,
                           durable: false,
                           exclusive: false,
                           autoDelete: false,
                           arguments: null);

            // Qos = Quality of service
            Channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            // Consumer callback
            var consumer = new AsyncEventingBasicConsumer(Channel);

            consumer.Received += async (sender, ea) => {
                // Refer the CorrelationId back
                var props = ea.BasicProperties;

                // Validate
                if (string.IsNullOrEmpty(props.ReplyTo) || string.IsNullOrEmpty(props.CorrelationId))
                    return;

                var replyProps = Channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                replyProps.AppId = props.AppId;

                // Result
                var result = await callback(ea.Body, ea.RoutingKey, ea.Redelivered);

                // Push back
                Channel.BasicPublish(exchange: string.Empty, routingKey: props.ReplyTo, basicProperties: replyProps, body: result);

                // Acknowledge
                Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            Channel.BasicConsume(queue: queue,
                                 autoAck: false,
                                 consumer: consumer);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // Reference or CreateConnection may fail, check existence
            if (Connection != null && newConnection)
                Connection.Dispose();

            // Connection.CreateModel may fail, check existence
            if (Channel != null)
                Channel.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
