using MySql.Data.MySqlClient;
using System;

namespace com.etsoo.CoreFramework.Database
{
    /// <summary>
    /// MySQL database
    /// MySQL 数据库
    /// </summary>
    public sealed class MySqlDatabase : CommonDatabase<MySqlConnection>
    {
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

        /// <summary>
        /// New database context
        /// 新数据库上下文
        /// </summary>
        /// <typeparam name="M">Generic context class</typeparam>
        /// <returns>Context</returns>
        public override MySqlDbContext<M> NewDbContext<M>() where M : class
        {
            return new(ConnectionString, SnakeNaming);
        }
    }
}
