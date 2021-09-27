using com.etsoo.CoreFramework.User;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.Database;
using Dapper;
using System.Data.Common;
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
        public virtual void AddSystemParameters(ICurrentUser user, DynamicParameters parameters)
        {
            parameters.Add("CurrentUser", user.Id);

            if (user.Organization != null)
                parameters.Add("CurrentOrg", user.Organization);
        }

        /// <summary>
        /// Build command name, ["member", "view"] => ep_member_view (default) or epMemberView (override to achieve)
        /// 构建命令名称
        /// </summary>
        /// <param name="identifier">Identifier, like procedure with 'p'</param>
        /// <param name="parts">Parts</param>
        /// <returns>Result</returns>
        public virtual string BuildCommandName(string identifier, IEnumerable<string> parts)
        {
            var command = $"e{identifier}_" + string.Join("_", parts);
            return command.ToLower();
        }

        /// <summary>
        /// Hash password
        /// 哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed bytes</returns>
        public string HashPassword(ReadOnlySpan<char> password)
        {
            return Convert.ToBase64String(CryptographyUtils.HMACSHA512(password, Configuration.PrivateKey));
        }

        /// <summary>
        /// Async hash password
        /// 异步哈希密码
        /// </summary>
        /// <param name="password">Raw password</param>
        /// <returns>Hashed password</returns>
        public async Task<string> HashPasswordAsync(string password)
        {
            return Convert.ToBase64String(await CryptographyUtils.HMACSHA512Async(password, Configuration.PrivateKey));
        }
    }
}
