using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using Dapper;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;

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
        /// Boolean suffix
        /// 逻辑值后缀
        /// </summary>
        protected const string BooleanSuffix = ":boolean";

        /// <summary>
        /// JSON suffix
        /// JSON值后缀
        /// </summary>
        protected const string JsonSuffix = ":json";

        /// <summary>
        /// Database name
        /// 数据库名称
        /// </summary>
        public DatabaseName Name { get; }

        /// <summary>
        /// Database connection string
        /// 数据库链接字符串
        /// </summary>
        protected readonly string ConnectionString;

        /// <summary>
        /// Support stored procedure or not
        /// 是否支持存储过程
        /// </summary>
        public virtual bool SupportStoredProcedure => false;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="name">Database name</param>
        /// <param name="connectionString">Connection string</param>
        public CommonDatabase(DatabaseName name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;

            // Default settings
            SqlMapper.Settings.UseSingleResultOptimization = true;

            // Database snake naming
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        /// <summary>
        /// Escape identifier
        /// 转义标识符
        /// </summary>
        /// <param name="name">Input name</param>
        /// <returns>Escaped name</returns>
        public abstract string EscapeIdentifier(string name);

        /// <summary>
        /// Escape SQL part
        /// 转义SQL部分
        /// </summary>
        /// <param name="part">SQL part</param>
        /// <returns>Result</returns>
        public virtual string EscapePart(string part)
        {
            var parts = part.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return EscapeIdentifier(parts[0]);
            }
            else
            {
                return string.Join('.', parts.Select(EscapeIdentifier));
            }
        }

        /// <summary>
        /// Get exception result
        /// 获取数据库异常结果
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Result</returns>
        public abstract IDbExceptionResult GetExceptionResult(Exception ex);

        /// <summary>
        /// Add Dapper parameter
        /// https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/types
        /// 添加 Dapper 参数
        /// </summary>
        /// <param name="parameters">Parameter collection</param>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        /// <param name="type">Value type</param>
        public virtual void AddParameter(IDbParameters parameters, string name, object? value, DbType type)
        {
            // When value is null, still need to keep the parameter
            parameters.Add(name, value, type);
        }

        /// <summary>
        /// Create command definition
        /// 创建命令定义
        /// </summary>
        /// <param name="name">Command name or text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="type">Type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public virtual CommandDefinition CreateCommand(string name, IDbParameters? parameters = null, CommandType? type = null, CancellationToken cancellationToken = default)
        {
            type ??=  SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

            // For stored procedure, remove null value parameters
            if (type == CommandType.StoredProcedure) parameters?.ClearNulls();

            return new CommandDefinition(name, parameters, commandType: type, cancellationToken: cancellationToken);
        }

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
        public virtual CommandDefinition CreateDeleteCommand<T>(string tableName, IEnumerable<T> ids, string idColumn = "id", CancellationToken cancellationToken = default)
            where T : struct
        {
            var sql = $"DELETE FROM {EscapeIdentifier(tableName)} WHERE {EscapeIdentifier(idColumn)} IN ({string.Join(",", ids)})";
            return CreateCommand(sql, null, CommandType.Text, cancellationToken);
        }

        /// <summary>
        /// Create delete command definition
        /// 创建删除命令定义
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="ids">Multiple ids</param>
        /// <param name="idColumn">Id column name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public virtual CommandDefinition CreateDeleteCommand(string tableName, IEnumerable<string> ids, string idColumn = "id", CancellationToken cancellationToken = default)
        {
            if (ids.Any(id => id.Length > 256 || id.Contains('\'') || id.Contains('"')))
            {
                throw new ArgumentException("Invalid id in the list");
            }

            var sql = $"DELETE FROM {EscapeIdentifier(tableName)} WHERE {EscapeIdentifier(idColumn)} IN ('{string.Join("','", ids)}')";

            return CreateCommand(sql, null, CommandType.Text, cancellationToken);
        }

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
            where K : notnull
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
            where K : notnull
        {
            return DatabaseUtils.DictionaryToJsonString(dic, keyType, valueType).ToDbStringSafe(true);
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
            return DatabaseUtils.GuidItemsToJsonString(items).ToDbStringSafe(true);
        }

        /// <summary>
        /// Join JSON fields
        /// 链接JSON字段
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="mappings">Mappings</param>
        /// <param name="policy">Naming policy</param>
        /// <param name="jsonPolicy">JSON Naming policy</param>
        /// <returns>Result</returns>
        public string JoinJsonFields(IEnumerable<string> fields, Dictionary<string, string> mappings, NamingPolicy? policy = null, NamingPolicy? jsonPolicy = null)
        {
            var formatted = fields.Select(item =>
            {
                string sField;
                string dbField;
                string result;
                string suffix;

                var (field, alias) = DatabaseUtils.SplitField(item);

                if (field.EndsWith(BooleanSuffix))
                {
                    suffix = BooleanSuffix;
                    field = field[..^BooleanSuffix.Length];
                    DoBoolFieldSuffix(ref field, ref suffix);
                }
                else if (field.EndsWith(JsonSuffix))
                {
                    suffix = JsonSuffix;
                    field = field[..^JsonSuffix.Length];
                    DoJsonFieldSuffix(ref field, ref suffix);
                }
                else
                {
                    suffix = string.Empty;
                }

                if (string.IsNullOrEmpty(alias))
                {
                    // Field maybe is "Name" or "u.Name"
                    // Should get rid of "u." to escape
                    var index = field.IndexOf('.');
                    if (index > 0 && index < 20)
                    {
                        sField = field[(index + 1)..];
                        dbField = sField.ToNamingCase(policy);
                        result = $"{field[..index]}.{EscapeIdentifier(dbField)}";
                    }
                    else
                    {
                        sField = field;
                        dbField = field.ToNamingCase(policy);
                        result = EscapeIdentifier(dbField);
                    }
                }
                else
                {
                    // Field may support multiple databases with format: SQLServer:**^SQLite:**
                    // Put the default one without database in the first position
                    var parts = field.Split('^');
                    var match = $"{Name}:";
                    var part = parts.FirstOrDefault(p => p.StartsWith(match))?[match.Length..] ?? parts[0];

                    sField = alias;
                    dbField = alias.ToNamingCase(policy);

                    // Replace the field
                    part = part.Replace("{F}", dbField);

                    result = $"{part} AS {EscapeIdentifier(dbField)}";
                }

                var jsonField = (jsonPolicy == null || jsonPolicy == policy) ? dbField : sField.ToNamingCase(jsonPolicy);
                mappings[jsonField] = dbField + suffix;

                return result;
            });

            return string.Join(", ", formatted);
        }

        /// <summary>
        /// Do boolean field suffix
        /// 处理逻辑字段后缀
        /// </summary>
        /// <param name="field">Select field</param>
        /// <param name="suffix">Suffix</param>
        protected abstract void DoBoolFieldSuffix(ref string field, ref string suffix);

        /// <summary>
        /// Do JSON field suffix
        /// 处理JSON字段后缀
        /// </summary>
        /// <param name="field">Select field</param>
        /// <param name="suffix">Suffix</param>
        protected abstract void DoJsonFieldSuffix(ref string field, ref string suffix);

        /// <summary>
        /// Join JSON fields
        /// 链接JSON字段
        /// </summary>
        /// <param name="mappings">Mapping fields</param>
        /// <param name="isObject">Is object node</param>
        /// <returns>Result</returns>
        public abstract string JoinJsonFields(Dictionary<string, string> mappings, bool isObject);

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
            // return string.Join(';', list).ToDbStringSafe(true);
            return DatabaseUtils.ListItemsToJsonString(list, type).ToDbStringSafe(true);
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <param name="func">Callback function</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public async Task WithConnection(Func<C, Task> func, CancellationToken cancellationToken = default)
        {
            await using var connection = NewConnection();

            await connection.OpenAsync(cancellationToken);

            await func(connection);

            await connection.CloseAsync();
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<T> WithConnection<T>(Func<C, Task<T>> func, CancellationToken cancellationToken = default)
        {
            await using var connection = NewConnection();

            await connection.OpenAsync(cancellationToken);

            return await func(connection);
        }

        /// <summary>
        /// With callback connection
        /// 带回调的数据库链接
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="func">Callback function</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async ValueTask<T> WithValueConnection<T>(Func<C, ValueTask<T>> func, CancellationToken cancellationToken = default)
        {
            await using var connection = NewConnection();

            await connection.OpenAsync(cancellationToken);

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
            }, command.CancellationToken);
        }

        /// <summary>
        /// Execute a command asynchronously
        /// SQL Server: SET NOCOUNT OFF, MySQL: UseAffectedRows = True
        /// </summary>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> ExecuteAsync(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType, cancellationToken: cancellationToken);
            return await ExecuteAsync(command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The first cell selected as T</returns>
        public async Task<T?> ExecuteScalarAsync<T>(CommandDefinition command)
        {
            return await WithConnection((connection) =>
            {
                return connection.ExecuteScalarAsync<T>(command);
            }, command.CancellationToken);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="commandText">The command to execute on this connection</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="commandType">Command type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The first cell selected as T</returns>
        public async Task<T?> ExecuteScalarAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType, cancellationToken: cancellationToken);
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
            }, command.CancellationToken);
        }

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
        public async Task<IEnumerable<T>> QueryAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType, cancellationToken: cancellationToken);
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
            }, command.CancellationToken);
        }

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
        public async Task<T?> QuerySingleAsync<T>(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType, cancellationToken: cancellationToken);
            return await QuerySingleAsync<T>(command);
        }

        /// <summary>
        /// Async query command as source generated object list
        /// 异步执行命令返回源生成对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async IAsyncEnumerable<D> QuerySourceAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>
        {
            await using var connection = NewConnection();

            await using var reader = await connection.ExecuteReaderAsync(command, CommandBehavior.SingleResult);

            var items = D.CreateAsync(reader, command.CancellationToken);
            await foreach (var item in items) yield return item;

            await reader.CloseAsync();
            await connection.CloseAsync();
        }

        /// <summary>
        /// Async query command as first object
        /// 异步执行命令返回源第一个对象
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async ValueTask<D?> QuerySourceFirstAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>
        {
            var items = QuerySourceAsync<D>(command);
            await foreach (var item in items) return item;
            return default;
        }

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
        public IAsyncEnumerable<D> QuerySourceAsync<D>(string commandText, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default) where D : IDataReaderParser<D>
        {
            commandType ??= (SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text);
            var command = new CommandDefinition(commandText, DatabaseUtils.FormatParameters(parameters), commandType: commandType, cancellationToken: cancellationToken);
            return QuerySourceAsync<D>(command);
        }

        /// <summary>
        /// Async query command as source generated object list
        /// 异步执行命令返回源生成对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<D[]> QueryListAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>
        {
            await using var connection = NewConnection();

            await using var reader = await connection.ExecuteReaderAsync(command, CommandBehavior.SingleResult);

            var items = await D.CreateListAsync(reader, command.CancellationToken);

            await reader.CloseAsync();
            await connection.CloseAsync();

            return [.. items];
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="D2">Generic dataset 2 object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<(D1[], D2[])> QueryListAsync<D1, D2>(CommandDefinition command)
            where D1 : IDataReaderParser<D1>
            where D2 : IDataReaderParser<D2>
        {
            await using var reader = await NewConnection().ExecuteReaderAsync(command, CommandBehavior.Default);

            var d1 = (await D1.CreateListAsync(reader, command.CancellationToken)).ToArray();

            var d2Next = await reader.NextResultAsync(command.CancellationToken);
            var d2 = d2Next ? [.. (await D2.CreateListAsync(reader, command.CancellationToken))] : Array.Empty<D2>();

            await reader.CloseAsync();
            await reader.DisposeAsync();

            return (d1, d2);
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="D2">Generic dataset 2 object type</typeparam>
        /// <typeparam name="D3">Generic dataset 3 object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<(D1[], D2[], D3[])> QueryListAsync<D1, D2, D3>(CommandDefinition command)
            where D1 : IDataReaderParser<D1>
            where D2 : IDataReaderParser<D2>
            where D3 : IDataReaderParser<D3>
        {
            await using var reader = await NewConnection().ExecuteReaderAsync(command, CommandBehavior.Default);

            var d1 = (await D1.CreateListAsync(reader, command.CancellationToken)).ToArray();

            var d2Next = await reader.NextResultAsync(command.CancellationToken);
            var d2 = d2Next ? (await D2.CreateListAsync(reader, command.CancellationToken)).ToArray() : [];

            D3[] d3;
            if (d2Next && await reader.NextResultAsync(command.CancellationToken))
            {
                d3 = [.. (await D3.CreateListAsync(reader, command.CancellationToken))];
            }
            else
            {
                d3 = [];
            }

            await reader.CloseAsync();
            await reader.DisposeAsync();

            return (d1, d2, d3);
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
            }, command.CancellationToken);
        }

        /// <summary>
        /// Join conditions
        /// 组合条件
        /// </summary>
        /// <param name="items">Condition items</param>
        /// <returns>Result</returns>
        public virtual string JoinConditions(IEnumerable<string> items)
        {
            var conditions = string.Join(" AND ", items);
            if (string.IsNullOrEmpty(conditions)) return string.Empty;
            return $"WHERE {conditions}";
        }

        /// <summary>
        /// Get query limit command
        /// 获取查询限制命令
        /// </summary>
        /// <param name="size">Lines to read</param>
        /// <param name="page">Current page, start from 0, means first page is 0</param>
        /// <returns>Query command</returns>
        public virtual string QueryLimit(uint size, uint page = 0)
        {
            if (page < 1)
            {
                return $"LIMIT {size}";
            }
            else
            {
                // Start from 0, 1 means reading the second page
                // 从0开始，1表示读取第二页
                var offset = page * size;
                return $"LIMIT {size} OFFSET {offset}";
            }
        }

        /// <summary>
        /// Get query limit command
        /// 获取查询限制命令
        /// </summary>
        /// <param name="data">Query paging data</param>
        /// <returns>Result</returns>
        public string QueryLimit(QueryPagingData? data)
        {
            return QueryLimit(data?.BatchSize ?? 8, data?.CurrentPage ?? 0);
        }

        /// <summary>
        /// Get update command
        /// 获取更新命令
        /// https://www.sqlite.org/lang_update.html
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="alias">Alias</param>
        /// <param name="fields">Update fields</param>
        /// <returns>Command</returns>
        public virtual StringBuilder GetUpdateCommand(string tableName, string alias, string fields)
        {
            var sql = new StringBuilder("UPDATE ");
            sql.Append(EscapeIdentifier(tableName));
            sql.Append($" AS {alias} SET ");
            sql.Append(fields);

            return sql;
        }
    }
}
