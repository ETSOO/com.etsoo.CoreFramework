namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Application configuration interface
    /// 程序配置接口
    /// </summary>
    public interface IAppConfiguration
    {
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
        string PrivateKey { get; }

        /// <summary>
        /// Unique name
        /// 唯一名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Web url
        /// 网页地址
        /// </summary>

        string WebUrl { get; }

        /// <summary>
        /// Api url
        /// 接口地址
        /// </summary>
        string ApiUrl { get; }

        /// <summary>
        /// Cache hours
        /// 缓存小时数
        /// </summary>
        double CacheHours { get; }
    }
}
