using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Minimal user creator interface
    /// 最小用户创建者接口
    /// </summary>
    /// <typeparam name="TSelf">Generic user type</typeparam>
    public interface IMinUserCreator<TSelf> where TSelf : IMinUserCreator<TSelf>
    {
        /// <summary>
        /// Create user from claims
        /// 从声明创建用户
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="reason">Failure reason</param>
        /// <returns>User</returns>
        static abstract TSelf? Create(ClaimsPrincipal? claims, out string? reason);
    }

    /// <summary>
    /// User creator interface
    /// 用户创建者接口
    /// </summary>
    /// <typeparam name="TSelf">Generic user type</typeparam>
    public interface IUserCreator<TSelf> : IMinUserCreator<TSelf> where TSelf : IUserCreator<TSelf>
    {
        /// <summary>
        /// Create user from result data
        /// 从操作结果数据创建用户
        /// </summary>
        /// <param name="data">Result data</param>
        /// <param name="ip">Ip address</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <returns>User</returns>
        static abstract TSelf? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region);
    }
}