﻿using com.etsoo.CoreFramework.Properties;
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
        /// <param name="field">Field</param>
        /// <returns>Action result</returns>
        public ActionResult AsResult(string? field = null)
        {
            return new ActionResult
            {
                Type = Type,
                Title = Title,
                Field = field
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
        /// Authorization failed
        /// 授权失败
        /// </summary>
        public static ApplicationError AuthFailed => new(nameof(AuthFailed), Resources.AuthFailed);

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
        /// Data Not Ready
        /// 数据尚未准备就绪
        /// </summary>
        public static ApplicationError DataNotReady => new(nameof(DataNotReady), Resources.DataNotReady);

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
        /// The record cannot be deleted due to the existence of referenced data
        /// 由于引用数据的存在，无法删除该记录
        /// </summary>
        public static ApplicationError DeleteReferencedData => new(nameof(DeleteReferencedData), Resources.DeleteReferencedData);

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
        /// Email address has been registered
        /// 电子邮箱已注册
        /// </summary>
        public static ApplicationError EmailExists => new(nameof(EmailExists), Resources.EmailExists);

        /// <summary>
        /// Request frequency is too high
        /// 请求频次过高
        /// </summary>
        public static ApplicationError HighRequestRrequency => new(nameof(HighRequestRrequency), Resources.HighRequestRrequency);

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
        /// IP address has been changed
        /// IP地址已更改
        /// </summary>
        public static ApplicationError IPAddressChanged => new(nameof(IPAddressChanged), Resources.IPAddressChanged);

        /// <summary>
        /// Item already exists
        /// 项目已存在
        /// </summary>
        public static ApplicationError ItemExists => new(nameof(ItemExists), Resources.ItemExists);

        /// <summary>
        /// Join Organization Required
        /// 需要加入组织
        /// </summary>
        public static ApplicationError JoinOrgRequired => new(nameof(JoinOrgRequired), Resources.JoinOrgRequired);

        /// <summary>
        /// Mobile number has been registered
        /// 手机号码已注册
        /// </summary>
        public static ApplicationError MobileExists => new(nameof(MobileExists), Resources.MobileExists);

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
        /// Schema validation error
        /// 模式验证错误
        /// </summary>
        public static ApplicationError SchemaValidationError => new(nameof(SchemaValidationError), Resources.SchemaValidationError);

        /// <summary>
        /// Sign expired error
        /// 签名已过期错误
        /// </summary>
        public static ApplicationError SignExpired => new(nameof(SignExpired), Resources.SignExpired);

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
                nameof(AuthFailed) => AuthFailed,
                nameof(CodeExpired) => CodeExpired,
                nameof(CodeFrozen) => CodeFrozen,
                nameof(CodesNoMatch) => CodesNoMatch,
                nameof(CodeSendingFailed) => CodeSendingFailed,
                nameof(DataNotReady) => DataNotReady,
                nameof(DataProcessingFailed) => DataProcessingFailed,
                nameof(DbConnectionFailed) => DbConnectionFailed,
                nameof(DeleteReferencedData) => DeleteReferencedData,
                nameof(DeviceDisabled) => DeviceDisabled,
                nameof(DeviceFrozen) => DeviceFrozen,
                nameof(EmailExists) => EmailExists,
                nameof(HighRequestRrequency) => HighRequestRrequency,
                nameof(InvalidAction) => InvalidAction,
                nameof(InvalidEmail) => InvalidEmail,
                nameof(InvalidMobile) => InvalidMobile,
                nameof(IPAddressChanged) => IPAddressChanged,
                nameof(ItemExists) => ItemExists,
                nameof(JoinOrgRequired) => JoinOrgRequired,
                nameof(MobileExists) => MobileExists,
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
                nameof(SignExpired) => SignExpired,
                nameof(TokenExpired) => TokenExpired,
                nameof(UserFrozen) => UserFrozen,
                nameof(UserRegistered) => UserRegistered,
                _ => null
            };
        }
    }
}
