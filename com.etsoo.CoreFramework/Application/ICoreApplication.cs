using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Storage;
using System;
using System.Data;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application interface
    /// 核心程序接口
    /// </summary>
    public interface ICoreApplication
    {
        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        Configuration Configuration { get; }

        /// <summary>
        /// Use database connection
        /// 使用数据库链接
        /// </summary>
        Func<IDbConnection> UseDbConnection { get; }

        /// <summary>
        /// Use message queue
        /// 使用消息队列
        /// </summary>
        Func<IMessageQueue> UseMessageQueue { get; }

        /// <summary>
        /// Use storage
        /// 使用存储
        /// </summary>
        Func<IStorage> UseStorage { get; }

        /// <summary>
        /// Hash password
        /// 哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed password</returns>
        string HashPassword(string password);
    }
}
