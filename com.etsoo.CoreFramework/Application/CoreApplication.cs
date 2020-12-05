using com.etsoo.CoreFramework.ActionResult;
using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Storage;
using com.etsoo.CoreFramework.Utils;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application
    /// 核心程序
    /// </summary>
    public record CoreApplication
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
        public virtual IConfiguration Configuration { get; }

        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Use database connection
        /// 使用数据库链接
        /// </summary>
        public virtual Func<SqlConnection> UseDbConnection { get; }

        /// <summary>
        /// Use message queue
        /// 使用消息队列
        /// </summary>
        public virtual Func<IMessageQueue>? UseMessageQueue { get; }

        /// <summary>
        /// Use storage
        /// 使用存储
        /// </summary>
        public virtual Func<IStorage> UseStorage { get; }

        /// <summary>
        /// Protected constructor to prevent direct initialization
        /// 受保护的构造函数防止直接初始化
        /// </summary>
        public CoreApplication(
            IConfiguration configuration,
            Func<SqlConnection> useDbConnection,
            ILogger? logger = null,
            Func<IMessageQueue>? useMessageQueue = null,
            Func<IStorage>? useStorage = null 
        )
        {
            // Default logger
            logger ??= CreateLogger();

            // Default storage
            useStorage ??= (() => new LocalStorage());

            // Update
            (
                Configuration,
                UseDbConnection,
                Logger,
                UseMessageQueue,
                UseStorage
            ) = (
                configuration,
                useDbConnection,
                logger,
                useMessageQueue,
                useStorage
            );
        }

        /// <summary>
        /// Async hash password
        /// 异步哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed password</returns>
        public async Task<string> HashPasswordAsync(string password)
        {
            return await CryptographyUtil.HMACSHA512ToBase64Async(password, Configuration.PrivateKey);
        }

        /// <summary>
        /// Query object list
        /// 查询对象列表
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Object list</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
        {
            using var connection = this.UseDbConnection();
            return await connection.QueryAsync<T>(command);
        }

        /// <summary>
        /// Query Json string
        /// 查询Json字符串
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Json string</returns>
        public async Task<string> QueryJsonAsync(CommandDefinition command)
        {
            using var connection = this.UseDbConnection();
            return await connection.QuerySingleAsync<string>(command);
        }

        /// <summary>
        /// Query single object
        /// 查询单个对象
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Object</returns>
        public async Task<T?> QueryObjectAsync<T>(CommandDefinition command)
        {
            using var connection = this.UseDbConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(command);
        }

        /// <summary>
        /// Query action result with empty return data
        /// 查询无返回数据的操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<IActionResultNoData> QueryResultAsync(CommandDefinition command)
        {
            var result = await QueryObjectAsync<ActionResultNoData>(command);
            result ??= ApplicationErrors.NoActionResult;
            return result;
        }

        /// <summary>
        /// Query action result with success data, split on column '@'
        /// 查询成功数据的操作结果，通过列名 @ 分割
        /// </summary>
        /// <typeparam name="T">Generic success data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<IActionResultSuccessData<T>> QueryResultAsync<T>(CommandDefinition command)
        {
            using var connection = this.UseDbConnection();
            using var multiple = await connection.QueryMultipleAsync(command);
            var result = multiple.Read<ActionResultSuccessData<T>, T, ActionResultSuccessData<T>>((result, data) => {
                if (result.Success && data != null)
                {
                    // It's possible to initialize data with primitive types, if does, data != null would omit
                    result.Data = data;
                }
                return result;
            }, splitOn: "@").FirstOrDefault();

            if (result == null)
            {
                result = (ApplicationErrors.NoActionResult as ActionResultSuccessData<T>)!;
            }

            return result;
        }

        /// <summary>
        /// Query action result, success data split on column '@', failure data split on column '!'
        /// 查询成操作结果，成功数据通过列名 @ 分割，失败数据通过 ! 分割
        /// </summary>
        /// <typeparam name="T">Generic success data type</typeparam>
        /// <typeparam name="TI">Generic success data items type</typeparam>
        /// <typeparam name="F">Generic failure data type</typeparam>
        /// <typeparam name="FI">Generic failure data items type</typeparam>
        /// <param name="command"></param>
        /// <returns>Result</returns>
        public async Task<IActionResultItems<T, TI, F, FI>> QueryResultAsync<T, TI, F, FI>(CommandDefinition command)
            where T : IActionResultDataItems<TI>
            where F : IActionResultDataItems<FI>
        {
            using var connection = this.UseDbConnection();
            using var multiple = await connection.QueryMultipleAsync(command);
            var result = multiple.Read<ActionResultItems<T, TI, F, FI>, T, F, ActionResultItems<T, TI, F, FI>>((result, success, failure) => {
                if (result.Success)
                {
                    result.Data = success;
                }
                else
                {
                    result.DataFailure = failure;
                }

                return result;
            }, splitOn: "@,!").FirstOrDefault();

            if (result == null)
            {
                result = (ApplicationErrors.NoActionResult as ActionResultItems<T, TI, F, FI>)!;
            }
            else if(result.Success)
            {
                if (result.Data != null)
                    result.Data.Items = await multiple.ReadAsync<TI>();
            }
            else
            {
                if (result.DataFailure != null)
                    result.DataFailure.Items = await multiple.ReadAsync<FI>();
            }

            return result;
        }
    }
}
