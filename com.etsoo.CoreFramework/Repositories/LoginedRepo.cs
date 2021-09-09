using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.User;
using Dapper;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Logined repository
    /// 用户已登录仓库
    /// </summary>
    /// <typeparam name="C">Generic connection type</typeparam>
    /// <typeparam name="T">Generic user id type</typeparam>
    /// <typeparam name="O">Generic organization id type</typeparam>
    public abstract class LoginedRepo<C, T, O> : RepoBase<C>, ILoginedRepo<T, O>
        where C : DbConnection
        where T : struct
        where O : struct
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        public ICurrentUser<T, O> User { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">App</param>
        /// <param name="user">User</param>
        public LoginedRepo(ICoreApplication<C> app, ICurrentUser<T, O> user) : base(app)
        {
            User = user;
        }

        /// <summary>
        /// Add system parameters
        /// 添加系统参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public virtual void AddSystemParameters(DynamicParameters parameters)
        {
            parameters.Add("CurrentUser", User.Id);
            parameters.Add("CurrentOrg", User.Organization);
        }
    }
}
