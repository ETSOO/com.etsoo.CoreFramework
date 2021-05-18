using com.etsoo.Utils.String;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Common Abstract EF Database Context
    /// 通用EF数据库上下文抽象类
    /// </summary>
    public abstract class CommonDbContext<M> : DbContext where M : class
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
        /// Entities
        /// 实体集合
        /// </summary>
        public DbSet<M>? Entities { get; init; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="snakeNaming">Is snake naming</param>
        public CommonDbContext(string connectionString, bool snakeNaming = false) => (ConnectionString, SnakeNaming) = (connectionString, snakeNaming);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if(SnakeNaming)
            {
                foreach (var entity in modelBuilder.Model.GetEntityTypes())
                {
                    // Replace the table name
                    entity.SetTableName(entity.GetTableName().ToSnakeCase().ToString());

                    // Stored table identifier
                    var tableIdentifier = StoreObjectIdentifier.Create(entity, StoreObjectType.Table).GetValueOrDefault();

                    // Replace column names            
                    foreach (var property in entity.GetProperties())
                    {
                        property.SetColumnName(property.GetColumnName(tableIdentifier).ToSnakeCase().ToString());
                    }

                    foreach (var key in entity.GetKeys())
                    {
                        key.SetName(key.GetName().ToSnakeCase().ToString());
                    }

                    foreach (var key in entity.GetForeignKeys())
                    {
                        key.SetConstraintName(key.GetConstraintName().ToSnakeCase().ToString());
                    }
                }
            }
        }
    }
}