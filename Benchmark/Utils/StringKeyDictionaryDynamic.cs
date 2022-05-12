using com.etsoo.Utils.String;

namespace Benchmark.Utils
{
    /// <summary>
    /// Ignore case of string key and dynamic value extended dictionary
    /// 不区分大小写的字符键和动态值扩展字典
    /// </summary>
    public class StringKeyDictionaryDynamic : StringKeyDictionary<dynamic>
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public StringKeyDictionaryDynamic() : base()
        {
        }

        /// <summary>
        /// Constructor with init data
        /// 带有初始数据的构造函数
        /// </summary>
        /// <param name="Dictionary"></param>
        public StringKeyDictionaryDynamic(IDictionary<string, dynamic?> Dictionary)
            : base(Dictionary)
        {
        }

        /// <summary>
        /// Get value
        /// 获取值
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public T? Get<T>(string key) where T : struct
        {
            var item = GetItem(key);
            return StringUtils.TryParseObject<T>(item);
        }

        /// <summary>
        /// Get value with default value
        /// 获取值，提供默认值
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Value</returns>
        public T Get<T>(string key, T defaultValue) where T : struct
        {
            return this.Get<T>(key).GetValueOrDefault(defaultValue);
        }

        /// <summary>
        /// Get string value
        /// 获取字符串值
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>String value</returns>
        public string? Get(string key)
        {
            var item = GetItem(key);
            return item == null ? null : Convert.ToString(item);
        }
    }
}
