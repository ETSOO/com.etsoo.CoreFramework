namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub options
    /// Google PubSub 选项
    /// </summary>
    public record GooglePubSubOptions
    {
        /// <summary>
        /// The endpoint to connect to, or null to use the default endpoint.
        /// </summary>
        public string? Endpoint { get; set; }

        /// <summary>
        /// The path to the credentials file to use.
        /// </summary>
        public string? CredentialsPath { get; set; }

        /// <summary>
        /// The credentials to use as a JSON string.
        /// </summary>
        public string? JsonCredentials { get; set; }

        /// <summary>
        /// The number of <see cref="SubscriberServiceApiClient"/>s to create and use within a <see cref="SubscriberClient"/> instance.
        /// </summary>
        public int? ClientCount { get; set; }

        /// <summary>
        /// The GCP project ID that should be used for quota and billing purposes.
        /// May be null.
        /// </summary>
        public string? QuotaProject { get; set; }

        /// <summary>
        /// A custom user agent to specify in the channel metadata.
        /// </summary>
        public string? UserAgent { get; set; }
    }
}
