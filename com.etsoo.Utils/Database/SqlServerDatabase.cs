using Dapper;
using Microsoft.Data.SqlClient;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// SQL Server database
    /// SQL Server 数据库
    /// </summary>
    public sealed class SqlServerDatabase : CommonDatabase<SqlConnection>
    {
        /// <summary>
        /// Database name
        /// 数据库名称
        /// </summary>
        public override string Name => "SQLSERVER";

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

        /// <summary>
        /// Convert id list to parameter value
        /// 转换编号列表为参数值
        /// </summary>
        /// <typeparam name="T">Id generic</typeparam>
        /// <param name="ids">Id list</param>
        /// <returns>Parameter value</returns>
        public override object AsListParameter<T>(IEnumerable<T> ids)
        {
            return SqlServerUtils.ListToTVP(ids);
        }
    }
}
