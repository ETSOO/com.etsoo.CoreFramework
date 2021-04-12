using com.etsoo.CoreFramework.Properties;
using com.etsoo.Utils.Actions;
using System;

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
        /// <param name="traceId">Trace id</param>
        /// <returns>Action result</returns>
        public IActionResult AsResult(string? title = null, string? traceId = null)
        {
            return new ActionResult
            {
                Type = Type,
                Title = title ?? Title,
                TraceId = traceId
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
        /// Your account has been disabled
        /// 您的帐户已被禁用
        /// </summary>
        public static ApplicationError AccountDisabled { get; }

        /// <summary>
        /// Account has expired
        /// 账户已到期
        /// </summary>
        public static ApplicationError AccountExpired { get; }

        /// <summary>
        /// Data Processing Failed
        /// 数据处理失败
        /// </summary>
        public static ApplicationError DataProcessingFailed { get; }

        /// <summary>
        /// Database Connection Failed
        /// 数据库连接失败
        /// </summary>
        public static ApplicationError DbConnectionFailed { get; }

        /// <summary>
        /// Device Disabled
        /// 设备已禁用
        /// </summary>
        public static ApplicationError DeviceDisabled { get; }

        /// <summary>
        /// The device has been temporarily blocked
        /// 该设备已被暂时禁止使用
        /// </summary>
        public static ApplicationError DeviceFrozen { get; }

        /// <summary>
        /// Join Organization Required
        /// 需要加入组织
        /// </summary>
        public static ApplicationError JoinOrgRequired { get; }

        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ApplicationError NoActionResult { get; }

        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ApplicationError NoDataReturned { get; }

        /// <summary>
        /// The passed ID does not exist
        /// 传递的编号不存在
        /// </summary>
        public static ApplicationError NoId { get; }

        /// <summary>
        /// No Organization Joined
        /// 未加入任何组织
        /// </summary>
        public static ApplicationError NoOrgJoined { get; }

        /// <summary>
        /// No user agent result error
        /// 没有用户代理错误
        /// </summary>
        public static ApplicationError NoUserAgent { get; }

        /// <summary>
        /// No user found error
        /// 找不到用户错误
        /// </summary>
        public static ApplicationError NoUserFound { get; }

        /// <summary>
        /// User name and password do not match error
        /// 用户名和密码不匹配错误
        /// </summary>
        public static ApplicationError NoUserMatch { get; }

        /// <summary>
        /// The organization has been disabled
        /// 机构已被禁用
        /// </summary>
        public static ApplicationError OrgDisabled { get; }

        /// <summary>
        /// Organization service has expired
        /// 机构服务已到期
        /// </summary>
        public static ApplicationError OrgExpired { get; }

        /// <summary>
        /// Out Of Memory
        /// 内存不足
        /// </summary>
        public static ApplicationError OutOfMemory { get; }

        /// <summary>
        /// User name and password do not match error
        /// 您的令牌已过期错误
        /// </summary>
        public static ApplicationError TokenExpired { get; }

        /// <summary>
        /// Your account has been temporarily blocked
        /// 您的帐户已被暂时禁止使用
        /// </summary>
        public static ApplicationError UserFrozen { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        static ApplicationErrors()
        {
            AccountDisabled = new ApplicationError(new Uri(nameof(AccountDisabled), UriKind.Relative), Resources.AccountDisabled);
            AccountExpired = new ApplicationError(new Uri(nameof(AccountExpired), UriKind.Relative), Resources.AccountExpired);
            DataProcessingFailed = new ApplicationError(new Uri(nameof(DataProcessingFailed), UriKind.Relative), Resources.DataProcessingFailed);
            DbConnectionFailed = new ApplicationError(new Uri(nameof(DbConnectionFailed), UriKind.Relative), Resources.DbConnectionFailed);
            DeviceDisabled = new ApplicationError(new Uri(nameof(DeviceDisabled), UriKind.Relative), Resources.DeviceDisabled);
            DeviceFrozen = new ApplicationError(new Uri(nameof(DeviceFrozen), UriKind.Relative), Resources.DeviceFrozen);
            JoinOrgRequired = new ApplicationError(new Uri(nameof(JoinOrgRequired), UriKind.Relative), Resources.JoinOrgRequired);
            NoActionResult = new ApplicationError(new Uri(nameof(NoActionResult), UriKind.Relative), Resources.NoActionResult);
            NoDataReturned = new ApplicationError(new Uri(nameof(NoDataReturned), UriKind.Relative), Resources.NoDataReturned);
            NoId = new ApplicationError(new Uri(nameof(NoId), UriKind.Relative), Resources.NoId);
            NoOrgJoined = new ApplicationError(new Uri(nameof(NoOrgJoined), UriKind.Relative), Resources.NoOrgJoined);
            NoUserAgent = new ApplicationError(new Uri(nameof(NoUserAgent), UriKind.Relative), Resources.NoUserAgent);
            NoUserFound = new ApplicationError(new Uri(nameof(NoUserFound), UriKind.Relative), Resources.NoUserFound);
            NoUserMatch = new ApplicationError(new Uri(nameof(NoUserMatch), UriKind.Relative), Resources.NoUserMatch);
            OrgDisabled = new ApplicationError(new Uri(nameof(OrgDisabled), UriKind.Relative), Resources.OrgDisabled);
            OrgExpired = new ApplicationError(new Uri(nameof(OrgExpired), UriKind.Relative), Resources.OrgExpired);
            OutOfMemory = new ApplicationError(new Uri(nameof(OutOfMemory), UriKind.Relative), Resources.OutOfMemory);
            TokenExpired = new ApplicationError(new Uri(nameof(TokenExpired), UriKind.Relative), Resources.TokenExpired);
            UserFrozen = new ApplicationError(new Uri(nameof(UserFrozen), UriKind.Relative), Resources.UserFrozen);
        }

        /// <summary>
        /// Get error with name
        /// 从名称获取错误
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Error</returns>
        public static ApplicationError? Get(string name)
        {
            return name switch
            {
                nameof(AccountDisabled) => AccountDisabled,
                nameof(AccountExpired) => AccountExpired,
                nameof(DataProcessingFailed) => DataProcessingFailed,
                nameof(DbConnectionFailed) => DbConnectionFailed,
                nameof(DeviceDisabled) => DeviceDisabled,
                nameof(DeviceFrozen) => DeviceFrozen,
                nameof(JoinOrgRequired) => JoinOrgRequired,
                nameof(NoActionResult) => NoActionResult,
                nameof(NoDataReturned) => NoDataReturned,
                nameof(NoId) => NoId,
                nameof(NoOrgJoined) => NoOrgJoined,
                nameof(NoUserAgent) => NoUserAgent,
                nameof(NoUserFound) => NoUserFound,
                nameof(NoUserMatch) => NoUserMatch,
                nameof(OrgDisabled) => OrgDisabled,
                nameof(OrgExpired) => OrgExpired,
                nameof(OutOfMemory) => OutOfMemory,
                nameof(TokenExpired) => TokenExpired,
                nameof(UserFrozen) => UserFrozen,
                _ => null
            };
        }
    }
}
