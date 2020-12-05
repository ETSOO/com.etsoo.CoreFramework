using com.etsoo.CoreFramework.ActionResult;
using com.etsoo.CoreFramework.MessageQueue;
using com.etsoo.CoreFramework.Storage;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        IConfiguration Configuration { get; }

        /// <summary>
        /// Use database connection
        /// 使用数据库链接
        /// </summary>
        Func<SqlConnection> UseDbConnection { get; }

        /// <summary>
        /// Use message queue
        /// 使用消息队列
        /// </summary>
        Func<IMessageQueue>? UseMessageQueue { get; }

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
        Task<string> HashPasswordAsync(string password);

        /// <summary>
        /// Query object list
        /// 查询对象列表
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Object list</returns>
        Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command);

        /// <summary>
        /// Query Json string
        /// 查询Json字符串
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Json string</returns>
        Task<string> QueryJsonAsync(CommandDefinition command);

        /// <summary>
        /// Query single object
        /// 查询单个对象
        /// </summary>
        /// <typeparam name="T">Generic</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Object</returns>
        Task<T?> QueryObjectAsync<T>(CommandDefinition command);

        /// <summary>
        /// Query action result with empty return data
        /// 查询无返回数据的操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<IActionResultNoData> QueryResultAsync(CommandDefinition command);

        /// <summary>
        /// Query action result with success data, split on column '@'
        /// 查询成功数据的操作结果，通过列名 @ 分割
        /// </summary>
        /// <typeparam name="T">Generic success data type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<IActionResultSuccessData<T>> QueryResultAsync<T>(CommandDefinition command);

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
        Task<IActionResultItems<T, TI, F, FI>> QueryResultAsync<T, TI, F, FI>(CommandDefinition command)
            where T : IActionResultDataItems<TI>
            where F : IActionResultDataItems<FI>;
    }
}
