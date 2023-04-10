using com.etsoo.CoreFramework.Models;
using com.etsoo.Utils.Actions;
using Microsoft.AspNetCore.Http;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Entity service interface
    /// 实体服务接口
    /// </summary>
    public interface IEntityServiceBase<T> : IServiceBase where T : struct
    {
        /// <summary>
        /// Create
        /// 创建
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> CreateAsync(object model);

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> DeleteAsync(T id);

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> DeleteAsync(IEnumerable<T> ids);

        /// <summary>
        /// List
        /// 列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key</param>
        /// <returns>Task</returns>
        Task ListAsync(TiplistRQ<T> model, HttpResponse response, string? queryKey = null);

        /// <summary>
        /// Merge
        /// 合并
        /// </summary>
        /// <typeparam name="M">Generic request data type</typeparam>
        /// <param name="rq">Request data</param>
        /// <returns>Result</returns>
        ValueTask<IActionResult> MergeAsync<M>(M rq) where M : MergeRQ;

        /// <summary>
        /// Query
        /// 查询
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key word, default is empty</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        Task QueryAsync<E>(QueryRQ<E> model, HttpResponse response, string? queryKey = null, IEnumerable<string>? collectionNames = null) where E : struct;

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        Task<E> ReadAsync<E>(T id, string range = "default");

        /// <summary>
        /// Read
        /// 浏览
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        Task ReadAsync(HttpResponse response, T id, string range = "default", IEnumerable<string>? collectionNames = null);

        /// <summary>
        /// Entity JSON report to HTTP Response
        /// 实体报告JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        Task ReportAsync(HttpResponse response, string range, object? modal = null, IEnumerable<string>? collectionNames = null);

        /// <summary>
        /// Sort data
        /// 数据排序
        /// </summary>
        /// <param name="sortData">Data to sort</param>
        /// <returns>Rows affected</returns>
        Task<int> SortAsync(Dictionary<T, short> sortData);

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> UpdateAsync(UpdateModel<T> model);

        /// <summary>
        /// Read for updae
        /// 更新浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        Task UpdateReadAsync(T id, HttpResponse response, IEnumerable<string>? collectionNames = null);
    }
}
