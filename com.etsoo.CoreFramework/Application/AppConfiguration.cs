using Microsoft.Extensions.Configuration;

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
        /// <param name="cultures">Supported cultures</param>
        /// <param name="modelValidated">Model DataAnnotations are validated or not</param>
        /// <param name="symmetricKey">Symmetric security key, for data exchange</param>
        /// <param name="webUrl">Web Url</param>
        public AppConfiguration(
            string privateKey,
            string[]? cultures = null,
            bool modelValidated = false,
            string? symmetricKey = null,
            string? webUrl = null
        )
        {
            // Default languages
            cultures ??= Array.Empty<string>();

            // Update
            (
                Cultures,
                ModelValidated,
                PrivateKey,
                SymmetricKey,
                WebUrl
            ) = (
                cultures,
                modelValidated,
                privateKey,
                symmetricKey,
                webUrl ?? "http://localhost"
            );
        }

        /// <summary>
        /// Constructor with configuration
        /// 使用配置的构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        /// <param name="modelValidated">Model DataAnnotations are validated or not</param>
        public AppConfiguration(IConfigurationSection section, bool modelValidated = false) : this(
            section.GetValue<string>("PrivateKey"),
            section.GetSection("Cultures").Get<IEnumerable<string>?>()?.ToArray(),
            modelValidated,
            section.GetValue<string?>("SymmetricKey"),
            section.GetValue<string?>("WebUrl"))
        {
        }

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

        /// <summary>
        /// Web url
        /// 网页地址
        /// </summary>

        public string WebUrl { get; }
    }
}
