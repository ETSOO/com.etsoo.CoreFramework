using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Database;
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
        where R : IRepoBase
    {
        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        virtual protected ICoreApplication<C> App { get; }

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
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        public ServiceBase(ICoreApplication<C> app, R repo, ILogger logger)
        {
            App = app;
            Repo = repo;
            Logger = logger;
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
        /// <param name="message">Message</param>
        /// <param name="critical">Is critical</param>
        protected void LogException(Exception ex, string message, bool critical = false)
        {
            if (critical)
                Logger.LogCritical(ex, message);
            else
                Logger.LogError(ex, message);
        }
    }
}
