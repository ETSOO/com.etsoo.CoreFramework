using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.etsoo.Database
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
        /// Support stored procedure or not
        /// 是否支持存储过程
        /// </summary>
        public virtual bool SupportStoredProcedure => false;

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
                DefaultTypeMap.MatchNamesWithUnderscores = true;
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
            return JsonSerializer.Serialize(dic, new JsonSerializerOptions { WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }).ToDbStringSafe(true);
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
            return JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = false, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }).ToDbStringSafe(true);
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
            return string.Join(';', list).ToDbStringSafe(true);
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <param name="func">Callback function</param>
        /// <returns>Task</returns>
        public async Task WithConnection(Func<C, Task> func)
        {
            await using var connection = NewConnection();

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
            await using var connection = NewConnection();

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
            await using var connection = NewConnection();

            await connection.OpenAsync();

            return await func(connection);
        }

        /// <summary>
        /// Execute a command asynchronously
        /// SQL Server: SET NOCOUNT OFF, MySQL: UseAffectedRows = True
        /// </summary>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> ExecuteAsync(CommandDefinition command)
        {
            return await WithConnection((connection) =>
            {
                return connection.ExecuteAsync(command);
            });
        }

        /// <summary>
        /// Execute a command asynchronously
        /// SQL Server: SET NOCOUNT OFF, MySQL: UseAffectedRows = True
        /// </summary>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> ExecuteAsync(string commandText, object? parameters = null, CommandType? commandType = null)
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType);
            return await ExecuteAsync(command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The first cell selected as T</returns>
        public async Task<T> ExecuteScalarAsync<T>(CommandDefinition command)
        {
            return await WithConnection((connection) =>
            {
                return connection.ExecuteScalarAsync<T>(command);
            });
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>The first cell selected as T</returns>
        public async Task<T> ExecuteScalarAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null)
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType);
            return await ExecuteScalarAsync<T>(command);
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command)
        {
            return await WithConnection((connection) =>
            {
                return connection.QueryAsync<T>(command);
            });
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>Result</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null)
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType);
            return await QueryAsync<T>(command);
        }

        /// <summary>
        /// Async query command as single object
        /// 异步执行命令返回单个对象
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<T?> QuerySingleAsync<T>(CommandDefinition command)
        {
            return await WithConnection((connection) =>
            {
                return connection.QueryFirstOrDefaultAsync<T>(command);
            });
        }

        /// <summary>
        /// Async query command as single object
        /// 异步执行命令返回单个对象
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>Result</returns>
        public async Task<T?> QuerySingleAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null)
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType);
            return await QuerySingleAsync<T>(command);
        }

        /// <summary>
        /// Async query command as source generated object list
        /// 异步执行命令返回源生成对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        [RequiresPreviewFeatures]
        public async IAsyncEnumerable<D> QuerySourceAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>
        {
            await using var connection = NewConnection();

            await using var reader = await connection.ExecuteReaderAsync(command);

            var items = D.CreateAsync(reader);

            await foreach (var item in items)
            {
                yield return item;
            }

            await reader.CloseAsync();
            await connection.CloseAsync();
        }

        /// <summary>
        /// Async query command as source generated object list
        /// 异步执行命令返回源生成对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>Result</returns>
        [RequiresPreviewFeatures]
        public IAsyncEnumerable<D> QuerySourceAsync<D>(string commandText, object? parameters = null, CommandType? commandType = null) where D : IDataReaderParser<D>
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType);
            return QuerySourceAsync<D>(command);
        }

        /// <summary>
        /// Async query command as action result
        /// 异步执行命令返回操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public async ValueTask<ActionResult?> QueryAsResultAsync(CommandDefinition command)
        {
            return await WithValueConnection((connection) =>
            {
                return connection.QueryAsResultAsync(command);
            });
        }
    }
}
