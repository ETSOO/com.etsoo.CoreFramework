using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using System;
using System.Collections.Generic;

namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Action result base interface
    /// https://tools.ietf.org/html/rfc7807
    /// 操作结果基础接口
    /// </summary>
    public interface IActionResult : IJsonSerialization
    {
        /// <summary>
        /// Successful or not result
        /// 是否为成功结果
        /// </summary>
        bool Success { get; init; }

        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        Uri Type { get; init; }

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        string? Title { get; init; }

        /// <summary>
        /// Detail
        /// 细节
        /// </summary>
        string? Detail { get; init; }

        /// <summary>
        /// Instance, API URI
        /// 实例，一般为接口地址
        /// </summary>
        Uri? Instance { get; init; }

        /// <summary>
        /// The HTTP status code
        /// HTTP状态码
        /// </summary>
        int? Status { get; init; }

        /// <summary>
        /// Problem field
        /// 问题字段
        /// </summary>
        string? Field { get; init; }

        /// <summary>
        /// Log trace id
        /// 日志跟踪编号
        /// </summary>
        string? TraceId { get; init; }

        /// <summary>
        /// Errors, grouped by field name
        /// 错误，按字段名称分组
        /// </summary>
        Dictionary<string, string[]>? Errors { get; }

        /// <summary>
        /// Additional data
        /// 更多数据
        /// </summary>
        StringKeyDictionaryObject Data { get; init; }

        /// <summary>
        /// Add error
        /// 添加错误
        /// </summary>
        /// <param name="error">Error</param>
        void AddError(ActionResultError error);

        /// <summary>
        /// Add errors
        /// 添加多个错误
        /// </summary>
        /// <param name="errors">Errors</param>
        void AddErrors(IEnumerable<ActionResultError> errors);

        /// <summary>
        /// Has any error
        /// 是否有任何错误
        /// </summary>
        /// <returns>Has or not</returns>
        bool HasError();

        /// <summary>
        /// Has specific field error
        /// 是否存在特定字段的错误
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Has or not</returns>
        bool HasError(string name);

        /// <summary>
        /// Data to id modal
        /// 转化数据为编号模块
        /// </summary>
        /// <typeparam name="T">Generic id type</typeparam>
        /// <returns>Modal</returns>
        IdModal<T> DataAsIdModal<T>();

        /// <summary>
        /// Data to int id modal
        /// 转化数据为整数编号模块
        /// </summary>
        /// <returns>Modal</returns>
        IdModal<int> DataAsIdModal();
    }
}
