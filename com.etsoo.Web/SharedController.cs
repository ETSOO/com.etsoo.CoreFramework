using com.etsoo.CoreFramework.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Net.Mime;
using IActionResult = com.etsoo.Utils.Actions.IActionResult;

namespace com.etsoo.Web
{
    /// <summary>
    /// Shared controller for Web API
    /// Web API的共享控制器
    /// </summary>
    public abstract class SharedController : ControllerBase
    {
        /// <summary>
        /// Core application
        /// 核心程序对象
        /// </summary>
        protected readonly ICoreApplicationBase CoreApp;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public SharedController(ICoreApplicationBase coreApp) : base()
        {
            CoreApp = coreApp;
        }

        /// <summary>
        /// Write header
        /// 写入头部信息
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        protected void WriteHeader(string key, StringValues value)
        {
            Response.Headers.Add(new KeyValuePair<string, StringValues>(key, value));
        }

        /// <summary>
        /// Direct write action result
        /// 直接写操作结果
        /// </summary>
        /// <param name="result">Action result</param>
        /// <returns>Task</returns>
        protected async Task WriteResultAsync(IActionResult result)
        {
            // Content type
            Response.ContentType = MediaTypeNames.Application.Json;

            // Write
            await result.ToJsonAsync(Response.BodyWriter, CoreApp.DefaultJsonSerializerOptions);
        }
    }
}
