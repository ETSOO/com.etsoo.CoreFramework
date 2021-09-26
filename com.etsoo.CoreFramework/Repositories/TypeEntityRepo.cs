using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.User;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Int id type entity repository
    /// 整形实体仓库
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public abstract class IntEntityRepo<C> : EntityRepo<C, int> where C : DbConnection
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="user">Current user</param>
        /// <param name="flag">Flag</param>
        /// <param name="procedureInitals">Procedure initials</param>
        public IntEntityRepo(ICoreApplication<C> app, ICurrentUser? user, string flag)
            : base(app, user, flag)
        {
        }
    }

    /// <summary>
    /// Guid id type entity repository
    /// 标识类型实体仓库
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public abstract class GuidEntityRepo<C> : EntityRepo<C, Guid> where C : DbConnection
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="user">Current user</param>
        /// <param name="flag">Flag</param>
        /// <param name="procedureInitals">Procedure initials</param>
        public GuidEntityRepo(ICoreApplication<C> app, ICurrentUser? user, string flag)
            : base(app, user, flag)
        {
        }
    }
}
