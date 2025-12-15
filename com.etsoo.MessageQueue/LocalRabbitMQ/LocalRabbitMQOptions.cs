using RabbitMQ.Client;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    public record LocalRabbitMQSslOptions
    {
        public string? CertPassphrase { get; set; }
        public string CertPath { get; set; } = default!;
        public bool CheckCertificateRevocation { get; set; }
        public bool Enabled { get; set; }
        public string ServerName { get; set; } = default!;
        public SslProtocols Version { get; set; }
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
        public string ClientProvidedName { get; set; } = string.Empty;

        /// <summary>
        /// Username to use when authenticating to the server.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Password to use when authenticating to the server.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// The host to connect to.
        /// </summary>
        public string? HostName { get; set; }

        /// <summary>
        /// Virtual host to access during this connection.
        /// </summary>
        public string? VirtualHost { get; set; }

        /// <summary>
        /// The port to connect on. <see cref="AmqpTcpEndpoint.UseDefaultPort"/>
        ///  indicates the default for the protocol should be used.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// TLS options setting.
        /// </summary>
        public LocalRabbitMQSslOptions? Ssl { get; set; }
    }
}
