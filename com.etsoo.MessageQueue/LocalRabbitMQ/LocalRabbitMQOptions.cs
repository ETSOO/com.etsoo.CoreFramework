using RabbitMQ.Client;

namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    /// <summary>
    /// Local RabbitMQ connection options
    /// 本地 RabbitMQ 链接选项
    /// </summary>
    public record LocalRabbitMQOptions
    {
        /// <summary>
        /// Default client provided name to be used for connections.
        /// </summary>
        public string ClientProvidedName { get; init; } = string.Empty;

        /// <summary>
        /// Username to use when authenticating to the server.
        /// </summary>
        public string? UserName { get; init; }

        /// <summary>
        /// Password to use when authenticating to the server.
        /// </summary>
        public string? Password { get; init; }

        /// <summary>
        /// The host to connect to.
        /// </summary>
        public string? HostName { get; init; }

        /// <summary>
        /// Virtual host to access during this connection.
        /// </summary>
        public string? VirtualHost { get; init; }

        /// <summary>
        /// The port to connect on. <see cref="AmqpTcpEndpoint.UseDefaultPort"/>
        ///  indicates the default for the protocol should be used.
        /// </summary>
        public int? Port { get; init; }

        /// <summary>
        /// TLS options setting.
        /// </summary>
        public SslOption? Ssl { get; init; }

        /// <summary>
        /// Set to a value greater than one to enable concurrent processing. For a concurrency greater than one <see cref="IBasicConsumer"/>
        /// will be offloaded to the worker thread pool so it is important to choose the value for the concurrency wisely to avoid thread pool overloading.
        /// <see cref="IAsyncBasicConsumer"/> can handle concurrency much more efficiently due to the non-blocking nature of the consumer.
        /// Defaults to 1.
        /// </summary>
        /// <remarks>For concurrency greater than one this removes the guarantee that consumers handle messages in the order they receive them.
        /// In addition to that consumers need to be thread/concurrency safe.</remarks>
        public int? ConsumerDispatchConcurrency { get; init; }
    }
}
