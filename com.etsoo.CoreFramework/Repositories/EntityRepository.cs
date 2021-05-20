using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.User;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.SpanMemory;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Entity repository for CURD(Create, Update, Read, Delete)
    /// 实体仓库，实现增删改查
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    /// <typeparam name="T">Generic id type</typeparam>
    public abstract class EntityRepository<C, T> : RepositoryBase<C>, IEntityRepository<T> where C : DbConnection where T : struct, IComparable
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
        public EntityRepository(ICoreApplication<C> app, ICurrentUser? user, string flag, string procedureInitals = "p", char? procedureJoinChar = '_') : base(app, user)
        {
            Flag = flag.AsMemory();
            ProcedureJoinChar = procedureJoinChar.HasValue ? new char[] { procedureJoinChar.Value } : Array.Empty<char>();

            var p = procedureInitals.AsSpan();
            var builder = new MemoryBuilder<char>(App.Configuration.AppId.Length + p.Length + ProcedureJoinChar.Length + Flag.Length + ProcedureJoinChar.Length);
            builder.Append(App.Configuration.AppId);
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
        /// Create entity command
        /// 创建实体命令
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewCreateCommand<M>(M model) where M : class
        {
            var name = GetCommandName("create");
            return CreateCommand(name, model);
        }

        /// <summary>
        /// Create entity
        /// 创建实体
        /// </summary>
        /// <typeparam name="M">Generic entity model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public async Task<IActionResult> CreateAsync<M>(M model) where M : class
        {
            var command = NewCreateCommand(model);
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
            var name = GetCommandName("delete");

            var parameters = new DynamicParameters();
            parameters.Add("ids", App.DB.AsListParameter(ids));

            return CreateCommand(name, parameters);
        }

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        public async Task<IActionResult> DeleteAsync(T id)
        {
            return await DeleteAsync(new T[] { id });
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        public async Task<IActionResult> DeleteAsync(IEnumerable<T> ids)
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

            var key = "read".AsSpan();
            var name = format.HasValue ? GetCommandName(key, range, format.Value.ToString().ToLower()) : GetCommandName(key, range);

            var parameters = new DynamicParameters();
            parameters.Add("id", id);

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
        public async Task<E> ReadAsync<E>(T id, string range = "default")
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
        public async Task ReadAsync(Stream stream, T id, string range = "default", DataFormat format = DataFormat.JSON)
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
        public async Task ReadAsync(PipeWriter writer, T id, string range = "default", DataFormat format = DataFormat.JSON)
        {
            var command = NewReadCommand(id, range, format);
            await ReadToStreamAsync(command, writer);
        }

        /// <summary>
        /// Create report command
        /// 创建报表命令
        /// </summary>
        /// <param name="range">Report range</param>
        /// <param name="model">Condition model</param>
        /// <param name="format">Date format</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewReportCommand<M>(ReadOnlySpan<char> range, M? model = null, DataFormat? format = null) where M : class
        {
            // Avoid possible SQL injection attack
            FilterRange(range);

            var key = "read".AsSpan();
            var name = format.HasValue ? GetCommandName(key, range, format.Value.ToString().ToLower()) : GetCommandName(key, range);
            return CreateCommand(name, model);
        }

        /// <summary>
        /// Entity report
        /// 实体报告
        /// </summary>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <returns>Task</returns>
        public async Task<IEnumerable<E>> ReportAsync<M, E>(string range, M? modal = null) where M : class
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
        public async Task ReportAsync<M>(Stream stream, string range, M? modal = null, DataFormat format = DataFormat.JSON) where M : class
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
        public async Task ReportAsync<M>(PipeWriter writer, string range, M? modal = null, DataFormat format = DataFormat.JSON) where M : class
        {
            var command = NewReportCommand(range, modal, format);
            await ReadToStreamAsync(command, writer);
        }

        /// <summary>
        /// Create update entity command
        /// 创建更新实体命令
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Command</returns>
        protected virtual CommandDefinition NewUpdateCommand<M>(M model) where M : class
        {
            var name = GetCommandName("update");
            return CreateCommand(name, model);
        }

        /// <summary>
        /// Update entity
        /// 更新实体
        /// </summary>
        /// <typeparam name="M">Generic entity model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public async Task<IActionResult> UpdateAsync<M>(M model) where M : class
        {
            var command = NewUpdateCommand(model);
            return await QueryAsResultAsync(command);
        }
    }
}
