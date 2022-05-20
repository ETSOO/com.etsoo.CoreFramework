namespace com.etsoo.Database
{
    /// <summary>
    /// Db exception type
    /// 数据库异常类型
    /// </summary>
    public enum DbExceptionType
    {
        /// <summary>
        /// Out of memory
        /// 内存不足
        /// </summary>
        OutOfMemory,

        /// <summary>
        /// Connection failed
        /// 连接失败
        /// </summary>
        ConnectionFailed,

        /// <summary>
        /// Data processing failed
        /// 数据处理失败
        /// </summary>
        DataProcessingFailed
    }

    /// <summary>
    /// Db exception result interface
    /// 数据库异常结果接口
    /// </summary>
    public interface IDbExceptionResult
    {
        /// <summary>
        /// Exception type
        /// 异常类型
        /// </summary>
        DbExceptionType Type { get; }

        /// <summary>
        /// Critical or not
        /// 严重与否
        /// </summary>
        bool Critical { get; }
    }

    /// <summary>
    /// Db exception result
    /// 数据库异常结果
    /// </summary>
    public record DbExceptionResult(DbExceptionType Type, bool Critical) : IDbExceptionResult;
}
