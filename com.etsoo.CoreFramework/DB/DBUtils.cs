using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Database;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace com.etsoo.CoreFramework.DB
{
    /// <summary>
    /// Database utilities
    /// 数据库工具
    /// </summary>
    public static class DBUtils
    {
        /// <summary>
        /// Set JSON content type
        /// 设置 JSON 内容类型
        /// </summary>
        /// <param name="response">HTTP Response</param>
        public static void JsonContentType(this HttpResponse response)
        {
            response.ContentType = "application/json";
        }

        /// <summary>
        /// Write raw JSON string
        /// 输出原始 JSON zifc
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <param name="raw">Raw JSON string</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public static async Task WriteRawJson(this HttpResponse response, string raw, CancellationToken cancellationToken = default)
        {
            // Content type
            response.JsonContentType();
            if (!string.IsNullOrEmpty(raw))
                await response.WriteAsync(raw, cancellationToken);
            else
                response.StatusCode = (int)HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Format parameters, extends DatabaseUtils.FormatParameters
        /// 格式化参数，扩展 DatabaseUtils.FormatParameters
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <param name="app">Application</param>
        /// <returns>Result</returns>
        public static IDbParameters FormatParameters(object parameters, ICoreApplicationBase app)
        {
            if (parameters is IModelParameters p)
            {
                return p.AsParameters(app);
            }

            return DatabaseUtils.FormatParameters(parameters) ?? new DbParameters();
        }
    }
}
