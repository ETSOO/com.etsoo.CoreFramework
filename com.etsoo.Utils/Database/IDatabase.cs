using System.Data.Common;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Databse interface
    /// 数据库接口
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Snake naming, like user_id
        /// 蛇形命名，如 user_id
        /// </summary>
        bool SnakeNaming { get; }

        /// <summary>
        /// Get exception result
        /// 获取数据库异常结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        IDbExceptionResult GetExceptionResult(Exception ex);

        /// <summary>
        /// Convert id list to parameter value
        /// 转换编号列表为参数值
        /// </summary>
        /// <typeparam name="T">Id generic</typeparam>
        /// <param name="ids">Id list</param>
        /// <returns>Parameter value</returns>
        object AsListParameter<T>(IEnumerable<T> ids) where T : struct;
    }

    /// <summary>
    /// Generic database interface
    /// 通用数据库接口
    /// </summary>
    /// <typeparam name="C">Generic connection type</typeparam>
    public interface IDatabase<C> : IDatabase where C : DbConnection
    {
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
