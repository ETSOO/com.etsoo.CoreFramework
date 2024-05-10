using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Microsoft.AspNetCore.Http;
using System.Buffers;

namespace com.etsoo.CoreFramework.Services
{
    /// <summary>
    /// Entity service interface
    /// 实体服务接口
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public interface IEntityServiceBase<T> : IServiceBase where T : struct
    {
        /// <summary>
        /// Create
        /// 创建
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> CreateAsync(object model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete single entity
        /// 删除单个实体
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <param name="range">Range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> DeleteAsync(T id, string? range = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete multiple entities
        /// 删除多个实体
        /// </summary>
        /// <param name="ids">Entity ids</param>
        /// <param name="range">Range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> DeleteAsync(IEnumerable<T> ids, string? range = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// List
        /// 列表
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task ListAsync(IModelParameters model, HttpResponse response, string? queryKey = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Merge
        /// 合并
        /// </summary>
        /// <typeparam name="M">Generic request data type</typeparam>
        /// <param name="rq">Request data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<IActionResult> MergeAsync<M>(M rq, CancellationToken cancellationToken = default) where M : MergeRQ;

        /// <summary>
        /// Query
        /// 查询
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="queryKey">Query key word, default is empty</param>
        /// <param name="collectionNames">Collection names</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task QueryAsync<E>(QueryRQ<E> model, HttpResponse response, string? queryKey = null, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default) where E : struct;

        /// <summary>
        /// Quick update
        /// 快速更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <param name="additionalPart">Additional SQL command part, please avoid injection</param>
        /// <param name="additionalParams">Additional parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<(IActionResult Result, UpdateResultData<T>? Data)> QuickUpdateAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null, CancellationToken cancellationToken = default)
            where M : IdItem<T>, IUpdateModel;

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity</returns>
        ValueTask<E?> ReadAsync<E>(T id, string range = "default", CancellationToken cancellationToken = default) where E : IDataReaderParser<E>;

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E">Generic entity data type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity</returns>
        Task<E?> ReadDirectAsync<E>(T id, string range = "default", CancellationToken cancellationToken = default);

        /// <summary>
        /// View entity
        /// 浏览实体
        /// </summary>
        /// <typeparam name="E1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="E2">Generic dataset 2 object type</typeparam>
        /// <param name="id">Entity id</param>
        /// <param name="range">Limited range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity</returns>
        Task<(E1?, E2[])> ReadAsync<E1, E2>(T id, string range = "default", CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Entity</returns>
        Task<(E1?, E2[], E3[])> ReadAsync<E1, E2, E3>(T id, string range = "default", CancellationToken cancellationToken = default)
            where E1 : IDataReaderParser<E1>
            where E2 : IDataReaderParser<E2>
            where E3 : IDataReaderParser<E3>;

        /// <summary>
        /// View entity to PipeWriter
        /// 浏览实体数据到PipeWriter
        /// </summary>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="id">Entity id</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task ReadAsync(IBufferWriter<byte> writer, T id, string range = "default", DataFormat? format = null, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Read
        /// 浏览
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="id">Id</param>
        /// <param name="range">Range</param>
        /// <param name="collectionNames">Collection names</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task ReadAsync(HttpResponse response, T id, string range = "default", IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Entity report
        /// 实体报告
        /// </summary>
        /// <typeparam name="E">Generic list item type</typeparam>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task<IEnumerable<E>> ReportAsync<E>(string range, object? modal = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Entity report to PipeWriter
        /// 实体报告数据到PipeWriter
        /// </summary>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task ReportAsync(IBufferWriter<byte> writer, string range, object? modal = null, DataFormat? format = null, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Entity JSON report to HTTP Response
        /// 实体报告JSON数据到HTTP响应
        /// </summary>
        /// <param name="response">HTTP Response</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task ReportAsync(HttpResponse response, string range, object? modal = null, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sort data
        /// 数据排序
        /// </summary>
        /// <param name="sortData">Data to sort</param>
        /// <param name="category">Category to group data</param>
        /// <param name="range">Sort range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Rows affected</returns>
        Task<int> SortAsync(Dictionary<T, short> sortData, byte? category = null, string? range = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        ValueTask<IActionResult> UpdateAsync<M>(M model, CancellationToken cancellationToken = default) where M : IdItem<T>, IUpdateModel;

        /// <summary>
        /// Read for updae
        /// 更新浏览
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task UpdateReadAsync(T id, HttpResponse response, IEnumerable<string>? collectionNames = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update status
        /// 更新状态
        /// </summary>
        /// <param name="rq">Request data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<IActionResult> UpdateStatusAsync(UpdateStatusRQ<T> rq, CancellationToken cancellationToken = default);
    }
}
