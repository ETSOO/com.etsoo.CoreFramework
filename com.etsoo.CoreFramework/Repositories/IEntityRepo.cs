using com.etsoo.CoreFramework.Models;
using com.etsoo.Utils.Actions;
using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Entity repository interface for CURD(Create, Update, Read, Delete)
    /// 实体仓库接口，实现增删改查
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public interface IEntityRepo<T> : IRepoBase
        where T : struct
    {
        /// <summary>
        /// Create entity
        /// 创建实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        ValueTask<ActionResult> CreateAsync(object model);

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        ValueTask<ActionResult> DeleteAsync(T id);

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        ValueTask<ActionResult> DeleteAsync(IEnumerable<T> ids);

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
        /// View entity to stream
        /// 浏览实体数据到流
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="id">Entity id</param>
        /// <param name="range">View range</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        Task ReadAsync(Stream stream, T id, string range = "default", DataFormat format = DataFormat.JSON);

        /// <summary>
        /// View entity to PipeWriter
        /// 浏览实体数据到PipeWriter
        /// </summary>
        /// <param name="writer">PipeWriter</param>
        /// <param name="id">Entity id</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        Task ReadAsync(PipeWriter writer, T id, string range = "default", DataFormat format = DataFormat.JSON);

        /// <summary>
        /// View entity JSON data HTTP Response
        /// 浏览实体JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <returns>Task</returns>
        Task ReadAsync(HttpResponse response, T id, string range = "default");

        /// <summary>
        /// Entity report
        /// 实体报告
        /// </summary>
        /// <typeparam name="D">Generic list item type</typeparam>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <returns>Task</returns>
        Task<IEnumerable<E>> ReportAsync<E>(string range, object? modal = null);

        /// <summary>
        /// Entity report to stream
        /// 实体报告数据到流
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        Task ReportAsync(Stream stream, string range, object? modal = null, DataFormat format = DataFormat.JSON);

        /// <summary>
        /// Entity report to PipeWriter
        /// 实体报告数据到PipeWriter
        /// </summary>
        /// <param name="writer">PipeWriter</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        Task ReportAsync(PipeWriter writer, string range, object? modal = null, DataFormat format = DataFormat.JSON);

        /// <summary>
        /// Entity JSON report to HTTP Response
        /// 实体报告JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <returns>Task</returns>
        Task ReportAsync(HttpResponse response, string range, object? modal = null);

        /// <summary>
        /// Sort data
        /// 数据排序
        /// </summary>
        /// <param name="sortData">Data to sort</param>
        /// <returns>Rows affected</returns>
        Task<int> SortAsync(Dictionary<T, short> sortData);

        /// <summary>
        /// Update entity
        /// 更新实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        ValueTask<ActionResult> UpdateAsync<M>(M model) where M : IUpdateModel<T>;

        /// <summary>
        /// Data list
        /// 数据列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        Task ListAsync(TiplistRQ<T> model, HttpResponse response);

        /// <summary>
        /// Query data
        /// 查询数据
        /// </summary>
        /// <typeparam name="E">Generic query id type</typeparam>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key word, default is empty</param>
        /// <returns>Task</returns>
        Task QueryAsync<E, M>(M model, HttpResponse response, string? queryKey = null) where E : struct where M : QueryRQ<E>;

        /// <summary>
        /// Quick update
        /// 快速更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <returns>Result</returns>
        ValueTask<(ActionResult, UpdateResultData<T>?)> QuickUpdateAsync<M>(M model, QuickUpdateConfigs configs) where M : IUpdateModel<T>;
    }
}
