using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Storage;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application base interface
    /// 核心程序基础接口
    /// </summary>
    public interface ICoreApplicationBase
    {
        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        IAppConfiguration Configuration { get; }

        /// <summary>
        /// Default Json serializer options
        /// 默认的Json序列化器选项
        /// </summary>
        JsonSerializerOptions DefaultJsonSerializerOptions { get; set; }

        /// <summary>
        /// Message queue
        /// 消息队列
        /// </summary>
        IMessageQueue? MessageQueue { get; }

        /// <summary>
        /// Storage
        /// 存储
        /// </summary>
        IStorage Storage { get; }

        /// <summary>
        /// Hash password
        /// 哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed bytes</returns>
        byte[] HashPassword(ReadOnlySpan<char> password);

        /// <summary>
        /// Hash password
        /// 哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed password</returns>
        Task<ReadOnlyMemory<char>> HashPasswordAsync(ReadOnlyMemory<char> password);
    }
}
