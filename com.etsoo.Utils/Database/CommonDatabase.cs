using com.etsoo.Utils.String;
using System.Data.Common;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Common database
    /// 通用数据库
    /// </summary>
    /// <typeparam name="C">Generic database connection type</typeparam>
    public abstract class CommonDatabase<C> : IDatabase<C> where C : DbConnection
    {
        /// <summary>
        /// Database connection string
        /// 数据库链接字符串
        /// </summary>
        protected readonly string ConnectionString;

        /// <summary>
        /// Snake naming, like user_id
        /// 蛇形命名，如 user_id
        /// </summary>
        public bool SnakeNaming { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="snakeNaming">Is snake naming</param>
        public CommonDatabase(string connectionString, bool snakeNaming = false)
        {
            ConnectionString = connectionString;
            SnakeNaming = snakeNaming;

            // Default settings
            Dapper.SqlMapper.Settings.UseSingleResultOptimization = true;

            // Database snake naming
            if (snakeNaming)
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            }
        }

        /// <summary>
        /// Escape identifier
        /// 转义标识符
        /// </summary>
        /// <param name="name">Input name</param>
        /// <returns>Escaped name</returns>
        public abstract string EscapeIdentifier(string name);

        /// <summary>
        /// Get exception result
        /// 获取数据库异常结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        public abstract IDbExceptionResult GetExceptionResult(Exception ex);

        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        public abstract C NewConnection();

        /// <summary>
        /// New database context
        /// 新数据库上下文
        /// </summary>
        /// <typeparam name="M">Generic context class</typeparam>
        /// <returns>Context</returns>
        public abstract CommonDbContext<M> NewDbContext<M>() where M : class;

        /// <summary>
        /// Convert id list to parameter value
        /// 转换编号列表为参数值
        /// </summary>
        /// <typeparam name="T">Id generic</typeparam>
        /// <param name="ids">Id list</param>
        /// <returns>Parameter value</returns>
        public virtual object AsListParameter<T>(IEnumerable<T> ids) where T : struct
        {
            return StringUtils.IEnumerableToString(ids);
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <param name="func">Callback function</param>
        /// <returns>Task</returns>
        public async Task WithConnection(Func<C, Task> func)
        {
            using var connection = NewConnection();

            await connection.OpenAsync();

            await func(connection);

            await connection.CloseAsync();
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <returns>Result</returns>
        public async Task<T> WithConnection<T>(Func<C, Task<T>> func)
        {
            using var connection = NewConnection();

            await connection.OpenAsync();

            return await func(connection);
        }
    }
}
