using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Max compatabile service
    /// 最大兼容服务
    /// </summary>
    /// <typeparam name="R">Generic repository type</typeparam>
    /// <typeparam name="A">Generic application type</typeparam>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="app">Application</param>
    /// <param name="repo">Repository</param>
    /// <param name="logger">Logger</param>
    public abstract class ServiceMax<R, A>(A app, R repo, ILogger logger) : ServiceBase<DbConnection, R, A>(app, repo, logger)
        where R : IRepoBase
        where A : ICoreApplication<DbConnection>
    {
    }
}
