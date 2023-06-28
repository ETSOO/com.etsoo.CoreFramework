using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Buffers;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Entity repository for CURD(Create, Update, Read, Delete)
    /// 实体仓库，实现增删改查
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    /// <typeparam name="T">Generic id type</typeparam>
    public abstract class EntityRepo<C, T, A> : RepoBase<C, A>, IEntityRepo<T>
        where C : DbConnection
        where T : struct
        where A : ICoreApplication<C>
    {
        /// <summary>
        /// Public data view range
        /// </summary>
        public const string PublicRange = "public";

        protected EntityRepo(A app, string flag, IServiceUser? user = null) :
            base(app, flag, user)
        {
        }

        /// <summary>
        /// Create entity
        /// 创建实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<IActionResult> CreateAsync(object model)
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("create"), parameters);

            return await QueryAsResultAsync(command);
        }

        /// <summary>
        /// Create delete command
        /// 创建删除命令
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewDeleteCommand(IEnumerable<T> ids)
        {
            var parameters = new DbParameters();

            var idParameter = App.DB.ListToParameter(ids, null, (type) => SqlServerUtils.GetListCommand(type, App.BuildCommandName));
            parameters.Add("ids", idParameter);

            AddSystemParameters(parameters);

            return CreateCommand(GetCommandName("delete"), parameters);
        }

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<IActionResult> DeleteAsync(T id)
        {
            return await DeleteAsync(new T[] { id });
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<IActionResult> DeleteAsync(IEnumerable<T> ids)
        {
            var command = NewDeleteCommand(ids);
            var result = await QueryAsResultAsync(command);

            // Send back the ids
            if (result.Ok) result.Data["Ids"] = ids;

            return result;
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
        /// Merge
        /// 合并
        /// </summary>
        /// <typeparam name="M">Generic request data type</typeparam>
        /// <param name="rq">Request data</param>
        /// <returns>Result</returns>
        public virtual async ValueTask<IActionResult> MergeAsync<M>(M rq) where M : MergeRQ
        {
            var parameters = FormatParameters(rq);
            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("merge"), parameters);

            return await QueryAsResultAsync(command);
        }

        /// <summary>
        /// Create read command
        /// 创建读取命令
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="range">View range</param>
        /// <param name="format">Date format</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewReadCommand(T id, string range, DataFormat? format = null)
        {
            var name = GetReadCommand(range, format);

            var parameters = new DbParameters();
            App.DB.AddParameter(parameters, "id", id, DatabaseUtils.TypeToDbType(typeof(T)).GetValueOrDefault());

            if (range != PublicRange)
            {
                // Deserve for public acess
                AddSystemParameters(parameters);
            }

            return CreateCommand(name, parameters);
        }

        /// <summary>
        /// View entity with direct way
        /// 直接方式浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        public virtual async Task<E?> ReadDirectAsync<E>(T id, string range = "default")
        {
            var command = NewReadCommand(id, range, null);

            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryFirstOrDefaultAsync<E>(command);
            }, CancellationToken);
        }

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        public virtual async ValueTask<E?> ReadAsync<E>(T id, string range = "default") where E : IDataReaderParser<E>
        {
            var command = NewReadCommand(id, range, null);
            return await QueryAsAsync<E>(command);
        }

        /// <summary>
        /// View entity to PipeWriter
        /// 浏览实体数据到PipeWriter
        /// </summary>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="id">Entity id</param>
        /// <param name="format">Data format</param>
        /// <param name="multipleResults">Multiple results</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(IBufferWriter<byte> writer, T id, string range = "default", DataFormat? format = null, IEnumerable<string>? collectionNames = null)
        {
            format ??= DataFormat.Json;
            var command = NewReadCommand(id, range, format);

            if (collectionNames == null)
                await ReadToStreamAsync(command, writer);
            else
                await ReadToStreamAsync(command, writer, format, collectionNames);
        }

        /// <summary>
        /// View entity JSON data HTTP Response
        /// 浏览实体JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(HttpResponse response, T id, string range = "default", IEnumerable<string>? collectionNames = null)
        {
            var command = NewReadCommand(id, range, DataFormat.Json);
            await ReadJsonToStreamAsync(command, response, collectionNames);
        }

        /// <summary>
        /// Create report command
        /// 创建报表命令
        /// </summary>
        /// <param name="range">Report range</param>
        /// <param name="model">Condition model</param>
        /// <param name="format">Date format</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewReportCommand(string range, object? model = null, DataFormat? format = null)
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

            return CreateCommand(name, parameters);
        }

        /// <summary>
        /// Entity report
        /// 实体报告
        /// </summary>
        /// <typeparam name="E">Generic list item type</typeparam>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <returns>Task</returns>
        public virtual async Task<IEnumerable<E>> ReportAsync<E>(string range, object? modal = null)
        {
            var command = NewReportCommand(range, modal, null);
            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryAsync<E>(command);
            }, CancellationToken);
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
        /// <returns>Task</returns>
        public virtual async Task ReportAsync(IBufferWriter<byte> writer, string range, object? modal = null, DataFormat? format = null, IEnumerable<string>? collectionNames = null)
        {
            format ??= DataFormat.Json;
            var command = NewReportCommand(range, modal, format);

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
        /// <returns>Task</returns>
        public virtual async Task ReportAsync(HttpResponse response, string range, object? modal = null, IEnumerable<string>? collectionNames = null)
        {
            var command = NewReportCommand(range, modal, DataFormat.Json);
            await ReadJsonToStreamAsync(command, response, collectionNames);
        }

        /// <summary>
        /// Sort data
        /// 数据排序
        /// </summary>
        /// <param name="sortData">Data to sort</param>
        /// <returns>Rows affected</returns>
        public virtual async Task<int> SortAsync(Dictionary<T, short> sortData)
        {
            var parameters = new DbParameters();
            parameters.Add("Items", App.DB.DictionaryToParameter(sortData, null, null, (keyType, valueType) => SqlServerUtils.GetDicCommand(keyType, valueType, App.BuildCommandName)));

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("sort"), parameters);

            return await ExecuteAsync(command);
        }

        /// <summary>
        /// Update entity
        /// 更新实体
        /// </summary>
        /// <typeparam name="M">Generic entity model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<IActionResult> UpdateAsync<M>(M model) where M : IdItem<T>, IUpdateModel
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("update"), parameters);

            var result = await QueryAsResultAsync(command);

            // Send back the Id
            if (result.Ok) result.Data["Id"] = model.Id;

            return result;
        }

        /// <summary>
        /// Update status
        /// 更新状态
        /// </summary>
        /// <param name="rq">Request data</param>
        /// <returns>Result</returns>
        public virtual async ValueTask<IActionResult> UpdateStatusAsync(UpdateStatusRQ<T> rq)
        {
            var parameters = new DbParameters();
            parameters.Add("Id", rq.Id);
            parameters.Add("Status", (byte)rq.Status);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("update status"), parameters);

            return await QueryAsResultAsync(command);
        }

        /// <summary>
        /// Data list
        /// 数据列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key</param>
        /// <returns>Task</returns>
        public async Task ListAsync(TiplistRQ<T> model, HttpResponse response, string? queryKey = null)
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var commandText = !string.IsNullOrEmpty(queryKey) && FilterRange(queryKey, false)
                ? $"list {queryKey} as json" : "list as json";

            var command = CreateCommand(GetCommandName(commandText), parameters);

            await ReadJsonToStreamAsync(command, response);
        }

        /// <summary>
        /// Query data
        /// 查询数据
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key word, default is empty</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <returns>Task</returns>
        public async Task QueryAsync<E, D>(D model, HttpResponse response, string? queryKey = null, IEnumerable<string>? collectionNames = null) where E : struct where D : QueryRQ<E>
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var commandText = !string.IsNullOrEmpty(queryKey) && FilterRange(queryKey, false)
                ? $"query {queryKey} as json" : "query as json";

            var command = CreateCommand(GetCommandName(commandText), parameters);

            await ReadJsonToStreamAsync(command, response, collectionNames);
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
        /// <returns>Result</returns>
        public async ValueTask<(IActionResult Result, UpdateResultData<T>? Data)> QuickUpdateAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null)
            where M : IdItem<T>, IUpdateModel
        {
            return await InlineUpdateAsync<T, M>(model, configs, additionalPart, additionalParams);
        }
    }
}
