using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;

namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    /// <summary>
    /// Local RabbitMQ utilities
    /// 本地 RabbitMQ 工具
    /// </summary>
    public static class LocalRabbitMQUtils
    {
        private static readonly ConcurrentDictionary<string, IConnection> connections = new();

        /// <summary>
        /// Create connection
        /// 创建链接
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Connection</returns>
        public static IConnection CreateConnection(LocalRabbitMQOptions options)
        {
            var clientProvidedName = options.ClientProvidedName;

            if (connections.TryGetValue(clientProvidedName, out var connection) && connection.IsOpen)
            {
                return connection;
            }

            var factory = new ConnectionFactory
            {
                ClientProvidedName = clientProvidedName,
                DispatchConsumersAsync = true
            };

            if (options.UserName != null) factory.UserName = options.UserName;
            if (options.Password != null) factory.Password = options.Password;
            if (options.VirtualHost != null) factory.VirtualHost = options.VirtualHost;
            if (options.HostName != null) factory.HostName = options.HostName;
            if (options.Port.HasValue) factory.Port = options.Port.Value;
            if (options.Ssl != null) factory.Ssl = options.Ssl;
            if (options.ConsumerDispatchConcurrency.HasValue) factory.ConsumerDispatchConcurrency = options.ConsumerDispatchConcurrency.Value;

            // Connection is thread safe
            connection = factory.CreateConnection();

            // Add or update
            return connections.AddOrUpdate(clientProvidedName, connection, (key, oldConnection) => connection);
        }

        /// <summary>
        /// Create received message properties
        /// 创建收到的消息属性
        /// </summary>
        /// <param name="e">Event handler arguments</param>
        /// <returns>Result</returns>
        public static MessageReceivedProperties CreatePropertiesFromArgs(BasicDeliverEventArgs e)
        {
            var bp = e.BasicProperties;

            var p = new MessageReceivedProperties
            {
                MessageId = bp.MessageId,
                CorrelationId = bp.CorrelationId,
                AppId = bp.AppId,
                ContentEncoding = bp.ContentEncoding,
                ContentType = bp.ContentType,
                Priority = bp.Priority,
                ReplyTo = bp.ReplyTo,
                Type = bp.Type,
                UserId = bp.UserId,
                Timestamp = bp.Timestamp.UnixTime,
                Headers = bp.Headers ?? new Dictionary<string, object>()
            };

            p.Headers[nameof(bp.Persistent)] = bp.Persistent;
            p.Headers[nameof(bp.CorrelationId)] = bp.CorrelationId;
            p.Headers[nameof(bp.Expiration)] = bp.Expiration;
            p.Headers[nameof(bp.ProtocolClassId)] = bp.ProtocolClassId;
            p.Headers[nameof(bp.ProtocolClassName)] = bp.ProtocolClassName;

            return p;
        }
    }
}
