using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Storage;
using System;
using System.Data;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application builder
    /// 核心程序生成器
    /// </summary>
    public partial record CoreApplication
    {
        /// <summary>
        /// Private constructor to prevent direct initialization
        /// 私有化构造函数防止直接初始化
        /// </summary>
        private CoreApplication(
            Configuration configuration,
            Func<IDbConnection> useDbConnection,
            Func<IMessageQueue> useMessageQueue,
            Func<IStorage> useStorage
        ) =>
            (
                Configuration,
                UseDbConnection,
                UseMessageQueue,
                UseStorage
            ) = (
                configuration,
                useDbConnection,
                useMessageQueue,
                useStorage
            );

        /// <summary>
        /// 静态生成器对象
        /// </summary>
        public static CoreApplicationBuilder Builder => new();

        /// <summary>
        /// Core application builder class
        /// 核心程序生成器类
        /// </summary>
        public class CoreApplicationBuilder
        {
            private Configuration _configuration;

            /// <summary>
            /// Set application configuration, required
            /// 设置程序配置，必填
            /// </summary>
            /// <param name="configuration">Application configuration</param>
            /// <returns>Builder</returns>
            public CoreApplicationBuilder Configuration(Configuration configuration)
            {
                _configuration = configuration;
                return this;
            }

            private Func<IDbConnection> _useDbConnection;

            /// <summary>
            /// Use database connection, required
            /// 使用数据库链接，必填
            /// </summary>
            /// <param name="useDbConnection">Use database connection</param>
            /// <returns>Builder</returns>
            public CoreApplicationBuilder UseDbConnection(Func<IDbConnection> useDbConnection)
            {
                _useDbConnection = useDbConnection;
                return this;
            }

            private Func<IMessageQueue> _useMessageQueue;

            /// <summary>
            /// Use message queue
            /// 使用消息队列
            /// </summary>
            /// <param name="useMessageQueue">Use message queue</param>
            /// <returns>Builder</returns>
            public CoreApplicationBuilder UseMessageQueue(Func<IMessageQueue> useMessageQueue)
            {
                _useMessageQueue = useMessageQueue;
                return this;
            }

            private Func<IStorage> _useStorage;

            /// <summary>
            /// Use storage, default is Local storage
            /// 使用存储，默认为本地存储
            /// </summary>
            /// <param name="useStorage">Use storage</param>
            /// <returns>Builder</returns>
            public CoreApplicationBuilder UseStorage(Func<IStorage> useStorage)
            {
                _useStorage = useStorage;
                return this;
            }

            /// <summary>
            /// Build
            /// 构造
            /// </summary>
            /// <returns>Core application</returns>
            public CoreApplication Build()
            {
                // Validation
                if (_configuration == null)
                {
                    throw new ArgumentNullException("Configuration");
                }

                if (_useDbConnection == null)
                {
                    throw new ArgumentNullException("UseDbConnection");
                }

                // Return an initialization
                return new(
                    _configuration,
                    _useDbConnection,
                    _useMessageQueue,
                    _useStorage ?? (() => { return new LocalStorage(); })
                );
            }
        }
    }
}
