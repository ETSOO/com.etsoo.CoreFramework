using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using Microsoft.IO;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

namespace com.etsoo.Utils
{
    /// <summary>
    /// Shared utils
    /// 共享工具类
    /// </summary>
    public static class SharedUtils
    {
        /// <summary>
        /// Json default serializer options
        /// Json 默认序列化选项
        /// </summary>
        public static JsonSerializerOptions JsonDefaultSerializerOptions => JsonDefaultSerializerOptionsSetup(new());

        /// <summary>
        /// Json default serializer options setup
        /// Json 默认序列化选项设置
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Options</returns>
        public static JsonSerializerOptions JsonDefaultSerializerOptionsSetup(JsonSerializerOptions options)
        {
            options.Encoder ??= JavaScriptEncoder.Create(UnicodeRanges.All);

            options.WriteIndented = false;
            options.PropertyNameCaseInsensitive = true;

            if (options.DefaultIgnoreCondition == JsonIgnoreCondition.Never)
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            options.PropertyNamingPolicy ??= JsonNamingPolicy.CamelCase;
            options.DictionaryKeyPolicy ??= JsonNamingPolicy.CamelCase;

            options.NumberHandling = JsonNumberHandling.AllowReadingFromString;

            return options;
        }

        /// <summary>
        /// Get enum value related keys
        /// 获取枚举值相关键
        /// </summary>
        /// <typeparam name="E">Generic enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns>All keys</returns>
        public static IEnumerable<string> GetKeys<E>(this E enumValue) where E : struct, Enum
        {
            return Enum.GetValues<E>().Where(v => enumValue.HasFlag(v)).Select(r => r.ToString());
        }

        /// <summary>
        /// Get claim value from collection
        /// 从声明集合中获取值
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="type">Claim type</param>
        /// <param name="comparisonType">Type comparison type</param>
        /// <returns></returns>
        public static string? GetValue(this IEnumerable<Claim> claims, string type, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            return claims.FirstOrDefault(c => c.Type.Equals(type, comparisonType))?.Value;
        }

        private static readonly DateTime JsBaseDateTime = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly RecyclableMemoryStreamManager manager = new();
        private const int defaultBufferSize = 1024;

        /// <summary>
        /// Get RecyclableMemoryStream
        /// https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream
        /// </summary>
        /// <returns>Stream</returns>
        public static RecyclableMemoryStream GetStream()
        {
            return manager.GetStream();
        }

        /// <summary>
        /// Get RecyclableMemoryStream
        /// https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream
        /// </summary>
        /// <param name="bytes">Initial bytes</param>
        /// <returns>Stream</returns>
        public static RecyclableMemoryStream GetStream(byte[] bytes)
        {
            return manager.GetStream(bytes);
        }

