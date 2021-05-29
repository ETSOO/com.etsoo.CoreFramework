using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.CoreFramework.User;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Database;
using Microsoft.Extensions.Logging;
using System;
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
        virtual protected ICoreApplication<C> App { get; }

        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        virtual protected ICurrentUser? User { get; }

        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Database repository
        /// 数据库仓库
        /// </summary>
        protected R Repo { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="user">Current user</param>
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        public ServiceBase(ICoreApplication<C> app, ICurrentUser? user, R repo, ILogger logger)
        {
            this.App = app;
            this.User = user;
            this.Repo = repo;
            this.Logger = logger;
        }

        /// <summary>
        /// Log exception and return simple user result
        /// 登记异常结果日志，并返回简介的用户结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        protected IActionResult LogException(Exception ex)
        {
            // Get the Db connection failure result
            var exResult = App.DB.GetExceptionResult(ex);

            // Transform
            var result = exResult.Type switch
            {
                DbExceptionType.OutOfMemory => ApplicationErrors.OutOfMemory.AsResult(),
                DbExceptionType.ConnectionFailed => ApplicationErrors.DbConnectionFailed.AsResult(),
                _ => ApplicationErrors.DataProcessingFailed.AsResult()
            };

            // Log the exception
            LogException(ex, result.Title!, exResult.Critical);

            // Return
            return result;
        }

        /// <summary>
        /// Log exception
        /// 登记异常日志
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="title">Title</param>
        /// <param name="critical">Is critical</param>
        protected void LogException(Exception ex, string title, bool critical = false)
        {
            if (critical)
                Logger.LogCritical(ex, title);
            else
                Logger.LogError(ex, title);
        }
    }
}
