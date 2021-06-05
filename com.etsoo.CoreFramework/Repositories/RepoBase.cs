using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Database;
using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.SpanMemory;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;

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
        protected CommandDefinition CreateCommand(string name, object? parameters = null, CommandType type = CommandType.StoredProcedure)
        {
            return new CommandDefinition(name, FormatParameters(parameters), commandType: type);
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
            var valid = range.All(char.IsLetterOrDigit);

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
        virtual protected object? FormatParameters(object? parameters)
        {
            if (parameters == null)
                return null;

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

            return parameters;
        }

        /// <summary>
        /// Async query command as object
        /// 异步执行命令返回对象
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public async Task<T?> QueryAsAsync<T>(CommandDefinition command, Func<Task<DbDataReader>, Task<IEnumerable<T>>> callback)
        {
            var list = await QueryAsListAsync<T>(command, callback);
            return list.FirstOrDefault();
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public async Task<IEnumerable<T>> QueryAsListAsync<T>(CommandDefinition command, Func<Task<DbDataReader>, Task<IEnumerable<T>>> callback)
        {
            return await App.DB.WithConnection((connection) =>
            {
                var readerTask = connection.ExecuteReaderAsync(command);
                return callback(readerTask);
            });
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
    }
}
