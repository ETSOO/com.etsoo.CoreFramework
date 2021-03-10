using com.etsoo.CoreFramework.Database;
using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Storage;
using com.etsoo.Utils.Crypto;
using System;
using System.Data.Common;
using System.Text.Json;
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
        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        public virtual IAppConfiguration Configuration { get; }

        /// <summary>
        /// Default Json serializer options
        /// 默认的Json序列化器选项
        /// </summary>
        public JsonSerializerOptions DefaultJsonSerializerOptions { get; set; }

        /// <summary>
        /// Database
        /// 数据库
        /// </summary>
        public virtual IDatabase<C> DB { get; }

        /// <summary>
        /// Message queue
        /// 消息队列
        /// </summary>
        public virtual IMessageQueue? MessageQueue { get; }

        /// <summary>
        /// Storage
        /// 存储
        /// </summary>
        public virtual IStorage Storage { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public CoreApplication(
            IAppConfiguration configuration,
            IDatabase<C> db,
            IMessageQueue? messageQueue = null,
            IStorage? storage = null 
        )
        {
            // Default storage
            storage ??= new LocalStorage();

            // Json options
            DefaultJsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web) { IgnoreNullValues = true };

            // Update
            (
                Configuration,
                DB,
                MessageQueue,
                Storage
            ) = (
                configuration,
                db,
                messageQueue,
                storage
            );
        }

        /// <summary>
        /// Constructor with tuple
        /// 元组的构造函数
        /// </summary>
        /// <param name="init">Init tuple</param>
        public CoreApplication((IAppConfiguration configuration,
            IDatabase<C> db,
            IMessageQueue? messageQueue,
            IStorage? storage) init) : this(init.configuration, init.db, init.messageQueue, init.storage)
        {

        }

        /// <summary>
        /// Hash password
        /// 哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed bytes</returns>
        public byte[] HashPassword(ReadOnlySpan<char> password)
        {
            return CryptographyUtil.HMACSHA512(password, Configuration.PrivateKey.Span);
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
