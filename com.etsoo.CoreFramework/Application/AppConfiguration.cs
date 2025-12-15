using com.etsoo.Utils.Crypto;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// Unique name
        /// 唯一名称
        /// </summary>
        public string Name { get; set; } = "SmartERP";

        /// <summary>
        /// Web url
        /// 网页地址
        /// </summary>
        [Url]
        public string WebUrl { get; set; } = "http://localhost";

        /// <summary>
        /// Api url
        /// 接口地址
        /// </summary>
        [Url]
        public string ApiUrl { get; set; } = "http://localhost/api";

        /// <summary>
        /// Cache hours
        /// 缓存小时数
        /// </summary>
        [Range(0, 2400)]
        public double CacheHours { get; set; } = 24D;

        /// <summary>
        /// Refresh token valid days
        /// 刷新令牌有效天数
        /// </summary>
        [Range(1, 300)]
        public int RefreshTokenDays { get; set; } = 30;

        /// <summary>
        /// Init call encryption identifier
        /// 初始化调用加密标识
        /// </summary>
        public string InitCallEncryptionIdentifier { get; set; } = "InitCall";
    }

    [OptionsValidator]
    public partial class ValidateAppConfiguration : IValidateOptions<AppConfiguration>
    {
    }
}
