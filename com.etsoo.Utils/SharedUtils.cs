using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using Microsoft.IO;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace com.etsoo.Utils
{
    /// <summary>
    /// Shared utils
    /// 共享工具类
    /// </summary>
    public static class SharedUtils
    {
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
            return (manager.GetStream() as RecyclableMemoryStream)!;
        }

        /// <summary>
        /// Get RecyclableMemoryStream
        /// https://github.com/microsoft/Microsoft.IO.RecyclableMemoryStream
        /// </summary>
        /// <param name="bytes">Initial bytes</param>
        /// <returns>Stream</returns>
        public static RecyclableMemoryStream GetStream(byte[] bytes)
        {
            return (manager.GetStream(bytes) as RecyclableMemoryStream)!;
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
            return (manager.GetStream(bytes) as RecyclableMemoryStream)!;
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
        /// <param name="data">Data</param>
        /// <param name="stream">Stream to hold the result</param>
        /// <param name="options">Options</param>
        /// <returns>Task</returns>
        public static async Task JsonSerializeAsync<D>(D data, RecyclableMemoryStream stream, JsonSerializerOptions? options = null)
        {
            options ??= new JsonSerializerOptions
            {
                WriteIndented = false
            };

            if (data is IJsonSerialization di)
            {
                await di.ToJsonAsync(stream, options);
            }
            else
            {
                await JsonSerializer.SerializeAsync(stream, data, options);
            }
        }

        /// <summary>
        /// Serialize as Json
        /// 序列化为Json内容
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns>Task</returns>
        public static async Task<string> JsonSerializeAsync<D>(D data, JsonSerializerOptions? options = null)
        {
            using var ms = GetStream();
            await JsonSerializeAsync(data, ms, options);
            return Encoding.UTF8.GetString(ms.GetReadOnlySequence());
        }

        /// <summary>
        /// Object to dictionary
        /// 转换对象为字典数据
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Result</returns>
        public static async Task<Dictionary<string, object>> ObjectToDictionaryAsync<D>(D obj)
        {
            // Stream
            using var ms = GetStream();

            // Serialize
            await JsonSerializeAsync(obj, ms);

            // Deserialize
            ms.Position = 0;
            return (await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(ms)) ?? new Dictionary<string, object>();
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
