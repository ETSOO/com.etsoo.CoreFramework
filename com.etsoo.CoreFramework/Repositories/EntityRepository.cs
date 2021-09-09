using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.SpanMemory;
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
    /// <typeparam name="T">Generic user id type</typeparam>
    /// <typeparam name="O">Generic organization id type</typeparam>
    public abstract class EntityRepository<C, T, O> : LoginedRepo<C, T, O>, IEntityRepository<T, O>
        where C : DbConnection
        where T : struct
        where O : struct
    {
        /// <summary>
        /// Flag
        /// 标识
        /// </summary>
        protected ReadOnlyMemory<char> Flag { get; }

        /// <summary>
        /// Procedure parts join char
        /// 存储过程部分连接字符
        /// </summary>
        protected ReadOnlyMemory<char> ProcedureJoinChar { get; }

        /// <summary>
        /// Procedure fixed inital part
        /// 存储过程固定首部分
        /// </summary>
        protected ReadOnlyMemory<char> ProcedureFixInitals { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="user">Current user</param>
        /// <param name="flag">Flag</param>
        /// <param name="procedureInitals">Procedure initials</param>
        public EntityRepository(ICoreApplication<C> app, ICurrentUser<T, O> user, string flag, string procedureInitals = "p", char? procedureJoinChar = '_') : base(app, user)
        {
            Flag = flag.AsMemory();
            ProcedureJoinChar = procedureJoinChar.HasValue ? new char[] { procedureJoinChar.Value } : Array.Empty<char>();

            var p = procedureInitals.AsSpan();

            // app, not App, because override exists, null is here now
            // Always use visible parameters
            var builder = new MemoryBuilder<char>(app.Configuration.AppId.Length + p.Length + ProcedureJoinChar.Length + Flag.Length + ProcedureJoinChar.Length);
            builder.Append(app.Configuration.AppId);
            builder.Append(p);
            builder.Append(ProcedureJoinChar.Span);
            builder.Append(Flag.Span);
            builder.Append(ProcedureJoinChar.Span);

            ProcedureFixInitals = builder.AsMemory();
        }

        /// <summary>
        /// Get command name, concat with AppId and Flag, normally is stored procedure name, pay attention to SQL injection
        /// 获取命令名称，附加程序编号和实体标识，一般是存储过程名称，需要防止注入攻击
        /// </summary>
        /// <param name="part">Part</param>
        /// <returns>Command name</returns>
        protected virtual string GetCommandName(ReadOnlySpan<char> part)
        {
            return string.Concat(
                ProcedureFixInitals.Span,
                part
            );
        }

        /// <summary>
        /// Get command name, concat with AppId and Flag, normally is stored procedure name, pay attention to SQL injection
        /// 获取命令名称，附加程序编号和实体标识，一般是存储过程名称，需要防止注入攻击
        /// </summary>
        /// <param name="part1">Part 1</param>
        /// <param name="part2">Part 2</param>
        /// <returns>Command name</returns>
        protected virtual string GetCommandName(ReadOnlySpan<char> part1, ReadOnlySpan<char> part2)
        {
            var builder = new SpanBuilder<char>(part1.Length + ProcedureJoinChar.Length + part2.Length);
            builder.Append(part1);
            builder.Append(ProcedureJoinChar.Span);
            builder.Append(part2);
            return GetCommandName(builder.AsSpan());
        }

        /// <summary>
        /// Get command name, concat with AppId and Flag, normally is stored procedure name, pay attention to SQL injection
        /// 获取命令名称，附加程序编号和实体标识，一般是存储过程名称，需要防止注入攻击
        /// </summary>
        /// <param name="part1">Part 1</param>
        /// <param name="part2">Part 2</param>
        /// <param name="part3">Part 3</param>
        /// <returns>Command name</returns>
        protected virtual string GetCommandName(ReadOnlySpan<char> part1, ReadOnlySpan<char> part2, ReadOnlySpan<char> part3)
        {
            var jLen = ProcedureJoinChar.Length;
            var builder = new SpanBuilder<char>(part1.Length + jLen + part2.Length + jLen + part3.Length);
            builder.Append(part1);
            builder.Append(ProcedureJoinChar.Span);
            builder.Append(part2);
            builder.Append(ProcedureJoinChar.Span);
            builder.Append(part3);
            return GetCommandName(builder.AsSpan());
        }

        /// <summary>
        /// Create entity
        /// 创建实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async Task<IActionResult> CreateAsync(object model)
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
            parameters.Add("ids", App.DB.AsListParameter(ids));

            AddSystemParameters(parameters);

            return CreateCommand(GetCommandName("delete"), parameters);
        }

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        public virtual async Task<IActionResult> DeleteAsync(T id)
        {
            return await DeleteAsync(new T[] { id });
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        public virtual async Task<IActionResult> DeleteAsync(IEnumerable<T> ids)
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
        protected virtual CommandDefinition NewReadCommand(T id, ReadOnlySpan<char> range, DataFormat? format = null)
        {
            // Avoid possible SQL injection attack
            FilterRange(range);

            // read_for
            var key = string.Concat("read", ProcedureJoinChar.Span, "for");
            
            var name = format.HasValue ? GetCommandName(key, range, format.Value.ToString().ToLower()) : GetCommandName(key, range);

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
        protected virtual CommandDefinition NewReportCommand(ReadOnlySpan<char> range, object? model = null, DataFormat? format = null)
        {
            // Avoid possible SQL injection attack
            FilterRange(range);

            var parameters = FormatParameters(model ?? new DynamicParameters());

            AddSystemParameters(parameters);

            var key = "read".AsSpan();
            var name = format.HasValue ? GetCommandName(key, range, format.Value.ToString().ToLower()) : GetCommandName(key, range);
            return CreateCommand(name, parameters);
        }

        /// <summary>
        /// Entity report
        /// 实体报告
        /// </summary>
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
        public virtual async Task<IActionResult> UpdateAsync<D>(D model) where D : IdModel<T>
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

            var command = CreateCommand(GetCommandName("list_json"), parameters);

            await ReadJsonToStreamAsync(command, response);
        }

        /// <summary>
        /// Query data
        /// 查询数据
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        public async Task QueryAsync<D>(D model, HttpResponse response) where D : QueryRQ
        {
            var parameters = FormatParameters(model);

            AddSystemParameters(parameters);

            var command = CreateCommand(GetCommandName("query_json"), parameters);

            await ReadJsonToStreamAsync(command, response);
        }
    }
}
