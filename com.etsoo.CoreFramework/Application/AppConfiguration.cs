﻿using com.etsoo.Utils.Crypto;
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
        /// Create mini application configuration
        /// 创建迷你应用程序配置
        /// </summary>
        /// <param name="privateKey">Private key</param>
        /// <returns>Result</returns>
        public static AppConfiguration Create(string? privateKey = null)
        {
            privateKey ??= CryptographyUtils.CreateRandString(RandStringKind.All, 12).ToString();
            return new AppConfiguration(privateKey, name: "MiniApp");
        }

        private const string PrivateKeyField = "PrivateKey";

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="privateKey">Private key for encryption/decryption</param>
        /// <param name="cultures">Supported cultures</param>
        /// <param name="name">Unique name</param>
        /// <param name="modelValidated">Model DataAnnotations are validated or not</param>
        /// <param name="webUrl">Web Url</param>
        /// <param name="apiUrl">API Url</param>
        /// <param name="cacheHours">Cache hours</param>
        public AppConfiguration(
            string privateKey,
            string[]? cultures = null,
            string? name = null,
            bool modelValidated = false,
            string? webUrl = null,
            string? apiUrl = null,
            double cacheHours = 24
        )
        {
            // Default languages
            Cultures = cultures ?? Array.Empty<string>();

            ModelValidated = modelValidated;
            WebUrl = webUrl ?? "http://localhost";
            ApiUrl = apiUrl ?? "http://localhost/api";
            CacheHours = cacheHours;

            if (string.IsNullOrEmpty(name))
            {
                // Default case
                name = "SmartERP";
            }
            else
            {
                // Add variable for security
                privateKey = name + privateKey;
            }

            Name = name;
            PrivateKey = privateKey;
        }

        /// <summary>
        /// Constructor with configuration
        /// 使用配置的构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        /// <param name="secureManager">Secure manager</param>
        /// <param name="modelValidated">Model DataAnnotations are validated or not</param>
        public AppConfiguration(IConfigurationSection section, Func<string, string, string>? secureManager = null, bool modelValidated = false) : this(
            CryptographyUtils.UnsealData(PrivateKeyField, section.GetValue<string>(PrivateKeyField), secureManager),
            section.GetSection("Cultures").Get<IEnumerable<string>?>()?.ToArray(),
            section.GetValue<string?>("Name"),
            modelValidated,
            section.GetValue<string?>("WebUrl"),
            section.GetValue<string?>("ApiUrl"),
            section.GetValue<double?>("CacheHours").GetValueOrDefault(24))
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
        /// Private key for hash or simple encryption/decryption, required
        /// 哈希或简单加密/解密私匙，必填
        /// </summary>
        public string PrivateKey { get; }

        /// <summary>
        /// Unique name
        /// 唯一名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Web url
        /// 网页地址
        /// </summary>
        public string WebUrl { get; }

        /// <summary>
        /// Api url
        /// 接口地址
        /// </summary>
        public string ApiUrl { get; }

        /// <summary>
        /// Cache hours
        /// 缓存小时数
        /// </summary>
        public double CacheHours { get; }
    }
}
