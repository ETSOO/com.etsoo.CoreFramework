using com.etsoo.Utils.String;
using System.Runtime.Versioning;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Dictionary to object parser
    /// 字典数据到对象解析器
    /// </summary>
    public interface IDictionaryParser<TSelf> where TSelf : IDictionaryParser<TSelf>
    {
        /// <summary>
        /// Create object from dictionary
        /// 从字典数据创建对象
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="dic">Dictionary data</param>
        /// <returns>Result</returns>
        [RequiresPreviewFeatures]
        static abstract TSelf Create(StringKeyDictionaryObject dic);
    }
}
