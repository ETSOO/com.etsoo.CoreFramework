﻿using com.etsoo.Utils.Actions;
using System;
using System.Configuration;
using System.Data.Common;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application error
    /// 程序错误对象
    /// </summary>
    public record ApplicationError(Uri Type, string Title)
    {
        /// <summary>
        /// As AcionResult
        /// 输出为操作结果
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="field">Field</param>
        /// <returns>Action result</returns>
        public IActionResult AsResult(string? title = null, string? field = null)
        {
            return new ActionResult
            {
                Type = Type,
                Title = title ?? Title,
                Field = field
            };
        }
    }

    /// <summary>
    /// Application errors
    /// 程序错误
    /// </summary>
    public static class ApplicationErrors
    {
        /// <summary>
        /// Out Of Memory
        /// </summary>
        public const string OutOfMemoryUrl = "https://api.etsoo.com/SmartERP/errors/OutOfMemory";

        /// <summary>
        /// No action result type url
        /// </summary>
        public const string NoActionResultUrl = "https://api.etsoo.com/SmartERP/errors/NoActionResult";

        /// <summary>
        /// No user agent type url
        /// </summary>
        public const string NoUserAgentUrl = "https://api.etsoo.com/SmartERP/errors/NoUserAgent";

        /// <summary>
        /// No user id found url
        /// </summary>
        public const string NoUserFoundUrl = "https://api.etsoo.com/SmartERP/errors/NoUserId";

        /// <summary>
        /// No user id and password match url
        /// </summary>
        public const string NoUserMatchUrl = "https://api.etsoo.com/SmartERP/errors/NoUserMatch";

        /// <summary>
        /// Your token has expired url
        /// </summary>
        public const string TokenExpiredUrl = "https://api.etsoo.com/SmartERP/errors/TokenExpired";

        /// <summary>
        /// Database Connection Failed url
        /// </summary>
        public const string DbConnectionFailedUrl = "https://api.etsoo.com/SmartERP/errors/DbConnectionFailed";

        /// <summary>
        /// Data Processing Failed
        /// </summary>
        public const string DataProcessingFailedUrl = "https://api.etsoo.com/SmartERP/errors/DataProcessingFailed";

        /// <summary>
        /// Out Of Memory
        /// 内存不足
        /// </summary>
        public static ApplicationError OutOfMemory => new ApplicationError(new Uri(OutOfMemoryUrl), Resources.Resource.OutOfMemory);

        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ApplicationError NoActionResult => new ApplicationError(new Uri(NoActionResultUrl), Resources.Resource.NoActionResult);

        /// <summary>
        /// No user agent result error
        /// 没有用户代理错误
        /// </summary>
        public static ApplicationError NoUserAgent => new ApplicationError(new Uri(NoUserAgentUrl), Resources.Resource.NoUserAgent);

        /// <summary>
        /// No user found error
        /// 找不到用户错误
        /// </summary>
        public static ApplicationError NoUserFound => new ApplicationError(new Uri(NoUserFoundUrl), Resources.Resource.NoUserFound);

        /// <summary>
        /// User name and password do not match error
        /// 用户名和密码不匹配错误
        /// </summary>
        public static ApplicationError NoUserMatch => new ApplicationError(new Uri(NoUserMatchUrl), Resources.Resource.NoUserMatch);

        /// <summary>
        /// User name and password do not match error
        /// 您的令牌已过期错误
        /// </summary>
        public static ApplicationError TokenExpired => new ApplicationError(new Uri(TokenExpiredUrl), Resources.Resource.TokenExpired);

        /// <summary>
        /// Database Connection Failed
        /// 数据库连接失败
        /// </summary>
        public static ApplicationError DbConnectionFailed => new ApplicationError(new Uri(DbConnectionFailedUrl), Resources.Resource.DbConnectionFailed);

        /// <summary>
        /// Data Processing Failed
        /// 数据处理失败
        /// </summary>
        public static ApplicationError DataProcessingFailed => new ApplicationError(new Uri(DataProcessingFailedUrl), Resources.Resource.DataProcessingFailed);

        /// <summary>
        /// Get Db exception result
        /// 获取数据库异常结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        public static IActionResult GetDbException(Exception ex)
        {
            var result = ex switch
            {
                OutOfMemoryException => OutOfMemory.AsResult(),

                InvalidOperationException
                or DbException
                or ConfigurationErrorsException => DbConnectionFailed.AsResult(),

                _ => DataProcessingFailed.AsResult()
            };

            return result;
        }
    }
}
