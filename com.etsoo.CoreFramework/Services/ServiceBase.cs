﻿using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Database;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Service base for business logic
    /// 业务逻辑的基础服务
    /// </summary>
    public abstract class ServiceBase<C, R>
        where C : DbConnection
        where R : IRepoBase
    {
        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        virtual protected ICoreApplication<C> App { get; }

        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Database repository
        /// 数据库仓库
        /// </summary>
        protected R Repo { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        public ServiceBase(ICoreApplication<C> app, R repo, ILogger logger)
        {
            App = app;
            Repo = repo;
            Logger = logger;
        }

        /// <summary>
        /// Log exception and return simple user result
        /// 登记异常结果日志，并返回简介的用户结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        protected IActionResult LogException(Exception ex)
        {
            // Get the Db connection failure result
            var exResult = App.DB.GetExceptionResult(ex);

            // Transform
            var result = exResult.Type switch
            {
                DbExceptionType.OutOfMemory => ApplicationErrors.OutOfMemory.AsResult(),
                DbExceptionType.ConnectionFailed => ApplicationErrors.DbConnectionFailed.AsResult(),
                _ => ApplicationErrors.DataProcessingFailed.AsResult()
            };

            // Log the exception
            LogException(ex, result.Title!, exResult.Critical);

            // Return
            return result;
        }

        /// <summary>
        /// Log exception
        /// 登记异常日志
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="title">Title</param>
        /// <param name="critical">Is critical</param>
        protected void LogException(Exception ex, string title, bool critical = false)
        {
            if (critical)
                Logger.LogCritical(ex, title);
            else
                Logger.LogError(ex, title);
        }

        /// <summary>
        /// Async read JSON data to HTTP Response
        /// 异步读取JSON数据到HTTP响应
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        protected async Task ReaJsonToStreamAsync(CommandDefinition command, HttpResponse response)
        {
            // Content type
            response.ContentType = "application/json";

            // Write to
            await Repo.ReadToStreamAsync(command, response.BodyWriter);
        }
    }
}
