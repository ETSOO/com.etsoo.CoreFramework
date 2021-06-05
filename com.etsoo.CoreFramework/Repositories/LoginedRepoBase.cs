using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Logined base repository
    /// 已登录基础仓库
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public abstract class LoginedRepoBase<C> : RepoBase<C>, ILoginedRepoBase where C : DbConnection
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        virtual protected ICurrentUser User { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        public LoginedRepoBase(ICoreApplication<C> app, ICurrentUser user) : base(app)
        {
            User = user;
        }

        /// <summary>
        /// Format parameters
        /// 格式化参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>Result</returns>
        override protected object? FormatParameters(object? parameters)
        {
            if (parameters == null)
                return null;

            if (parameters is IModelUserParameters p)
            {
                return p.AsParameters(App, User);
            }

            return base.FormatParameters(parameters);
        }
    }
}
