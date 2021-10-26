using com.etsoo.Utils.Localization;
using com.etsoo.Utils.String;
using Dapper;
using System.Data.Common;

namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Action result, compatible with RFC7807
    /// https://tools.ietf.org/html/rfc7807
    /// 操作结果
    /// </summary>
    public record ActionResult<T> : ActionResultBase
    {
        /// <summary>
        /// Create action result
        /// 创建操作结果
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <returns>Action result</returns>
        public static async Task<ActionResult<T>?> CreateAsync(DbDataReader reader)
        {
            if (await reader.ReadAsync())
            {
                // Fields
                var ok = false;
                string? type = null;
                string? title = null;
                string? field = null;
                int? status = null;

                var data = new StringKeyDictionaryObject();

                for (var f = 0; f < reader.FieldCount; f++)
                {
                    // Ignore NULL values
                    if (await reader.IsDBNullAsync(f))
                        continue;

                    // Column name
                    var name = reader.GetName(f);

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

                    // Additional data
                    var addValue = await reader.GetFieldValueAsync<object>(f);
                    if (UtcDateTime && addValue is DateTime dt)
                    {
                        addValue = LocalizationUtils.SetUtcKind(dt);
                    }

                    data.Add(name, addValue);
                }

                var result = new ActionResult<T>
                {
                    Ok = ok,
                    Field = field,
                    Type = type, // Field first, type may contain field data
                    Title = title
                };

                if(data.Count > 0)
                {
                    // Type
                    var dataType = typeof(T);

                    if (dataType.IsValueType)
                    {
                        // Value type, read the returned data field value
                        result.Data = (T?)data.GetItem("Data");
                    }
                    else if (data is T d)
                    {
                        result.Data = d;
                    }
                    else
                    {
                        var parser = reader.GetRowParser<T>(dataType);
                        result.Data = parser(reader);
                    }
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// Create a success result
        /// 创建一个成功的操作结果
        /// </summary>
        /// <returns></returns>
        public static ActionResult<T> Success => new()
        {
            Ok = true
        };

        /// <summary>
        /// Data
        /// 数据
        /// </summary>
        public T? Data { get; set; }
    }
}
