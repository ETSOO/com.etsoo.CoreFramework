using System;
using System.Threading.Tasks;

namespace com.etsoo.Utils.MessageQueue
{
    /// <summary>
    /// Consume data delegate
    /// 消费数据委托
    /// </summary>
    /// <param name="body">Message body</param>
    /// <param name="routingKey">Originally published routing key</param>
    /// <param name="redelivered">Is redelivered</param>
    /// <returns>Result</returns>
    public delegate Task<ReadOnlyMemory<byte>> ConsumeDataDelegate(ReadOnlyMemory<byte> body, string routingKey, bool redelivered);
}
