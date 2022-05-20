using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace com.etsoo.Database
{
    /// <summary>
    /// Databse interface
    /// 数据库接口
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Snake naming, like user_id
        /// 蛇形命名，如 user_id
        /// </summary>
        bool SnakeNaming { get; }

        /// <summary>
        /// Support stored procedure or not
        /// 是否支持存储过程
        /// </summary>
        bool SupportStoredProcedure { get; }

        /// <summary>
        /// Escape identifier
        /// 转义标识符
        /// </summary>
        /// <param name="name">Input name</param>
        /// <returns>Escaped name</returns>
        string EscapeIdentifier(string name);

        /// <summary>
        /// Get exception result
        /// 获取数据库异常结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        IDbExceptionResult GetExceptionResult(Exception ex);

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
        object DictionaryToParameter<K, V>(Dictionary<K, V> dic, long? keyMaxLength = null, long? valueMaxLength = null, Func<SqlDbType, SqlDbType, string>? tvpFunc = null)
            where K : struct
            where V : struct;

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
        object DictionaryToParameter<K, V>(Dictionary<K, V> dic, DbType keyType, DbType valueType, long? keyMaxLength = null, long? valueMaxLength = null, Func<SqlDbType, SqlDbType, string>? tvpFunc = null)
            where K : struct
            where V : struct;

        /// <summary>
        /// Guid items to Dapper parameters
        /// 转换Guid项目为Dapper参数
        /// </summary>
        /// <param name="items">Items</param>
        /// <param name="maxLength">Item max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        object GuidItemsToParameter(IEnumerable<GuidItem> items, long? maxLength = null, Func<string>? tvpFunc = null);

        /// <summary>
        /// List to Dapper parameter
        /// 列表转换为Dapper参数
        /// </summary>
        /// <typeparam name="T">Generic item type</typeparam>
        /// <param name="list">List</param>
        /// <param name="maxLength">Char/byte item max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        object ListToParameter<T>(IEnumerable<T> list, long? maxLength = null, Func<SqlDbType, string>? tvpFunc = null) where T : struct;

        /// <summary>
        /// List to Dapper parameter
        /// 列表转换为Dapper参数
        /// </summary>
        /// <typeparam name="T">Generic item type</typeparam>
        /// <param name="list">List</param>
        /// <param name="type">DbType</param>
        /// <param name="maxLength">Char/byte item max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        object ListToParameter(IEnumerable list, DbType type, long? maxLength = null, Func<SqlDbType, string>? tvpFunc = null);
    }

    /// <summary>
    /// Generic database interface
    /// 通用数据库接口
    /// </summary>
    /// <typeparam name="C">Generic connection type</typeparam>
    public interface IDatabase<C> : IDatabase where C : DbConnection
    {
        /// <summary>
        /// New database connection
        /// 新数据库链接对象
        /// </summary>
        /// <returns>Connection</returns>
        C NewConnection();

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <param name="func">Callback function</param>
        /// <returns>Task</returns>
        Task WithConnection(Func<C, Task> func);

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <returns>Result</returns>
        Task<T> WithConnection<T>(Func<C, Task<T>> func);

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <returns>Result</returns>
        ValueTask<T> WithValueConnection<T>(Func<C, ValueTask<T>> func);

        /// <summary>
        /// Execute a command asynchronously
        /// </summary>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The number of rows affected</returns>
        Task<int> ExecuteAsync(CommandDefinition command);

        /// <summary>
        /// Execute a command asynchronously
        /// </summary>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>The number of rows affected</returns>
        Task<int> ExecuteAsync(string commandText, object? parameters = null, CommandType? commandType = null);

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The first cell selected as T</returns>
        Task<T> ExecuteScalarAsync<T>(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>The first cell selected as T</returns>
        Task<T> ExecuteScalarAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null);

        /// <summary>
        /// Async query command as source generated object list
        /// 异步执行命令返回源生成对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        IAsyncEnumerable<D> QuerySourceAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>;

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command);

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>Result</returns>
        Task<IEnumerable<T>> QueryAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null);

        /// <summary>
        /// Async query command as single object
        /// 异步执行命令返回单个对象
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<T?> QuerySingleAsync<T>(CommandDefinition command);

        /// <summary>
        /// Async query command as single object
        /// 异步执行命令返回单个对象
        /// </summary>
        /// <typeparam name="T">Generic object type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>Result</returns>
        Task<T?> QuerySingleAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null);

        /// <summary>
        /// Async query command as source generated object list
        /// 异步执行命令返回源生成对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <returns>Result</returns>
        IAsyncEnumerable<D> QuerySourceAsync<D>(string commandText, object? parameters = null, CommandType? commandType = null) where D : IDataReaderParser<D>;

        /// <summary>
        /// Async query command as action result
        /// 异步执行命令返回操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        ValueTask<ActionResult?> QueryAsResultAsync(CommandDefinition command);
    }
}
