using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Services;
using com.etsoo.UserAgentParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Diagnostics.CodeAnalysis;
using System.Net;
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
        /// IP address
        /// IP地址
        /// </summary>
        protected readonly IPAddress Ip;

        /// <summary>
        /// User agent
        /// 用户代理信息
        /// </summary>
        protected readonly string? UserAgent;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="coreApp">Core app</param>
        /// <param name="context">Http context accessor</param>
        public SharedController(ICoreApplicationBase coreApp, IHttpContextAccessor context) : base()
        {
            var ip = context.HttpContext?.Connection.RemoteIpAddress;
            if (ip == null)
            {
                throw new NullReferenceException(nameof(ip));
            }
            Ip = ip;

            CoreApp = coreApp;

            // IHeaderDictionary will return StringValues.Empty for missing entries
            UserAgent = context.HttpContext?.Request.Headers[HeaderNames.UserAgent];
        }

        /// <summary>
        /// Check device data
        /// 检查设备信息
        /// </summary>
        /// <param name="result">Action result</param>
        /// <param name="parser">Parser</param>
        /// <returns>Valid or not</returns>
        protected bool CheckDevice([NotNullWhen(false)] out IActionResult? result, [NotNullWhen(true)] out UAParser? parser)
        {
            // User-Agent validatation
            parser = new UAParser(UserAgent);
            if (!parser.Valid || parser.IsBot)
            {
                result = ApplicationErrors.NoUserAgent.AsResult();
                parser = null;
                return false;
            }

            result = null;
            return true;
        }

        /// <summary>
        /// Check device data
        /// 检查设备信息
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="deviceId">Device id from client</param>
        /// <param name="result">Action result</param>
        /// <param name="data">Result data</param>
        /// <returns>Valid or not</returns>
        protected bool CheckDevice(IServiceBase service, string deviceId, [NotNullWhen(false)] out IActionResult? result, [NotNullWhen(true)] out (string DeviceCore, UAParser Parser)? data)
        {
            data = null;

            if (!CheckDevice(out result, out var parser))
            {
                return false;
            }

            string? deviceCore;
            try
            {
                deviceCore = service.DecryptDeviceCore(deviceId, parser.ToShortName());
            }
            catch
            {
                // Exception happened when device upgraded or changed view model (windows to mobile)
                // Client should response to it to clear cached device id
                deviceCore = null;
            }

            if (string.IsNullOrEmpty(deviceCore))
            {
                result = ApplicationErrors.NoValidData.AsResult("Device");
                return false;
            }

            result = null;
            data = (deviceCore, parser);

            return true;
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
