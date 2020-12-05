using System;
using System.Collections.Generic;

namespace com.etsoo.CoreFramework.ActionResult
{
    /// <summary>
    /// Action result interface
    /// https://tools.ietf.org/html/rfc7807
    /// 操作结果接口
    /// </summary>
    /// <typeparam name="T">Generic success data type</typeparam>
    /// <typeparam name="F">Generic failure data type</typeparam>
    public interface IActionResult<T, F>
    {
        /// <summary>
        /// Successful or not result
        /// 是否为成功结果
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        Uri Type { get; set; }

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        string? Title { get; set; }

        /// <summary>
        /// Detail
        /// 细节
        /// </summary>
        string? Detail { get; set; }

        /// <summary>
        /// Instance, API URI
        /// 实例，一般为接口地址
        /// </summary>
        Uri? Instance { get; set; }

        /// <summary>
        /// The HTTP status code
        /// HTTP状态码
        /// </summary>
        int? Status { get; set; }

        /// <summary>
        /// Problem field
        /// 问题字段
        /// </summary>
        string? Field { get; set; }

        /// <summary>
        /// Log trace id
        /// 日志跟踪编号
        /// </summary>
        string? TraceId { get; set; }

        /// <summary>
        /// Related success data
        /// 关联的成功数据
        /// </summary>
        T? Data { get; set; }

        /// <summary>
        /// Related failure data
        /// 关联的失败数据
        /// </summary>
        F? DataFailure { get; set; }

        /// <summary>
        /// Errors, grouped by field name
        /// 错误，按字段名称分组
        /// </summary>
        Dictionary<string, string[]>? Errors { get; }

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
    }
}
