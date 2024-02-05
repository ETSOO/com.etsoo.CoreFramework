using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Max compatible entity service
    /// 最大兼容实体服务
    /// </summary>
    /// <typeparam name="R">Generic repository type</typeparam>
    /// <typeparam name="T">Generic id type</typeparam>
    /// <typeparam name="A">Generic application type</typeparam>
    /// <param name="app">App</param>
    /// <param name="repo">Repository</param>
    /// <param name="logger">Logger</param>
    public abstract class EntityServiceMax<R, T, A>(A app, R repo, ILogger logger) : EntityServiceBase<DbConnection, R, T, A>(app, repo, logger)
        where R : IEntityRepo<T>
        where T : struct
        where A : ICoreApplication<DbConnection>
    {
    }
}
