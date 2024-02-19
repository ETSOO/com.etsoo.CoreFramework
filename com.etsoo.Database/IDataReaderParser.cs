using System.Data.Common;

namespace com.etsoo.Database
{
    /// <summary>
    /// DbDataReader row data to object parser
    /// DbDataReader行数据到对象解析器
    /// </summary>
    public interface IDataReaderParser<TSelf> where TSelf : IDataReaderParser<TSelf>
    {
        /// <summary>
        /// Parser inner fields
        /// 解析器内部字段
        /// </summary>
        static abstract IEnumerable<string> ParserInnerFields { get; }

        /// <summary>
        /// Create object async list from DataReader
        /// 从DataReader创建异步对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        static abstract IAsyncEnumerable<TSelf> CreateAsync(DbDataReader reader, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create object list from DataReader
        /// 从DataReader创建对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="reader">DataReader</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        static abstract Task<List<TSelf>> CreateListAsync(DbDataReader reader, CancellationToken cancellationToken = default);
    }
}
