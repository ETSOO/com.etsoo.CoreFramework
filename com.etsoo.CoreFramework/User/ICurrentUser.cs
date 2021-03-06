using System.Net;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user interface
    /// 当前用户接口
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// Unique connection id
        /// 唯一连接编号
        /// </summary>
        public string ConnectionId { get; init; }

        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Role
        /// 角色
        /// </summary>
        public string Role { get; init; }

        /// <summary>
        /// Client IP address
        /// 客户端IP地址
        /// </summary>
        IPAddress? ClientIp { get; init; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        string Language { get; init; }
    }
}
