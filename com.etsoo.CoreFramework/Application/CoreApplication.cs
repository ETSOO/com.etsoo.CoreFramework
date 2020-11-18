using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Storage;
using com.etsoo.CoreFramework.Utils;
using System;
using System.Data;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application
    /// 核心程序
    /// </summary>
    public partial record CoreApplication : ICoreApplication
    {
        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        public Configuration Configuration { get; }

        /// <summary>
        /// Use database connection
        /// 使用数据库链接
        /// </summary>
        public Func<IDbConnection> UseDbConnection { get; }

        /// <summary>
        /// Use message queue
        /// 使用消息队列
        /// </summary>
        public Func<IMessageQueue> UseMessageQueue { get; }

        /// <summary>
        /// Use storage
        /// 使用存储
        /// </summary>
        public Func<IStorage> UseStorage { get; }

        /// <summary>
        /// Hash password
        /// 哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed password</returns>
        public string HashPassword(string password)
        {
            return CryptographyUtil.HMACSHA512ToBase64(password, Configuration.PrivateKey);
        }
    }
}
