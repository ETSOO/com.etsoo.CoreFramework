using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.Database;
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
        /// Constructor
        /// 构造函数
        /// </summary>
        public CoreApplication(
            IAppConfiguration configuration,
            IDatabase<C> db
        )
        {
            // Json options
            DefaultJsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web) { IgnoreNullValues = true };

            // Update
            (
                Configuration,
                DB
            ) = (
                configuration,
                db
            );
        }

        /// <summary>
        /// Constructor with tuple
        /// 元组的构造函数
        /// </summary>
        /// <param name="init">Init tuple</param>
        public CoreApplication((IAppConfiguration configuration,
            IDatabase<C> db) init) : this(init.configuration, init.db)
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
            return CryptographyUtils.HMACSHA512(password, Configuration.PrivateKey.Span);
        }

        /// <summary>
        /// Async hash password
        /// 异步哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed password</returns>
        public async Task<ReadOnlyMemory<char>> HashPasswordAsync(ReadOnlyMemory<char> password)
        {
            return await CryptographyUtils.HMACSHA512ToBase64Async(password, Configuration.PrivateKey);
        }
    }
}
