﻿using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data.Common;
using System.IO.Pipelines;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Entity repository for CURD(Create, Update, Read, Delete)
    /// 实体仓库，实现增删改查
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    /// <typeparam name="T">Generic id type</typeparam>
    public abstract class EntityRepo<C, T> : RepoBase<C>, IEntityRepo<T>
        where C : DbConnection
        where T : struct
    {
        /// <summary>
        /// Public data view range
        /// </summary>
        public const string PublicRange = "public";


        protected EntityRepo(ICoreApplication<C> app, string flag, IServiceUser? user = null) : base(app, flag, user)
        {
        }

        /// <summary>
        /// Create entity
        /// 创建实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<ActionResult> CreateAsync(object model)
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
            var parameters = new DynamicParameters();

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
        public virtual async ValueTask<ActionResult> DeleteAsync(T id)
        {
            return await DeleteAsync(new T[] { id });
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<ActionResult> DeleteAsync(IEnumerable<T> ids)
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

            var parameters = new DynamicParameters();
            parameters.Add("id", id);

            if (range != PublicRange)
            {
                // Deserve for public acess
                AddSystemParameters(parameters);
            }

            return CreateCommand(name, parameters);
        }

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        public virtual async Task<E> ReadAsync<E>(T id, string range = "default")
        {
            var command = NewReadCommand(id, range);

            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryFirstAsync<E>(command);
            });
        }

        /// <summary>
        /// View entity to stream
        /// 浏览实体数据到流
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="id">Entity id</param>
        /// <param name="range">View range</param>
        /// <param name="format">Data format</param>
        /// <param name="multipleResults">Multiple results</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(Stream stream, T id, string range = "default", DataFormat? format = null, bool multipleResults = false)
        {
            format ??= DataFormat.Json;
            var command = NewReadCommand(id, range, format);
            await ReadToStreamAsync(command, stream, format, multipleResults);
        }

        /// <summary>
        /// View entity to PipeWriter
        /// 浏览实体数据到PipeWriter
        /// </summary>
        /// <param name="writer">PipeWriter</param>
        /// <param name="id">Entity id</param>
        /// <param name="format">Data format</param>
        /// <param name="multipleResults">Multiple results</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(PipeWriter writer, T id, string range = "default", DataFormat? format = null, bool multipleResults = false)
        {
            format ??= DataFormat.Json;
            var command = NewReadCommand(id, range, format);
            await ReadToStreamAsync(command, writer, format, multipleResults);
        }

        /// <summary>
        /// View entity JSON data HTTP Response
        /// 浏览实体JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <param name="multipleResults">Multiple results</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(HttpResponse response, T id, string range = "default", bool multipleResults = false)
        {
            var command = NewReadCommand(id, range, DataFormat.Json);
            await ReadJsonToStreamAsync(command, response, multipleResults);
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

            var parameters = FormatParameters(model ?? new DynamicParameters());

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
            var command = NewReportCommand(range, modal);
            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryAsync<E>(command);
            });
        }

        /// <summary>
        /// Entity report to stream
        /// 实体报告数据到流
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        public virtual async Task ReportAsync(Stream stream, string range, object? modal = null, DataFormat? format = null, bool multipleResults = false)
        {
            format ??= DataFormat.Json;
            var command = NewReportCommand(range, modal, format);
            await ReadToStreamAsync(command, stream, format, multipleResults);
        }

        /// <summary>
        /// Entity report to PipeWriter
        /// 实体报告数据到PipeWriter
        /// </summary>
        /// <param name="writer">PipeWriter</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        public virtual async Task ReportAsync(PipeWriter writer, string range, object? modal = null, DataFormat? format = null, bool multipleResults = false)
        {
            format ??= DataFormat.Json;
            var command = NewReportCommand(range, modal, format);
            await ReadToStreamAsync(command, writer, format, multipleResults);
        }

        /// <summary>
        /// Entity JSON report to HTTP Response
        /// 实体报告JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <returns>Task</returns>
        public virtual async Task ReportAsync(HttpResponse response, string range, object? modal = null, bool multipleResults = false)
        {
            var command = NewReportCommand(range, modal, DataFormat.Json);
            await ReadJsonToStreamAsync(command, response, multipleResults);
        }

        /// <summary>
        /// Sort data
        /// 数据排序
        /// </summary>
        /// <param name="sortData">Data to sort</param>
        /// <returns>Rows affected</returns>
        public virtual async Task<int> SortAsync(Dictionary<T, short> sortData)
        {
            var parameters = new DynamicParameters();
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
        public virtual async ValueTask<ActionResult> UpdateAsync<M>(M model) where M : IdItem<T>, IUpdateModel
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
        /// Data list
        /// 数据列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        public async Task ListAsync(TiplistRQ<T> model, HttpResponse response)
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("list as json"), parameters);

            await ReadJsonToStreamAsync(command, response);
        }

        /// <summary>
        /// Query data
        /// 查询数据
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key word, default is empty</param>
        /// <param name="multipleResults">Multiple results</param>
        /// <returns>Task</returns>
        public async Task QueryAsync<E, D>(D model, HttpResponse response, string? queryKey = null, bool multipleResults = false) where E : struct where D : QueryRQ<E>
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var commandText = "query as json";
            if (!string.IsNullOrEmpty(queryKey) && FilterRange(queryKey, false))
            {
                commandText = commandText.Replace("query as", $"query {queryKey} as");
            }

            var command = CreateCommand(GetCommandName(commandText), parameters);

            await ReadJsonToStreamAsync(command, response, multipleResults);
        }

        /// <summary>
        /// Quick update
        /// 快速更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <returns>Result</returns>
        public async ValueTask<(ActionResult Result, UpdateResultData<T>? Data)> QuickUpdateAsync<M>(M model, QuickUpdateConfigs configs)
            where M : IdItem<T>, IUpdateModel
        {
            return await InlineUpdateAsync<T, M>(model, configs);
        }
    }
}
