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
    public class IntEntityService<C> : EntityServiceBase<C, IntEntityRepo<C>, int> where C : DbConnection
    {
        public IntEntityService(ICoreApplication<C> app, ICurrentUser user, IntEntityRepo<C> repo, ILogger logger) : base(app, user, repo, logger)
        {
        }
    }

    /// <summary>
    /// Long id entity service
    /// 长整形编号实体服务
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public class LongEntityService<C> : EntityServiceBase<C, LongEntityRepo<C>, long> where C : DbConnection
    {
        public LongEntityService(ICoreApplication<C> app, ICurrentUser user, LongEntityRepo<C> repo, ILogger logger) : base(app, user, repo, logger)
        {
        }
    }

    /// <summary>
    /// Guid id entity service
    /// 标识编号实体服务
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public class GuidEntityService<C> : EntityServiceBase<C, GuidEntityRepo<C>, Guid> where C : DbConnection
    {
        public GuidEntityService(ICoreApplication<C> app, ICurrentUser user, GuidEntityRepo<C> repo, ILogger logger) : base(app, user, repo, logger)
        {
        }
    }
}
