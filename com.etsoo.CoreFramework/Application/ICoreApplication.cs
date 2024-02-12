using com.etsoo.Database;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application interface
    /// 核心程序接口
    /// </summary>
    /// <typeparam name="S">Generic configuration type</typeparam>
    /// <typeparam name="C">Generic database connection type</typeparam>
    public interface ICoreApplication<S, C> : ICoreApplicationBase
        where S : AppConfiguration
        where C : DbConnection
    {
        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        new S Configuration { get; }

        /// <summary>
        /// Database
        /// 数据库
        /// </summary>
        new IDatabase<C> DB { get; }
    }
}