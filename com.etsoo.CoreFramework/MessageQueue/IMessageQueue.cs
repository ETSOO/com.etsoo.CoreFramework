using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.MessageQueue
{
    /// <summary>
    /// Message queue interface
    /// 消息队列接口
    /// </summary>
    public interface IMessageQueue : IDisposable
    {
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
        bool PrepareProduce(string exchange, string? routingKey = null, bool durable = false, bool exclusive = false, bool autoDelete = false, IDictionary<string, object>? arguments = null);

        /// <summary>
        /// Produce
        /// 生产
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="exchange">Exchange</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="arguments">Arguments</param>
        void Produce(ReadOnlyMemory<byte> body, string exchange, string routingKey, IDictionary<string, object>? arguments = null);

        /// <summary>
        /// Produce
        /// 生产
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="exchange">Exchange</param>
        /// <param name="routingKey">Routing key</param>
        /// <param name="persistent">Persistent</param>
        void Produce(ReadOnlyMemory<byte> body, string exchange, string routingKey, bool persistent);

        /// <summary>
        /// Remote procedure call
        /// 远程过程调用
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="queue">Queue name</param>
        ReadOnlyMemory<byte> PRCCall(ReadOnlyMemory<byte> body, string queue);

        /// <summary>
        /// Async remote procedure call
        /// 异步远程过程调用
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="queue">Queue name</param>
        Task<ReadOnlyMemory<byte>> PRCCallAsync(ReadOnlyMemory<byte> body, string queue);

        /// <summary>
        /// Produce confirm
        /// 生产确认
        /// </summary>
        /// <param name="callback">Callback</param>
        void ProduceConfirm(AckownledgeDelegate callback);

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
        void PrepareConsume(ConsumeDelegate callback, string exchange, string routingKey, bool durable = false, bool exclusive = false, bool autoDelete = false, bool autoAck = true, IDictionary<string, object>? arguments = null);

        /// <summary>
        /// Prepare for PRC client
        /// 准备远程调用客户端
        /// </summary>
        void PreparePRCClient();

        /// <summary>
        /// Prepare for PRC server
        /// 准备远程调用服务器
        /// </summary>
        /// <param name="callback">Callback</param>
        /// <param name="queue">Queue name</param>
        void PreparePRCServer(ConsumeDataDelegate callback, string queue);
    }
}
