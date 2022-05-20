using System.Data.Common;
using System.Runtime.Versioning;

namespace com.etsoo.Database
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
        [RequiresPreviewFeatures]
        static abstract IAsyncEnumerable<TSelf> CreateAsync(DbDataReader reader);
    }
}
