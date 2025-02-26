﻿using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;

namespace com.etsoo.Database
{
    /// <summary>
    /// Databse interface
    /// 数据库接口
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Database name
        /// 数据库名称
        /// </summary>
        DatabaseName Name { get; }

        /// <summary>
        /// Support stored procedure or not
        /// 是否支持存储过程
        /// </summary>
        bool SupportStoredProcedure { get; }

        /// <summary>
        /// Add Dapper parameter
        /// https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/types
        /// 添加 Dapper 参数
        /// </summary>
        /// <param name="parameters">Parameter collection</param>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="type">Value type</param>
        void AddParameter(IDbParameters parameters, string name, object? value, DbType type);

        /// <summary>
        /// Create command definition
        /// 创建命令定义
        /// </summary>
        /// <param name="name">Command name or text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="type">Type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        CommandDefinition CreateCommand(string name, IDbParameters? parameters = null, CommandType? type = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create delete command definition
        /// 创建删除命令定义
        /// </summary>
        /// <typeparam name="T">Generic id type</typeparam>
        /// <param name="tableName">Table name</param>
        /// <param name="ids">Multiple ids</param>
        /// <param name="idColumn">Id column name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        CommandDefinition CreateDeleteCommand<T>(string tableName, IEnumerable<T> ids, string idColumn = "id", CancellationToken cancellationToken = default) where T : struct;

        /// <summary>
        /// Create delete command definition
        /// 创建删除命令定义
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="ids">Multiple ids</param>
        /// <param name="idColumn">Id column name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        CommandDefinition CreateDeleteCommand(string tableName, IEnumerable<string> ids, string idColumn = "id", CancellationToken cancellationToken = default);

        /// <summary>
        /// Escape identifier
        /// 转义标识符
        /// </summary>
        /// <param name="name">Input name</param>
        /// <returns>Escaped name</returns>
        string EscapeIdentifier(string name);

        /// <summary>
        /// Escape SQL part
        /// 转义SQL部分
        /// </summary>
        /// <param name="part">SQL part</param>
        /// <returns>Result</returns>
        string EscapePart(string part);

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
            where K : notnull;

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
            where K : notnull;

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

        /// <summary>
        /// Join conditions
        /// 组合条件
        /// </summary>
        /// <param name="items">Condition items</param>
        /// <returns>Result</returns>
        string JoinConditions(IEnumerable<string> items);

        /// <summary>
        /// Join JSON fields
        /// 链接JSON字段
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="mappings">Mappings</param>
        /// <param name="policy">Naming policy</param>
        /// <param name="jsonPolicy">JSON Naming policy</param>
        /// <returns>Result</returns>
        string JoinJsonFields(IEnumerable<string> fields, Dictionary<string, string> mappings, NamingPolicy? policy = null, NamingPolicy? jsonPolicy = null);

        /// <summary>
        /// Join JSON fields
        /// 链接JSON字段
        /// </summary>
        /// <param name="mappings">Mapping fields</param>
        /// <param name="isObject">Is object node</param>
        /// <returns>Result</returns>
        string JoinJsonFields(Dictionary<string, string> mappings, bool isObject);

        /// <summary>
        /// Get query limit command
        /// 获取查询限制命令
        /// </summary>
        /// <param name="size">Lines to read</param>
        /// <param name="page">Current page</param>
        /// <returns>Query command</returns>
        string QueryLimit(uint size, uint page = 0);

        /// <summary>
        /// Get query limit command
        /// 获取查询限制命令
        /// </summary>
        /// <param name="data">Query paging data</param>
        /// <returns>Result</returns>
        string QueryLimit(QueryPagingData? data);

        /// <summary>
        /// Get update command
        /// 获取更新命令
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="alias">Alias</param>
        /// <param name="fields">Update fields</param>
        /// <returns>Command</returns>
        StringBuilder GetUpdateCommand(string tableName, string alias, string fields);

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The number of rows affected</returns>
        Task<int> ExecuteAsync(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The first cell selected as T</returns>
        Task<T?> ExecuteScalarAsync<T>(CommandDefinition command);

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The first cell selected as T</returns>
        Task<T?> ExecuteScalarAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<IEnumerable<T>> QueryAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<T?> QuerySingleAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async query command as source generated object list
        /// 异步执行命令返回源生成对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        IAsyncEnumerable<D> QuerySourceAsync<D>(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default) where D : IDataReaderParser<D>;

        /// <summary>
        /// Async query command as first object
        /// 异步执行命令返回源第一个对象
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        ValueTask<D?> QuerySourceFirstAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>;

        /// <summary>
        /// Async query command as source generated object list
        /// 异步执行命令返回源生成对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<D[]> QueryListAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>;

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="D2">Generic dataset 2 object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<(D1[], D2[])> QueryListAsync<D1, D2>(CommandDefinition command)
            where D1 : IDataReaderParser<D1>
            where D2 : IDataReaderParser<D2>;

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="D2">Generic dataset 2 object type</typeparam>
        /// <typeparam name="D3">Generic dataset 3 object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        Task<(D1[], D2[], D3[])> QueryListAsync<D1, D2, D3>(CommandDefinition command)
            where D1 : IDataReaderParser<D1>
            where D2 : IDataReaderParser<D2>
            where D3 : IDataReaderParser<D3>;

        /// <summary>
        /// Async query command as action result
        /// 异步执行命令返回操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        ValueTask<ActionResult?> QueryAsResultAsync(CommandDefinition command);
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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task WithConnection(Func<C, Task> func, CancellationToken cancellationToken = default);

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<T> WithConnection<T>(Func<C, Task<T>> func, CancellationToken cancellationToken = default);

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        ValueTask<T> WithValueConnection<T>(Func<C, ValueTask<T>> func, CancellationToken cancellationToken = default);
    }
}
