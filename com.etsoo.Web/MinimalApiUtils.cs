using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Services;
using com.etsoo.UserAgentParser;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using IActionResult = com.etsoo.Utils.Actions.IActionResult;

namespace com.etsoo.Web
{
    /// <summary>
    /// Minimal API utilities
    /// 最小API工具
    /// </summary>
    public static class MinimalApiUtils
    {
        /// <summary>
        /// Check device data
        /// 检查设备信息
        /// </summary>
        /// <param name="userAgent">User agent string</param>
        /// <param name="result">Action result</param>
        /// <param name="parser">Parser</param>
        /// <returns>Valid or not</returns>
        public static bool CheckDevice(string? userAgent, [NotNullWhen(false)] out IActionResult? result, [NotNullWhen(true)] out UAParser? parser)
        {
            // User-Agent validatation
            parser = new UAParser(userAgent);
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
        /// Check device data without additional data
        /// 检查设备信息，不返回额外数据
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="userAgent">User agent string</param>
        /// <param name="deviceId">Device id from client</param>
        /// <returns>Check result</returns>
        public static bool CheckDevice(this IServiceBase service, string? userAgent, string deviceId)
        {
            if (!CheckDevice(userAgent, out _, out var parser))
            {
                return false;
            }

            try
            {
                var deviceCore = service.DecryptDeviceCore(deviceId, parser.ToShortName());
                if (string.IsNullOrEmpty(deviceCore))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check device data
        /// 检查设备信息
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="userAgent">User agent string</param>
        /// <param name="deviceId">Device id from client</param>
        /// <param name="result">Action result</param>
        /// <param name="data">Result data</param>
        /// <returns>Check result</returns>
        public static bool CheckDevice(this IServiceBase service, string? userAgent, string deviceId, [NotNullWhen(false)] out IActionResult? result, [NotNullWhen(true)] out (string DeviceCore, UAParser Parser)? data)
        {
            data = null;

            if (!CheckDevice(userAgent, out result, out var parser))
            {
                return false;
            }

            try
            {
                var deviceCore = service.DecryptDeviceCore(deviceId, parser.ToShortName());

                if (string.IsNullOrEmpty(deviceCore))
                {
                    result = ApplicationErrors.NoValidData.AsResult("Device");
                    return false;
                }

                result = null;
                data = (deviceCore, parser);

                return true;
            }
            catch (Exception ex)
            {
                // Exception happened when device upgraded or changed view model (windows to mobile)
                // Client should response to it to clear cached device id
                result = service.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Output refresh token
        /// 输出刷新令牌
        /// </summary>
        /// <param name="accessor">HTTP accessor</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="headerName">Header name</param>
        public static void OutputRefreshToken(IHttpContextAccessor accessor, string refreshToken, string headerName = Constants.RefreshTokenHeaderName)
        {
            var response = accessor.HttpContext?.Response;
            if (response == null)
            {
                return;
            }
            OutputRefreshToken(response, refreshToken, headerName);
        }

        /// <summary>
        /// Output refresh token
        /// 输出刷新令牌
        /// </summary>
        /// <param name="response">HTTP response</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="headerName">Header name</param>
        public static void OutputRefreshToken(HttpResponse response, string refreshToken, string headerName = Constants.RefreshTokenHeaderName)
        {
            response.Headers.Append(headerName, refreshToken);
        }
    }
}