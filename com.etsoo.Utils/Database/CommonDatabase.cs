using com.etsoo.Utils.Models;
using com.etsoo.Utils.String;
using Dapper;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Common database
    /// 通用数据库
    /// </summary>
    /// <typeparam name="C">Generic database connection type</typeparam>
    public abstract class CommonDatabase<C> : IDatabase<C> where C : DbConnection
    {
        /// <summary>
        /// Database connection string
        /// 数据库链接字符串
        /// </summary>
        protected readonly string ConnectionString;

        /// <summary>
        /// Snake naming, like user_id
        /// 蛇形命名，如 user_id
        /// </summary>
        public bool SnakeNaming { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="snakeNaming">Is snake naming</param>
        public CommonDatabase(string connectionString, bool snakeNaming = false)
        {
            ConnectionString = connectionString;
            SnakeNaming = snakeNaming;

            // Default settings
            Dapper.SqlMapper.Settings.UseSingleResultOptimization = true;

            // Database snake naming
            if (snakeNaming)
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            }
        }

        /// <summary>
        /// Escape identifier
        /// 转义标识符
        /// </summary>
        /// <param name="name">Input name</param>
        /// <returns>Escaped name</returns>
        public abstract string EscapeIdentifier(string name);

        /// <summary>
        /// Get exception result
        /// 获取数据库异常结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        public abstract IDbExceptionResult GetExceptionResult(Exception ex);

        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        public abstract C NewConnection();

        /// <summary>
        /// New database context
        /// 新数据库上下文
        /// </summary>
        /// <typeparam name="M">Generic context class</typeparam>
        /// <returns>Context</returns>
        public abstract CommonDbContext<M> NewDbContext<M>() where M : class;

        /// <summary>
        /// Dictionary to Dapper parameter
        /// 字典转换为Dapper参数
        /// </summary>
        /// <typeparam name="K">Generic key type</typeparam>
        /// <typeparam name="V">Generic value type</typeparam>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyMaxLength">Char/byte key max length</param>
        /// <param name="valueMaxLength">Char/byte value max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        public object DictionaryToParameter<K, V>(Dictionary<K, V> dic, long? keyMaxLength = null, long? valueMaxLength = null, Func<SqlDbType, SqlDbType, string>? tvpFunc = null)
            where K : struct
            where V : struct
        {
            return DictionaryToParameter(dic,
                DatabaseUtils.TypeToDbType(typeof(K)).GetValueOrDefault(),
                DatabaseUtils.TypeToDbType(typeof(V)).GetValueOrDefault(),
                keyMaxLength, valueMaxLength, tvpFunc);
        }

        /// <summary>
        /// Dictionary to Dapper parameter
        /// 字典转换为Dapper参数
        /// </summary>
        /// <typeparam name="K">Generic key type</typeparam>
        /// <typeparam name="V">Generic value type</typeparam>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyType">Key type</param>
        /// <param name="valueType">Value type</param>
        /// <param name="keyMaxLength">Char/byte key max length</param>
        /// <param name="valueMaxLength">Char/byte value max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        public virtual object DictionaryToParameter<K, V>(Dictionary<K, V> dic, DbType keyType, DbType valueType, long? keyMaxLength = null, long? valueMaxLength = null, Func<SqlDbType, SqlDbType, string>? tvpFunc = null)
            where K : struct
            where V : struct
        {
            return JsonSerializer.Serialize(dic, new JsonSerializerOptions { WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }).ToDbString(true);
        }

        /// <summary>
        /// Guid items to Dapper parameters
        /// 转换Guid项目为Dapper参数
        /// </summary>
        /// <param name="items">Items</param>
        /// <param name="maxLength">Item max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        public virtual object GuidItemsToParameter(IEnumerable<GuidItem> items, long? maxLength = null, Func<string>? tvpFunc = null)
        {
            return JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }).ToDbString(true);
        }

        /// <summary>
        /// List to Dapper parameter
        /// 列表转换为Dapper参数
        /// </summary>
        /// <typeparam name="T">Generic item type</typeparam>
        /// <param name="list">List</param>
        /// <param name="maxLength">Char/byte item max length</param>
        /// <param name="tvpFunc">TVP building function or delimiter</param>
        /// <returns>Result</returns>
        public object ListToParameter<T>(IEnumerable<T> list, long? maxLength = null, Func<SqlDbType, string>? tvpFunc = null) where T : struct
        {
            return ListToParameter(list, DatabaseUtils.TypeToDbType(typeof(T)).GetValueOrDefault(), maxLength, tvpFunc);
        }

        /// <summary>
        /// List to Dapper parameter
        /// 列表转换为Dapper参数
        /// </summary>
        /// <typeparam name="T">Generic item type</typeparam>
        /// <param name="list">List</param>
        /// <param name="type">DbType</param>
        /// <param name="maxLength">Char/byte item max length</param>
        /// <param name="tvpFunc">TVP building function or delimiter</param>
        /// <returns>Result</returns>
        public virtual object ListToParameter(IEnumerable list, DbType type, long? maxLength = null, Func<SqlDbType, string>? tvpFunc = null)
        {
            // Default to be ANSI
            return string.Join(';', list).ToDbString(true);
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <param name="func">Callback function</param>
        /// <returns>Task</returns>
        public async Task WithConnection(Func<C, Task> func)
        {
            using var connection = NewConnection();

            await connection.OpenAsync();

            await func(connection);

            await connection.CloseAsync();
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <returns>Result</returns>
        public async Task<T> WithConnection<T>(Func<C, Task<T>> func)
        {
            using var connection = NewConnection();

            await connection.OpenAsync();

            return await func(connection);
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <returns>Result</returns>
        public async ValueTask<T> WithValueConnection<T>(Func<C, ValueTask<T>> func)
        {
            using var connection = NewConnection();

            await connection.OpenAsync();

            return await func(connection);
        }
    }
}
