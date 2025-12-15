using RabbitMQ.Client;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    public record LocalRabbitMQSslOptions
    {
        public string? CertPassphrase { get; init; }
        public string CertPath { get; init; } = default!;
        public bool CheckCertificateRevocation { get; init; }
        public bool Enabled { get; init; }
        public string ServerName { get; init; } = default!;
        public SslProtocols Version { get; init; }
    }

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
        public LocalRabbitMQSslOptions? Ssl { get; init; }
    }
}
