using com.etsoo.Utils.Crypto;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application configuration
    /// 程序配置
    /// </summary>
    public record AppConfiguration
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
            return new AppConfiguration
            {
                Name = "MiniApp",
                PrivateKey = privateKey
            };
        }

        /// <summary>
        /// Supported cultures, like zh-CN, zh-Hans-CN, en
        /// 支持的文化，比如zh-CN, zh-Hans-CN, en
        /// </summary>
        public string[] Cultures { get; init; } = [];

        /// <summary>
        /// Model DataAnnotations are validated, true under Web API to avoid double validation
        /// 模块数据标记已验证，在Web API下可以设置为true以避免重复验证
        /// </summary>
        public bool ModelValidated { get; init; }

        /// <summary>
        /// Private key for hash or simple encryption/decryption, required
        /// 哈希或简单加密/解密私匙，必填
        /// </summary>
        public string PrivateKey { get; private set; } = string.Empty;

        /// <summary>
        /// Unique name
        /// 唯一名称
        /// </summary>
        public string Name { get; init; } = "SmartERP";

        /// <summary>
        /// Web url
        /// 网页地址
        /// </summary>
        public string WebUrl { get; init; } = "http://localhost";

        /// <summary>
        /// Api url
        /// 接口地址
        /// </summary>
        public string ApiUrl { get; init; } = "http://localhost/api";

        /// <summary>
        /// Cache hours
        /// 缓存小时数
        /// </summary>
        public double CacheHours { get; init; } = 24D;

        /// <summary>
        /// Init call encryption identifier
        /// 初始化调用加密标识
        /// </summary>
        public string InitCallEncryptionIdentifier { get; init; } = "InitCall";

        /// <summary>
        /// Unseal data
        /// 解封数据
        /// </summary>
        /// <param name="secureManager">Secure manager</param>
        public virtual void UnsealData(Func<string, string, string>? secureManager = null)
        {
            PrivateKey = CryptographyUtils.UnsealData(nameof(PrivateKey), PrivateKey, secureManager);
        }
    }
}
