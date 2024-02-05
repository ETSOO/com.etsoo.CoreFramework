using com.etsoo.CoreFramework.Application;
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
    /// <typeparam name="A">Generic application type</typeparam>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="app">App</param>
    /// <param name="repo">Repository</param>
    /// <param name="logger">Logger</param>
    public abstract class EntityServiceBase<C, R, T, A>(A app, R repo, ILogger logger) : ServiceBase<C, R, A>(app, repo, logger), IEntityServiceBase<T>
        where C : DbConnection
        where R : IEntityRepo<T>
        where T : struct
        where A : ICoreApplication<C>
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        protected readonly IServiceUser? User = repo.User;

        /// <summary>
        /// Create
        /// 创建
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual ValueTask<IActionResult> CreateAsync(object model)
        {
            return Repo.CreateAsync(model);
        }

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        public virtual ValueTask<IActionResult> DeleteAsync(T id)
        {
            return Repo.DeleteAsync(id);
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        public virtual ValueTask<IActionResult> DeleteAsync(IEnumerable<T> ids)
        {
            return Repo.DeleteAsync(ids);
        }

        /// <summary>
        /// List
        /// 列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key</param>
        /// <returns>Task</returns>
        public virtual Task ListAsync(TiplistRQ<T> model, HttpResponse response, string? queryKey = null)
        {
            return Repo.ListAsync(model, response, queryKey);
        }

        /// <summary>
        /// Merge
        /// 合并
        /// </summary>
        /// <typeparam name="M">Generic request data type</typeparam>
        /// <param name="rq">Request data</param>
        /// <returns>Result</returns>
        public virtual ValueTask<IActionResult> MergeAsync<M>(M rq) where M : MergeRQ
        {
            return Repo.MergeAsync(rq);
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
        public virtual Task QueryAsync<E>(QueryRQ<E> model, HttpResponse response, string? queryKey = null, IEnumerable<string>? collectionNames = null) where E : struct
        {
            return Repo.QueryAsync<E, QueryRQ<E>>(model, response, queryKey, collectionNames);
        }

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        public virtual ValueTask<E?> ReadAsync<E>(T id, string range = "default") where E : IDataReaderParser<E>
        {
            return Repo.ReadAsync<E>(id, range);
        }

        /// <summary>
        /// View entity with direct way
        /// 直接方式浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        public virtual Task<E?> ReadDirectAsync<E>(T id, string range = "default")
        {
            return Repo.ReadDirectAsync<E>(id, range);
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
        public virtual Task ReadAsync(HttpResponse response, T id, string range = "default", IEnumerable<string>? collectionNames = null)
        {
            return Repo.ReadAsync(response, id, range, collectionNames);
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
        public virtual Task ReportAsync(HttpResponse response, string range, object? modal = null, IEnumerable<string>? collectionNames = null)
        {
            return Repo.ReportAsync(response, range, modal, collectionNames);
        }

        /// <summary>
        /// Sort data
        /// 数据排序
        /// </summary>
        /// <param name="sortData">Data to sort</param>
        /// <param name="category">Category to group data</param>
        /// <param name="range">Sort range</param>
        /// <returns>Rows affected</returns>
        public virtual Task<int> SortAsync(Dictionary<T, short> sortData, byte? category = null, string? range = null)
        {
            return Repo.SortAsync(sortData, category, range);
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual ValueTask<IActionResult> UpdateAsync(UpdateModel<T> model)
        {
            return Repo.UpdateAsync(model);
        }

        /// <summary>
        /// Read for updae
        /// 更新浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        public virtual Task UpdateReadAsync(T id, HttpResponse response, IEnumerable<string>? collectionNames = null)
        {
            return Repo.ReadAsync(response, id, "update", collectionNames);
        }

        /// <summary>
        /// Update status
        /// 更新状态
        /// </summary>
        /// <param name="rq">Request data</param>
        /// <returns>Result</returns>
        public virtual ValueTask<IActionResult> UpdateStatusAsync(UpdateStatusRQ<T> rq)
        {
            return Repo.UpdateStatusAsync(rq);
        }
    }
}
