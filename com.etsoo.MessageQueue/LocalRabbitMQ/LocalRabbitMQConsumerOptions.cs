namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    /// <summary>
    /// BasicQos options
    /// </summary>
    public record LocalRabbitMQConsumerQosOptions
    {
        public uint PrefetchSize { get; init; }
        public ushort PrefetchCount { get; init; }
        public bool Global { get; init; }
    }

    /// <summary>
    /// Local RabbitMQ message consumer options
    /// 本地 RabbitMQ 消息消费者选项
    /// </summary>
    public record LocalRabbitMQConsumerOptions : LocalRabbitMQOptions
    {
        /// <summary>
        /// Queue name
        /// 队列名称
        /// </summary>
        public string? QueueName { get; init; }

        /// <summary>
        /// The name of the exchange
        /// The empty string denotes the default or nameless exchange: 
        /// messages are routed to the queue with the name specified by routingKey, if it exists
        /// </summary>
        public string? Exchange { get; init; }

        /// <summary>
        /// Exchange type includes: direct, topic, headers and fanout
        /// 交换类型
        /// </summary>
        public string? ExchangeType { get; init; }

        /// <summary>
        /// Routing key for routing and topics 
        /// </summary>
        public string? RoutingKey { get; init; }

        /// <summary>
        /// Should this queue will survive a broker restart?
        /// </summary>
        public bool Durable { get; init; }

        /// <summary>
        /// Should this queue use be limited to its declaring connection?
        /// </summary>
        public bool Exclusive { get; init; }

        /// <summary>
        /// Should this queue be auto-deleted when its last consumer (if any) unsubscribes?
        /// </summary>
        public bool AutoDelete { get; init; }

        /// <summary>
        /// This flag tells the server how to react if a message cannot be routed to a queue
        /// </summary>
        public bool Mandatory { get; init; }

        /// <summary>
        /// Additional queue arguments, e.g. "x-queue-type"
        /// 额外的队列参数，例如 “x-queue-type”
        /// </summary>
        public IDictionary<string, object>? QueueArguments { get; init; }

        /// <summary>
        /// BasicQos options
        /// </summary>
        public LocalRabbitMQConsumerQosOptions? QosOptions { get; init; }
    }
}
