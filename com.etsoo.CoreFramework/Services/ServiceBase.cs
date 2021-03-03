using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.CoreFramework.User;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Service base for business logic
    /// 业务逻辑的基础服务
    /// </summary>
    public abstract class ServiceBase<C, R>
        where C : DbConnection
        where R : IRepositoryBase
    {
        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        protected ICoreApplication<C> App { get; init; }

        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        protected ICurrentUser User { get; init; }

        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        protected ILogger Logger { get; init; }

        /// <summary>
        /// Database repository
        /// 数据库仓库
        /// </summary>
        protected R Repo { get; init; }

        public ServiceBase(ICoreApplication<C> app, ICurrentUser user, R repo, ILogger logger)
        {
            this.App = app;
            this.User = user;
            this.Repo = repo;
            this.Logger = logger;
        }
    }
}
