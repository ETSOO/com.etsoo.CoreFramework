﻿using com.etsoo.Utils.Models;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// SQL Server database
    /// SQL Server 数据库
    /// </summary>
    public sealed class SqlServerDatabase : CommonDatabase<SqlConnection>
    {
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
        /// New database context
        /// 新数据库上下文
        /// </summary>
        /// <typeparam name="M">Generic context class</typeparam>
        /// <returns>Context</returns>
        public override SqlServerDbContext<M> NewDbContext<M>() where M : class
        {
            return new(ConnectionString, SnakeNaming);
        }
    }
}
