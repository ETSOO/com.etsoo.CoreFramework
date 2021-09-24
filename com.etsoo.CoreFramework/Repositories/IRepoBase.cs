using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Serialization;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Base repository interface
    /// 基础仓库接口
    /// </summary>
    public interface IRepoBase
    {
        /// <summary>
        /// Add system parameters
        /// 添加系统参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        void AddSystemParameters(DynamicParameters parameters);

        /// <summary>
        /// Async query command as object
        /// 异步执行命令返回对象
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        ValueTask<T?> QueryAsAsync<T>(CommandDefinition command) where T : IDataReaderParser<T>;

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        IAsyncEnumerable<T> QueryAsListAsync<T>(CommandDefinition command) where T : IDataReaderParser<T>;

        /// <summary>
        /// Query command as action result
        /// 执行命令返回操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        Task<IActionResult> QueryAsResultAsync(CommandDefinition command);

        /// <summary>
        /// Async read text data (JSON/XML) to stream
        /// 异步读取文本数据(JSON或者XML)到流
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="stream">Stream</param>
        /// <returns>Has content or not</returns>
        Task<bool> ReadToStreamAsync(CommandDefinition command, Stream stream);

        /// <summary>
        /// Async read text data (JSON/XML) to PipeWriter
        /// 异步读取文本数据(JSON或者XML)到PipeWriter
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="writer">PipeWriter</param>
        /// <returns>Has content or not</returns>
        Task<bool> ReadToStreamAsync(CommandDefinition command, PipeWriter writer);

        /// <summary>
        /// Async read JSON data to HTTP Response
        /// 异步读取JSON数据到HTTP响应
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        Task ReadJsonToStreamAsync(CommandDefinition command, HttpResponse response);

        /// <summary>
        /// Quick read data
        /// 快速读取数据
        /// </summary>
        /// <typeparam name="E">Generic return type</typeparam>
        /// <returns>Result</returns>
        Task<E> QuickReadAsync<E>(string sql, DynamicParameters? parameters = null);
    }
}
