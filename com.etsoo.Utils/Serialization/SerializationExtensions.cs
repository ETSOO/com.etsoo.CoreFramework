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
        /// Get array from JsonElement
        /// 从 JsonElement 获取数组
        /// </summary>
        /// <typeparam name="T">Generic array type</typeparam>
        /// <param name="element">Json element</param>
        /// <returns>Result</returns>
        public static IEnumerable<T?> GetArray<T>(this JsonElement element)
        {
            // When not a Array
            if (element.ValueKind != JsonValueKind.Array) yield break;

            // Default value, string or other classes will be null
            var dv = default(T);

            // Options
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

            foreach (var item in element.EnumerateArray())
            {
                // Kind
                var kind = item.ValueKind;

                object? value = dv switch
                {
                    bool => (kind == JsonValueKind.True || kind == JsonValueKind.False) && item.TryGetByte(out var v) ? v : null,
                    byte[] => kind == JsonValueKind.String && item.TryGetBytesFromBase64(out var v) ? v : null,
                    DateTime => kind == JsonValueKind.String && item.TryGetDateTime(out var v) ? v : null,
                    DateTimeOffset => kind == JsonValueKind.String && item.TryGetDateTimeOffset(out var v) ? v : null,
                    decimal => kind == JsonValueKind.Number && item.TryGetDecimal(out var v) ? v : null,
                    double => kind == JsonValueKind.Number && item.TryGetDouble(out var v) ? v : null,
                    Guid => kind == JsonValueKind.String && item.TryGetGuid(out var v) ? v : null,
                    short => kind == JsonValueKind.Number && item.TryGetInt16(out var v) ? v : null,
                    int => kind == JsonValueKind.Number && item.TryGetInt32(out var v) ? v : null,
                    long => kind == JsonValueKind.Number && item.TryGetInt64(out var v) ? v : null,
                    sbyte => kind == JsonValueKind.Number && item.TryGetSByte(out var v) ? v : null,
                    float => kind == JsonValueKind.Number && item.TryGetSingle(out var v) ? v : null,
                    ushort => kind == JsonValueKind.Number && item.TryGetUInt16(out var v) ? v : null,
                    uint => kind == JsonValueKind.Number && item.TryGetUInt32(out var v) ? v : null,
                    ulong => kind == JsonValueKind.Number && item.TryGetUInt64(out var v) ? v : null,
                    _ => kind == JsonValueKind.String || typeof(T) == Types.StringType ? item.ToString() : item.Deserialize<T>(options)
                };

                if (value == null) yield return default;
                else if (value is T tv) yield return tv;
            }
        }
    }
}
