using Microsoft.Data.Sqlite;

namespace com.etsoo.Database
{
    /// <summary>
    /// Sqlite database
    /// Sqlite 数据库
    /// </summary>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="connectionString">Connection string</param>
    public sealed class SqliteDatabase(string connectionString) : CommonDatabase<SqliteConnection>(DatabaseName.SQLite, connectionString)
    {
        /// <summary>
        /// Escape identifier
        /// 转义标识符
        /// </summary>
        /// <param name="name">Input name</param>
        /// <returns>Escaped name</returns>
        public override string EscapeIdentifier(string name)
        {
            return $"\"{name}\"";
        }

        /// <summary>
        /// Get exception result
        /// https://www.sqlite.org/rescode.html
        /// 获取数据库异常结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        public override IDbExceptionResult GetExceptionResult(Exception ex)
        {
            if (ex is OutOfMemoryException)
            {
                return new DbExceptionResult(DbExceptionType.OutOfMemory, true);
            }

            if (ex is SqliteException)
            {
                return new DbExceptionResult(DbExceptionType.ConnectionFailed, true);
            }

            return new DbExceptionResult(DbExceptionType.DataProcessingFailed, false);
        }

        /// <summary>
        /// Do boolean field suffix
        /// 处理逻辑字段后缀
        /// </summary>
        /// <param name="field">Select field</param>
        /// <param name="suffix">Suffix</param>
        protected override void DoBoolFieldSuffix(ref string field, ref string suffix)
        {
        }

        /// <summary>
        /// Do JSON field suffix
        /// 处理JSON字段后缀
        /// </summary>
        /// <param name="field">Select field</param>
        /// <param name="suffix">Suffix</param>
        protected override void DoJsonFieldSuffix(ref string field, ref string suffix)
        {
        }

        /// <summary>
        /// Join JSON fields
        /// 链接JSON字段
        /// </summary>
        /// <param name="mappings">Mapping fields</param>
        /// <param name="isObject">Is object node</param>
        /// <returns>Result</returns>
        public override string JoinJsonFields(Dictionary<string, string> mappings, bool isObject)
        {
            var items = mappings.SelectMany(m =>
            {
                var v = m.Value;
                if (v.EndsWith(BooleanSuffix))
                {
                    v = v[..^BooleanSuffix.Length];
                    if (v.StartsWith("IIF("))
                    {
                        v = v.Replace("TRUE", "'true'").Replace("FALSE", "'false'");
                        v = $"json({v})";
                    }
                    else
                    {
                        v = SqliteUtils.ToJsonBool(v);
                    }
                }
                else if (v.EndsWith(JsonSuffix))
                {
                    v = v[..^JsonSuffix.Length];
                    v = $"json({v})";
                }
                return new[] { $"'{m.Key}'", v };
            }).ToList();

            var command = $"json_object({string.Join(", ", items)})";

            if (isObject)
                return command;
            else
                return $"json_group_array({command})";
        }

        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        public override SqliteConnection NewConnection()
        {
            return new(ConnectionString);
        }
    }
}
