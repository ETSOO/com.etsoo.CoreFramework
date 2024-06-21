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
        public string[] Cultures { get; set; } = [];

        /// <summary>
        /// Private key for hash or simple encryption/decryption, required
        /// 哈希或简单加密/解密私匙，必填
        /// </summary>
        public required string PrivateKey { get; set; }

        /// <summary>
        /// Unique name
        /// 唯一名称
        /// </summary>
        public string Name { get; set; } = "SmartERP";

        /// <summary>
        /// Web url
        /// 网页地址
        /// </summary>
        public string WebUrl { get; set; } = "http://localhost";

        /// <summary>
        /// Api url
        /// 接口地址
        /// </summary>
        public string ApiUrl { get; set; } = "http://localhost/api";

        /// <summary>
        /// Cache hours
        /// 缓存小时数
        /// </summary>
        public double CacheHours { get; set; } = 24D;

        /// <summary>
        /// Init call encryption identifier
        /// 初始化调用加密标识
        /// </summary>
        public string InitCallEncryptionIdentifier { get; set; } = "InitCall";
    }
}
