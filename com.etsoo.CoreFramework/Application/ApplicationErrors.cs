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
    /// Application errors, static constructor will be failed with multiple cultures
    /// 程序错误，静态构造函数初始化会导致多文化无法切换
    /// </summary>
    public static class ApplicationErrors
    {
        /// <summary>
        /// Your account has been disabled
        /// 您的帐户已被禁用
        /// </summary>
        public static ApplicationError AccountDisabled => new(new Uri(nameof(AccountDisabled), UriKind.Relative), Resources.AccountDisabled);

        /// <summary>
        /// Account has expired
        /// 账户已到期
        /// </summary>
        public static ApplicationError AccountExpired => new(new Uri(nameof(AccountExpired), UriKind.Relative), Resources.AccountExpired);

        /// <summary>
        /// Data Processing Failed
        /// 数据处理失败
        /// </summary>
        public static ApplicationError DataProcessingFailed => new(new Uri(nameof(DataProcessingFailed), UriKind.Relative), Resources.DataProcessingFailed);

        /// <summary>
        /// Database Connection Failed
        /// 数据库连接失败
        /// </summary>
        public static ApplicationError DbConnectionFailed => new(new Uri(nameof(DbConnectionFailed), UriKind.Relative), Resources.DbConnectionFailed);

        /// <summary>
        /// Device Disabled
        /// 设备已禁用
        /// </summary>
        public static ApplicationError DeviceDisabled => new(new Uri(nameof(DeviceDisabled), UriKind.Relative), Resources.DeviceDisabled);

        /// <summary>
        /// The device has been temporarily blocked
        /// 该设备已被暂时禁止使用
        /// </summary>
        public static ApplicationError DeviceFrozen => new(new Uri(nameof(DeviceFrozen), UriKind.Relative), Resources.DeviceFrozen);

        /// <summary>
        /// Join Organization Required
        /// 需要加入组织
        /// </summary>
        public static ApplicationError JoinOrgRequired => new(new Uri(nameof(JoinOrgRequired), UriKind.Relative), Resources.JoinOrgRequired);

        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ApplicationError NoActionResult => new(new Uri(nameof(NoActionResult), UriKind.Relative), Resources.NoActionResult);

        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ApplicationError NoDataReturned => new(new Uri(nameof(NoDataReturned), UriKind.Relative), Resources.NoDataReturned);

        /// <summary>
        /// The passed ID does not exist
        /// 传递的编号不存在
        /// </summary>
        public static ApplicationError NoId => new(new Uri(nameof(NoId), UriKind.Relative), Resources.NoId);

        /// <summary>
        /// No Organization Joined
        /// 未加入任何组织
        /// </summary>
        public static ApplicationError NoOrgJoined => new(new Uri(nameof(NoOrgJoined), UriKind.Relative), Resources.NoOrgJoined);

        /// <summary>
        /// No user agent result error
        /// 没有用户代理错误
        /// </summary>
        public static ApplicationError NoUserAgent => new(new Uri(nameof(NoUserAgent), UriKind.Relative), Resources.NoUserAgent);

        /// <summary>
        /// No user found error
        /// 找不到用户错误
        /// </summary>
        public static ApplicationError NoUserFound => new(new Uri(nameof(NoUserFound), UriKind.Relative), Resources.NoUserFound);

        /// <summary>
        /// User name and password do not match error
        /// 用户名和密码不匹配错误
        /// </summary>
        public static ApplicationError NoUserMatch => new(new Uri(nameof(NoUserMatch), UriKind.Relative), Resources.NoUserMatch);

        /// <summary>
        /// The organization has been disabled
        /// 机构已被禁用
        /// </summary>
        public static ApplicationError OrgDisabled => new(new Uri(nameof(OrgDisabled), UriKind.Relative), Resources.OrgDisabled);

        /// <summary>
        /// Organization service has expired
        /// 机构服务已到期
        /// </summary>
        public static ApplicationError OrgExpired => new(new Uri(nameof(OrgExpired), UriKind.Relative), Resources.OrgExpired);

        /// <summary>
        /// Out Of Memory
        /// 内存不足
        /// </summary>
        public static ApplicationError OutOfMemory => new(new Uri(nameof(OutOfMemory), UriKind.Relative), Resources.OutOfMemory);

        /// <summary>
        /// User name and password do not match error
        /// 您的令牌已过期错误
        /// </summary>
        public static ApplicationError TokenExpired => new(new Uri(nameof(TokenExpired), UriKind.Relative), Resources.TokenExpired);

        /// <summary>
        /// Your account has been temporarily blocked
        /// 您的帐户已被暂时禁止使用
        /// </summary>
        public static ApplicationError UserFrozen => new(new Uri(nameof(UserFrozen), UriKind.Relative), Resources.UserFrozen);

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
