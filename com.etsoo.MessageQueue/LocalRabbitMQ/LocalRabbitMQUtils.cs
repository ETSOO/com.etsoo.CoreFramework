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
        public const string LoginUserIdField = "LoginUserId";

        private static readonly ConcurrentDictionary<string, IConnection> connections = new();

        /// <summary>
        /// Create connection
        /// 创建链接
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Connection</returns>
        public static async Task<IConnection> CreateConnectionAsync(LocalRabbitMQOptions options, CancellationToken cancellationToken = default)
        {
            var clientProvidedName = options.ClientProvidedName;

            if (connections.TryGetValue(clientProvidedName, out var connection) && connection.IsOpen)
            {
                return connection;
            }

            var factory = new ConnectionFactory
            {
                ClientProvidedName = clientProvidedName
            };

            if (options.UserName != null) factory.UserName = options.UserName;
            if (options.Password != null) factory.Password = options.Password;
            if (options.VirtualHost != null) factory.VirtualHost = options.VirtualHost;
            if (options.HostName != null) factory.HostName = options.HostName;
            if (options.Port.HasValue) factory.Port = options.Port.Value;
            if (options.Ssl != null) factory.Ssl = options.Ssl;

            // Connection is thread safe
            connection = await factory.CreateConnectionAsync(cancellationToken);

            // Add or update
            return connections.AddOrUpdate(clientProvidedName, connection, (key, oldConnection) => connection);
        }

        private static Dictionary<string, object> FilterHeader(IDictionary<string, object?>? headers)
        {
            return headers?.Where(item => item.Value != null)
                               .ToDictionary(item => item.Key, item => item.Value!)
                       ?? [];
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
                MessageId = bp.MessageId ?? string.Empty,
                CorrelationId = bp.CorrelationId,
                AppId = bp.AppId,
                ContentEncoding = bp.ContentEncoding,
                ContentType = bp.ContentType,
                Priority = bp.Priority,
                ReplyTo = bp.ReplyTo,
                Type = bp.Type,
                Timestamp = bp.Timestamp.UnixTime,
                Headers = FilterHeader(bp.Headers)
            };

            var userId = p.Headers!.GetHeaderValue(nameof(p.UserId));
            if (userId != null) p.UserId = userId;

            if (bp.UserId != null)
                p.Headers[LoginUserIdField] = bp.UserId;

            p.Headers[nameof(bp.Persistent)] = bp.Persistent;

            if (bp.CorrelationId != null)
                p.Headers[nameof(bp.CorrelationId)] = bp.CorrelationId;

            if (bp.Expiration != null)
                p.Headers[nameof(bp.Expiration)] = bp.Expiration;

            return p;
        }
    }
}
