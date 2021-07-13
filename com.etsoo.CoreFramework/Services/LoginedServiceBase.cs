using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.CoreFramework.User;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Logined service base for business logic
    /// 已登录业务逻辑的基础服务
    /// </summary>
    public abstract class LoginedServiceBase<C, R, T> : ServiceBase<C, R>
        where C : DbConnection
        where R : IRepoBase
        where T : struct
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        virtual protected ICurrentUser<T> User { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="user">Current user</param>
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        public LoginedServiceBase(ICoreApplication<C> app, ICurrentUser<T> user, R repo, ILogger logger) : base(app, repo, logger)
        {
            User = user;
        }
    }
}
