using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// User authorized entity service
    /// 已授权用户实体服务
    /// </summary>
    /// <typeparam name="S">Generic configuration type</typeparam>
    /// <typeparam name="C">Generic connection type</typeparam>
    /// <typeparam name="A">Generic application type</typeparam>
    /// <typeparam name="U">Generic current user type</typeparam>
    /// <typeparam name="T">Generic id type</typeparam>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="app">Application</param>
    /// <param name="user">Current user</param>
    /// <param name="flag">Flag</param>
    /// <param name="logger">Logger</param>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="app">Application</param>
    /// <param name="user">Current user</param>
    /// <param name="flag">Flag</param>
    /// <param name="logger">Logger</param>
    public abstract class EntityServiceBase<S, C, A, U, T>(A app, U user, string flag, ILogger logger)
        : UserServiceBase<S, C, A, U>(app, user, flag, logger), IEntityServiceBase<T>
        where S : AppConfiguration
        where C : DbConnection
        where A : ICoreApplication<S, C>
        where U : IServiceUser
        where T : struct
    {
        /// <summary>
        /// Public data view range
        /// 公共数据浏览范围
        /// </summary>
        public const string PublicRange = "public";

        /// <summary>
        /// Create
        /// 创建
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        public virtual ValueTask<IActionResult> CreateAsync(object model, CancellationToken cancellationToken = default)
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("create"), parameters, null, cancellationToken);

            return QueryAsResultAsync(command);
        }

        /// <summary>
        /// Create delete command
        /// 创建删除命令
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <param name="range">Range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewDeleteCommand(IEnumerable<T> ids, string? range = null, CancellationToken cancellationToken = default)
        {
            var parameters = new DbParameters();

            var idParameter = App.DB.ListToParameter(ids, null, (type) => SqlServerUtils.GetListCommand(type, App.BuildCommandName));
            parameters.Add("ids", idParameter);

            AddSystemParameters(parameters);

            var command = string.IsNullOrEmpty(range) ? "delete" : range;
            return CreateCommand(GetCommandName(command), parameters, null, cancellationToken);
        }

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="range">Range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        public virtual ValueTask<IActionResult> DeleteAsync(T id, string? range = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync([id], range, cancellationToken);
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <param name="range">Range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        public async virtual ValueTask<IActionResult> DeleteAsync(IEnumerable<T> ids, string? range = null, CancellationToken cancellationToken = default)
        {
            var command = NewDeleteCommand(ids, range, cancellationToken);
            var result = await QueryAsResultAsync(command);

            // Send back the ids
            if (result.Ok) result.Data["Ids"] = ids;

            return result;
        }

        /// <summary>
        /// List
        /// 列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual Task ListAsync(TiplistRQ<T> model, HttpResponse response, string? queryKey = null, CancellationToken cancellationToken = default)
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var commandText = !string.IsNullOrEmpty(queryKey) && FilterRange(queryKey, false)
                ? $"list {queryKey} as json" : "list as json";

            var command = CreateCommand(GetCommandName(commandText), parameters, null, cancellationToken);

            return ReadJsonToStreamAsync(command, response);
        }

        /// <summary>
        /// Merge
        /// 合并
        /// </summary>
        /// <typeparam name="M">Generic request data type</typeparam>
        /// <param name="rq">Request data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public virtual ValueTask<IActionResult> MergeAsync<M>(M rq, CancellationToken cancellationToken = default) where M : MergeRQ
        {
            var parameters = FormatParameters(rq);
            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("merge"), parameters, null, cancellationToken);

            return QueryAsResultAsync(command);
        }

        /// <summary>
        /// Query
        /// 查询
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key word, default is empty</param>
        /// <param name="collectionNames">Collection names</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual Task QueryAsync<E>(QueryRQ<E> model, HttpResponse response, string? queryKey = null, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default)
            where E : struct
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var commandText = !string.IsNullOrEmpty(queryKey) && FilterRange(queryKey, false)
                ? $"query {queryKey} as json" : "query as json";

            var command = CreateCommand(GetCommandName(commandText), parameters, null, cancellationToken);

            return ReadJsonToStreamAsync(command, response, collectionNames);
        }

        /// <summary>
        /// Quick update
        /// 快速更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <param name="additionalPart">Additional SQL command part, please avoid injection</param>
        /// <param name="additionalParams">Additional parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public virtual ValueTask<(IActionResult Result, UpdateResultData<T>? Data)> QuickUpdateAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null, CancellationToken cancellationToken = default)
            where M : IdItem<T>, IUpdateModel
        {
            return InlineUpdateAsync<T, M>(model, configs, additionalPart, additionalParams, cancellationToken);
        }

        /// <summary>
        /// Get read command name
        /// </summary>
        /// <param name="range">Range</param>
        /// <param name="format">Format</param>
        /// <returns>Command name</returns>
        protected virtual string GetReadCommand(string range, DataFormat? format = null)
        {
            // Avoid possible SQL injection attack
            FilterRange(range);

            // Keys
            var keys = new List<string> { "read", "for", range };

            if (format != null)
                keys.Add(format.Name.ToLower());

            return GetCommandNameBase(keys);
        }

        /// <summary>
        /// Create read command
        /// 创建读取命令
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="range">View range</param>
        /// <param name="format">Date format</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewReadCommand(T id, string range, DataFormat? format = null, CancellationToken cancellationToken = default)
        {
            var name = GetReadCommand(range, format);

            var parameters = new DbParameters();
            App.DB.AddParameter(parameters, "id", id, DatabaseUtils.TypeToDbType(typeof(T)).GetValueOrDefault());

            if (range != PublicRange)
            {
                // Deserve for public acess
                AddSystemParameters(parameters);
            }

            return CreateCommand(name, parameters, null, cancellationToken);
        }

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity</returns>
        public virtual ValueTask<E?> ReadAsync<E>(T id, string range = "default", CancellationToken cancellationToken = default) where E : IDataReaderParser<E>
        {
            var command = NewReadCommand(id, range, null, cancellationToken);
            return QueryAsAsync<E>(command);
        }

        /// <summary>
        /// View entity with direct way
        /// 直接方式浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity</returns>
        public virtual Task<E?> ReadDirectAsync<E>(T id, string range = "default", CancellationToken cancellationToken = default)
        {
            var command = NewReadCommand(id, range, null, cancellationToken);

            return App.DB.WithConnection((connection) =>
            {
                return connection.QueryFirstOrDefaultAsync<E>(command);
            }, cancellationToken);
        }

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="E2">Generic dataset 2 object type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity</returns>
        public virtual async Task<(E1?, E2[])> ReadAsync<E1, E2>(T id, string range = "default", CancellationToken cancellationToken = default)
            where E1 : IDataReaderParser<E1>
            where E2 : IDataReaderParser<E2>
        {
            var command = NewReadCommand(id, range, null, cancellationToken);
            var (list1, list2) = await QueryAsListAsync<E1, E2>(command);
            return (list1.FirstOrDefault(), list2);
        }

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="E2">Generic dataset 2 object type</typeparam>
        /// <typeparam name="E3">Generic dataset 3 object type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity</returns>
        public virtual async Task<(E1?, E2[], E3[])> ReadAsync<E1, E2, E3>(T id, string range = "default", CancellationToken cancellationToken = default)
            where E1 : IDataReaderParser<E1>
            where E2 : IDataReaderParser<E2>
            where E3 : IDataReaderParser<E3>
        {
            var command = NewReadCommand(id, range, null, cancellationToken);
            var (list1, list2, list3) = await QueryAsListAsync<E1, E2, E3>(command);
            return (list1.FirstOrDefault(), list2, list3);
        }

        /// <summary>
        /// View entity to PipeWriter
        /// 浏览实体数据到PipeWriter
        /// </summary>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="id">Entity id</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(IBufferWriter<byte> writer, T id, string range = "default", DataFormat? format = null, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default)
        {
            format ??= DataFormat.Json;
            var command = NewReadCommand(id, range, format, cancellationToken);

            if (collectionNames == null)
                await ReadToStreamAsync(command, writer);
            else
                await ReadToStreamAsync(command, writer, format, collectionNames);
        }

        /// <summary>
        /// Read
        /// 浏览
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <param name="collectionNames">Collection names</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual Task ReadAsync(HttpResponse response, T id, string range = "default", IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default)
        {
            var command = NewReadCommand(id, range, DataFormat.Json, cancellationToken);
            return ReadJsonToStreamAsync(command, response, collectionNames);
        }

        /// <summary>
        /// Create report command
        /// 创建报表命令
        /// </summary>
        /// <param name="range">Report range</param>
        /// <param name="model">Condition model</param>
        /// <param name="format">Date format</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewReportCommand(string range, object? model = null, DataFormat? format = null, CancellationToken cancellationToken = default)
        {
            // Avoid possible SQL injection attack
            FilterRange(range);

            var parameters = model == null ? new DbParameters() : FormatParameters(model);

            AddSystemParameters(parameters);

            // Keys
            var keys = new List<string> { "report", "for", range };

            if (format != null)
            {
                keys.Add("as");
                keys.Add(format.Name.ToLower());
            }

            var name = GetCommandNameBase(keys);

            return CreateCommand(name, parameters, null, cancellationToken);
        }

        /// <summary>
        /// Entity report
        /// 实体报告
        /// </summary>
        /// <typeparam name="E">Generic list item type</typeparam>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual Task<IEnumerable<E>> ReportAsync<E>(string range, object? modal = null, CancellationToken cancellationToken = default)
        {
            var command = NewReportCommand(range, modal, null, cancellationToken);
            return App.DB.WithConnection((connection) =>
            {
                return connection.QueryAsync<E>(command);
            }, cancellationToken);
        }

        /// <summary>
        /// Entity report to PipeWriter
        /// 实体报告数据到PipeWriter
        /// </summary>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual async Task ReportAsync(IBufferWriter<byte> writer, string range, object? modal = null, DataFormat? format = null, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default)
        {
            format ??= DataFormat.Json;
            var command = NewReportCommand(range, modal, format, cancellationToken);

            if (collectionNames == null)
                await ReadToStreamAsync(command, writer);
            else
                await ReadToStreamAsync(command, writer, format, collectionNames);
        }

        /// <summary>
        /// Entity JSON report to HTTP Response
        /// 实体报告JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual Task ReportAsync(HttpResponse response, string range, object? modal = null, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default)
        {
            var command = NewReportCommand(range, modal, DataFormat.Json, cancellationToken);
            return ReadJsonToStreamAsync(command, response, collectionNames);
        }

        /// <summary>
        /// Sort data
        /// 数据排序
        /// </summary>
        /// <param name="sortData">Data to sort</param>
        /// <param name="category">Category to group data</param>
        /// <param name="range">Sort range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Rows affected</returns>
        public virtual Task<int> SortAsync(Dictionary<T, short> sortData, byte? category = null, string? range = null, CancellationToken cancellationToken = default)
        {
            var parameters = new DbParameters();
            if (category.HasValue) parameters.Add("Category", category.Value);
            parameters.Add("Items", App.DB.DictionaryToParameter(sortData, null, null, (keyType, valueType) => SqlServerUtils.GetDicCommand(keyType, valueType, App.BuildCommandName)));

            AddSystemParameters(parameters);

            var commandText = !string.IsNullOrEmpty(range) && FilterRange(range, false)
                ? $"sort {range}" : "sort";

            var command = CreateCommand(GetCommandName(commandText), parameters, null, cancellationToken);

            return ExecuteAsync(command);
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        public async virtual ValueTask<IActionResult> UpdateAsync<M>(M model, CancellationToken cancellationToken = default) where M : IdItem<T>, IUpdateModel
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("update"), parameters, null, cancellationToken);

            var result = await QueryAsResultAsync(command);

            // Send back the Id
            if (result.Ok) result.Data["Id"] = model.Id;

            return result;
        }

        /// <summary>
        /// Read for updae
        /// 更新浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual Task UpdateReadAsync(T id, HttpResponse response, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default)
        {
            return ReadAsync(response, id, "update", collectionNames, cancellationToken);
        }

        /// <summary>
        /// Update status
        /// 更新状态
        /// </summary>
        /// <param name="rq">Request data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public virtual ValueTask<IActionResult> UpdateStatusAsync(UpdateStatusRQ<T> rq, CancellationToken cancellationToken = default)
        {
            var parameters = new DbParameters();
            parameters.Add("Id", rq.Id);
            parameters.Add("Status", (byte)rq.Status);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("update status"), parameters, null, cancellationToken);

            return QueryAsResultAsync(command);
        }
    }
}
