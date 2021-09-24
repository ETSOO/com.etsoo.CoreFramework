using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.CoreFramework.User;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Logined service base for business logic
    /// 已登录业务逻辑的基础服务
    /// </summary>
    /// <typeparam name="C">Generic connection type</typeparam>
    /// <typeparam name="R">Generic repository type</typeparam>
    /// <typeparam name="T">Generic user id type</typeparam>
    /// <typeparam name="O">Generic organization id type</typeparam>
    public abstract class LoginedServiceBase<C, R, T, O> : ServiceBase<C, R>
        where C : DbConnection
        where R : IRepoBase
        where T : struct
        where O : struct
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        virtual protected ICurrentUser<T, O> User { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="user">Current user</param>
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        public LoginedServiceBase(ICoreApplication<C> app, ICurrentUser<T, O> user, R repo, ILogger logger) : base(app, repo, logger)
        {
            User = user;
        }
    }
}
