using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using System.Text.Json;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application base interface
    /// 核心程序基础接口
    /// </summary>
    public interface ICoreApplicationBase
    {
        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        AppConfiguration Configuration { get; }

        /// <summary>
        /// Database
        /// 数据库
        /// </summary>
        IDatabase DB { get; }

        /// <summary>
        /// Default Json serializer options
        /// 默认的Json序列化器选项
        /// </summary>
        JsonSerializerOptions DefaultJsonSerializerOptions { get; set; }

        /// <summary>
        /// Add system parameters, override it to localize parameters' type
        /// 添加系统参数，可以重写本地化参数类型
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="parameters">Parameters</param>
        void AddSystemParameters(IServiceUser user, IDbParameters parameters);

        /// <summary>
        /// Build command name, ["member", "view"] => ep_member_view (default) or epMemberView (override to achieve)
        /// 构建命令名称
        /// </summary>
        /// <param name="identifier">Identifier, like procedure with 'p'</param>
        /// <param name="parts">Parts</param>
        /// <param name="isSystem">Is system command</param>
        /// <returns>Result</returns>
        string BuildCommandName(string identifier, IEnumerable<string> parts, bool isSystem = false);

        /// <summary>
        /// Decript data
        /// 解密数据
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="key">Key</param>
        /// <returns>Result</returns>
        string DecriptData(string cipherText, string key = "");

        /// <summary>
        /// Encript data
        /// 加密数据
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="key">Key</param>
        /// <returns>Result</returns>
        string EncriptData(string plainText, string key = "");

        /// <summary>
        /// Get API user data
        /// 获取API用户数据
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="userId">User id</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        Task<IActionResult?> GetApiUserDataAsync(int organizationId, int userId, int deviceId = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get exchange key
        /// </summary>
        /// <param name="key">Encryption key</param>
        /// <param name="serviceId">Service id</param>
        /// <returns>Result</returns>
        string GetExchangeKey(string key, int serviceId);

        /// <summary>
        /// Hash password bytes
        /// 哈希密码字节数组
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed bytes</returns>
        byte[] HashPasswordBytes(ReadOnlySpan<char> password);

        /// <summary>
        /// Async hash password bytes
        /// 异步哈希密码字节数组
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed bytes</returns>
        Task<byte[]> HashPasswordBytesAsync(string password);

        /// <summary>
        /// Hash password
        /// 哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed result</returns>
        string HashPassword(ReadOnlySpan<char> password);

        /// <summary>
        /// Async hash password
        /// 异步哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed result</returns>
        Task<string> HashPasswordAsync(string password);
    }
}
