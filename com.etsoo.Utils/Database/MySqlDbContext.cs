using Microsoft.EntityFrameworkCore;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// MySQL EF Database Context
    /// MySQL EF 数据库上下文
    /// </summary>
    public class MySqlDbContext<M> : CommonDbContext<M> where M : class
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="snakeNaming">Is snake naming</param>
        public MySqlDbContext(string connectionString, bool snakeNaming = false) : base(connectionString, snakeNaming)
        {
        }

        /// <summary>
        /// Override OnConfiguring to setup
        /// 重写配置初始化
        /// </summary>
        /// <param name="optionsBuilder">Options builder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ConnectionString);
        }
    }
}
