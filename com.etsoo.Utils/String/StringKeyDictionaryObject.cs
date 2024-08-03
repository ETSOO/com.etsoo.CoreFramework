using com.etsoo.Utils.Serialization;
using System.Text.Json;

namespace com.etsoo.Utils.String
{
    /// <summary>
    /// Ignore case of string key and object value extended dictionary, better performance than dynamic value
    /// 不区分大小写的字符键和对象值扩展字典，比dynamic的值效率更高
    /// </summary>
    public class StringKeyDictionaryObject : StringKeyDictionary<object?>
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public StringKeyDictionaryObject() : base()
        {
        }

        /// <summary>
        /// Constructor with init data
        /// 带有初始数据的构造函数
        /// </summary>
        /// <param name="Dictionary"></param>
        public StringKeyDictionaryObject(IDictionary<string, object?> Dictionary)
            : base(Dictionary)
        {
        }

        /// <summary>
        /// Get value
        /// If T is bool then false, bool? then null
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
            return Get<T>(key) ?? defaultValue;
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

        /// <summary>
        /// Get exact object
        /// If T is bool then false, bool? then null
        /// 获取确切的对象
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Object</returns>
        public T? GetExact<T>(string key)
        {
            var item = GetItem(key);
            if (item != null && item is T tItem)
                return tItem;

            // If T is bool then false, bool? then null
            return default;
        }

        /// <summary>
        /// As target object
        /// 转换为目标对象
        /// </summary>
        /// <typeparam name="TSelf">Generic return type</typeparam>
        /// <param name="requiredFields">Required fields</param>
        /// <returns>Result</returns>
        public TSelf? As<TSelf>(params string[] requiredFields) where TSelf : IDictionaryParser<TSelf>
        {
            // Check the required fields
            if (requiredFields.Length > 0 && requiredFields.Any(field => !ContainsKey(field))) return default;

            // Create the object
            return TSelf.Create(this);
        }

        /// <summary>
        /// Get array value
        /// 获取数组值
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="loose">Loose Json type check, true means string "1" also considered as number 1</param>
        /// <param name="splitter">Splitter</param>
        /// <returns>Value</returns>
        public IEnumerable<T> GetArray<T>(string key, bool loose = false, char splitter = ',') where T : struct
        {
            var item = GetItem(key);
            if (item == null) return [];

            if (item is IEnumerable<T> array)
            {
                return array;
            }

            if (item is string str)
            {
                if (str.StartsWith('[') && str.EndsWith(']'))
                {
                    var doc = JsonDocument.Parse(str);
                    return doc.RootElement.GetArray<T>(loose);
                }

                return StringUtils.AsEnumerable<T>(str, splitter);
            }

            return [];
        }

        /// <summary>
        /// Get string array
        /// 获取字符串数组
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="splitter">Splitter</param>
        /// <returns>Result</returns>
        public IEnumerable<string>? GetArray(string key, char splitter = ',')
        {
            var item = GetItem(key);
            if (item == null) return null;

            if (item is IEnumerable<string> stringArray)
            {
                return stringArray;
            }

            if (item is string str)
            {
                if (str.StartsWith('[') && str.EndsWith(']'))
                {
                    var doc = JsonDocument.Parse(str);
                    return doc.RootElement.GetArray(CommonJsonSerializerContext.Default.String);
                }

                return StringUtils.AsEnumerable(str, splitter);
            }

            return null;
        }
    }
}
