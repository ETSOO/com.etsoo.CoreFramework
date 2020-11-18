using System;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application configuration
    /// 程序配置
    /// </summary>
    public record Configuration
    {
        /// <summary>
        /// Private constructor to prevent direct initialization
        /// 私有化构造函数防止直接初始化
        /// </summary>
        private Configuration(
            string[] languages,
            bool modelValidated,
            string privateKey,
            string symmetricKey
        ) =>
            (
                Languages,
                ModelValidated,
                PrivateKey,
                SymmetricKey
            ) = (
                languages,
                modelValidated,
                privateKey,
                symmetricKey
            );

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
        public string PrivateKey { get; }

        /// <summary>
        /// Symmetric security key, for data exchange, null means prevention exchange
        /// 对称安全私匙，用于数据交换，不设置标识禁止交换信息
        /// </summary>
        public string SymmetricKey { get; }

        /// <summary>
        /// 静态生成器对象
        /// </summary>
        public static ConfigurationBuilder Builder => new();

        /// <summary>
        /// Application configuration builder
        /// 程序配置生成器
        /// </summary>
        public class ConfigurationBuilder
        {
            /// <summary>
            /// Build
            /// 构造
            /// </summary>
            /// <returns>Core application</returns>
            public Configuration Build()
            {
                // Validation
                if (string.IsNullOrEmpty(_privateKey))
                {
                    throw new ArgumentNullException("PrivateKey");
                }

                // Return an initialization
                return new(
                    _languages ?? Array.Empty<string>(),
                    _modelValidated,
                    _privateKey,
                    _symmetricKey
                );
            }

            private string[] _languages;

            /// <summary>
            /// Set supported languages
            /// 设置支持的语言
            /// </summary>
            /// <param name="languages">Supported languages</param>
            /// <returns>Builder</returns>
            public ConfigurationBuilder Languages(string[] languages)
            {
                _languages = languages;
                return this;
            }

            private bool _modelValidated;

            /// <summary>
            /// Set Model DataAnnotations are validated, true under Web API to avoid double validation
            /// 设置模块数据标记已验证，在Web API下可以设置为true以避免重复验证
            /// </summary>
            /// <param name="modelValidated">Model validated or not</param>
            /// <returns>Builder</returns>
            public ConfigurationBuilder ModelValidated(bool modelValidated)
            {
                _modelValidated = modelValidated;
                return this;
            }

            private string _privateKey;

            /// <summary>
            /// Set private key for encryption/decryption
            /// 设置加解密私匙
            /// </summary>
            /// <param name="privateKey">Private key for encryption</param>
            /// <returns>Builder</returns>
            public ConfigurationBuilder PrivateKey(string privateKey)
            {
                _privateKey = privateKey;
                return this;
            }

            private string _symmetricKey;

            /// <summary>
            /// Set symmetric security key, for data exchange
            /// 设置对称安全私匙，用于数据交换
            /// </summary>
            /// <param name="symmetricKey">Symmetric security key, for data exchange</param>
            /// <returns>Builder</returns>
            public ConfigurationBuilder SymmetricKey(string symmetricKey)
            {
                _symmetricKey = symmetricKey;
                return this;
            }
        }
    }
}
