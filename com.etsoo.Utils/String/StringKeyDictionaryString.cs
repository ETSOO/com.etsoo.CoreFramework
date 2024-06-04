using com.etsoo.Utils.Serialization;
using System.Text.Json;

namespace com.etsoo.Utils.String
{
    /// <summary>
    /// Ignore case of string key and string value extended dictionary
    /// 不区分大小写的字符键和字符值扩展字典
    /// </summary>
    public class StringKeyDictionaryString : StringKeyDictionary<string?>
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public StringKeyDictionaryString() : base()
        {
        }

        /// <summary>
        /// Constructor with init data
        /// 带有初始数据的构造函数
        /// </summary>
        /// <param name="Dictionary"></param>
        public StringKeyDictionaryString(IDictionary<string, string?> Dictionary)
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
            return StringUtils.TryParse<T>(item);
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
            return Get<T>(key) ?? defaultValue;
        }

        /// <summary>
        /// Get value with default value
        /// 获取值，提供默认值
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="loose">Loose Json type check, true means string "1" also considered as number 1</param>
        /// <returns>Value</returns>
        public IEnumerable<T> GetArray<T>(string key, bool loose = false) where T : struct
        {
            var json = GetItem(key);
            if (string.IsNullOrEmpty(json) || !json.StartsWith('[') || !json.EndsWith(']'))
            {
                return [];
            }

            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetArray<T>(loose);
        }
    }
}