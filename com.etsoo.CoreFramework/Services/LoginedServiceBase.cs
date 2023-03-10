﻿using com.etsoo.CoreFramework.Application;
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
    public abstract class LoginedServiceBase<C, R, A> : ServiceBase<C, R, A>
        where C : DbConnection
        where R : IRepoBase
        where A : ICoreApplication<C>
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        protected readonly IServiceUser User;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public LoginedServiceBase(A app, R repo, ILogger logger) : base(app, repo, logger)
        {
            if (repo.User == null)
            {
                throw new UnauthorizedAccessException("No Logined User");
            }

            User = repo.User;
        }
    }
}
