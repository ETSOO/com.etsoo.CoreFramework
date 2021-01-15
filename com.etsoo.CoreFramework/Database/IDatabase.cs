using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Database
{
    /// <summary>
    /// Database interface
    /// 数据库接口
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public interface IDatabase<C> where C : DbConnection
    {
        /// <summary>
        /// Snake naming, like user_id
        /// 蛇形命名，如 user_id
        /// </summary>
        bool SnakeNaming { get; }

        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        C NewConnection();

        /// <summary>
        /// New database context
        /// 新数据库上下文
        /// </summary>
        /// <typeparam name="M">Generic context class</typeparam>
        /// <returns>Context</returns>
        CommonDbContext<M> NewDbContext<M>() where M : class;

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <param name="func">Callback function</param>
        /// <returns>Task</returns>
        Task WithConnection(Func<C, Task> func);

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <returns>Result</returns>
        Task<T> WithConnection<T>(Func<C, Task<T>> func);
    }
}
