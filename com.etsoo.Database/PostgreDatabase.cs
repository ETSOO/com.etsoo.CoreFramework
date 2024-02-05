﻿using Npgsql;

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
    public sealed class PostgreDatabase(string connectionString) : CommonDatabase<NpgsqlConnection>(connectionString)
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