        /// <summary>
        /// Get RecyclableMemoryStream
        /// https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream
        /// </summary>
        /// <param name="input">Input UTF8 string</param>
        /// <returns>Stream</returns>
        public static RecyclableMemoryStream GetStream(string input)
        {
            return GetStream(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Get RecyclableMemoryStream
        /// https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream
        /// </summary>
        /// <param name="bytes">Initial bytes</param>
        /// <returns>Stream</returns>
        public static RecyclableMemoryStream GetStream(ReadOnlySpan<byte> bytes)
        {
            return manager.GetStream(bytes);
        }

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

        /// <summary>
        /// Serialize as Json
        /// 序列化为Json内容
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <param name="stream">Stream to hold the result</param>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <param name="fields">Fields allowed</param>
        /// <returns>Task</returns>
        [RequiresDynamicCode("JsonSerializeAsync 'D' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("JsonSerializeAsync 'D' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task JsonSerializeAsync<D>(RecyclableMemoryStream stream, D data, JsonSerializerOptions? options = null, IEnumerable<string>? fields = null)
        {
            options ??= JsonDefaultSerializerOptions;

            if (data is IJsonSerialization di)
            {
                await di.ToJsonAsync(stream, options, fields);
            }
            else if (fields == null)
            {
                await JsonSerializer.SerializeAsync(stream, data, options);
            }
            else
            {
                // var newOptions = new JsonSerializerOptions(options);
                await using var ms = GetStream();
                await JsonSerializer.SerializeAsync(ms, data, options);
                ms.Position = 0;
                await using var newStream = await JsonKeepNodesAsync(ms, fields);
                await newStream.CopyToAsync(stream);
            }
        }

        /// <summary>
        /// Serialize as Json
        /// 序列化为Json内容
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <param name="stream">Stream to hold the result</param>
        /// <param name="data">Data</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="fields">Fields allowed</param>
        /// <returns>Task</returns>
        public static async Task JsonSerializeAsync<D>(RecyclableMemoryStream stream, D data, JsonTypeInfo<D> typeInfo, IEnumerable<string>? fields = null)
        {
            if (data is IJsonSerialization di)
            {
                await di.ToJsonAsync(stream, JsonDefaultSerializerOptions, fields);
            }
            else if (fields == null)
            {
                await JsonSerializer.SerializeAsync(stream, data, typeInfo);
            }
            else
            {
                await using var ms = GetStream();
                await JsonSerializer.SerializeAsync(ms, data, typeInfo);
                ms.Position = 0;
                await using var newStream = await JsonKeepNodesAsync(ms, fields);
                await newStream.CopyToAsync(stream);
            }
        }

        /// <summary>
        /// Json get node with property name or path
        /// 获取Json节点，支持属性名或路径
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="nodeName">Node name or path</param>
        /// <returns>Result</returns>
        public static (JsonObject, JsonNode)? JsonGetNode(JsonObject root, string nodeName)
        {
            var path = nodeName.Split('.');
            var len = path.Length;

            var obj = root;

            for (var i = 0; i < len; i++)
            {
                var p = path[i];

                var item = obj.FirstOrDefault(item => item.Key.Equals(p, StringComparison.OrdinalIgnoreCase));

                if (!item.Equals(default) && item.Value != null)
                {
                    var node = item.Value;
                    if (i + 1 == len)
                    {
                        return (obj, node);
                    }
                    else
                    {
                        if (node is JsonObject o)
                        {
                            obj = o;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// Json remove nodes
        /// 删除Json节点
        /// </summary>
        /// <param name="jsonStream">Json stream</param>
        /// <param name="nodes">Nodes of name or path</param>
        /// <returns>Result</returns>
        public static async Task<Stream> JsonRemoveNodesAsync(Stream jsonStream, IEnumerable<string> nodes)
        {
            jsonStream.Position = 0;

            using var doc = await JsonDocument.ParseAsync(jsonStream);

            if (doc.RootElement.ValueKind != JsonValueKind.Object) return jsonStream;

            var root = JsonObject.Create(doc.RootElement);
            if (root == null) return jsonStream;

            foreach (var node in nodes)
            {
                var nodeInfo = JsonGetNode(root, node);
                if (nodeInfo == null) continue;

                var (parent, child) = nodeInfo.Value;
                parent.Remove(child.GetPropertyName());
            }

            return GetStream(root.ToJsonString());
        }

        /// <summary>
        /// Json keep nodes, only support first level names
        /// 保留Json节点，仅支持第一级名称
        /// </summary>
        /// <param name="jsonStream">Json stream</param>
        /// <param name="nodes">Nodes of name</param>
        /// <returns>Result</returns>
        public static async Task<Stream> JsonKeepNodesAsync(Stream jsonStream, IEnumerable<string> nodes)
        {
            using var doc = await JsonDocument.ParseAsync(jsonStream);

            if (doc.RootElement.ValueKind != JsonValueKind.Object) return jsonStream;

            var root = JsonObject.Create(doc.RootElement);
            if (root == null) return jsonStream;

            foreach (var item in root.ToList())
            {
                if (!nodes.Contains(item.Key, StringComparer.OrdinalIgnoreCase))
                {
                    root.Remove(item.Key);
                }
            }

            return GetStream(root.ToJsonString());
        }

        /// <summary>
        /// Serialize as Json
        /// 序列化为Json内容
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <param name="fields">Fields allowed</param>
        /// <returns>Task</returns>
        [RequiresDynamicCode("JsonSerializeAsync 'D' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("JsonSerializeAsync 'D' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task<string> JsonSerializeAsync<D>(D data, JsonSerializerOptions? options = null, IEnumerable<string>? fields = null)
        {
            await using var ms = GetStream();
            await JsonSerializeAsync(ms, data, options, fields);
            return Encoding.UTF8.GetString(ms.GetReadOnlySequence());
        }

        /// <summary>
        /// Serialize as Json
        /// 序列化为Json内容
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="fields">Fields allowed</param>
        /// <returns>Task</returns>
        public static async Task<string> JsonSerializeAsync<D>(D data, JsonTypeInfo<D> typeInfo, IEnumerable<string>? fields = null)
        {
            await using var ms = GetStream();
            await JsonSerializeAsync(ms, data, typeInfo, fields);
            return Encoding.UTF8.GetString(ms.GetReadOnlySequence());
        }

        /// <summary>
        /// Serialize as Json
        /// 序列化为Json内容
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <param name="writer">Json stream writer</param>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <param name="fields">Fields allowed</param>
        /// <returns>Task</returns>
        [RequiresDynamicCode("JsonSerializeAsync 'D' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("JsonSerializeAsync 'D' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task JsonSerializeAsync<D>(Utf8JsonWriter writer, D data, JsonSerializerOptions? options = null, IEnumerable<string>? fields = null)
        {
            await using var ms = GetStream();
            await JsonSerializeAsync(ms, data, options, fields);
            writer.WriteRawValue(ms.GetReadOnlySequence(), true);
        }

        /// <summary>
        /// Serialize as Json
        /// 序列化为Json内容
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <param name="writer">Json stream writer</param>
        /// <param name="data">Data</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="fields">Fields allowed</param>
        /// <returns>Task</returns>
        public static async Task JsonSerializeAsync<D>(Utf8JsonWriter writer, D data, JsonTypeInfo<D> typeInfo, IEnumerable<string>? fields = null)
        {
            await using var ms = GetStream();
            await JsonSerializeAsync(ms, data, typeInfo, fields);
            writer.WriteRawValue(ms.GetReadOnlySequence(), true);
        }

        /// <summary>
        /// Join two data as audit Json string
        /// 合并两个数据作为审计的Json字符串
        /// </summary>
        /// <typeparam name="O">Generic old data type</typeparam>
        /// <typeparam name="N">Generic new data type</typeparam>
        /// <param name="oldData">Old data, object or json</param>
        /// <param name="newData">New data, object or json</param>
        /// <param name="fields">Fields allowed</param>
        /// <param name="options">Json options</param>
        /// <param name="oldDataName">Old data Json node name</param>
        /// <param name="newDataName">New data Json node name</param>
        /// <returns>Json string</returns>
        [RequiresDynamicCode("JoinAsAuditJsonAsync 'O' and 'N' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("JoinAsAuditJsonAsync 'O' and 'N' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task<string> JoinAsAuditJsonAsync<O, N>(O oldData, N newData, IEnumerable<string> fields, JsonSerializerOptions? options = null, string? oldDataName = null, string? newDataName = null)
        {
            // Default options
            options ??= JsonDefaultSerializerOptions;
            oldDataName ??= nameof(oldData);
            newDataName ??= nameof(newData);

            // Stream
            await using var ms = GetStream();

            // Object start {
            ms.WriteByte(123);

            // Old data
            await ms.WriteAsync(Encoding.UTF8.GetBytes($"\"{oldDataName}\":"));

            if (oldData is string oldJson)
            {
                await ms.WriteAsync(Encoding.UTF8.GetBytes(oldJson));
            }
            else
            {
                await JsonSerializeAsync(ms, oldData, options, fields);
            }

            // ,
            ms.WriteByte(44);

            // New data
            await ms.WriteAsync(Encoding.UTF8.GetBytes($"\"{newDataName}\":"));

            if (newData is string newJson)
            {
                await ms.WriteAsync(Encoding.UTF8.GetBytes(newJson));
            }
            else
            {
                await JsonSerializeAsync(ms, newData, options, fields);
            }

            // Object end }
            ms.WriteByte(125);

            return Encoding.UTF8.GetString(ms.GetReadOnlySequence());
        }

        /// <summary>
        /// Join two data as audit Json string
        /// 合并两个数据作为审计的Json字符串
        /// </summary>
        /// <typeparam name="O">Generic old data type</typeparam>
        /// <typeparam name="N">Generic new data type</typeparam>
        /// <param name="oldData">Old data, object or json</param>
        /// <param name="oldDataTypeInfo">Old data json type info</param>
        /// <param name="newData">New data, object or json</param>
        /// <param name="newDataTypeInfo">New data json type info</param>
        /// <param name="fields">Fields allowed</param>
        /// <param name="oldDataName">Old data Json node name</param>
        /// <param name="newDataName">New data Json node name</param>
        /// <returns>Json string</returns>
        public static async Task<string> JoinAsAuditJsonAsync<O, N>(O oldData, JsonTypeInfo<O> oldDataTypeInfo, N newData, JsonTypeInfo<N> newDataTypeInfo, IEnumerable<string> fields, string? oldDataName = null, string? newDataName = null)
        {
            oldDataName ??= nameof(oldData);
            newDataName ??= nameof(newData);

            // Stream
            await using var ms = GetStream();

            // Object start {
            ms.WriteByte(123);

            // Old data
            await ms.WriteAsync(Encoding.UTF8.GetBytes($"\"{oldDataName}\":"));

            if (oldData is string oldJson)
            {
                await ms.WriteAsync(Encoding.UTF8.GetBytes(oldJson));
            }
            else
            {
                await JsonSerializeAsync(ms, oldData, oldDataTypeInfo, fields);
            }

            // ,
            ms.WriteByte(44);

            // New data
            await ms.WriteAsync(Encoding.UTF8.GetBytes($"\"{newDataName}\":"));

            if (newData is string newJson)
            {
                await ms.WriteAsync(Encoding.UTF8.GetBytes(newJson));
            }
            else
            {
                await JsonSerializeAsync(ms, newData, newDataTypeInfo, fields);
            }

            // Object end }
            ms.WriteByte(125);

            return Encoding.UTF8.GetString(ms.GetReadOnlySequence());
        }

        /// <summary>
        /// Object to dictionary
        /// 转换对象为字典数据
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("ObjectToDictionaryAsync 'D' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("ObjectToDictionaryAsync 'D' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task<Dictionary<string, object>> ObjectToDictionaryAsync<D>(D obj)
        {
            // Stream
            await using var ms = GetStream();

            // Serialize
            await JsonSerializer.SerializeAsync(ms, obj);

            // Deserialize
            ms.Position = 0;
            return (await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(ms)) ?? [];
        }

        /// <summary>
        /// Object to dictionary
        /// 转换对象为字典数据
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="typeInfo">Json type info</param>
        /// <returns>Result</returns>
        public static async Task<Dictionary<string, object>> ObjectToDictionaryAsync<D>(D obj, JsonTypeInfo<D> typeInfo)
        {
            // Stream
            await using var ms = GetStream();

            // Serialize
            await JsonSerializer.SerializeAsync(ms, obj, typeInfo);

            // Deserialize
            ms.Position = 0;
            return (await JsonSerializer.DeserializeAsync(ms, CommonJsonSerializerContext.Default.DictionaryStringObject)) ?? [];
        }

        /// <summary>
        /// Set datatime's Utc kind
        /// 设置日期时间的类型为Utc
        /// </summary>
        /// <param name="input">Input datetime</param>
        /// <returns>Utc datetime</returns>
        public static DateTime? SetUtcKind(DateTime? input)
        {
            if (input == null)
                return input;
            return SetUtcKind(input.Value);
        }

        /// <summary>
        /// Set datatime's Utc kind
        /// 设置日期时间的类型为Utc
        /// </summary>
        /// <param name="input">Input datetime</param>
        /// <returns>Utc datetime</returns>
        public static DateTime SetUtcKind(DateTime input)
        {
            return input.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(input, DateTimeKind.Utc) : input;
        }

        /// <summary>
        /// Stream to bytes
        /// 流到字节
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <returns>Bytes</returns>
        public static ReadOnlySpan<byte> StreamToBytes(Stream stream)
        {
            var writer = new ArrayBufferWriter<byte>(defaultBufferSize);
            int bytesRead;
            while ((bytesRead = stream.Read(writer.GetSpan(defaultBufferSize))) > 0)
            {
                writer.Advance(bytesRead);
            }

            return writer.WrittenSpan;
        }

        /// <summary>
        /// Async stream to bytes
        /// 异步流到字节
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <returns>Bytes</returns>
        public static async Task<ReadOnlyMemory<byte>> StreamToBytesAsync(Stream stream)
        {
            var writer = new ArrayBufferWriter<byte>(defaultBufferSize);
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(writer.GetMemory(defaultBufferSize))) > 0)
            {
                writer.Advance(bytesRead);
            }

            return writer.WrittenMemory;
        }

        /// <summary>
        /// Stream to UTF8 string
        /// 流到UTF8字符串
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <returns>String</returns>
        public static string StreamToString(Stream stream)
        {
            var bytes = StreamToBytes(stream);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Async stream to UTF8 string
        /// 异步流到UTF8字符串
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <returns>String</returns>
        public static async Task<string> StreamToStringAsync(Stream stream)
        {
            var bytes = await StreamToBytesAsync(stream);
            return Encoding.UTF8.GetString(bytes.Span);
        }

        /// <summary>
        /// JS date miliseconds to C# DateTime UTC
        /// JS 日期毫秒到 C# DateTime UTC
        /// </summary>
        /// <param name="miliseconds">Miliseconds</param>
        /// <returns>DateTime UTC</returns>
        public static DateTime JsMilisecondsToUTC(long miliseconds)
        {
            return JsBaseDateTime.AddMilliseconds(miliseconds);
        }

        /// <summary>
        /// Tuncate miliseconds off a datetime
        /// https://stackoverflow.com/questions/1004698/how-to-truncate-milliseconds-off-of-a-net-datetime
        /// </summary>
        /// <param name="input">Input datetime</param>
        /// <returns>Result</returns>
        public static DateTime TruncateDateTime(DateTime input)
        {
            return input.AddTicks(-(input.Ticks % TimeSpan.TicksPerSecond));
        }

        /// <summary>
        /// Unix seconds from 1970 to UTC datetime
        /// Unix 秒数转换为 UTC时间
        /// </summary>
        /// <param name="seconds">Unix seconds</param>
        /// <returns>UTC datetime</returns>
        public static DateTime UnixSecondsToUTC(long seconds)
        {
            return JsBaseDateTime.AddSeconds(seconds);
        }

        /// <summary>
        /// DateTime UTC to JS date miliseconds
        /// C# DateTime UTC 到JS 日期毫秒
        /// </summary>
        /// <param name="dt">UTC DateTime</param>
        /// <returns>Miliseconds</returns>
        public static long UTCToJsMiliseconds(DateTime? dt = null)
        {
            return (long)(SetUtcKind(dt).GetValueOrDefault(DateTime.UtcNow) - JsBaseDateTime).TotalMilliseconds;
        }

        /// <summary>
        /// DateTime UTC to Unix seconds
        /// C# DateTime UTC 到 Unix 秒数
        /// </summary>
        /// <param name="dt">UTC Datetime</param>
        /// <returns>Seconds</returns>
        public static long UTCToUnixSeconds(DateTime? dt = null)
        {
            return (long)(SetUtcKind(dt).GetValueOrDefault(DateTime.UtcNow) - JsBaseDateTime).TotalSeconds;
        }
    }
}
