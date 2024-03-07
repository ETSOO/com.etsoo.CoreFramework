using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.User;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// User authorized service base for business logic
    /// 已授权用户业务逻辑服务基类
    /// </summary>
    /// <typeparam name="S">Generic configuration type</typeparam>
    /// <typeparam name="C">Generic connection type</typeparam>
    /// <typeparam name="A">Generic application type</typeparam>
    /// <typeparam name="U">Generic current user type</typeparam>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="app">Application</param>
    /// <param name="user">Current user</param>
    /// <param name="flag">Flag</param>
    /// <param name="logger">Logger</param>
    public abstract class UserServiceBase<S, C, A, U>(A app, U user, string flag, ILogger logger)
        : ServiceBase<S, C, A, U>(app, user, flag, logger), IServiceBase
        where S : AppConfiguration
        where C : DbConnection
        where A : ICoreApplication<S, C>
        where U : IServiceUser
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        protected override U User { get; } = user;
    }
}
