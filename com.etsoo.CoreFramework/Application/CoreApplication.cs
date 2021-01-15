using com.etsoo.CoreFramework.Database;
using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Storage;
using com.etsoo.Utils.Crypto;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application
    /// 核心程序
    /// </summary>
    /// <typeparam name="C">Generic database connection type</typeparam>
    public record CoreApplication<C> : ICoreApplication<C> where C : DbConnection
    {
        // Create logger
        private static ILogger CreateLogger()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .Build();

            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        public virtual IConfiguration Configuration { get; init; }

        /// <summary>
        /// Database
        /// 数据库
        /// </summary>
        public virtual IDatabase<C> DB { get; init; }

        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        public ILogger Logger { get; init; }

        /// <summary>
        /// Message queue
        /// 消息队列
        /// </summary>
        public virtual IMessageQueue? MessageQueue { get; init; }

        /// <summary>
        /// Storage
        /// 存储
        /// </summary>
        public virtual IStorage Storage { get; init; }

        /// <summary>
        /// Protected constructor to prevent direct initialization
        /// 受保护的构造函数防止直接初始化
        /// </summary>
        public CoreApplication(
            IConfiguration configuration,
            IDatabase<C> db,
            ILogger? logger = null,
            IMessageQueue? messageQueue = null,
            IStorage? storage = null 
        )
        {
            // Default logger
            logger ??= CreateLogger();

            // Default storage
            storage ??= new LocalStorage();

            // Update
            (
                Configuration,
                DB,
                Logger,
                MessageQueue,
                Storage
            ) = (
                configuration,
                db,
                logger,
                messageQueue,
                storage
            );
        }

        /// <summary>
        /// Async hash password
        /// 异步哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed password</returns>
        public async Task<ReadOnlyMemory<char>> HashPasswordAsync(ReadOnlyMemory<char> password)
        {
            return await CryptographyUtil.HMACSHA512ToBase64Async(password, Configuration.PrivateKey);
        }
    }
}
