using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
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
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Base repository
    /// 基础仓库
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public abstract class RepositoryBase<C> : IRepositoryBase where C : DbConnection
    {
        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        virtual protected ICoreApplication<C> App { get; }

        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        virtual protected ICurrentUser? User { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        public RepositoryBase(ICoreApplication<C> app, ICurrentUser? user) => (App, User) = (app, user);

        /// <summary>
        /// Add default parameters
        /// 添加默认参数
        /// </summary>
        /// <param name="parameters">Parameters collection</param>
        protected virtual void AddDefaultParameters(DynamicParameters parameters)
        {

        }

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
            if(parameters != null)
            {
                AddDefaultParameters(parameters);
            }

            return new CommandDefinition(name, parameters, commandType: type);
        }

        /// <summary>
        /// Create model parameters command
        /// 创建模块参数命令
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="name">Command text</param>
        /// <param name="type">Command type</param>
        /// <returns>Command</returns>
        protected CommandDefinition CreateModelCommand<M>(M? model, string name, CommandType type = CommandType.StoredProcedure) where M : class
        {
            return CreateCommand(name, ModelToParameters(model), type);
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
        /// Model to parameters
        /// 模块转化为参数
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>Parameters</returns>
        protected DynamicParameters? ModelToParameters<M>(M? model) where M : class
        {
            if (model == null)
                return null;

            if (model is IAutoParameters ap)
            {
                return ap.AsParameters();
            }

            if (model is IModelParameters p)
            {
                return p.AsParameters(App, User);
            }

            return null;
        }

        /// <summary>
        /// Async query command as object
        /// 异步执行命令返回对象
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public async Task<IEnumerable<T>> QueryAsAsync<T>(CommandDefinition command, Func<Task<DbDataReader>, Task<IEnumerable<T>>> callback)
        {
            return await App.DB.WithConnection((connection) =>
            {
                var reader = connection.ExecuteReaderAsync(command);
                return callback(reader);
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
            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryAsResultAsync(command);
            }) ?? ApplicationErrors.NoActionResult.AsResult();
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
