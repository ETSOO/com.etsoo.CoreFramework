using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
        /// <param name="section">Configuration section</param>
        /// <param name="privateKey">Private key for encryption/decryption</param>
        /// <param name="appId">Application id, default is "e"</param>
        /// <param name="cultures">Supported cultures</param>
        /// <param name="modelValidated">Model DataAnnotations are validated or not</param>
        /// <param name="symmetricKey">Symmetric security key, for data exchange</param>
        public AppConfiguration(
            IConfigurationSection section,
            string privateKey,
            string? appId = null,
            string[]? cultures = null,
            bool modelValidated = false,
            string? symmetricKey = null
        )
        {
            // Default languages
            cultures ??= Array.Empty<string>();

            // Update
            (
                Section,
                AppId,
                Cultures,
                ModelValidated,
                PrivateKey,
                SymmetricKey
            ) = (
                section,
                appId ?? "e",
                cultures,
                modelValidated,
                privateKey,
                symmetricKey
            );
        }

        /// <summary>
        /// Constructor with configuration
        /// 使用配置的构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        /// <param name="modelValidated">Model DataAnnotations are validated or not</param>
        public AppConfiguration(IConfigurationSection section, bool modelValidated = false) : this(section,
            section.GetValue<string>("PrivateKey"),
            section.GetValue<string>("AppId"),
            section.GetSection("Cultures").Get<IEnumerable<string>?>()?.ToArray(),
            modelValidated,
            section.GetValue<string>("SymmetricKey"))
        {
        }

        /// <summary>
        /// Configuration section
        /// 配置部分
        /// </summary>
        public IConfigurationSection Section { get; }

        /// <summary>
        /// Application id
        /// 程序编号
        /// </summary>
        public string AppId { get; }

        /// <summary>
        /// Supported cultures, like zh-CN, en
        /// 支持的文化，比如zh-CN, en
        /// </summary>
        public string[] Cultures { get; }
        
        /// <summary>
        /// Model DataAnnotations are validated, true under Web API to avoid double validation
        /// 模块数据标记已验证，在Web API下可以设置为true以避免重复验证
        /// </summary>
        public bool ModelValidated { get; }

        /// <summary>
        /// Private key for encryption/decryption, required
        /// 加解密私匙，必填
        /// </summary>
        public string PrivateKey { get; }

        /// <summary>
        /// Symmetric security key, for data exchange, null means prevention exchange
        /// 对称安全私匙，用于数据交换，不设置标识禁止交换信息
        /// </summary>
        public string? SymmetricKey { get; }
    }
}
