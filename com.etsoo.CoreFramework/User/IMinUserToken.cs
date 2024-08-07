using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Minimal user token interface
    /// 最小用户令牌接口
    /// </summary>
    public interface IMinUserToken
    {
        /// <summary>
        /// User Id
        /// 用户编号
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Int id
        /// 整数编号
        /// </summary>
        int IdInt { get; }

        /// <summary>
        /// Unique connection id
        /// 唯一连接编号
        /// </summary>
        string? ConnectionId { get; }

        /// <summary>
        /// Scopes
        /// 范围
        /// </summary>
        IEnumerable<string>? Scopes { get; }

        /// <summary>
        /// Create identity
        /// 创建身份
        /// </summary>
        /// <returns>Identity</returns>
        ClaimsIdentity CreateIdentity();
    }
}
