namespace com.etsoo.Utils
{
    /// <summary>
    /// Intermitten exception, used for retry
    /// 间隙性异常，用于重新处理
    /// </summary>
    public class IntermittenException : Exception
    {
        public IntermittenException(string message, Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}