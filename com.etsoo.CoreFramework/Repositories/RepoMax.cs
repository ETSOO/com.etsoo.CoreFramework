using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.User;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Max compatible repository
    /// 最大兼容仓库
    /// </summary>
    /// <typeparam name="A">Generic application type</typeparam>
    public abstract class RepoMax<A> : RepoBase<DbConnection, A>
        where A : ICoreApplication<DbConnection>
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="flag">Flag</param>
        /// <param name="user">Current user</param>
        protected RepoMax(A app, string flag, IServiceUser? user = null) : base(app, flag, user)
        {
        }
    }
}