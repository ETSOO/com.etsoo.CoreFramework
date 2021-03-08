using com.etsoo.Utils.String;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application configuration
    /// 程序配置
    /// </summary>
    public record AppConfiguration : IAppConfiguration
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="privateKey">Private key for encryption/decryption</param>
        /// <param name="appId">Application id, default is "e"</param>
        /// <param name="languages">Supported languages</param>
        /// <param name="modelValidated">Model DataAnnotations are validated or not</param>
        /// <param name="symmetricKey">Symmetric security key, for data exchange</param>
        public AppConfiguration(
            string privateKey,
            string? appId = null,
            string[]? languages = null,
            bool modelValidated = false,
            string? symmetricKey = null
        )
        {
            // Default languages
            languages ??= Array.Empty<string>();

            // Update
            (
                AppId,
                Languages,
                ModelValidated,
                PrivateKey,
                SymmetricKey
            ) = (
                (appId ?? "e").AsMemory(),
                languages,
                modelValidated,
                privateKey.AsMemory(),
                symmetricKey.AsMemory()
            );
        }

        /// <summary>
        /// Constructor with configuration
        /// 使用配置的构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        /// <param name="modelValidated">Model DataAnnotations are validated or not</param>
        public AppConfiguration(IConfigurationSection section, bool modelValidated = false) : this(section.GetValue<string>("PrivateKey"),
            section.GetValue<string>("AppId"),
            StringUtil.AsEnumerable(section.GetValue<string>("Languages")).ToArray(),
            modelValidated,
            section.GetValue<string>("SymmetricKey"))
        {
        }

        /// <summary>
        /// Application id
        /// 程序编号
        /// </summary>
        public ReadOnlyMemory<char> AppId { get; }

        /// <summary>
        /// Supported languages, like zh-CN, en
        /// 支持的语言，比如zh-CN, en
        /// </summary>
        public string[] Languages { get; }
        
        /// <summary>
        /// Model DataAnnotations are validated, true under Web API to avoid double validation
        /// 模块数据标记已验证，在Web API下可以设置为true以避免重复验证
        /// </summary>
        public bool ModelValidated { get; }

        /// <summary>
        /// Private key for encryption/decryption, required
        /// 加解密私匙，必填
        /// </summary>
        public ReadOnlyMemory<char> PrivateKey { get; }

        /// <summary>
        /// Symmetric security key, for data exchange, null means prevention exchange
        /// 对称安全私匙，用于数据交换，不设置标识禁止交换信息
        /// </summary>
        public ReadOnlyMemory<char>? SymmetricKey { get; }
    }
}
