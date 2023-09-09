using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Microsoft.AspNetCore.Http;
using System.Buffers;

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
        ValueTask<IActionResult> CreateAsync(object model);

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="range">Range</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> DeleteAsync(T id, string? range = null);

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <param name="range">Range</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> DeleteAsync(IEnumerable<T> ids, string? range = null);

        /// <summary>
        /// Merge
        /// 合并
        /// </summary>
        /// <typeparam name="M">Generic request data type</typeparam>
        /// <param name="rq">Request data</param>
        /// <returns>Result</returns>
        ValueTask<IActionResult> MergeAsync<M>(M rq) where M : MergeRQ;

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        ValueTask<E?> ReadAsync<E>(T id, string range = "default") where E : IDataReaderParser<E>;

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="E2">Generic dataset 2 object type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        Task<(E1?, E2[])> ReadAsync<E1, E2>(T id, string range = "default")
            where E1 : IDataReaderParser<E1>
            where E2 : IDataReaderParser<E2>;

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="E2">Generic dataset 2 object type</typeparam>
        /// <typeparam name="E3">Generic dataset 3 object type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        Task<(E1?, E2[], E3[])> ReadAsync<E1, E2, E3>(T id, string range = "default")
            where E1 : IDataReaderParser<E1>
            where E2 : IDataReaderParser<E2>
            where E3 : IDataReaderParser<E3>;

        /// <summary>
        /// View entity with direct way
        /// 直接方式浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <returns>Entity</returns>
        Task<E?> ReadDirectAsync<E>(T id, string range = "default");

        /// <summary>
        /// View entity to PipeWriter
        /// 浏览实体数据到PipeWriter
        /// </summary>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="id">Entity id</param>
        /// <param name="range">View range</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        Task ReadAsync(IBufferWriter<byte> writer, T id, string range = "default", DataFormat? format = null, IEnumerable<string>? collectionNames = null);

        /// <summary>
        /// View entity JSON data HTTP Response
        /// 浏览实体JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        Task ReadAsync(HttpResponse response, T id, string range = "default", IEnumerable<string>? collectionNames = null);

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
        /// Entity report to PipeWriter
        /// 实体报告数据到PipeWriter
        /// </summary>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Task</returns>
        Task ReportAsync(IBufferWriter<byte> writer, string range, object? modal = null, DataFormat? format = null, IEnumerable<string>? collectionNames = null);

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
        /// Update entity
        /// 更新实体
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> UpdateAsync<M>(M model) where M : IdItem<T>, IUpdateModel;

        /// <summary>
        /// Update status
        /// 更新状态
        /// </summary>
        /// <param name="rq">Request data</param>
        /// <returns>Result</returns>
        ValueTask<IActionResult> UpdateStatusAsync(UpdateStatusRQ<T> rq);

        /// <summary>
        /// Data list
        /// 数据列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key</param>
        /// <returns>Task</returns>
        Task ListAsync(TiplistRQ<T> model, HttpResponse response, string? queryKey = null);

        /// <summary>
        /// Query data
        /// 查询数据
        /// </summary>
        /// <typeparam name="E">Generic query id type</typeparam>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key word, default is empty</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <returns>Task</returns>
        Task QueryAsync<E, M>(M model, HttpResponse response, string? queryKey = null, IEnumerable<string>? collectionNames = null) where E : struct where M : QueryRQ<E>;

        /// <summary>
        /// Quick update
        /// 快速更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <param name="additionalPart">Additional SQL command part, please avoid injection</param>
        /// <param name="additionalParams">Additional parameters</param>
        /// <returns>Result</returns>
        ValueTask<(IActionResult Result, UpdateResultData<T>? Data)> QuickUpdateAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null) where M : IdItem<T>, IUpdateModel;
    }
}
