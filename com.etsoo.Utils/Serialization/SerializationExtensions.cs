using com.etsoo.Utils.String;
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Serialization extensions
    /// 序列化扩展
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Mask for serialization
        /// 序列化掩码
        /// </summary>
        public const string Mask = "***";

        /// <summary>
        /// Create Utf8 Json writer
        /// Utf8 Json创建器
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="writer">Buffer writer</param>
        /// <returns></returns>
        public static Utf8JsonWriter CreateJsonWriter(this JsonSerializerOptions options, IBufferWriter<byte> writer)
        {
            return new Utf8JsonWriter(writer, new JsonWriterOptions { Encoder = options.Encoder, Indented = options.WriteIndented });
        }

        /// <summary>
        /// Convert name
        /// 转化名称
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="name">Name</param>
        /// <returns>Converted name</returns>
        public static string ConvertName(this JsonSerializerOptions options, string name)
        {
            var pnp = options.PropertyNamingPolicy;

            if (pnp == null)
            {
                return name;
            }

            return pnp.ConvertName(name);
        }

        /// <summary>
        /// Convert IDictionary key name
        /// 转化字段键名称
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="name">Name</param>
        /// <returns>Converted name</returns>
        public static string ConvertKeyName(this JsonSerializerOptions options, string name)
        {
            var pnp = options.DictionaryKeyPolicy;

            if (pnp == null)
            {
                return name;
            }

            return pnp.ConvertName(name);
        }

        /// <summary>
        /// Is Json serialization writable
        /// 是否支持Json序列化输出
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="isNull">Is null value</param>
        /// <param name="isField">Is field or property</param>
        /// <param name="isReadonly">Is readonly modifier</param>
        /// <returns>Writable</returns>
        public static bool IsWritable(this JsonSerializerOptions options, bool isNull, bool isField = false, bool isReadonly = false)
        {
            if (options.DefaultIgnoreCondition == JsonIgnoreCondition.Always)
                return false;

            if (options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull && isNull)
                return false;

            if (isField)
            {
                if (!options.IncludeFields)
                    return false;

                if (options.IgnoreReadOnlyFields && isReadonly)
                    return false;
            }

            if (options.IgnoreReadOnlyProperties && isReadonly)
                return false;

            return true;
        }

        /// <summary>
        /// PII attribute masking policy
        /// PII 属性屏蔽策略
        /// </summary>
        /// <param name="typeInfo">Type info</param>
        public static void PIIAttributeMaskingPolicy(JsonTypeInfo typeInfo)
        {
            foreach (var propertyInfo in typeInfo.Properties)
            {
                var hasPII = propertyInfo.AttributeProvider?.IsDefined(typeof(PIIAttribute), true);
                if (hasPII == true)
                {
                    var getProperty = propertyInfo.Get;
                    if (getProperty is not null)
                    {
                        propertyInfo.Get = null;
                    }
                }
            }
        }

        /// <summary>
        /// Get property of JsonElement with case insensitive
        /// 获取不区分大小写的 JsonElement 属性
        /// </summary>
        /// <param name="element">Json element</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Result</returns>
        public static JsonElement? GetPropertyCaseInsensitive(this JsonElement element, string propertyName)
        {
            foreach (var property in element.EnumerateObject().OfType<JsonProperty>())
            {
                if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)) return property.Value;
            }
            return null;
        }

        /// <summary>
        /// Get value
        /// 获取值
        /// </summary>
        /// <typeparam name="T">Generic value type</typeparam>
        /// <param name="element">Json element</param>
        /// <param name="loose">Loose Json type check, true means string "1" also considered as number 1</param>
        /// <returns>Result</returns>
        public static T? GetValue<T>(this JsonElement element, bool loose = false) where T : struct
        {
            // Default value, string or other classes will be null
            var dv = default(T);

            // Kind
            var kind = element.ValueKind;

            object? value = dv switch
            {
                bool => (kind == JsonValueKind.True || kind == JsonValueKind.False) && element.TryGetByte(out var v) ? v : (loose ? StringUtils.TryParse<bool>(element.ToString()) : null),
                DateTime => kind == JsonValueKind.String && element.TryGetDateTime(out var v) ? v : null,
                DateTimeOffset => kind == JsonValueKind.String && element.TryGetDateTimeOffset(out var v) ? v : null,
                decimal => kind == JsonValueKind.Number && element.TryGetDecimal(out var v) ? v : (loose ? StringUtils.TryParse<decimal>(element.ToString()) : null),
                double => kind == JsonValueKind.Number && element.TryGetDouble(out var v) ? v : (loose ? StringUtils.TryParse<double>(element.ToString()) : null),
                Guid => kind == JsonValueKind.String && element.TryGetGuid(out var v) ? v : null,
                short => kind == JsonValueKind.Number && element.TryGetInt16(out var v) ? v : (loose ? StringUtils.TryParse<short>(element.ToString()) : null),
                int => kind == JsonValueKind.Number && element.TryGetInt32(out var v) ? v : (loose ? StringUtils.TryParse<int>(element.ToString()) : null),
                long => kind == JsonValueKind.Number && element.TryGetInt64(out var v) ? v : (loose ? StringUtils.TryParse<long>(element.ToString()) : null),
                sbyte => kind == JsonValueKind.Number && element.TryGetSByte(out var v) ? v : (loose ? StringUtils.TryParse<byte>(element.ToString()) : null),
                float => kind == JsonValueKind.Number && element.TryGetSingle(out var v) ? v : (loose ? StringUtils.TryParse<float>(element.ToString()) : null),
                ushort => kind == JsonValueKind.Number && element.TryGetUInt16(out var v) ? v : (loose ? StringUtils.TryParse<ushort>(element.ToString()) : null),
                uint => kind == JsonValueKind.Number && element.TryGetUInt32(out var v) ? v : (loose ? StringUtils.TryParse<uint>(element.ToString()) : null),
                ulong => kind == JsonValueKind.Number && element.TryGetUInt64(out var v) ? v : (loose ? StringUtils.TryParse<ulong>(element.ToString()) : null),
                _ => null
            };

            if (value is T tv) return tv;
            else return null;
        }

        /// <summary>
        /// Get value
        /// 获取值
        /// </summary>
        /// <typeparam name="T">Generic value type</typeparam>
        /// <param name="element">Json element</param>
        /// <param name="loose">Loose Json type check, true means string "1" also considered as number 1</param>
        /// <returns>Result</returns>
        public static T? GetValue<T>(this JsonElement element) where T : class
        {
            // Kind
            var kind = element.ValueKind;

            // Value
            var type = typeof(T);
            if (kind == JsonValueKind.String && type == Types.StringType)
            {
                return element.ToString() as T;
            }
            else if (kind == JsonValueKind.String && type == Types.ByteArrayType)
            {
                if (element.TryGetBytesFromBase64(out var v)) return v as T;
                else return default;
            }
            else if (kind == JsonValueKind.Object)
            {
                return element.Deserialize<T>(SharedUtils.JsonDefaultSerializerOptions);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Get array from JsonElement
        /// 从 JsonElement 获取数组
        /// </summary>
        /// <typeparam name="T">Generic array type</typeparam>
        /// <param name="element">Json element</param>
        /// <param name="loose">Loose Json type check, true means string "1" also considered as number 1</param>
        /// <returns>Result</returns>
        public static IEnumerable<T?> GetArray<T>(this JsonElement element, bool loose = false) where T : struct
        {
            // When not a Array
            if (element.ValueKind != JsonValueKind.Array) yield break;

            foreach (var item in element.EnumerateArray())
            {
                var value = item.GetValue<T>(loose);
                yield return value;
            }
        }

        /// <summary>
        /// Get array from JsonElement
        /// 从 JsonElement 获取数组
        /// </summary>
        /// <typeparam name="T">Generic array type</typeparam>
        /// <param name="element">Json element</param>
        /// <param name="loose">Loose Json type check, true means string "1" also considered as number 1</param>
        /// <returns>Result</returns>
        public static IEnumerable<T?> GetArray<T>(this JsonElement element) where T : class
        {
            // When not a Array
            if (element.ValueKind != JsonValueKind.Array) yield break;

            foreach (var item in element.EnumerateArray())
            {
                var value = item.GetValue<T>();
                yield return value;
            }
        }
    }
}
