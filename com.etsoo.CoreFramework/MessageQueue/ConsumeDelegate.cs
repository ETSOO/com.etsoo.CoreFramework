using System;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.MessageQueue
{
    /// <summary>
    /// Consume delegate
    /// 消费委托
    /// </summary>
    /// <param name="body">Message body</param>
    /// <param name="routingKey">Originally published routing key</param>
    /// <param name="redelivered">Is redelivered</param>
    /// <returns>Success</returns>
    public delegate Task<bool> ConsumeDelegate(ReadOnlyMemory<byte> body, string routingKey, bool redelivered);
}
