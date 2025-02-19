using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.String;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.Json;

namespace com.etsoo.CoreFramework.Application
{
    /// <summary>
    /// Core application
    /// 核心程序
    /// </summary>
    /// <typeparam name="S">Generic configuration type</typeparam>
    /// <typeparam name="C">Generic database connection type</typeparam>
    public class CoreApplication<S, C> : ICoreApplication<S, C>
        where S : AppConfiguration
        where C : DbConnection
    {
        /// <summary>
        /// Get secret data
        /// 获取秘密数据
        /// </summary>
        /// <param name="appName">Application name</param>
        /// <returns>Result</returns>
        /// <exception cref="ApplicationException"></exception>
        protected static string GetSecretData(string appName)
        {
            var guid = Environment.GetEnvironmentVariable(appName, EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(guid)) throw new ApplicationException($"Secret data for {appName} is not defined");

            guid = guid.Replace("-", "");
            guid = (char)((guid[0] + guid.Last()) / 2) + guid[2..];

            if (appName != "SmartERP") guid += appName;

            return guid;
        }

        /// <summary>
        /// Unseal data
        /// 解密信息
        /// </summary>
        /// <param name="secretData">Secret data</param>
        /// <param name="field">Field name</param>
        /// <param name="input">Input data</param>
        /// <returns>Unsealed data</returns>
        protected static string UnsealData(string secretData, string field, string? input)
        {
            if (string.IsNullOrEmpty(input)) throw new ApplicationException($"Empty input for {field}");

            var bytes = CryptographyUtils.AESDecrypt(input, secretData);

            return bytes == null
                ? throw new ApplicationException($"Unseal input {StringUtils.HideData(input)} for {field} failed")
                : Encoding.UTF8.GetString(bytes);
        }

        private JsonSerializerOptions? _defaultJsonSerializerOptions;
        /// <summary>
        /// Default Json serializer options
        /// 默认的Json序列化器选项
        /// </summary>
        public JsonSerializerOptions DefaultJsonSerializerOptions
        {
            get
            {
                if (_defaultJsonSerializerOptions == null)
                {
                    var defaultOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                    _defaultJsonSerializerOptions = ConfigureJsonSerializerOptions(defaultOptions);
                }
                return _defaultJsonSerializerOptions!;
            }
            set
            {
                _defaultJsonSerializerOptions = ConfigureJsonSerializerOptions(value);
            }
        }

        /// <summary>
        /// Application id
        /// 应用编号
        /// </summary>
        public int AppId { get; protected init; }

        /// <summary>
        /// Application configuration
        /// 程序配置
        /// </summary>
        public S Configuration { get; }
        AppConfiguration ICoreApplicationBase.Configuration => Configuration;

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
        /// Model DataAnnotations are validated, true under Web API to avoid double validation
        /// 模块数据标记已验证，在Web API下可以设置为true以避免重复验证
        /// </summary>
        public bool ModelValidated { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="db">Database</param>
        /// <param name="modelValidated">Model validated or not</param>
        public CoreApplication(
            S configuration,
            IDatabase<C> db,
            bool modelValidated = false
        )
        {
            // Update
            (
                Configuration,
                DB,
                ModelValidated
            ) = (
                configuration,
                db,
                modelValidated
            );
        }

        /// <summary>
        /// Constructor with tuple
        /// 元组的构造函数
        /// </summary>
        /// <param name="init">Init tuple</param>
        /// <param name="modelValidated">Model validated or not</param>
        public CoreApplication((S configuration,
            IDatabase<C> db) init, bool modelValidated = false) : this(init.configuration, init.db, modelValidated)
        {
        }

        /// <summary>
        /// Add system parameters, override it to localize parameters' type
        /// 添加系统参数，可以重写本地化参数类型
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="parameters">Parameters</param>
        public virtual void AddSystemParameters(IUserToken user, IDbParameters parameters)
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
        /// Decript data
        /// 解密数据
        /// </summary>
        /// <param name="cipherText">Cipher text</param>
        /// <param name="key">Key</param>
        /// <returns>Result</returns>
        public string DecriptData(string cipherText, string key = "")
        {
            var bytes = CryptographyUtils.AESDecrypt(cipherText, key + Configuration.PrivateKey) ?? throw new ApplicationException("Decript Data Failed");
            var text = Encoding.UTF8.GetString(bytes);

            // Check timestamp
            var tsIndex = text.LastIndexOf("`TS");
            if (tsIndex > 0)
            {
                var ts = text[(tsIndex + 3)..];
                if (long.TryParse(ts, out var tsValue))
                {
                    text = text[..tsIndex];
                    if (DateTime.FromBinary(tsValue) < DateTime.UtcNow)
                    {
                        throw new ApplicationException("Decript Data Time Expired");
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Encript data
        /// 加密数据
        /// </summary>
        /// <param name="plainText">Plain text</param>
        /// <param name="key">Key</param>
        /// <param name="seconds">Valid in seconds, default is 3 hours</param>
        /// <returns>Result</returns>
        public string EncriptData(string plainText, string key = "", int seconds = 10800)
        {
            if (seconds > 0)
            {
                // Add timestamp
                plainText += "`TS" + DateTime.UtcNow.AddSeconds(seconds).ToBinary();
            }

            return CryptographyUtils.AESEncrypt(plainText, key + Configuration.PrivateKey);
        }

        /// <summary>
        /// Get exchange key
        /// </summary>
        /// <param name="appId">Application id</param>
        /// <param name="key">Encryption key</param>
        /// <returns>Result</returns>
        public virtual string GetExchangeKey(int appId, string key)
        {
            return $"App{appId}-{key}";
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

        /// <summary>
        /// Configure default Json serializer options
        /// 配置默认的 Json 序列化器选项
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        protected virtual JsonSerializerOptions ConfigureJsonSerializerOptions(JsonSerializerOptions options)
        {
            return SharedUtils.JsonDefaultSerializerOptionsSetup(options);
        }

        /// <summary>
        /// Get API user data
        /// 获取API用户数据
        /// </summary>
        /// <param name="organizationId">Organization id</param>
        /// <param name="userId">User id</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Action result</returns>
        public virtual async Task<IActionResult?> GetApiUserDataAsync(int organizationId, int userId, int deviceId, CancellationToken cancellationToken = default)
        {
            var parameters = new DbParameters();
            parameters.Add("CurrentOrg", organizationId);
            parameters.Add("CurrentUser", userId);
            parameters.Add("CurrentDevice", deviceId);

            var command = new CommandDefinition("ep_auth_get_api_user_data", parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return await DB.QueryAsResultAsync(command);
        }
    }
}
