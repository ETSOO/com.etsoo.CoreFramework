using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application interface
    /// 核心程序接口
    /// </summary>
    /// <typeparam name="C">Generic database connection type</typeparam>
    public interface ICoreApplication<C> : ICoreApplicationBase where C : DbConnection
    {
        /// <summary>
        /// Database
        /// 数据库
        /// </summary>
        new IDatabase<C> DB { get; }

        /// <summary>
        /// Get API user data
        /// 获取API用户数据
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="userId">User id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        Task<IActionResult?> GetApiUserDataAsync(int organizationId, int userId, CancellationToken cancellationToken = default);
    }
}