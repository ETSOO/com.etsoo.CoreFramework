using com.etsoo.Utils.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Entity repository interface for CURD(Create, Update, Read, Delete)
    /// 实体仓库接口，实现增删改查
    /// </summary>
    public interface IEntityRepository<T> : IRepositoryBase where T : struct, IComparable
    {
        /// <summary>
        /// Create entity
        /// 创建实体
        /// </summary>
        /// <typeparam name="M">Generic entity model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        Task<IActionResult> CreateAsync<M>(M model) where M : class;

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
        /// Entity report
        /// 实体报告
        /// </summary>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <returns>Task</returns>
        Task<IEnumerable<E>> ReportAsync<M, E>(string range, M? modal = null) where M : class;

        /// <summary>
        /// Entity report to stream
        /// 实体报告数据到流
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        Task ReportAsync<M>(Stream stream, string range, M? modal = null, DataFormat format = DataFormat.JSON) where M : class;

        /// <summary>
        /// Entity report to PipeWriter
        /// 实体报告数据到PipeWriter
        /// </summary>
        /// <param name="writer">PipeWriter</param>
        /// <param name="range">View range</param>
        /// <param name="modal">Condition modal</param>
        /// <param name="format">Data format</param>
        /// <returns>Task</returns>
        Task ReportAsync<M>(PipeWriter writer, string range, M? modal = null, DataFormat format = DataFormat.JSON) where M : class;

        /// <summary>
        /// Update entity
        /// 更新实体
        /// </summary>
        /// <typeparam name="M">Generic entity model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>Action result</returns>
        Task<IActionResult> UpdateAsync<M>(M model) where M : class;
    }
}
