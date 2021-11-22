namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Refresh token interface
    /// 刷新令牌接口
    /// </summary>
    public interface IRefreshToken : IUserToken
    {
        /// <summary>
        /// Service identifier
        /// 服务识别号
        /// </summary>
        string? Sid { get; }
    }
}