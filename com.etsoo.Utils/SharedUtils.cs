using com.etsoo.Utils.String;

namespace com.etsoo.Utils
{
    /// <summary>
    /// Shared utils
    /// 共享工具类
    /// </summary>
    public static class SharedUtils
    {
        /// <summary>
        /// Enum is defined
        /// 枚举是否定义
        /// </summary>
        /// <typeparam name="T">Enum generic type</typeparam>
        /// <param name="item">Item</param>
        /// <returns>Result</returns>
        public static bool EnumIsDefined<T>(T item) where T : Enum
        {
            return long.TryParse(item.ToString(), out _) is false;
        }

        /// <summary>
        /// Get according data
        /// 获取相应的数据
        /// </summary>
        /// <typeparam name="F">Generic field type</typeparam>
        /// <typeparam name="V">Generic value type</typeparam>
        /// <param name="fields">Fields</param>
        /// <param name="values">Values</param>
        /// <param name="field">Current field</param>
        /// <param name="defaultIndex">Default field index</param>
        /// <returns>Value</returns>
        public static V? GetAccordingData<F, V>(IList<F>? fields, IList<V> values, F field, int defaultIndex = -1)
        {
            var index = fields == null ? defaultIndex : fields.IndexOf(field);
            if (index == -1 || index >= values.Count) return default;

            return values[index];
        }

        /// <summary>
        /// Get according value
        /// 获取相应的值
        /// </summary>
        /// <typeparam name="F">Generic field type</typeparam>
        /// <typeparam name="V">Generic value type</typeparam>
        /// <typeparam name="R">Generic return type</typeparam>
        /// <param name="fields">Fields</param>
        /// <param name="values">Values</param>
        /// <param name="field">Current field</param>
        /// <param name="defaultIndex">Default field index</param>
        /// <returns>Value</returns>
        public static R? GetAccordingValue<F, V, R>(IList<F>? fields, IList<V> values, F field, int defaultIndex = -1) where R : struct
        {
            var value = GetAccordingData(fields, values, field, defaultIndex);
            if (value == null) return default;

            return StringUtils.TryParseObject<R>(value);
        }

        /// <summary>
        /// Get according value
        /// 获取相应的值
        /// </summary>
        /// <typeparam name="R">Generic return type</typeparam>
        /// <param name="fields">Fields</param>
        /// <param name="values">Values</param>
        /// <param name="field">Current field</param>
        /// <param name="defaultIndex">Default field index</param>
        /// <returns>Value</returns>
        public static R? GetAccordingValue<R>(IList<string>? fields, IList<string> values, string field, int defaultIndex = -1) where R : struct
        {
            return GetAccordingValue<string, string, R>(fields, values, field, defaultIndex);
        }
    }
}
