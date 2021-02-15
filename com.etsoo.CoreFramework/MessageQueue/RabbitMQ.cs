using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.MessageQueue
{
    /// <summary>
    /// RabbitMQ implementation
    /// https://www.rabbitmq.com/
    /// </summary>
    public class RabbitMQ : IMessageQueue
    {
        // Factory
        readonly ConnectionFactory factory;

        // Client name for indication
        readonly string clientName;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="initFactory">Initial factory</param>
        /// <param name="initClientName">Inital client name</param>
        public RabbitMQ(ConnectionFactory initFactory, string initClientName) => (factory, clientName) = (initFactory, initClientName);

        public void Produce()
        {
            using var connection = factory.CreateConnection(clientName);

            connection.CreateModel();
        }
    }
}
