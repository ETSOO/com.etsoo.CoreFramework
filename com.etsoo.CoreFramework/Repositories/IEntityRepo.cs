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
    /// <typeparam name="T">Generic user id type</typeparam>
    /// <typeparam name="O">Generic organization id type</typeparam>
    public interface IEntityRepo<T, O> : IRepoBase
        where T : struct
        where O : struct
    {
        /// <summary>
        /// Create entity
        /// 创建实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        Task<IActionResult> CreateAsync(object model);

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Action result</returns>
        Task<IActionResult> DeleteAsync(T id);

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <returns>Action result</returns>
        Task<IActionResult> DeleteAsync(IEnumerable<T> ids);

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
        /// Update entity
        /// 更新实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        Task<IActionResult> UpdateAsync<D>(D model) where D : IUpdateModel<T>;

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
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <returns>Task</returns>
        Task QueryAsync<D>(D model, HttpResponse response) where D : QueryRQ;

        /// <summary>
        /// Quick update
        /// 快速更新
        /// </summary>
        /// <typeparam name="D">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <returns>Result</returns>
        Task<IActionResult> QuickUpdateAsync<D>(D model, QuickUpdateConfigs configs) where D : IUpdateModel<T>;
    }
}
