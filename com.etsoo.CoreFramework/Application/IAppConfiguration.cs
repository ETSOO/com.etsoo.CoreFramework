using System;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application configuration interface
    /// 程序配置接口
    /// </summary>
    public interface IAppConfiguration
    {
        /// <summary>
        /// Application id
        /// 程序编号
        /// </summary>
        ReadOnlyMemory<char> AppId { get; }

        /// <summary>
        /// Supported cultures, like zh-CN, en
        /// 支持的文化，比如zh-CN, en
        /// </summary>
        string[] Cultures { get; }

        /// <summary>
        /// Model DataAnnotations are validated, true under Web API to avoid double validation
        /// 模块数据标记已验证，在Web API下可以设置为true以避免重复验证
        /// </summary>
        bool ModelValidated { get; }

        /// <summary>
        /// Private key for encryption/decryption, required
        /// 加解密私匙，必填
        /// </summary>
        ReadOnlyMemory<char> PrivateKey { get; }

        /// <summary>
        /// Symmetric security key, for data exchange, null means prevention exchange
        /// 对称安全私匙，用于数据交换，不设置标识禁止交换信息
        /// </summary>
        ReadOnlyMemory<char>? SymmetricKey { get; }
    }
}
