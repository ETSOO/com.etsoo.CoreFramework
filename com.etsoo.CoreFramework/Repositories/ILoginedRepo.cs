using com.etsoo.CoreFramework.User;
using Dapper;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Logined repository interface
    /// 用户已登录仓库接口
    /// </summary>
    /// <typeparam name="T">Generic user id type</typeparam>
    /// <typeparam name="O">Generic organization id type</typeparam>
    public interface ILoginedRepo<T, O> : IRepoBase
        where T : struct
        where O : struct
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        ICurrentUser<T, O> User { get; }

        /// <summary>
        /// Add system parameters
        /// 添加系统参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        void AddSystemParameters(DynamicParameters parameters);
    }
}
