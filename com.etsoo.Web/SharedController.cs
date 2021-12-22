using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Services;
using com.etsoo.UserAgentParser;
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
        /// Async parse device core for multiple decryption
        /// 异步解析设备核心密码以用于多次解密
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="userAgent">User agent</param>
        /// <returns>Result</returns>
        protected (IActionResult result, string? serviceCore) ParseDeviceCore(IServiceBase service, string deviceId, string? userAgent)
        {
            var result = ParseUserAgent(userAgent, out string identifier);
            if (!result.Ok) return (result, null);

            var deviceCore = service.DecryptDeviceCore(deviceId, identifier);
            if (deviceCore == null)
            {
                return (ApplicationErrors.NoValidData.AsResult("Device"), null);
            }

            return (Utils.Actions.ActionResult.Success, deviceCore);
        }

        /// <summary>
        /// Async parse device message
        /// 异步解析设备信息
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="encryptedMessage">Encypted message</param>
        /// <param name="userAgent">User agent</param>
        /// <returns>Result</returns>
        protected (IActionResult result, string? data) ParseDeviceMessage(IServiceBase service, string deviceId, string encryptedMessage, string? userAgent)
        {
            var (result, deviceCore) = ParseDeviceCore(service, deviceId, userAgent);
            if (!result.Ok || deviceCore == null) return (result, null);

            var data = service.DecryptDeviceData(encryptedMessage, deviceCore);
            if (data == null)
            {
                return (ApplicationErrors.NoValidData.AsResult(), null);
            }

            return (result, data);
        }

        /// <summary>
        /// Parse user agent
        /// 解析用户代理信息
        /// </summary>
        /// <param name="userAgent">User agent</param>
        /// <param name="identifier">Identifier</param>
        /// <returns>Action result</returns>
        protected IActionResult ParseUserAgent(string? userAgent, out string identifier)
        {
            identifier = string.Empty;

            // User-Agent validatation
            var parser = new UAParser(userAgent);
            if (!parser.Valid || parser.IsBot)
                return ApplicationErrors.NoUserAgent.AsResult();

            identifier = parser.ToShortName();

            return Utils.Actions.ActionResult.Success;
        }

        /// <summary>
        /// Parse user agent
        /// 解析用户代理信息
        /// </summary>
        /// <param name="userAgent">User agent</param>
        /// <param name="parser">UAParser</param>
        /// <returns>Action result</returns>
        protected IActionResult ParseUserAgent(string? userAgent, out UAParser parser)
        {
            // User-Agent validatation
            parser = new UAParser(userAgent);
            if (!parser.Valid || parser.IsBot)
                return ApplicationErrors.NoUserAgent.AsResult();

            return Utils.Actions.ActionResult.Success;
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
