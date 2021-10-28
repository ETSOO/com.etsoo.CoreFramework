using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Database;
using com.etsoo.Utils.String;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;
using System.IO.Pipelines;
using System.Text;

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
        protected EntityRepo(ICoreApplication<C> app, string flag, ICurrentUser? user = null) : base(app, flag, user)
        {
        }

        /// <summary>
        /// Create entity
        /// 创建实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async Task<ActionResult> CreateAsync(object model)
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
        public virtual async Task<ActionResult> DeleteAsync(T id)
        {
            return await DeleteAsync(new T[] { id });
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        public virtual async Task<ActionResult> DeleteAsync(IEnumerable<T> ids)
        {
            var command = NewDeleteCommand(ids);
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
            // Avoid possible SQL injection attack
            FilterRange(range);

            // Keys
            var keys = new List<string> { "read", "for", range };

            if (format.HasValue)
                keys.Add(format.Value.ToString().ToLower());

            var name = GetCommandNameBase(keys);

            var parameters = new DynamicParameters();
            parameters.Add("id", id);

            AddSystemParameters(parameters);

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
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(Stream stream, T id, string range = "default", DataFormat format = DataFormat.JSON)
        {
            var command = NewReadCommand(id, range, format);
            await ReadToStreamAsync(command, stream);
        }

        /// <summary>
        /// View entity to PipeWriter
        /// 浏览实体数据到PipeWriter
        /// </summary>
        /// <param name="writer">PipeWriter</param>
        /// <param name="id">Entity id</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(PipeWriter writer, T id, string range = "default", DataFormat format = DataFormat.JSON)
        {
            var command = NewReadCommand(id, range, format);
            await ReadToStreamAsync(command, writer);
        }

        /// <summary>
        /// View entity JSON data HTTP Response
        /// 浏览实体JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(HttpResponse response, T id, string range = "default")
        {
            var command = NewReadCommand(id, range, DataFormat.JSON);
            await ReadJsonToStreamAsync(command, response);
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

            if (format.HasValue)
            {
                keys.Add("as");
                keys.Add(format.Value.ToString().ToLower());
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
        public virtual async Task ReportAsync(Stream stream, string range, object? modal = null, DataFormat format = DataFormat.JSON)
        {
            var command = NewReportCommand(range, modal, format);
            await ReadToStreamAsync(command, stream);
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
        public virtual async Task ReportAsync(PipeWriter writer, string range, object? modal = null, DataFormat format = DataFormat.JSON)
        {
            var command = NewReportCommand(range, modal, format);
            await ReadToStreamAsync(command, writer);
        }

        /// <summary>
        /// Entity JSON report to HTTP Response
        /// 实体报告JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <returns>Task</returns>
        public virtual async Task ReportAsync(HttpResponse response, string range, object? modal = null)
        {
            var command = NewReportCommand(range, modal, DataFormat.JSON);
            await ReadJsonToStreamAsync(command, response);
        }

        /// <summary>
        /// Update entity
        /// 更新实体
        /// </summary>
        /// <typeparam name="M">Generic entity model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async Task<ActionResult> UpdateAsync<M>(M model) where M : IUpdateModel<T>
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("update"), parameters);

            return await QueryAsResultAsync(command);
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
        /// <returns>Task</returns>
        public async Task QueryAsync<E, D>(D model, HttpResponse response) where E : struct where D : QueryRQ<E>
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("query as json"), parameters);

            await ReadJsonToStreamAsync(command, response);
        }

        /// <summary>
        /// Quick update
        /// 快速更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <returns>Result</returns>
        public async Task<(ActionResult, UpdateResultData<T>?)> QuickUpdateAsync<M>(M model, QuickUpdateConfigs configs) where M : IUpdateModel<T>
        {
            // Validate
            if (model.ChangedFields == null || !model.ChangedFields.Any())
            {
                return (ApplicationErrors.NoValidData.AsResult(UpdateResultData<T>.ChangedFields), null);
            }

            if (!configs.UpdatableFields.Any())
            {
                return (ApplicationErrors.NoValidData.AsResult(UpdateResultData<T>.UpdatableFields), null);
            }

            if (!string.IsNullOrEmpty(configs.Conditions) && !DatabaseUtils.IsSafeSQLPart(configs.Conditions))
            {
                return (ApplicationErrors.NoValidData.AsResult(UpdateResultData<T>.Conditions), null);
            }

            // Update fields
            var updateFields = configs.UpdatableFields
                .Where(field => model.ChangedFields.Contains(field, StringComparer.OrdinalIgnoreCase))
                .Select(field => $"{App.DB.EscapeIdentifier(field)} = @{field}");

            if (!updateFields.Any())
            {
                return (ApplicationErrors.NoValidData.AsResult(UpdateResultData<T>.UpdateFields), null);
            }

            // Default table name
            configs.TableName ??= StringUtils.LinuxStyleToPascalCase(Flag).ToString();
            var tableName = App.DB.EscapeIdentifier(configs.TableName);

            // SQL
            var sql = new StringBuilder("UPDATE ");
            sql.Append(tableName);
            sql.Append(" SET ");
            sql.Append(string.Join(", ", updateFields));
            sql.Append(" FROM ");
            sql.Append(tableName);
            sql.Append(" u WHERE u.");
            sql.Append(App.DB.EscapeIdentifier(configs.IdField));
            sql.Append(" = @Id");

            if (!string.IsNullOrEmpty(configs.Conditions))
            {
                sql.Append(" AND ");
                sql.Append(configs.Conditions);
            }

            // Parameters
            var parameters = FormatParameters(model);
            AddSystemParameters(parameters);

            var command = CreateCommand(sql.ToString(), parameters, CommandType.Text);
            var records = await App.DB.NewConnection().ExecuteAsync(command);

            // Success
            return (ActionResult.Success, new UpdateResultData<T> { Id = model.Id, RowsAffected = records });
        }
    }
}
