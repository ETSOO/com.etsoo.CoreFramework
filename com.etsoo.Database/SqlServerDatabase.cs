using com.etsoo.Utils.Models;
using com.etsoo.Utils.String;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Text;

namespace com.etsoo.Database
{
    /// <summary>
    /// SQL Server database
    /// SQL Server 数据库
    /// </summary>
    public sealed class SqlServerDatabase : CommonDatabase<SqlConnection>
    {
        /// <summary>
        /// Support stored procedure or not
        /// 是否支持存储过程
        /// </summary>
        public override bool SupportStoredProcedure => true;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="snakeNaming">Is snake naming</param>
        public SqlServerDatabase(string connectionString, bool snakeNaming = false) : base(connectionString, snakeNaming)
        {
        }

        /// <summary>
        /// Escape identifier
        /// 转义标识符
        /// </summary>
        /// <param name="name">Input name</param>
        /// <returns>Escaped name</returns>
        public override string EscapeIdentifier(string name)
        {
            return $"[{name.Replace("[", "[[").Replace("]", "]]")}]";
        }

        /// <summary>
        /// Get exception result
        /// https://docs.microsoft.com/en-us/sql/relational-databases/errors-events/database-engine-events-and-errors?view=sql-server-ver15
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

            if (ex is SqlException se && se.Number < 100)
            {
                return new DbExceptionResult(DbExceptionType.ConnectionFailed, true);
            }

            return new DbExceptionResult(DbExceptionType.DataProcessingFailed, false);
        }

        /// <summary>
        /// Dictionary to parameter
        /// 字典转换为参数
        /// </summary>
        /// <typeparam name="K">Generic key type</typeparam>
        /// <typeparam name="V">Generic value type</typeparam>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyType">Key type</param>
        /// <param name="valueType">Value type</param>
        /// <param name="keyMaxLength">Char/byte key max length</param>
        /// <param name="valueMaxLength">Char/byte value max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        public override object DictionaryToParameter<K, V>(Dictionary<K, V> dic, DbType keyType, DbType valueType, long? keyMaxLength = null, long? valueMaxLength = null, Func<SqlDbType, SqlDbType, string>? tvpFunc = null)
            where K : struct
            where V : struct
        {
            return SqlServerUtils.DictionaryToTVP(dic, keyType, valueType, keyMaxLength, valueMaxLength, tvpFunc);
        }

        /// <summary>
        /// Guid items to parameters
        /// 转换Guid项目为TVP参数
        /// </summary>
        /// <param name="items">Items</param>
        /// <param name="maxLength">Item max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        public override object GuidItemsToParameter(IEnumerable<GuidItem> items, long? maxLength = null, Func<string>? tvpFunc = null)
        {
            return SqlServerUtils.GuidItemsToParameter(items, maxLength, tvpFunc);
        }

        /// <summary>
        /// List to parameter
        /// 列表转换为参数
        /// </summary>
        /// <typeparam name="T">Generic item type</typeparam>
        /// <param name="list">List</param>
        /// <param name="type">DbType</param>
        /// <param name="maxLength">Char/byte item max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        public override object ListToParameter(IEnumerable list, DbType type, long? maxLength = null, Func<SqlDbType, string>? tvpFunc = null)
        {
            return SqlServerUtils.ListToTVP(list, type, maxLength, tvpFunc);
        }

        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        public override SqlConnection NewConnection()
        {
            return new(ConnectionString);
        }

        /// <summary>
        /// Get query limit command
        /// 获取查询限制命令
        /// </summary>
        /// <param name="size">Lines to read</param>
        /// <param name="page">Current page</param>
        /// <returns>Query command</returns>
        public override string QueryLimit(uint size, uint page = 0)
        {
            var offset = page * size;
            return $" OFFSET {offset} ROWS FETCH NEXT {size} ROWS ONLY";
        }

        /// <summary>
        /// Get update command
        /// 获取更新命令
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="alias">Alias</param>
        /// <param name="fields">Update fields</param>
        /// <returns>Command</returns>
        public override StringBuilder GetUpdateCommand(string tableName, string alias, string fields)
        {
            tableName = EscapeIdentifier(tableName);

            var sql = new StringBuilder("UPDATE ");
            sql.Append(tableName);
            sql.Append(" SET ");
            sql.Append(fields);
            sql.Append(" FROM ");
            sql.Append(tableName);
            sql.Append(" AS ");
            sql.Append(alias);

            return sql;
        }

        /// <summary>
        /// Add Dapper parameter
        /// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/configuring-parameters-and-parameter-data-types?redirectedfrom=MSDN
        /// 添加 Dapper 参数
        /// </summary>
        /// <param name="parameters">Parameter collection</param>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="type">Value type</param>
        public override void AddParameter(DynamicParameters parameters, string name, object? value, DbType type)
        {
            if (value == null) return;

            switch (type)
            {
                case DbType.VarNumeric:
                    parameters.Add(name, StringUtils.TryParseObject<decimal>(value), DbType.Decimal);
                    break;
                case DbType.SByte:
                    parameters.Add(name, StringUtils.TryParseObject<short>(value), DbType.Int16);
                    break;
                case DbType.UInt16:
                    parameters.Add(name, StringUtils.TryParseObject<int>(value), DbType.Int32);
                    break;
                case DbType.UInt32:
                case DbType.UInt64:
                    parameters.Add(name, StringUtils.TryParseObject<long>(value), DbType.Int64);
                    break;
                default:
                    parameters.Add(name, value, type);
                    break;
            }
        }
    }
}
