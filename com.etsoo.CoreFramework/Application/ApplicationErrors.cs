using com.etsoo.CoreFramework.Properties;
using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application error
    /// 程序错误对象
    /// </summary>
    public record ApplicationError(string Type, string Title)
    {
        /// <summary>
        /// As AcionResult
        /// 输出为操作结果
        /// </summary>
        /// <param name="title">Title</param>
        /// <returns>Action result</returns>
        public IActionResult AsResult(string? title = null)
        {
            return new ActionResult(Type)
            {
                Title = title ?? Title
            };
        }

        /// <summary>
        /// As AcionResult with trace id
        /// 输出为带追踪编号的操作结果
        /// </summary>
        /// <param name="traceId">Trace id</param>
        /// <returns>Action result</returns>
        public IActionResult AsResultWithTraceId(string traceId)
        {
            return new ActionResult(Type)
            {
                TraceId = traceId
            };
        }
    }

    /// <summary>
    /// Application errors, static constructor will be failed with multiple cultures
    /// 程序错误，静态构造函数初始化会导致多文化无法切换
    /// </summary>
    public class ApplicationErrors
    {
        /// <summary>
        /// Access denied
        /// 访问被拒绝
        /// </summary>
        public static ApplicationError AccessDenied => new(nameof(AccessDenied), Resources.AccessDenied);

        /// <summary>
        /// Your account has been disabled
        /// 您的帐户已被禁用
        /// </summary>
        public static ApplicationError AccountDisabled => new(nameof(AccountDisabled), Resources.AccountDisabled);

        /// <summary>
        /// Account has expired
        /// 账户已到期
        /// </summary>
        public static ApplicationError AccountExpired => new(nameof(AccountExpired), Resources.AccountExpired);

        /// <summary>
        /// Code Expired
        /// 验证码已过期
        /// </summary>
        public static ApplicationError CodeExpired => new(nameof(CodeExpired), Resources.CodeExpired);

        /// <summary>
        /// CodeFrozen
        /// 验证码功能已被禁止
        /// </summary>
        public static ApplicationError CodeFrozen => new(nameof(CodeFrozen), Resources.CodeFrozen);

        /// <summary>
        /// Codes do not match
        /// 验证码不匹配
        /// </summary>
        public static ApplicationError CodesNoMatch => new(nameof(CodesNoMatch), Resources.CodesNoMatch);

        /// <summary>
        /// Failed to send the code
        /// 验证码发送失败
        /// </summary>
        public static ApplicationError CodeSendingFailed => new(nameof(CodeSendingFailed), Resources.CodeSendingFailed);

        /// <summary>
        /// Data Processing Failed
        /// 数据处理失败
        /// </summary>
        public static ApplicationError DataProcessingFailed => new(nameof(DataProcessingFailed), Resources.DataProcessingFailed);

        /// <summary>
        /// Database Connection Failed
        /// 数据库连接失败
        /// </summary>
        public static ApplicationError DbConnectionFailed => new(nameof(DbConnectionFailed), Resources.DbConnectionFailed);

        /// <summary>
        /// Device Disabled
        /// 设备已禁用
        /// </summary>
        public static ApplicationError DeviceDisabled => new(nameof(DeviceDisabled), Resources.DeviceDisabled);

        /// <summary>
        /// The device has been temporarily blocked
        /// 该设备已被暂时禁止使用
        /// </summary>
        public static ApplicationError DeviceFrozen => new(nameof(DeviceFrozen), Resources.DeviceFrozen);

        /// <summary>
        /// Invalid Action
        /// 无效的操作
        /// </summary>
        public static ApplicationError InvalidAction => new(nameof(InvalidAction), Resources.InvalidAction);

        /// <summary>
        /// It is not a valid Email address
        /// 不是有效的Email地址
        /// </summary>
        public static ApplicationError InvalidEmail => new(nameof(InvalidEmail), Resources.InvalidEmail);

        /// <summary>
        /// It is not a valid mobile phone number
        /// 不是有效的手机号码
        /// </summary>
        public static ApplicationError InvalidMobile => new(nameof(InvalidMobile), Resources.InvalidMobile);

        /// <summary>
        /// Join Organization Required
        /// 需要加入组织
        /// </summary>
        public static ApplicationError JoinOrgRequired => new(nameof(JoinOrgRequired), Resources.JoinOrgRequired);

        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ApplicationError NoActionResult => new(nameof(NoActionResult), Resources.NoActionResult);

        /// <summary>
        /// No action result error
        /// 没有操作结果错误
        /// </summary>
        public static ApplicationError NoDataReturned => new(nameof(NoDataReturned), Resources.NoDataReturned);

        /// <summary>
        /// No Device Matched
        /// 没有找到匹配的设备
        /// </summary>
        public static ApplicationError NoDeviceMatch => new(nameof(NoDeviceMatch), Resources.NoDeviceMatch);

        /// <summary>
        /// The passed ID does not exist
        /// 传递的编号不存在
        /// </summary>
        public static ApplicationError NoId => new(nameof(NoId), Resources.NoId);

        /// <summary>
        /// No Organization Joined
        /// 未加入任何组织
        /// </summary>
        public static ApplicationError NoOrgJoined => new(nameof(NoOrgJoined), Resources.NoOrgJoined);

        /// <summary>
        /// No Password Match
        /// 密码不匹配
        /// </summary>
        public static ApplicationError NoPasswordMatch => new(nameof(NoPasswordMatch), Resources.NoPasswordMatch);

        /// <summary>
        /// No user agent result error
        /// 没有用户代理错误
        /// </summary>
        public static ApplicationError NoUserAgent => new(nameof(NoUserAgent), Resources.NoUserAgent);

        /// <summary>
        /// No user found error
        /// 找不到用户错误
        /// </summary>
        public static ApplicationError NoUserFound => new(nameof(NoUserFound), Resources.NoUserFound);

        /// <summary>
        /// User name and password do not match error
        /// 用户名和密码不匹配错误
        /// </summary>
        public static ApplicationError NoUserMatch => new(nameof(NoUserMatch), Resources.NoUserMatch);

        /// <summary>
        /// No valid data is passed
        /// 没有传递有效的数据
        /// </summary>
        public static ApplicationError NoValidData => new(nameof(NoValidData), Resources.NoValidData);

        /// <summary>
        /// The organization has been disabled
        /// 机构已被禁用
        /// </summary>
        public static ApplicationError OrgDisabled => new(nameof(OrgDisabled), Resources.OrgDisabled);

        /// <summary>
        /// The organization already exists
        /// 机构已存在
        /// </summary>
        public static ApplicationError OrgExists => new(nameof(OrgExists), Resources.OrgExists);

        /// <summary>
        /// Organization service has expired
        /// 机构服务已到期
        /// </summary>
        public static ApplicationError OrgExpired => new(nameof(OrgExpired), Resources.OrgExpired);

        /// <summary>
        /// Out Of Memory
        /// 内存不足
        /// </summary>
        public static ApplicationError OutOfMemory => new(nameof(OutOfMemory), Resources.OutOfMemory);

        /// <summary>
        /// User name and password do not match error
        /// 您的令牌已过期错误
        /// </summary>
        public static ApplicationError TokenExpired => new(nameof(TokenExpired), Resources.TokenExpired);

        /// <summary>
        /// Your account has been temporarily blocked
        /// 您的帐户已被暂时禁止使用
        /// </summary>
        public static ApplicationError UserFrozen => new(nameof(UserFrozen), Resources.UserFrozen);

        /// <summary>
        /// Username is already registered
        /// 用户名已注册
        /// </summary>
        public static ApplicationError UserRegistered => new(nameof(UserRegistered), Resources.UserRegistered);

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
                nameof(AccessDenied) => AccessDenied,
                nameof(AccountDisabled) => AccountDisabled,
                nameof(AccountExpired) => AccountExpired,
                nameof(CodeExpired) => CodeExpired,
                nameof(CodeFrozen) => CodeFrozen,
                nameof(CodesNoMatch) => CodesNoMatch,
                nameof(CodeSendingFailed) => CodeSendingFailed,
                nameof(DataProcessingFailed) => DataProcessingFailed,
                nameof(DbConnectionFailed) => DbConnectionFailed,
                nameof(DeviceDisabled) => DeviceDisabled,
                nameof(DeviceFrozen) => DeviceFrozen,
                nameof(InvalidAction) => InvalidAction,
                nameof(InvalidEmail) => InvalidEmail,
                nameof(InvalidMobile) => InvalidMobile,
                nameof(JoinOrgRequired) => JoinOrgRequired,
                nameof(NoActionResult) => NoActionResult,
                nameof(NoDataReturned) => NoDataReturned,
                nameof(NoDeviceMatch) => NoDeviceMatch,
                nameof(NoId) => NoId,
                nameof(NoOrgJoined) => NoOrgJoined,
                nameof(NoPasswordMatch) => NoPasswordMatch,
                nameof(NoUserAgent) => NoUserAgent,
                nameof(NoUserFound) => NoUserFound,
                nameof(NoUserMatch) => NoUserMatch,
                nameof(NoValidData) => NoValidData,
                nameof(OrgDisabled) => OrgDisabled,
                nameof(OrgExists) => OrgExists,
                nameof(OrgExpired) => OrgExpired,
                nameof(OutOfMemory) => OutOfMemory,
                nameof(TokenExpired) => TokenExpired,
                nameof(UserFrozen) => UserFrozen,
                nameof(UserRegistered) => UserRegistered,
                _ => null
            };
        }
    }
}
