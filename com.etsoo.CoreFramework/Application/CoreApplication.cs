using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.String;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application
    /// 核心程序
    /// </summary>
    /// <typeparam name="C">Generic database connection type</typeparam>
    public record CoreApplication<C> : ICoreApplication<C> where C : DbConnection
    {
        /// <summary>
        /// Application name
        /// 程序名称
        /// </summary>
        public static string AppName => "SmartERP";

        /// <summary>
        /// Get secret data
        /// 获取私密数据
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="ApplicationException"></exception>
        protected static string GetSecretData()
        {
            var guid = Environment.GetEnvironmentVariable(AppName, EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(guid)) throw new ApplicationException($"Secret data for {AppName} is not defined");
            guid = guid.Replace("-", "");
            guid = (char)((guid[0] + guid.Last()) / 2) + guid[2..];
            return guid;
        }

        /// <summary>
        /// Unseal data
        /// 解密信息
        /// </summary>
        /// <param name="input">Base64 input data</param>
        /// <returns>Unsealed data</returns>
        public static string UnsealData(string field, string? input)
        {
            if (string.IsNullOrEmpty(input)) throw new ApplicationException($"Empty input for {field}");

            var bytes = CryptographyUtils.AESDecrypt(input, GetSecretData());

            if (bytes == null) throw new ApplicationException($"Unseal input {StringUtils.HideData(input)} for {field} failed");

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        public virtual IAppConfiguration Configuration { get; }

        /// <summary>
        /// Default Json serializer options
        /// 默认的Json序列化器选项
        /// </summary>
        public JsonSerializerOptions DefaultJsonSerializerOptions { get; set; }

        /// <summary>
        /// Database
        /// 数据库
        /// </summary>
        public virtual IDatabase<C> DB { get; }

        /// <summary>
        /// ICoreApplicationBase.DB implementation
        /// </summary>
        IDatabase ICoreApplicationBase.DB => DB;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public CoreApplication(
            IAppConfiguration configuration,
            IDatabase<C> db
        )
        {
            // Json options
            DefaultJsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

            // Update
            (
                Configuration,
                DB
            ) = (
                configuration,
                db
            );
        }

        /// <summary>
        /// Constructor with tuple
        /// 元组的构造函数
        /// </summary>
        /// <param name="init">Init tuple</param>
        public CoreApplication((IAppConfiguration configuration,
            IDatabase<C> db) init) : this(init.configuration, init.db)
        {

        }

        /// <summary>
        /// Add system parameters, override it to localize parameters' type
        /// 添加系统参数，可以重写本地化参数类型
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="parameters">Parameters</param>
        public virtual void AddSystemParameters(IServiceUser user, IDbParameters parameters)
        {
            // Keep blank
        }

        /// <summary>
        /// Build command name, ["member", "view"] => ep_member_view (default) or epMemberView (override to achieve)
        /// 构建命令名称
        /// </summary>
        /// <param name="identifier">Identifier, like procedure with 'p'</param>
        /// <param name="parts">Parts</param>
        /// <param name="isSystem">Is system command</param>
        /// <returns>Result</returns>
        public virtual string BuildCommandName(string identifier, IEnumerable<string> parts, bool isSystem = false)
        {
            var command = $"e{identifier}_" + string.Join("_", parts);
            return command.ToLower();
        }

        /// <summary>
        /// Hash password bytes
        /// 哈希密码字节数组
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed bytes</returns>
        public byte[] HashPasswordBytes(ReadOnlySpan<char> password)
        {
            return CryptographyUtils.HMACSHA512(password, Configuration.PrivateKey);
        }

        /// <summary>
        /// Async hash password bytes
        /// 异步哈希密码字节数组
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed bytes</returns>
        public async Task<byte[]> HashPasswordBytesAsync(string password)
        {
            return await CryptographyUtils.HMACSHA512Async(password, Configuration.PrivateKey);
        }

        /// <summary>
        /// Hash password
        /// 哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed result</returns>
        public string HashPassword(ReadOnlySpan<char> password)
        {
            return Convert.ToBase64String(HashPasswordBytes(password));
        }

        /// <summary>
        /// Async hash password
        /// 异步哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed result</returns>
        public async Task<string> HashPasswordAsync(string password)
        {
            return Convert.ToBase64String(await HashPasswordBytesAsync(password));
        }
    }
}
