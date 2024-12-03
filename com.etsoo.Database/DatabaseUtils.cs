using com.etsoo.Utils.Models;
using com.etsoo.Utils.Serialization;
using Dapper;
using System.Collections;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace com.etsoo.Database
{
    /// <summary>
    /// Database util
    /// 数据库工具
    /// </summary>
    public static partial class DatabaseUtils
    {
        /// <summary>
        /// Is the order by fields valid
        /// 排序字段是否有效
        /// </summary>
        /// <param name="data">Pagination data</param>
        /// <returns>Result</returns>
        public static bool IsOrderByValid(this QueryPagingData? data)
        {
            if (data == null || data.OrderBy == null) return true;
            return !data.OrderBy.Any(o => !IsValidField(o.Field));
        }

        /// <summary>
        /// Get order command
        /// 获取排序命令
        /// </summary>
        /// <param name="data">Query paging data</param>
        /// <param name="db">Database</param>
        /// <returns>Command</returns>
        public static string? GetOrderCommand(this QueryPagingData? data, IDatabase? db = null)
        {
            var orderBy = data?.OrderBy;
            if (orderBy?.Any() is null or false) return null;

            var result = string.Join(", ", orderBy.Select(o => IsValidField(o.Field) ? $"{(db == null ? o.Field : db.EscapePart(o.Field))} {(o.Desc ? "DESC" : "ASC")}" : null).Where(o => o != null));

            return $"ORDER BY {result}";
        }

        /// <summary>
        /// Get keyset conditions
        /// 获取键集条件
        /// </summary>
        /// <param name="data">Pagination data</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="conditions">Conditions</param>
        public static void GetKeysetConditions(this QueryPagingData? data, DbParameters parameters, List<string> conditions)
        {
            if (data == null || data.Keysets?.Any() is null or false) return;

            // Reset the current page
            data.CurrentPage = null;

            var condition = new StringBuilder();
            condition.Append('(');

            var len = data.Keysets.Count();
            var fields = new List<string>();
            for (var k = 0; k < len; k++)
            {
                var keyset = data.Keysets.ElementAt(k);
                var orderBy = (data.OrderBy?.ElementAtOrDefault(k)) ?? throw new Exception($"Field in {k} is not found");

                var field = orderBy.Field;
                parameters.Add(field, keyset);

                var main = $"{field} {(orderBy.Desc ? "<" : ">")} @{field}";

                if (k == 0)
                {
                    condition.Append(main);
                }
                else
                {
                    condition.Append(" OR (");

                    condition.Append(string.Join(" AND ", fields.Select(f => $"{f} = @{f}")));
                    condition.Append(" AND ");
                    condition.Append(main);

                    condition.Append(')');
                }

                if (orderBy.Unique) break;

                fields.Add(field);
            }

            condition.Append(')');

            conditions.Add(condition.ToString());
        }

        /// <summary>
        /// Is the string only ansii characters
        /// 字符串是唯一的ASCII字符吗
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        public static bool IsAnsi(this string input)
        {
            return !input.Any(c => c > 127);
        }

        /// <summary>
        /// Is the field valid
        /// 字段是否有效
        /// </summary>
        /// <param name="field">Field</param>
        /// <returns>Result</returns>
        public static bool IsValidField(string field)
        {
            return OrderFieldRegex().IsMatch(field);
        }

        /// <summary>
        /// To Dapper DbString
        /// 转化为 Dapper DbString
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="isAnsi">Is non-unicode</param>
        /// <param name="length">Length</param>
        /// <param name="fixedLength">Fixed length</param>
        /// <returns>DbString</returns>
        public static DbString? ToDbString(this string input, bool? isAnsi = null, int? length = null, bool? fixedLength = null)
        {
            // Remove unnecessary empty
            input = input.Trim();
            if (input == string.Empty) return null;

            return input.ToDbStringSafe(isAnsi, length, fixedLength);
        }

        /// <summary>
        /// To Dapper DbString without empty by any chance.
        /// 确保没有任何空白的转化为 Dapper DbString
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="isAnsi">Is non-unicode</param>
        /// <param name="length">Length</param>
        /// <param name="fixedLength">Fixed length</param>
        /// <returns>DbString</returns>
        public static DbString ToDbStringSafe(this string input, bool? isAnsi = null, int? length = null, bool? fixedLength = null)
        {
            return new DbString { IsAnsi = isAnsi ?? input.IsAnsi(), IsFixedLength = fixedLength.GetValueOrDefault(), Value = input, Length = length ?? input.Length };
        }

        /// <summary>
        /// Format parameters to Dapper parameters
        /// 将参数格式化为 Dapper 参数
        /// </summary>
        /// <param name="parameters">Dynamic parameters</param>
        /// <returns>Result</returns>
        public static IDbParameters? FormatParameters(object? parameters)
        {
            if (parameters == null) return null;

            if (parameters is IDbParameters dp)
            {
                return dp;
            }

            if (parameters is IAutoParameters ap)
            {
                return ap.AsParameters();
            }

            return new DbParameters(parameters);
        }

        /// <summary>
        /// Determine whether it is a safe SQL statement in order to avoid SQL injection
        /// 判断是否为安全SQL语句，防止注水攻击
        /// <remarks>确保不包括执行命令、换行符、分号(;)、米字符(*)、注释符(--)、引号('")、等于号(=)、系统命令(sp_, xp_)</remarks>
        /// </summary>
        /// <param name="input">要验证的SQL语句</param>
        /// <param name="maxLength">最大长度，默认值为1024</param>
        /// <returns>是否安全</returns>
        public static bool IsSafeSQLPart(string? input, int maxLength = 1024)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > maxLength) return false;

            if (!MyRegex().IsMatch(input) && !MyRegex1().IsMatch(input))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Convert Type to DbType
        /// 转化 Type 到 DbType
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>DbType</returns>
        public static DbType? TypeToDbType(Type type)
        {
            // Considerate nullable type
            var nullableType = Nullable.GetUnderlyingType(type) ?? type;

            // Return
            return TypeToDbType(nullableType.Name);
        }

        /// <summary>
        /// To SQL IN clause
        /// 转化为SQL IN子句
        /// </summary>
        /// <typeparam name="T">Generic item type</typeparam>
        /// <param name="items">Items</param>
        /// <returns>Result</returns>
        public static string ToInClause<T>(IEnumerable<T> items) where T : struct
        {
            return $"({string.Join(',', items)})";
        }

        /// <summary>
        /// String array to SQL IN string clause
        /// 字符串数组转化为SQL IN字符串子句
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Results</returns>
        public static string ToStringInClause(IEnumerable<string> items)
        {
            var safeItems = items.Select(i => i.Replace("'", "''"));
            return $"('{string.Join("','", safeItems)}')";
        }

        /// <summary>
        /// Array to SQL IN string items clause
        /// 数组转化为SQL IN字符串子句
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Results</returns>
        public static string ToStringInClause<T>(IEnumerable<T> items) where T : struct
        {
            return $"('{string.Join("','", items)}')";
        }

        /// <summary>
        /// Convert Type to DbType from name
        /// 从类型名称转化 Type 到 DbType
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <returns>DbType</returns>
        public static DbType? TypeToDbType(string typeName)
        {
            // Name not match case
            if (typeName.Equals("TimeSpan", StringComparison.OrdinalIgnoreCase))
            {
                return DbType.Time;
            }

            // Try to parse with name
            if (Enum.TryParse<DbType>(typeName, true, out var dbType))
            {
                return dbType;
            }

            // Default
            return null;
        }

        /// <summary>
        /// Dictionary to JSON string
        /// 字典转化为JSON字符串
        /// </summary>
        /// <typeparam name="K">Generic key type</typeparam>
        /// <typeparam name="V">Generic value type</typeparam>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyType">Key type</param>
        /// <param name="valueType">Value type</param>
        /// <returns>Result</returns>
        public static string DictionaryToJsonString<K, V>(Dictionary<K, V> dic, DbType keyType, DbType valueType)
            where K : notnull
        {
            JsonWriterOptions writerOptions = new() { Indented = false, SkipValidation = true };

            using MemoryStream stream = new();
            using Utf8JsonWriter writer = new(stream, writerOptions);

            writer.WriteStartArray();

            foreach (var item in dic)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("key");
                WriteJsonItem(writer, item.Key, keyType);

                writer.WritePropertyName("value");
                WriteJsonItem(writer, item.Value, valueType);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Guid items to JSON string
        /// Guid 列表项转化为JSON字符串
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Result</returns>
        public static string GuidItemsToJsonString(IEnumerable<GuidItem> items)
        {
            JsonWriterOptions writerOptions = new() { Indented = false, SkipValidation = true };

            using MemoryStream stream = new();
            using Utf8JsonWriter writer = new(stream, writerOptions);

            writer.WriteStartArray();

            foreach (var item in items)
            {
                writer.WriteStartObject();

                writer.WriteString("id", item.Id);
                writer.WriteString("label", item.Label);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// List items to JSON string
        /// 列表项转化为JSON字符串
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="type">Item type</param>
        /// <returns>Result</returns>
        public static string ListItemsToJsonString(IEnumerable list, DbType type)
        {
            JsonWriterOptions writerOptions = new() { Indented = false, SkipValidation = true };

            using MemoryStream stream = new();
            using Utf8JsonWriter writer = new(stream, writerOptions);

            writer.WriteStartArray();

            foreach (var item in list)
            {
                WriteJsonItem(writer, item, type);
            }

            writer.WriteEndArray();

            writer.Flush();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Split field and alias
        /// 拆分字段和别名
        /// </summary>
        /// <param name="field">Raw field</param>
        /// <returns>Result</returns>
        public static (string field, string? alias) SplitField(string field)
        {
            var pos = field.LastIndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
            if (pos > 0)
            {
                return (field[..pos].Trim(), field[(pos + 4)..].Trim());
            }
            else
            {
                return (field.Trim(), null);
            }
        }

        private static void WriteJsonItem(Utf8JsonWriter writer, object? item, DbType type)
        {
            if (item == null)
            {
                writer.WriteNullValue();
                return;
            }

            switch (type)
            {
                case DbType.Boolean:
                    writer.WriteBooleanValue((bool)item);
                    break;
                case DbType.Int32:
                    writer.WriteNumberValue((int)item);
                    break;
                case DbType.Int64:
                    writer.WriteNumberValue((long)item);
                    break;
                case DbType.Double:
                    writer.WriteNumberValue((double)item);
                    break;
                case DbType.Single:
                    writer.WriteNumberValue((float)item);
                    break;
                case DbType.Decimal:
                case DbType.Currency:
                    writer.WriteNumberValue((decimal)item);
                    break;
                case DbType.Byte:
                    writer.WriteNumberValue((byte)item);
                    break;
                case DbType.SByte:
                    writer.WriteNumberValue((sbyte)item);
                    break;
                case DbType.Int16:
                    writer.WriteNumberValue((short)item);
                    break;
                case DbType.UInt16:
                    writer.WriteNumberValue((ushort)item);
                    break;
                case DbType.UInt32:
                    writer.WriteNumberValue((uint)item);
                    break;
                case DbType.UInt64:
                    writer.WriteNumberValue((ulong)item);
                    break;
                case DbType.VarNumeric:
                    writer.WriteNumberValue((long)item);
                    break;
                case DbType.Binary:
                    writer.WriteBase64StringValue((byte[])item);
                    break;
                case DbType.Object:
                    Utils.SharedUtils.JsonSerializeAsync(writer, item, CommonJsonSerializerContext.Default.Object).Wait();
                    break;
                default:
                    writer.WriteStringValue(item.ToString());
                    break;
            }
        }

        [GeneratedRegex("(\\n|\\r\\n?|;|\\*|--|'|\"|=|sp_|xp_)")]
        private static partial Regex MyRegex();

        [GeneratedRegex("(^|\\s+)(exec|execute|select|insert|update|delete|union|join|create|alter|drop|rename|truncate|backup|restore)\\s", RegexOptions.IgnoreCase)]
        private static partial Regex MyRegex1();

        [GeneratedRegex("^[0-9a-zA-Z_\\.]+$")]
        private static partial Regex OrderFieldRegex();
    }
}
