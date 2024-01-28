using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using System.Buffers;
using System.Collections;
using System.Data.Common;
using System.Text.Json;

namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Action result, compatible with RFC7807
    /// https://tools.ietf.org/html/rfc7807
    /// 操作结果
    /// </summary>
    public record ActionResult : IActionResult
    {
        /// <summary>
        /// Is auto set datetime to Utc kind
        /// 是否设置日期时间为Utc类型
        /// </summary>
        public static bool UtcDateTime { get; set; }

        /// <summary>
        /// Create action result
        /// 创建操作结果
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <returns>Action result</returns>
        public static async ValueTask<ActionResult?> CreateAsync(DbDataReader reader)
        {
            if (await reader.ReadAsync())
            {
                // Fields
                var ok = false;
                string? type = null;
                string? title = null;
                string? field = null;
                int? status = null;
                string? detail = null;
                string? traceId = null;

                // For object field
                var startIndex = 0;

                var data = new StringKeyDictionaryObject();

                for (var f = 0; f < reader.FieldCount; f++)
                {
                    // Ignore NULL values
                    if (await reader.IsDBNullAsync(f))
                        continue;

                    // Column name
                    var name = reader.GetName(f);

                    // Support same name properties
                    // Should after all ActionResult fields
                    if (startIndex == 0)
                    {
                        if (name.Equals("ok", StringComparison.OrdinalIgnoreCase))
                        {
                            // true/false, 1/0
                            ok = StringUtils.TryParseObject<bool>(await reader.GetFieldValueAsync<object>(f)).GetValueOrDefault();
                            continue;
                        }

                        if (name.Equals("type", StringComparison.OrdinalIgnoreCase))
                        {
                            type = await reader.GetFieldValueAsync<string>(f);
                            continue;
                        }

                        if (name.Equals("title", StringComparison.OrdinalIgnoreCase))
                        {
                            title = await reader.GetFieldValueAsync<string>(f);
                            continue;
                        }

                        if (name.Equals("field", StringComparison.OrdinalIgnoreCase))
                        {
                            field = await reader.GetFieldValueAsync<string>(f);
                            continue;
                        }

                        if (name.Equals("status", StringComparison.OrdinalIgnoreCase))
                        {
                            status = await reader.GetFieldValueAsync<int>(f);
                            continue;
                        }

                        if (name.Equals("detail", StringComparison.OrdinalIgnoreCase))
                        {
                            detail = await reader.GetFieldValueAsync<string>(f);
                            continue;
                        }

                        if (name == "trace_id" || name.Equals("traceId", StringComparison.OrdinalIgnoreCase))
                        {
                            // Number or string are supported
                            // await reader.GetFieldValueAsync<string>(f) will fail for numbers
                            traceId = Convert.ToString(await reader.GetFieldValueAsync<object>(f));
                            continue;
                        }

                        // Custom properties
                        startIndex = f;
                    }

                    // Additional data
                    var addValue = await reader.GetFieldValueAsync<object>(f);
                    if (UtcDateTime && addValue is DateTime dt)
                    {
                        addValue = SharedUtils.SetUtcKind(dt);
                    }

                    data.Add(name, addValue);
                }

                return new ActionResult
                {
                    Ok = ok,
                    Field = field,
                    Type = type,
                    Title = title,
                    Status = status,
                    Detail = detail,
                    TraceId = traceId,
                    Data = data
                };
            }

            return null;
        }

        /// <summary>
        /// Create result from exception
        /// 从异常中创建操作结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        public static IActionResult From(Exception ex)
        {
            var result = new ActionResult
            {
                Title = ex.Message,
                Field = ex.Source
            };

            foreach (DictionaryEntry entry in ex.Data)
            {
                var key = entry.Key.ToString();
                if (key == null) continue;
                result.Data[key] = entry.Value;
            }

            return result;
        }

        /// <summary>
        /// Async create result from HTTP response message
        /// 异步从 HTTP 响应消息创建结果
        /// </summary>
        /// <param name="response">Response</param>
        /// <returns>Result</returns>
        public static async Task<IActionResult> FromAsync(HttpResponseMessage response)
        {
            var reason = response.ReasonPhrase;
            var title = "HTTP Response Error" + (string.IsNullOrEmpty(reason) ? string.Empty : $" ({reason})");

            var result = new ActionResult
            {
                Title =  title,
                Detail = await response.Content.ReadAsStringAsync(),
                Status = (int)response.StatusCode
            };

            return result;
        }

        /// <summary>
        /// Create a success result
        /// 创建一个成功的操作结果
        /// </summary>
        /// <returns></returns>
        public static ActionResult Success => new()
        {
            Ok = true
        };

        /// <summary>
        /// Ok or not
        /// 是否成功
        /// </summary>
        public bool Ok { get; init; }

        string? type;
        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        public string? Type
        {
            get { return type; }
            init
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Support type/field format
                    var items = value.Split('/');
                    if (items.Length > 1)
                    {
                        type = items[0];
                        field = items[1];
                        return;
                    }
                }

                type = value;
            }
        }

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        string? field;
        /// <summary>
        /// Field
        /// 字段
        /// </summary>
        public string? Field
        {
            get { return field; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    field = value;
            }
        }

        /// <summary>
        /// Status code
        /// 状态码
        /// </summary>
        public int? Status { get; init; }

        /// <summary>
        /// Detail
        /// 细节
        /// </summary>
        public string? Detail { get; set; }

        /// <summary>
        /// Trace id
        /// 跟踪编号
        /// </summary>
        public string? TraceId { get; set; }

        /// <summary>
        /// Data
        /// 数据
        /// </summary>
        public StringKeyDictionaryObject Data { get; init; } = [];

        /// <summary>
        /// To Json
        /// 转化为 Json
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="options">Options</param>
        /// <param name="fields">Fields allowed</param>
        public async Task ToJsonAsync(IBufferWriter<byte> writer, JsonSerializerOptions options, IEnumerable<string>? fields = null)
        {
            // Utf8JsonWriter
            await using var w = options.CreateJsonWriter(writer);

            // Object start {
            w.WriteStartObject();

            if (options.IsWritable(false))
            {
                var okName = options.ConvertName(nameof(Ok));
                if (fields == null || fields.Contains(nameof(Ok)) || fields.Contains(okName))
                    w.WriteBoolean(okName, Ok);
            }

            if (options.IsWritable(Type == null))
            {
                var typeName = options.ConvertName(nameof(Type));
                if (Type == null)
                    w.WriteNull(typeName);
                else if (fields == null || fields.Contains(nameof(Type)) || fields.Contains(typeName))
                    w.WriteString(typeName, Type);
            }

            if (options.IsWritable(Title == null))
            {
                var titleName = options.ConvertName(nameof(Title));
                if (Title == null)
                    w.WriteNull(titleName);
                else if (fields == null || fields.Contains(nameof(Title)) || fields.Contains(titleName))
                    w.WriteString(titleName, Title);
            }

            if (options.IsWritable(Field == null))
            {
                var fieldName = options.ConvertName(nameof(Field));
                if (Field == null)
                    w.WriteNull(fieldName);
                else if (fields == null || fields.Contains(nameof(Field)) || fields.Contains(fieldName))
                    w.WriteString(fieldName, Field);
            }

            if (options.IsWritable(Detail == null))
            {
                var detailName = options.ConvertName(nameof(Detail));
                if (Detail == null)
                    w.WriteNull(detailName);
                else if (fields == null || fields.Contains(nameof(Detail)) || fields.Contains(detailName))
                    w.WriteString(detailName, Detail);
            }

            if (options.IsWritable(Status == null))
            {
                var statusName = options.ConvertName(nameof(Status));
                if (Status == null)
                    w.WriteNull(statusName);
                else if (fields == null || fields.Contains(nameof(Status)) || fields.Contains(statusName))
                    w.WriteNumber(statusName, Status.Value);
            }

            if (options.IsWritable(TraceId == null))
            {
                var traceIdName = options.ConvertName(nameof(TraceId));
                if (TraceId == null)
                    w.WriteNull(traceIdName);
                else if (fields == null || fields.Contains(nameof(TraceId)) || fields.Contains(traceIdName))
                    w.WriteString(traceIdName, TraceId);
            }

            if (options.IsWritable(Data == null))
            {
                var dataName = options.ConvertName(nameof(Data));
                if (Data == null)
                    w.WriteNull(dataName);
                else if (fields == null || fields.Contains(nameof(Data)) || fields.Contains(dataName))
                {
                    w.WritePropertyName(dataName);

                    // Serialization for the Data
                    await SharedUtils.JsonSerializeAsync(w, Data, CommonJsonSerializerContext.Default.StringKeyDictionaryObject);
                }
            }

            // Object end }
            w.WriteEndObject();

            // Flush & dispose
            await w.DisposeAsync();
        }
    }
}
