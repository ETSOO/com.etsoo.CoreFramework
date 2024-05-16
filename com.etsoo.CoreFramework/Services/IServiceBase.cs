using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Buffers;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Service base interface
    /// 基础服务接口
    /// </summary>
    public interface IServiceBase
    {
        /// <summary>
        /// Flag
        /// 标识
        /// </summary>
        string Flag { get; }

        /// <summary>
        /// Add system parameters
        /// 添加系统参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <param name="userRequired">User required or not</param>
        void AddSystemParameters(IDbParameters parameters, bool userRequired = true);

        /// <summary>
        /// Decrypt device core with user identifier for multiple decryption
        /// 使用用户识别码解密设备核心以用于多次解密
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <param name="identifier">User identifier</param>
        /// <returns>Result</returns>
        string? DecryptDeviceCore(string deviceId, string identifier);

        /// <summary>
        /// Async decrypt device data with passphrase
        /// 使用密码异步解密设备数据
        /// </summary>
        /// <param name="encryptedMessage">Encrypted message</param>
        /// <param name="deviceCore">Device core passphrase</param>
        /// <returns>Result</returns>
        string? DecryptDeviceData(string encryptedMessage, string deviceCore);

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
        Task<T?> ExecuteScalarAsync<T>(CommandDefinition command);

        /// <summary>
        /// Async init call
        /// 异步初始化调用
        /// </summary>
        /// <param name="rq">Request data</param>
        /// <param name="secret">Encryption secret</param>
        /// <returns>Result</returns>
        ValueTask<ActionResult> InitCallAsync(InitCallRQ rq, string secret);

        /// <summary>
        /// Log exception and return simple user result
        /// 登记异常结果日志，并返回简介的用户结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        ActionResult LogException(Exception ex);

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
        ValueTask<IActionResult> QueryAsResultAsync(CommandDefinition command);

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<E> QuickReadAsync<E>(string sql, IDbParameters? parameters = null, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<(IActionResult Result, UpdateResultData<T>? Data)> InlineUpdateAsync<T, M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null, CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<(IActionResult Result, UpdateResultData? Data)> InlineUpdateAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null, CancellationToken cancellationToken = default) where M : IdItem, IUpdateModel;

        /// <summary>
        /// Delete records with SQL asynchronously
        /// SQL语句异步删除记录
        /// </summary>
        /// <typeparam name="T">Generic query data type</typeparam>
        /// <param name="data">Related data</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<IActionResult> SqlDeleteAsync<T>(T data, bool? addSystemParameters = null, CancellationToken cancellationToken = default) where T : ISqlDelete;

        /// <summary>
        /// Delete records with SQL asynchronously
        /// SQL语句异步删除记录
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="tableName">Table name, default is the 'Flag'</param>
        /// <param name="idColumn">Id column</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<IActionResult> SqlDeleteAsync(IEnumerable<string> ids, string? tableName = null, string idColumn = "id", CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete records with SQL asynchronously
        /// SQL语句异步删除记录
        /// </summary>
        /// <typeparam name="T">Generic id type</typeparam>
        /// <param name="ids">Ids</param>
        /// <param name="tableName">Table name, default is the 'Flag'</param>
        /// <param name="idColumn">Id column</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<IActionResult> SqlDeleteAsync<T>(IEnumerable<T> ids, string? tableName = null, string idColumn = "id", CancellationToken cancellationToken = default) where T : struct;


        /// <summary>
        /// Insert records with SQL asynchronously
        /// SQL语句异步插入记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <typeparam name="I">Generic inserted data type</typeparam>
        /// <param name="data">Related data</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<I?> SqlInsertAsync<T, I>(T data, bool? addSystemParameters = null, CancellationToken cancellationToken = default) where T : ISqlInsert;

        /// <summary>
        /// Select records with SQL asynchronously
        /// SQL语句异步选择记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <typeparam name="D">Generic selected data type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<D[]> SqlSelectAsync<T, D>(T data, bool? addSystemParameters = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default)
            where T : ISqlSelect
            where D : IDataReaderParser<D>;

        /// <summary>
        /// Select records with SQL asynchronously
        /// SQL语句异步选择记录
        /// </summary>
        /// <typeparam name="D">Generic selected data type</typeparam>
        /// <param name="result">Select result type</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<D[]> SqlSelectAsync<D>(ISqlSelectResult<D> result, bool? addSystemParameters = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="fields">Fields</param>
        /// <param name="response">HTTP response</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="mappingDelegate">Query fields mapping delegate</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task SqlSelectJsonAsync<T>(T data, IEnumerable<string> fields, HttpResponse response, bool? addSystemParameters = null, SqlMappingDelegate? mappingDelegate = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default)
            where T : ISqlSelect;

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <typeparam name="D">Geneirc fields type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="response">HTTP response</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="mappingDelegate">Query fields mapping delegate</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task SqlSelectJsonAsync<T, D>(T data, HttpResponse response, bool? addSystemParameters = null, SqlMappingDelegate? mappingDelegate = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default)
            where T : ISqlSelect
            where D : IDataReaderParser<D>;

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="fields">Fields</param>
        /// <param name="writer">Buffer writer</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="mappingDelegate">Query fields mapping delegate</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task SqlSelectJsonAsync<T>(T data, IEnumerable<string> fields, IBufferWriter<byte> writer, bool? addSystemParameters = null, SqlMappingDelegate? mappingDelegate = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default)
            where T : ISqlSelect;

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <typeparam name="D">Geneirc fields type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="writer">Buffer writer</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="mappingDelegate">Query fields mapping delegate</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task SqlSelectJsonAsync<T, D>(T data, IBufferWriter<byte> writer, bool? addSystemParameters = null, SqlMappingDelegate? mappingDelegate = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default)
            where T : ISqlSelect
            where D : IDataReaderParser<D>;

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <param name="result">Select result type</param>
        /// <param name="response">HTTP response</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task SqlSelectJsonAsync<T>(ISqlSelectResult<T> result, HttpResponse response, bool? addSystemParameters = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <param name="result">Select result type</param>
        /// <param name="writer">Buffer writer</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="conditionDelegate">Query condition delegate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task SqlSelectJsonAsync<T>(ISqlSelectResult<T> result, IBufferWriter<byte> writer, bool? addSystemParameters = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update records with SQL asynchronously
        /// SQL语句异步更新记录
        /// </summary>
        /// <param name="data">Related data</param>
        /// <param name="addSystemParameters">Auto add system parameters or not, null means no action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<IActionResult> SqlUpdateAsync<T>(T data, bool? addSystemParameters = null, CancellationToken cancellationToken = default) where T : ISqlUpdate;
    }
}
