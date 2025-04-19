using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;

namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Action result interface
    /// 操作结果接口
    /// </summary>
    public interface IActionResult : IJsonSerialization
    {
        /// <summary>
        /// Ok or not
        /// 是否成功
        /// </summary>
        bool Ok { get; }

        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        string? Type { get; }

        /// <summary>
        /// Title
        /// 标题
        /// </summary>
        string? Title { get; set; }

        /// <summary>
        /// Field
        /// 字段
        /// </summary>
        string? Field { get; }

        /// <summary>
        /// Status code
        /// 状态码
        /// </summary>
        int? Status { get; }

        /// <summary>
        /// Detail
        /// 细节
        /// </summary>
        string? Detail { get; set; }

        /// <summary>
        /// Trace id
        /// 跟踪编号
        /// </summary>
        string? TraceId { get; set; }

        /// <summary>
        /// Data
        /// 数据
        /// </summary>
        StringKeyDictionaryObject Data { get; }

        /// <summary>
        /// To Json
        /// 转化为 Json
        /// </summary>
        /// <param name="utf8Stream">Stream to writer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task ToJsonAsync(Stream utf8Stream, CancellationToken cancellationToken = default);
    }
}
