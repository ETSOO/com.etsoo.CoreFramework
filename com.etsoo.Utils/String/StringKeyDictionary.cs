using System;
using System.Collections.Generic;

namespace com.etsoo.Utils.String
{
    /// <summary>
    /// Ignore case of string key extended dictionary
    /// 不区分大小写的字符键扩展字典
    /// </summary>
    /// <typeparam name="V">Generic value type</typeparam>
    public class StringKeyDictionary<V> : Dictionary<string, V?>
    {
        /// <summary>
        /// Constructor with init data
        /// 带有初始数据的构造函数
        /// </summary>
        /// <param name="Dictionary"></param>
        public StringKeyDictionary(IDictionary<string, V?> Dictionary)
            : base(Dictionary, StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        ///  Constructor
        ///  构造函数
        /// </summary>
        public StringKeyDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Get item with key
        /// 通过键名获取值
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public V? GetItem(string key)
        {
            if (this.TryGetValue(key, out var value))
            {
                return value;
            }

            return default;
        }
    }
}
