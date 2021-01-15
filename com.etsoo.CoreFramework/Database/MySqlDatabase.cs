using MySql.Data.MySqlClient;

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
