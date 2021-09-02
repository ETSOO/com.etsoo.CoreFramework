using System.Data.Common;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// DbDataReader row data to object parser
    /// DbDataReader行数据到对象解析器
    /// </summary>
    public interface IDataReaderParser<TSelf> where TSelf : IDataReaderParser<TSelf>
    {
        /// <summary>
        /// Create object list from DataReader
        /// 从DataReader创建对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="reader">DataReader</param>
        /// <returns>Result</returns>
        static abstract IAsyncEnumerable<TSelf> CreateAsync(DbDataReader reader);
    }
}
