﻿using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.DB;
using com.etsoo.CoreFramework.Services;
using com.etsoo.UserAgentParser;
using com.etsoo.WebUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Net;
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
        /// HttpCotext accessor
        /// HttpCotext 访问器
        /// </summary>
        protected readonly IHttpContextAccessor context;

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
        /// Cancellation token
        /// 取消令牌
        /// </summary>
        protected readonly CancellationToken CancellationToken;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="coreApp">Core app</param>
        /// <param name="context">Http context accessor</param>
        public SharedController(ICoreApplicationBase coreApp, IHttpContextAccessor context) : base()
        {
            // Accessor
            this.context = context;

            // Remote IP
            var ip = context.RemoteIpAddress();
            if (ip == null)
            {
                throw new NullReferenceException(nameof(ip));
            }
            Ip = ip;

            // IHeaderDictionary will return StringValues.Empty for missing entries
            UserAgent = context.UserAgent();

            // Cancellation token
            CancellationToken = context.CancellationToken();

            // App reference
            CoreApp = coreApp;
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
            catch (Exception ex)
            {
                // Exception happened when device upgraded or changed view model (windows to mobile)
                // Client should response to it to clear cached device id
                result = service.LogException(ex);
                return false;
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
            Response.JsonContentType();

            // Write
            await result.ToJsonAsync(Response.BodyWriter, CoreApp.DefaultJsonSerializerOptions);
        }
    }
}
