﻿using MySql.Data.MySqlClient;

namespace com.etsoo.Database
{
    /// <summary>
    /// MySQL database
    /// MySQL 数据库
    /// </summary>
    public sealed class MySqlDatabase : CommonDatabase<MySqlConnection>
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
        public MySqlDatabase(string connectionString, bool snakeNaming = false) : base(connectionString, snakeNaming)
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
            return $"`{name}`";
        }

        /// <summary>
        /// Get exception result
        /// https://dev.mysql.com/doc/dev/connector-net/8.0/html/T_MySql_Data_MySqlClient_MySqlErrorCode.htm
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

            if (ex is MySqlException se)
            {
                return se.ErrorCode switch
                {
                    1037 or 1038 or 1041 => new DbExceptionResult(DbExceptionType.OutOfMemory, true),
                    1040 or 1044 or 1045 or 1046 => new DbExceptionResult(DbExceptionType.OutOfMemory, true),
                    _ => new DbExceptionResult(DbExceptionType.DataProcessingFailed, false)
                };
            }

            return new DbExceptionResult(DbExceptionType.DataProcessingFailed, false);
        }

        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        public override MySqlConnection NewConnection()
        {
            return new(ConnectionString);
        }
    }
}