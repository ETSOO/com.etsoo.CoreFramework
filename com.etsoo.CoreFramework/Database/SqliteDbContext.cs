using Microsoft.EntityFrameworkCore;

namespace com.etsoo.CoreFramework.Database
{
    /// <summary>
    /// Sqlite EF Database Context
    /// Sqlite EF 数据库上下文
    /// </summary>
    public class SqliteDbContext<M> : CommonDbContext<M> where M : class
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="snakeNaming">Is snake naming</param>
        public SqliteDbContext(string connectionString, bool snakeNaming = false) : base(connectionString, snakeNaming)
        {
        }

        /// <summary>
        /// Override OnConfiguring to setup
        /// 重写配置初始化
        /// </summary>
        /// <param name="optionsBuilder">Options builder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }
}
