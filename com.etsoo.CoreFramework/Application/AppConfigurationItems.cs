namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application configuration items
    /// 程序配置项目
    /// </summary>
    public record AppConfigurationItems
    {
        /// <summary>
        /// Supported cultures, like zh-CN, zh-Hans-CN, en
        /// 支持的文化，比如zh-CN, zh-Hans-CN, en
        /// </summary>
        public string[]? Cultures { get; init; }

        /// <summary>
        /// Private key for hash or simple encryption/decryption, required
        /// 哈希或简单加密/解密私匙，必填
        /// </summary>
        public string? PrivateKey { get; init; }

        /// <summary>
        /// Unique name
        /// 唯一名称
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Web url
        /// 网页地址
        /// </summary>
        public string? WebUrl { get; init; }

        /// <summary>
        /// Api url
        /// 接口地址
        /// </summary>
        public string? ApiUrl { get; init; }

        /// <summary>
        /// Cache hours
        /// 缓存小时数
        /// </summary>
        public double? CacheHours { get; init; }
    }
}
