using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.User;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Max compatible entity repository
    /// 最大兼容实体库
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    /// <typeparam name="A">Generic application type</typeparam>
    public abstract class EntityRepoMax<T, A> : EntityRepo<DbConnection, T, A>
        where T : struct
        where A : ICoreApplication<DbConnection>
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="flag">Flag</param>
        /// <param name="user">User</param>
        protected EntityRepoMax(A app, string flag, IServiceUser? user = null) : base(app, flag, user)
        {

        }
    }
}
