using com.etsoo.CoreFramework.Database;
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
        IDatabase<C> DB { get; init; }
    }
}
