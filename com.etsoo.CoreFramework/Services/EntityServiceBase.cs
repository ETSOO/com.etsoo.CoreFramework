using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.Repositories;
using com.etsoo.CoreFramework.User;
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
    /// <typeparam name="T">Generic user id type</typeparam>
    /// <typeparam name="O">Generic organization id type</typeparam>
    public abstract class EntityServiceBase<C, R, T, O> : LoginedServiceBase<C, R, T, O>
        where C : DbConnection
        where R : IEntityRepository<T, O>
        where T : struct
        where O : struct
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">App</param>
        /// <param name="user">User</param>
        /// <param name="repo">Repository</param>
        /// <param name="logger">Logger</param>
        public EntityServiceBase(ICoreApplication<C> app, ICurrentUser<T, O> user, R repo, ILogger logger) : base(app, user, repo, logger)
        {
        }

        /// <summary>
        /// Create
        /// 创建
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async Task<IActionResult> CreateAsync(object model)
        {
            return await Repo.CreateAsync(model);
        }

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        public virtual async Task<IActionResult> DeleteAsync(T id)
        {
            return await Repo.DeleteAsync(id);
        }

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        public virtual async Task<IActionResult> DeleteAsync(IEnumerable<T> ids)
        {
            return await Repo.DeleteAsync(ids);
        }

        /// <summary>
        /// List
        /// 列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        public async Task ListAsync(TiplistRQ<T> model, HttpResponse response)
        {
            await Repo.ListAsync(model, response);
        }

        /// <summary>
        /// Query
        /// 查询
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        public virtual async Task QueryAsync(QueryRQ model, HttpResponse response)
        {
            await Repo.QueryAsync(model, response);
        }

        /// <summary>
        /// Read
        /// 浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        public async Task ReadAsync(T id, HttpResponse response)
        {
            await Repo.ReadAsync(response, id);
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
            await Repo.ReportAsync(response, range, modal);
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        public virtual async Task<IActionResult> UpdateAsync(UpdateModel<T> model)
        {
            return await Repo.UpdateAsync(model);
        }

        /// <summary>
        /// Read for updae
        /// 更新浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        public async Task UpdateReadAsync(T id, HttpResponse response)
        {
            await Repo.ReadAsync(response, id, "update");
        }
    }
}
