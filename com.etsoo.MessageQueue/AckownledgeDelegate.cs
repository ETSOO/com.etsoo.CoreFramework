using System.Threading.Tasks;

namespace com.etsoo.Utils.MessageQueue
{
    /// <summary>
    /// Ackownledge delegate
    /// 确认委托
    /// </summary>
    /// <param name="deliveryTag">Delivery tag</param>
    /// <param name="multiple">Multiple</param>
    /// <param name="success">Success</param>
    /// <returns>Task</returns>
    public delegate Task AckownledgeDelegate(ulong deliveryTag, bool multiple, bool success);
}
