﻿using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Entity service
    /// 实体服务
    /// </summary>
    /// <typeparam name="C">Generic connection type</typeparam>
    /// <typeparam name="R">Generic repository type</typeparam>
    /// <typeparam name="T">Generic id type</typeparam>
    public abstract class EntityServiceBase<C, R, T, A> : ServiceBase<C, R, A>, IEntityServiceBase<T>
        where C : DbConnection
        where R : IEntityRepo<T>
        where T : struct
        where A : ICoreApplication<C>
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        protected readonly IServiceUser? User;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">App</param>
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        public EntityServiceBase(A app, R repo, ILogger logger)
            : base(app, repo, logger)
        {
            // RepoBase.AddSystemParameters will check when the user login required
            // For constructor, userAccessor.UserSafe can also check
            User = repo.User;
        }

        /// <summary>
        /// Create
        /// 创建
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<IActionResult> CreateAsync(object model)
        {
            return await Repo.CreateAsync(model);
        }

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<IActionResult> DeleteAsync(T id)
        {
            return await Repo.DeleteAsync(id);
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<IActionResult> DeleteAsync(IEnumerable<T> ids)
        {
            return await Repo.DeleteAsync(ids);
        }

        /// <summary>
        /// List
        /// 列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key</param>
        /// <returns>Task</returns>
        public virtual async Task ListAsync(TiplistRQ<T> model, HttpResponse response, string? queryKey = null)
        {
            await Repo.ListAsync(model, response, queryKey);
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
            return await Repo.MergeAsync(rq);
        }

        /// <summary>
        /// Query
        /// 查询
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key word, default is empty</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        public virtual async Task QueryAsync<E>(QueryRQ<E> model, HttpResponse response, string? queryKey = null, IEnumerable<string>? collectionNames = null) where E : struct
        {
            await Repo.QueryAsync<E, QueryRQ<E>>(model, response, queryKey, collectionNames);
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
            return await Repo.ReadAsync<E>(id, range);
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
            return await Repo.ReadDirectAsync<E>(id, range);
        }

        /// <summary>
        /// Read
        /// 浏览
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        public virtual async Task ReadAsync(HttpResponse response, T id, string range = "default", IEnumerable<string>? collectionNames = null)
        {
            await Repo.ReadAsync(response, id, range, collectionNames);
        }

        /// <summary>
        /// Entity JSON report to HTTP Response
        /// 实体报告JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        public virtual async Task ReportAsync(HttpResponse response, string range, object? modal = null, IEnumerable<string>? collectionNames = null)
        {
            await Repo.ReportAsync(response, range, modal, collectionNames);
        }

        /// <summary>
        /// Sort data
        /// 数据排序
        /// </summary>
        /// <param name="sortData">Data to sort</param>
        /// <returns>Rows affected</returns>
        public virtual async Task<int> SortAsync(Dictionary<T, short> sortData)
        {
            return await Repo.SortAsync(sortData);
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async ValueTask<IActionResult> UpdateAsync(UpdateModel<T> model)
        {
            return await Repo.UpdateAsync(model);
        }

        /// <summary>
        /// Read for updae
        /// 更新浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        public virtual async Task UpdateReadAsync(T id, HttpResponse response, IEnumerable<string>? collectionNames = null)
        {
            await Repo.ReadAsync(response, id, "update", collectionNames);
        }

        /// <summary>
        /// Update status
        /// 更新状态
        /// </summary>
        /// <param name="rq">Request data</param>
        /// <returns>Result</returns>
        public virtual async ValueTask<IActionResult> UpdateStatusAsync(UpdateStatusRQ<T> rq)
        {
            return await Repo.UpdateStatusAsync(rq);
        }
    }
}
