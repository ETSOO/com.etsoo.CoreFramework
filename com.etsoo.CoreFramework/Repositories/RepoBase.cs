using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Database;
using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.SpanMemory;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;
using System.IO.Pipelines;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Base repository
    /// 基础仓库
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public abstract class RepoBase<C> : IRepoBase where C : DbConnection
    {
        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        virtual protected ICoreApplication<C> App { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        public RepoBase(ICoreApplication<C> app) => (App) = (app);

        /// <summary>
        /// Create command, default parameters added
        /// 创建命令，附加默认参数
        /// </summary>
        /// <param name="name">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="type">Command type</param>
        /// <returns>Command</returns>
        protected CommandDefinition CreateCommand(string name, DynamicParameters? parameters = null, CommandType type = CommandType.StoredProcedure)
        {
            return new CommandDefinition(name, parameters, commandType: type);
        }

        /// <summary>
        /// Filter range
        /// 过滤区域
        /// </summary>
        /// <param name="range"></param>
        /// <param name="triggerFailureExcpetion"></param>
        /// <returns></returns>
        protected bool FilterRange(ReadOnlySpan<char> range, bool triggerFailureExcpetion = true)
        {
            var valid = range.All(c => c is '_' or (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9'));

            if (!valid && triggerFailureExcpetion)
            {
                throw new ArgumentOutOfRangeException(nameof(range));
            }

            return valid;
        }

        /// <summary>
        /// Format parameters
        /// 格式化参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>Result</returns>
        virtual protected DynamicParameters FormatParameters(object parameters)
        {
            if (parameters is DynamicParameters dp)
            {
                return dp;
            }

            if (parameters is IAutoParameters ap)
            {
                return ap.AsParameters();
            }

            if (parameters is IModelParameters p)
            {
                return p.AsParameters(App);
            }

            return new DynamicParameters(parameters);
        }

        /// <summary>
        /// Async query command as object
        /// 异步执行命令返回对象
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async ValueTask<T?> QueryAsAsync<T>(CommandDefinition command) where T : IDataReaderParser<T>
        {
            var list = QueryAsListAsync<T>(command);
            return await list.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async IAsyncEnumerable<T> QueryAsListAsync<T>(CommandDefinition command) where T : IDataReaderParser<T>
        {
            using var connection = App.DB.NewConnection();

            using var reader = await connection.ExecuteReaderAsync(command);

            var items = T.CreateAsync(reader);

            await foreach (var item in items)
            {
                yield return item;
            }

            await reader.CloseAsync();
            await connection.CloseAsync();
        }

        /// <summary>
        /// Async query command as action result
        /// 异步执行命令返回操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public async Task<IActionResult> QueryAsResultAsync(CommandDefinition command)
        {
            var result = await App.DB.WithConnection((connection) =>
            {
                return connection.QueryAsResultAsync(command);
            });

            if (result != null && !result.Success && result.Title == null)
            {
                var name = result.Type.ToString().Split('/')[0];
                var error = ApplicationErrors.Get(name);
                if (error != null)
                {
                    result.Title = error.Title;
                }
            }

            return result ?? ApplicationErrors.NoActionResult.AsResult();
        }

        /// <summary>
        /// Async read text data (JSON/XML) to stream
        /// 异步读取文本数据(JSON或者XML)到流
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="stream">Stream</param>
        /// <returns>Has content or not</returns>
        public async Task<bool> ReadToStreamAsync(CommandDefinition command, Stream stream)
        {
            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryToStreamAsync(command, stream);
            });
        }

        /// <summary>
        /// Async read text data (JSON/XML) to PipeWriter
        /// 异步读取文本数据(JSON或者XML)到PipeWriter
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="writer">PipeWriter</param>
        /// <returns>Has content or not</returns>
        public async Task<bool> ReadToStreamAsync(CommandDefinition command, PipeWriter writer)
        {
            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryToStreamAsync(command, writer);
            });
        }

        /// <summary>
        /// Async read JSON data to HTTP Response
        /// 异步读取JSON数据到HTTP响应
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        public async Task ReadJsonToStreamAsync(CommandDefinition command, HttpResponse response)
        {
            // Content type
            response.ContentType = "application/json";

            // Write to
            await ReadToStreamAsync(command, response.BodyWriter);
        }

        /// <summary>
        /// Quick read data
        /// 快速读取数据
        /// </summary>
        /// <typeparam name="E">Generic return type</typeparam>
        /// <returns>Result</returns>
        public async Task<E> QuickReadAsync<E>(string sql, DynamicParameters? parameters = null)
        {
            var command = CreateCommand(sql, parameters, CommandType.Text);

            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryFirstAsync<E>(command);
            });
        }
    }
}
