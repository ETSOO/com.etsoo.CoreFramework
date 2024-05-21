using Npgsql;

namespace com.etsoo.Database
{
    /// <summary>
    /// Postgre (Npg) database
    /// Postgre (Npg) 数据库
    /// </summary>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="connectionString">Connection string</param>
    public sealed class PostgreDatabase(string connectionString) : CommonDatabase<NpgsqlConnection>(DatabaseName.PostgreSQL, connectionString)
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

            if (ex is NpgsqlException)
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
            field = PostgreUtils.ConvertIIFToCaseWhen(field);
            suffix = string.Empty;
        }

        /// <summary>
        /// Do JSON field suffix
        /// 处理JSON字段后缀
        /// </summary>
        /// <param name="field">Select field</param>
        /// <param name="suffix">Suffix</param>
        protected override void DoJsonFieldSuffix(ref string field, ref string suffix)
        {
            // When the field type is "JSON", no need to add suffix
            suffix = string.Empty;
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
            var items = mappings.SelectMany(m => new[] { $"'{m.Key}'", m.Value }).ToList();
            var command = $"json_build_object({string.Join(", ", items)})";

            if (isObject)
                return command;
            else
                return $"json_agg({command})";
        }

        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        public override NpgsqlConnection NewConnection()
        {
            return new(ConnectionString);
        }
    }
}
