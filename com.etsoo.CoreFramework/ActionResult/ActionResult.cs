using System;
using System.Collections.Generic;
using System.Linq;

namespace com.etsoo.CoreFramework.ActionResult
{
    /// <summary>
    /// Action result, compatible with RFC7807
    /// https://tools.ietf.org/html/rfc7807
    /// 操作结果
    /// </summary>
    /// <typeparam name="T">Generic success data type</typeparam>
    /// <typeparam name="F">Generic failure data type</typeparam>
    public class ActionResult<T, F> : IActionResult<T, F>
    {
        /// <summary>
        /// Successful or not result
        /// 是否为成功结果
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        public Uri Type { get; set; } = new Uri("about:blank");

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Detail
        /// 细节
        /// </summary>
        public string? Detail { get; set; }

        /// <summary>
        /// Instance, API URI
        /// 实例，一般为接口地址
        /// </summary>
        public Uri? Instance { get; set; }

        /// <summary>
        /// The HTTP status code
        /// HTTP状态码
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Problem field
        /// 问题字段
        /// </summary>
        public string? Field { get; set; }

        /// <summary>
        /// Log trace id
        /// 日志跟踪编号
        /// </summary>
        public string? TraceId { get; set; }

        /// <summary>
        /// Related success data
        /// 关联的成功数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Related failure data
        /// 关联的失败数据
        /// </summary>
        public F? DataFailure { get; set; }

        private readonly List<ActionResultError> errors = new List<ActionResultError>();

        /// <summary>
        /// Errors, grouped by field name
        /// 错误，按字段名称分组
        /// </summary>
        public Dictionary<string, string[]>? Errors
        {
            get
            {
                if (errors.Count == 0)
                    return null;

                return errors.GroupBy(e => e.Name, StringComparer.InvariantCultureIgnoreCase).ToDictionary(k => k.Key, v => v.Select(f => f.Reason).ToArray());
            }
        }

        /// <summary>
        /// Add error
        /// 添加错误
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(ActionResultError error)
        {
            errors.Add(error);
        }

        /// <summary>
        /// Add errors
        /// 添加多个错误
        /// </summary>
        /// <param name="errors">Errors</param>
        public void AddErrors(IEnumerable<ActionResultError> errors)
        {
            this.errors.AddRange(errors);
        }

        /// <summary>
        /// Has any error
        /// 是否有任何错误
        /// </summary>
        /// <returns>Has or not</returns>
        public bool HasError()
        {
            return errors.Count > 0;
        }

        /// <summary>
        /// Has specific field error
        /// 是否存在特定字段的错误
        /// </summary>
        /// <param name="name">Field name</param>
        /// <returns>Has or not</returns>
        public bool HasError(string name)
        {
            return errors.Exists(error => error.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
