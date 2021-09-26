using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.CoreFramework.User;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Int id entity service
    /// 整形编号实体服务
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public class IntEntityService<C, R> : EntityServiceBase<C, R, int>
        where C : DbConnection
        where R: IntEntityRepo<C>
    {
        public IntEntityService(ICoreApplication<C> app, ICurrentUser user, R repo, ILogger logger) : base(app, user, repo, logger)
        {
        }
    }

    /// <summary>
    /// Long id entity service
    /// 长整形编号实体服务
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public class LongEntityService<C, R> : EntityServiceBase<C, R, long>
        where C : DbConnection
        where R : LongEntityRepo<C>
    {
        public LongEntityService(ICoreApplication<C> app, ICurrentUser user, R repo, ILogger logger) : base(app, user, repo, logger)
        {
        }
    }

    /// <summary>
    /// Guid id entity service
    /// 标识编号实体服务
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public class GuidEntityService<C, R> : EntityServiceBase<C, R, Guid>
        where C : DbConnection
        where R : GuidEntityRepo<C>
    {
        public GuidEntityService(ICoreApplication<C> app, ICurrentUser user, R repo, ILogger logger) : base(app, user, repo, logger)
        {
        }
    }
}
