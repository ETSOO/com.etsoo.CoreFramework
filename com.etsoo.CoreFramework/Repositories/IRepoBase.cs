using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Buffers;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Base repository interface
    /// 基础仓库接口
    /// </summary>
    public interface IRepoBase
    {
        /// <summary>
        /// Flag
        /// 标识
        /// </summary>
        string Flag { get; }

        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        IServiceUser? User { get; }

        /// <summary>
        /// Cancellation token, with the feature, only transient or scoped scenario can used
        /// 取消令牌，使用该功能，只能使用瞬态或范围场景
        /// </summary>
        CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Add system parameters
        /// 添加系统参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        void AddSystemParameters(IDbParameters parameters);

        /// <summary>
        /// Execute a command asynchronously
        /// </summary>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The number of rows affected</returns>
        Task<int> ExecuteAsync(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The first cell selected as T</returns>
        Task<T> ExecuteScalarAsync<T>(CommandDefinition command);

        /// <summary>
        /// Async query command as object
        /// 异步执行命令返回对象
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        ValueTask<T?> QueryAsAsync<T>(CommandDefinition command) where T : IDataReaderParser<T>;

        /// <summary>
        /// Async query command as object source list
        /// 异步执行命令返回对象源列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        IAsyncEnumerable<D> QueryAsSourceAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>;

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<D[]> QueryAsListAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>;

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="D2">Generic dataset 2 object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<(D1[], D2[])> QueryAsListAsync<D1, D2>(CommandDefinition command)
            where D1 : IDataReaderParser<D1>
            where D2 : IDataReaderParser<D2>;

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="D2">Generic dataset 2 object type</typeparam>
        /// <typeparam name="D3">Generic dataset 3 object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<(D1[], D2[], D3[])> QueryAsListAsync<D1, D2, D3>(CommandDefinition command)
            where D1 : IDataReaderParser<D1>
            where D2 : IDataReaderParser<D2>
            where D3 : IDataReaderParser<D3>;

        /// <summary>
        /// Query command as action result
        /// 执行命令返回操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        ValueTask<ActionResult> QueryAsResultAsync(CommandDefinition command);

        /// <summary>
        /// Async read text data (JSON/XML) to PipeWriter
        /// 异步读取文本数据(JSON或者XML)到PipeWriter
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <returns>Has content or not</returns>
        Task<bool> ReadToStreamAsync(CommandDefinition command, IBufferWriter<byte> writer);

        /// <summary>
        /// Async read text data (JSON/XML) to PipeWriter
        /// 异步读取文本数据(JSON或者XML)到PipeWriter
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Has content or not</returns>
        Task<bool> ReadToStreamAsync(CommandDefinition command, IBufferWriter<byte> writer, DataFormat format, IEnumerable<string>? collectionNames = null);

        /// <summary>
        /// Async read JSON data to HTTP Response
        /// 异步读取JSON数据到HTTP响应
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <returns>Task</returns>
        Task ReadJsonToStreamAsync(CommandDefinition command, HttpResponse response, IEnumerable<string>? collectionNames = null);

        /// <summary>
        /// Async read JSON data to HTTP Response and return the bytes
        /// 异步读取JSON数据到HTTP响应并返回写入的字节
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <returns>Task</returns>
        Task<ReadOnlyMemory<byte>> ReadJsonToStreamWithReturnAsync(CommandDefinition command, HttpResponse response, IEnumerable<string>? collectionNames = null);

        /// <summary>
        /// Quick read data
        /// 快速读取数据
        /// </summary>
        /// <typeparam name="E">Generic return type</typeparam>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">Parameter</param>
        /// <returns>Result</returns>
        Task<E> QuickReadAsync<E>(string sql, IDbParameters? parameters = null);

        /// <summary>
        /// Inline SQL command update
        /// 内联 SQL 命令更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <typeparam name="T">Generic id type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <param name="additionalPart">Additional SQL command part, please avoid injection</param>
        /// <param name="additionalParams">Additional parameters</param>
        /// <returns>Result</returns>
        ValueTask<(ActionResult Result, UpdateResultData<T>? Data)> InlineUpdateAsync<T, M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null)
            where T : struct
            where M : IdItem<T>, IUpdateModel;

        /// <summary>
        /// Inline SQL command update
        /// 内联 SQL 命令更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <param name="additionalPart">Additional SQL command part, please avoid injection</param>
        /// <param name="additionalParams">Additional parameters</param>
        /// <returns>Result</returns>
        ValueTask<(ActionResult Result, UpdateResultData? Data)> InlineUpdateAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null) where M : IdItem, IUpdateModel;
    }
}
