using com.etsoo.Utils.Localization;
using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Action result, compatible with RFC7807
    /// https://tools.ietf.org/html/rfc7807
    /// 操作结果
    /// </summary>
    public record ActionResult : IActionResult
    {
        private static Uri ParseType(string? type)
        {
            if (!string.IsNullOrEmpty(type) && Uri.TryCreate(type, UriKind.RelativeOrAbsolute, out var typeUri))
            {
                return typeUri;
            }

            return new Uri("about:blank");
        }

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
        public static async Task<ActionResult?> CreateAsync(DbDataReader reader)
        {
            if (await reader.ReadAsync())
            {
                // Fields
                var success = false;
                var data = new StringKeyDictionaryObject();

                string? title = null;
                string? type = null;
                string? detail = null;
                int? status = null;
                string? traceId = null;

                for (var f = 0; f < reader.FieldCount; f++)
                {
                    // Ignore NULL values
                    if (await reader.IsDBNullAsync(f))
                        continue;

                    // Column name
                    var name = reader.GetName(f);

                    if (name.Equals("success", StringComparison.OrdinalIgnoreCase))
                    {
                        // true/false, 1/0
                        success = StringUtils.TryParseObject<bool>(await reader.GetFieldValueAsync<object>(f)).GetValueOrDefault();
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

                    if (name.Equals("detail", StringComparison.OrdinalIgnoreCase))
                    {
                        detail = await reader.GetFieldValueAsync<string>(f);
                        continue;
                    }

                    if (name.Equals("status", StringComparison.OrdinalIgnoreCase))
                    {
                        status = await reader.GetFieldValueAsync<int>(f);
                        continue;
                    }

                    if (name == "trace_id" || name.Equals("traceId", StringComparison.OrdinalIgnoreCase))
                    {
                        traceId = await reader.GetFieldValueAsync<string>(f);
                        continue;
                    }

                    // Additional data
                    var addValue = await reader.GetFieldValueAsync<object>(f);
                    if(UtcDateTime && addValue is DateTime dt)
                    {
                        addValue = LocalizationUtils.SetUtcKind(dt);
                    }

                    data.Add(name, addValue);
                }

                return new ActionResult(type, data)
                {
                    Success = success,
                    Title = title,
                    Detail = detail,
                    Status = status,
                    TraceId = traceId
                };
            }

            return null;
        }

        /// <summary>
        /// Successful or not result
        /// 是否为成功结果
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        public Uri Type { get; init; }

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Detail
        /// 细节
        /// </summary>
        public string? Detail { get; set; }

        /// <summary>
        /// The HTTP status code
        /// HTTP状态码
        /// </summary>
        public int? Status { get; init; }

        /// <summary>
        /// Log trace id
        /// 日志跟踪编号
        /// </summary>
        public string? TraceId { get; set; }

        /// <summary>
        /// Additional data
        /// 更多数据
        /// </summary>
        public StringKeyDictionaryObject Data { get; init; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public ActionResult(string? type = null, StringKeyDictionaryObject? data = null) : this(ParseType(type), data)
        {
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public ActionResult(Uri type, StringKeyDictionaryObject ? data = null)
        {
            Success = false;
            Type = type;
            Data = data ?? new StringKeyDictionaryObject();
        }

        private readonly List<ActionResultError> errors = new List<ActionResultError>();

        /// <summary>
        /// Errors, grouped by field name
        /// 错误，按字段名称分组
        /// </summary>
        public Dictionary<string, string[]>? Errors
        {
            get
            {
                if (errors.Count == 0)
                    return null;

                return errors.GroupBy(e => e.Name, StringComparer.InvariantCultureIgnoreCase).ToDictionary(k => k.Key, v => v.Select(f => f.Reason).ToArray());
            }
        }

        /// <summary>
        /// Add error
        /// 添加错误
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(ActionResultError error)
        {
            errors.Add(error);
        }

        /// <summary>
        /// Add errors
        /// 添加多个错误
        /// </summary>
        /// <param name="errors">Errors</param>
        public void AddErrors(IEnumerable<ActionResultError> errors)
        {
            this.errors.AddRange(errors);
        }

        /// <summary>
        /// Format title
        /// 格式化标题
        /// </summary>
        /// <param name="data">Format data</param>
        /// <returns>Self</returns>
        public IActionResult FormatTitle(params object[] data)
        {
            if(this.Title != null)
            {
                this.Title = string.Format(this.Title, data);
            }
            return this;
        }

        /// <summary>
        /// Has any error
        /// 是否有任何错误
        /// </summary>
        /// <returns>Has or not</returns>
        public bool HasError()
        {
            return errors.Count > 0;
        }

        /// <summary>
        /// Has specific field error
        /// 是否存在特定字段的错误
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Has or not</returns>
        public bool HasError(string name)
        {
            return errors.Exists(error => error.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Data to id modal
        /// 转化数据为编号模块
        /// </summary>
        /// <typeparam name="T">Generic id type</typeparam>
        /// <returns>Modal</returns>
        public IdModal<T> DataAsIdModal<T>()
        {
            return IdModal<T>.Create(Data);
        }

        /// <summary>
        /// Data to int id modal
        /// 转化数据为整数编号模块
        /// </summary>
        /// <returns>Modal</returns>
        public IdModal<int> DataAsIdModal()
        {
            return IdModal<int>.Create(Data);
        }

        /// <summary>
        /// To Json
        /// 转化为 Json
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="options">Options</param>
        public async Task ToJsonAsync(IBufferWriter<byte> writer, JsonSerializerOptions options)
        {
            // Utf8JsonWriter
            using var w = options.CreateJsonWriter(writer);

            // Object start {
            w.WriteStartObject();

            if (options.IsWritable(false))
            {
                var successName = options.ConvertName("Success");
                w.WriteBoolean(successName, Success);
            }

            if (options.IsWritable(false))
            {
                var typeName = options.ConvertName("Type");
                w.WriteString(typeName, Type.ToString());
            }

            if (options.IsWritable(Title == null))
            {
                var titleName = options.ConvertName("Title");
                if (Title == null)
                    w.WriteNull(titleName);
                else
                    w.WriteString(titleName, Title);
            }

            if (options.IsWritable(Detail == null))
            {
                var detailName = options.ConvertName("Detail");
                if (Detail == null)
                    w.WriteNull(detailName);
                else
                    w.WriteString(detailName, Detail);
            }

            if (options.IsWritable(Status == null))
            {
                var statusName = options.ConvertName("Status");
                if (Status == null)
                    w.WriteNull(statusName);
                else
                    w.WriteNumber(statusName, Status.Value);
            }

            if (options.IsWritable(TraceId == null))
            {
                var traceIdName = options.ConvertName("TraceId");
                if (TraceId == null)
                    w.WriteNull(traceIdName);
                else
                    w.WriteString(traceIdName, TraceId);
            }

            var errors = Errors;
            if (options.IsWritable(errors == null))
            {
                var errorsName = options.ConvertName("Errors");
                if (errors == null)
                    w.WriteNull(errorsName);
                else
                {
                    w.WriteStartObject(errorsName);

                    foreach (var errorItem in errors)
                    {
                        w.WritePropertyName(options.ConvertKeyName(errorItem.Key));

                        w.WriteStartArray();

                        foreach (var errorReason in errorItem.Value)
                        {
                            w.WriteStringValue(errorReason);
                        }

                        w.WriteEndArray();
                    }

                    w.WriteEndObject();
                }
            }

            if (options.IsWritable(Data == null))
            {
                var dataName = options.ConvertName("Data");
                if (Data == null)
                    w.WriteNull(dataName);
                else if(Data.Count > 0)
                {
                    w.WritePropertyName(dataName);

                    // Serialization for the Data
                    JsonSerializer.Serialize(w, Data, options);
                }
            }

            // Object end }
            w.WriteEndObject();

            // Flush & dispose
            await w.DisposeAsync();
        }
    }
}
