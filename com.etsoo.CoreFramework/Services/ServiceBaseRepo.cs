using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.DB;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.SpanMemory;
using com.etsoo.Utils.String;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text.RegularExpressions;

namespace com.etsoo.CoreFramework.Services
{
    public abstract partial class ServiceBase<S, C, A, U> : IServiceBase
        where S : AppConfiguration
        where C : DbConnection
        where A : ICoreApplication<S, C>
        where U : IServiceUser
    {
        /// <summary>
        /// Add system parameters
        /// 添加系统参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public virtual void AddSystemParameters(IDbParameters parameters)
        {
            // When the user is not required, there may still be an associated user
            // 当用户不是必须的时候，仍然可能存在一个关联的用户
            if (User != null)
            {
                App.AddSystemParameters(User, parameters);
            }
        }

        /// <summary>
        /// Create command, default parameters added
        /// 创建命令，附加默认参数
        /// </summary>
        /// <param name="name">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="type">Command type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Command</returns>
        protected CommandDefinition CreateCommand(string name, IDbParameters? parameters = null, CommandType? type = null, CancellationToken cancellationToken = default)
        {
            var command = App.DB.CreateCommand(name, parameters, type, cancellationToken);
            return command;
        }

        /// <summary>
        /// Execute a command asynchronously
        /// SQL Server: SET NOCOUNT OFF, MySQL: UseAffectedRows = True
        /// </summary>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The number of rows affected</returns>
        public async Task<int> ExecuteAsync(CommandDefinition command)
        {
            return await App.DB.ExecuteAsync(command);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="command">The command to execute on this connection</param>
        /// <returns>The first cell selected as T</returns>
        public async Task<T?> ExecuteScalarAsync<T>(CommandDefinition command)
        {
            return await App.DB.ExecuteScalarAsync<T>(command);
        }

        /// <summary>
        /// Filter range
        /// 过滤区域
        /// </summary>
        /// <param name="range"></param>
        /// <param name="triggerFailureExcpetion"></param>
        /// <returns></returns>
        protected bool FilterRange(ReadOnlySpan<char> range, bool triggerFailureExcpetion = true)
        {
            var valid = range.All(c => c is '_' or >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9');

            if (!valid && triggerFailureExcpetion)
            {
                throw new ArgumentOutOfRangeException(nameof(range));
            }

            return valid;
        }

        /// <summary>
        /// Format parameters
        /// 格式化参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns>Result</returns>
        virtual protected IDbParameters FormatParameters(object parameters)
        {
            return DBUtils.FormatParameters(parameters, App);
        }

        /// <summary>
        /// Get command name, concat with AppId and Flag, normally is stored procedure name, pay attention to SQL injection
        /// 获取命令名称，附加程序编号和实体标识，一般是存储过程名称，需要防止注入攻击
        /// </summary>
        /// <param name="parts">Parts</param>
        /// <returns>Command name</returns>
        protected string GetCommandName(params string[] parts)
        {
            if (parts.Length == 1)
            {
                // Only one item, support to pass blank or underscore seperated item, like "read as json" to be "read_as_json"
                return GetCommandNameBase(parts[0].Split(separators, StringSplitOptions.RemoveEmptyEntries));
            }

            return GetCommandNameBase(parts);
        }

        /// <summary>
        /// Get command name base, concat with AppId and Flag, normally is stored procedure name, pay attention to SQL injection
        /// 获取命令名称基础，附加程序编号和实体标识，一般是存储过程名称，需要防止注入攻击
        /// </summary>
        /// <param name="parts">Parts</param>
        /// <returns>Command name</returns>
        protected virtual string GetCommandNameBase(IEnumerable<string> parts)
        {
            if (!parts.Any())
            {
                throw new ArgumentNullException(nameof(parts));
            }

            return App.BuildCommandName(CommandIdentifier.Procedure, parts.Prepend(Flag), false);
        }

        /// <summary>
        /// Async query command as object
        /// 异步执行命令返回对象
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async ValueTask<D?> QueryAsAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>
        {
            return await App.DB.QuerySourceFirstAsync<D>(command);
        }

        /// <summary>
        /// Async query command as object source list
        /// 异步执行命令返回对象源列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public IAsyncEnumerable<D> QueryAsSourceAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>
        {
            return App.DB.QuerySourceAsync<D>(command);
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<D[]> QueryAsListAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>
        {
            return await App.DB.QueryListAsync<D>(command);
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D1">Generic dataset 1 object type</typeparam>
        /// <typeparam name="D2">Generic dataset 2 object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public async Task<(D1[], D2[])> QueryAsListAsync<D1, D2>(CommandDefinition command)
            where D1 : IDataReaderParser<D1>
            where D2 : IDataReaderParser<D2>
        {
            return await App.DB.QueryListAsync<D1, D2>(command);
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
        public async Task<(D1[], D2[], D3[])> QueryAsListAsync<D1, D2, D3>(CommandDefinition command)
            where D1 : IDataReaderParser<D1>
            where D2 : IDataReaderParser<D2>
            where D3 : IDataReaderParser<D3>
        {
            return await App.DB.QueryListAsync<D1, D2, D3>(command);
        }

        /// <summary>
        /// Async query command as action result
        /// 异步执行命令返回操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public async ValueTask<IActionResult> QueryAsResultAsync(CommandDefinition command)
        {
            var result = await App.DB.QueryAsResultAsync(command);

            if (result == null)
            {
                return ApplicationErrors.NoActionResult.AsResult();
            }

            if (!result.Ok && result.Title == null && result.Type != null)
            {
                var error = ApplicationErrors.Get(result.Type);
                if (error != null)
                {
                    result.Title = error.Title;
                }
            }

            return result;
        }

        /// <summary>
        /// Async read text data (JSON/XML) to PipeWriter
        /// 异步读取文本数据(JSON或者XML)到PipeWriter
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <returns>Has content or not</returns>
        public async Task<bool> ReadToStreamAsync(CommandDefinition command, IBufferWriter<byte> writer)
        {
            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryToStreamAsync(command, writer);
            }, command.CancellationToken);
        }

        /// <summary>
        /// Async read text data (JSON/XML) to PipeWriter
        /// 异步读取文本数据(JSON或者XML)到PipeWriter
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="format">Data format</param>
        /// <param name="collectionNames">Collection names</param>
        /// <returns>Has content or not</returns>
        public async Task<bool> ReadToStreamAsync(CommandDefinition command, IBufferWriter<byte> writer, DataFormat format, IEnumerable<string>? collectionNames = null)
        {
            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryToStreamAsync(command, writer, format, collectionNames);
            }, command.CancellationToken);
        }

        /// <summary>
        /// Async read JSON data to HTTP Response
        /// 异步读取JSON数据到HTTP响应
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <returns>Task</returns>
        public async Task ReadJsonToStreamAsync(CommandDefinition command, HttpResponse response, IEnumerable<string>? collectionNames = null)
        {
            // Content type
            response.JsonContentType();

            // Result
            bool result;

            // Write to
            if (collectionNames == null)
                result = await ReadToStreamAsync(command, response.BodyWriter);
            else
                result = await ReadToStreamAsync(command, response.BodyWriter, DataFormat.Json, collectionNames);

            if (!result) response.StatusCode = (int)HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Async read JSON data to HTTP Response and return the bytes
        /// 异步读取JSON数据到HTTP响应并返回写入的字节
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="response">HTTP Response</param>
        /// <param name="collectionNames">Collection names, null means single collection</param>
        /// <returns>Task</returns>
        public async Task<ReadOnlyMemory<byte>> ReadJsonToStreamWithReturnAsync(CommandDefinition command, HttpResponse response, IEnumerable<string>? collectionNames = null)
        {
            // Content type
            response.JsonContentType();

            // Result
            bool result;

            var writer = new ArrayBufferWriter<byte>();

            // Write to
            if (collectionNames == null)
                result = await ReadToStreamAsync(command, writer);
            else
                result = await ReadToStreamAsync(command, writer, DataFormat.Json, collectionNames);

            // Write bytes
            var bytes = writer.WrittenMemory;
            await response.BodyWriter.WriteAsync(bytes, command.CancellationToken);

            if (!result) response.StatusCode = (int)HttpStatusCode.NoContent;

            return bytes;
        }

        /// <summary>
        /// Quick read data
        /// 快速读取数据
        /// </summary>
        /// <typeparam name="E">Generic return type</typeparam>
        /// <param name="sql">SQL script</param>
        /// <param name="parameters">Parameter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<E> QuickReadAsync<E>(string sql, IDbParameters? parameters = null, CancellationToken cancellationToken = default)
        {
            var command = CreateCommand(sql, parameters, CommandType.Text, cancellationToken);

            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryFirstAsync<E>(command);
            }, cancellationToken);
        }

        /// <summary>
        /// Inline SQL command update
        /// 内联 SQL 命令更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <typeparam name="T">Generic id type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <param name="additionalPart">Additional SQL command part, please avoid injection</param>
        /// <param name="additionalParams">Additional parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async ValueTask<(IActionResult Result, UpdateResultData<T>? Data)> InlineUpdateAsync<T, M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null, CancellationToken cancellationToken = default)
            where T : struct
            where M : IdItem<T>, IUpdateModel
        {
            var (result, rows) = await InlineUpdateBaseAsync(model, configs, additionalPart, additionalParams, cancellationToken);

            if (result.Ok)
            {
                // Success
                return (result, new UpdateResultData<T> { Id = model.Id, RowsAffected = rows });
            }
            else
            {
                return (result, null);
            }
        }

        /// <summary>
        /// Inline SQL command update
        /// 内联 SQL 命令更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <param name="additionalPart">Additional SQL command part, please avoid injection</param>
        /// <param name="additionalParams">Additional parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async ValueTask<(IActionResult Result, UpdateResultData? Data)> InlineUpdateAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null, CancellationToken cancellationToken = default)
            where M : IdItem, IUpdateModel
        {
            var (result, rows) = await InlineUpdateBaseAsync(model, configs, additionalPart, additionalParams, cancellationToken);

            if (result.Ok)
            {
                // Success
                return (result, new UpdateResultData { Id = model.Id, RowsAffected = rows });
            }
            else
            {
                return (result, null);
            }
        }

        /// <summary>
        /// Inline SQL command update
        /// 内联 SQL 命令更新
        /// </summary>
        /// <typeparam name="M">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="configs">Configs</param>
        /// <param name="additionalPart">Additional SQL command part, please avoid injection</param>
        /// <param name="additionalParams">Additional parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        private async ValueTask<(IActionResult Result, int Rows)> InlineUpdateBaseAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null, CancellationToken cancellationToken = default)
            where M : IUpdateModel
        {
            // Validate
            if (model.ChangedFields == null || !model.ChangedFields.Any())
            {
                return (ApplicationErrors.NoValidData.AsResult(UpdateResultData.ChangedFields), 0);
            }

            if (!configs.UpdatableFields.Any())
            {
                return (ApplicationErrors.NoValidData.AsResult(UpdateResultData.UpdatableFields), 0);
            }

            if (!string.IsNullOrEmpty(configs.Conditions) && !DatabaseUtils.IsSafeSQLPart(configs.Conditions))
            {
                return (ApplicationErrors.NoValidData.AsResult(UpdateResultData.Conditions), 0);
            }

            // Update fields
            var updateFields = configs.UpdatableFields
                .Select(field =>
                {
                    string matchField, value;
                    var index = field.IndexOf('=');
                    if (index == -1)
                    {
                        matchField = field;

                        // Database side always 3 common cases: FieldName, field_name, or fieldName
                        // Code side, parameter name is always FieldName
                        value = $"@{field.ToPascalCase()}";
                    }
                    else
                    {
                        matchField = field[..index].Trim();
                        value = field[(index + 1)..].Trim();
                        if (MyRegex().IsMatch(value))
                        {
                            value = $"@{value}";
                        }
                    }

                    string? alias = null;
                    (matchField, alias) = DatabaseUtils.SplitField(matchField);
                    return (matchField, alias, value);
                })
                .Where(field => model.ChangedFields.Contains(field.matchField, StringComparer.OrdinalIgnoreCase)
                    || field.alias != null && model.ChangedFields.Contains(field.alias, StringComparer.OrdinalIgnoreCase))
                .Select(field => $"{App.DB.EscapeIdentifier(field.matchField)} = {field.value}");

            if (!updateFields.Any())
            {
                return (ApplicationErrors.NoValidData.AsResult(UpdateResultData.UpdateFields), 0);
            }

            // Default table name
            var tableName = configs.TableName ?? StringUtils.LinuxStyleToPascalCase(Flag).ToString();

            // SQL
            var fieldsSql = string.Join(", ", updateFields);
            if (!string.IsNullOrEmpty(additionalPart))
            {
                // Keep '='
                additionalPart = additionalPart.Replace('=', '`');
                if (DatabaseUtils.IsSafeSQLPart(additionalPart))
                {
                    additionalPart = additionalPart.Replace('`', '=');
                    fieldsSql += ", " + additionalPart;
                }
            }

            var sql = App.DB.GetUpdateCommand(tableName, "t", fieldsSql);
            sql.Append(" WHERE t.");
            sql.Append(App.DB.EscapeIdentifier(configs.IdField));
            sql.Append(" = @Id");

            if (!string.IsNullOrEmpty(configs.Conditions))
            {
                sql.Append(" AND ");
                sql.Append(configs.Conditions);
            }

            // Parameters
            var parameters = FormatParameters(model);

            // Additional parameters
            if (additionalParams != null)
            {
                foreach (var (Key, Value) in additionalParams)
                {
                    parameters.Add(Key, Value);
                }
            }

            // User data
            AddSystemParameters(parameters);

            var commandText = sql.ToString();
            var command = CreateCommand(commandText, parameters, CommandType.Text, cancellationToken);

            try
            {
                var records = await ExecuteAsync(command);

                // Log
                Logger.LogInformation("Successful update command: {commandText}", commandText);

                // Success
                return (ActionResult.Success, records);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Command update failed: {commandText}", commandText);
                throw new Exception("Command update failed", ex);
            }
        }

        /// <summary>
        /// Delete records with SQL asynchronously
        /// SQL语句异步删除记录
        /// </summary>
        /// <param name="data">Related data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<IActionResult> SqlDeleteAsync<T>(T data, CancellationToken cancellationToken = default) where T : ISqlDelete
        {
            var (sql, parameters) = data.CreateSqlDelete(App.DB);
            var command = CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
            var result = await App.DB.ExecuteAsync(command);
            return result == 0 ? ApplicationErrors.NoId.AsResult() : ActionResult.Success;
        }

        /// <summary>
        /// Delete records with SQL asynchronously
        /// SQL语句异步删除记录
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="tableName">Table name, default is the 'Flag'</param>
        /// <param name="idColumn">Id column</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async ValueTask<IActionResult> SqlDeleteAsync(IEnumerable<string> ids, string? tableName = null, string idColumn = "id", CancellationToken cancellationToken = default)
        {
            if (!ids.Any()) return ApplicationErrors.NoId.AsResult();

            var command = App.DB.CreateDeleteCommand(tableName ?? Flag, ids, idColumn, cancellationToken);
            var result = await App.DB.ExecuteAsync(command);
            return result == 0 ? ApplicationErrors.NoId.AsResult() : ActionResult.Success;
        }

        /// <summary>
        /// Delete records with SQL asynchronously
        /// SQL语句异步删除记录
        /// </summary>
        /// <typeparam name="T">Generic id type</typeparam>
        /// <param name="ids">Ids</param>
        /// <param name="tableName">Table name, default is the 'Flag'</param>
        /// <param name="idColumn">Id column</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async ValueTask<IActionResult> SqlDeleteAsync<T>(IEnumerable<T> ids, string? tableName = null, string idColumn = "id", CancellationToken cancellationToken = default) where T : struct
        {
            if (!ids.Any()) return ApplicationErrors.NoId.AsResult();

            var command = App.DB.CreateDeleteCommand(tableName ?? Flag, ids, idColumn, cancellationToken);
            var result = await App.DB.ExecuteAsync(command);
            return result == 0 ? ApplicationErrors.NoId.AsResult() : ActionResult.Success;
        }

        /// <summary>
        /// Insert records with SQL asynchronously
        /// SQL语句异步插入记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <typeparam name="I">Generic inserted data id type</typeparam>
        /// <param name="data">Related data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<I?> SqlInsertAsync<T, I>(T data, CancellationToken cancellationToken = default) where T : ISqlInsert
        {
            var (sql, parameters) = data.CreateSqlInsert(App.DB);
            var command = CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
            return await ExecuteScalarAsync<I>(command);
        }

        /// <summary>
        /// Select records with SQL asynchronously
        /// SQL语句异步选择记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <typeparam name="D">Generic selected data id type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<D[]> SqlSelectAsync<T, D>(T data, CancellationToken cancellationToken = default)
            where T : ISqlSelect
            where D : IDataReaderParser<D>
        {
            var (sql, parameters) = data.CreateSqlSelect(App.DB, D.ParserInnerFields);
            var command = CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
            return await QueryAsListAsync<D>(command);
        }

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="fields">Fields</param>
        /// <param name="response">HTTP response</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task SqlSelectJsonAsync<T>(T data, IEnumerable<string> fields, HttpResponse response, CancellationToken cancellationToken = default)
            where T : ISqlSelect
        {
            var (sql, parameters) = data.CreateSqlSelectJson(App.DB, fields);
            var command = CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
            await ReadJsonToStreamAsync(command, response);
        }

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <typeparam name="D">Geneirc fields type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="response">HTTP response</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task SqlSelectJsonAsync<T, D>(T data, HttpResponse response, CancellationToken cancellationToken = default)
            where T : ISqlSelect
            where D : IDataReaderParser<D>
        {
            await SqlSelectJsonAsync(data, D.ParserInnerFields, response, cancellationToken);
        }

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="fields">Fields</param>
        /// <param name="writer">Buffer writer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task SqlSelectJsonAsync<T>(T data, IEnumerable<string> fields, IBufferWriter<byte> writer, CancellationToken cancellationToken = default)
            where T : ISqlSelect
        {
            var (sql, parameters) = data.CreateSqlSelectJson(App.DB, fields);
            var command = CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
            await ReadToStreamAsync(command, writer);
        }

        /// <summary>
        /// Select records as JSON with SQL asynchronously
        /// SQL语句异步选择为JSON记录
        /// </summary>
        /// <typeparam name="T">Generic data type</typeparam>
        /// <typeparam name="D">Geneirc fields type</typeparam>
        /// <param name="data">Query data</param>
        /// <param name="writer">Buffer writer</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task SqlSelectJsonAsync<T, D>(T data, IBufferWriter<byte> writer, CancellationToken cancellationToken = default)
            where T : ISqlSelect
            where D : IDataReaderParser<D>
        {
            await SqlSelectJsonAsync(data, D.ParserInnerFields, writer, cancellationToken);
        }

        /// <summary>
        /// Update records with SQL asynchronously
        /// SQL语句异步更新记录
        /// </summary>
        /// <param name="data">Related data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<IActionResult> SqlUpdateAsync<T>(T data, CancellationToken cancellationToken = default) where T : ISqlUpdate
        {
            var (sql, parameters) = data.CreateSqlUpdate(App.DB);
            var command = CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
            var result = await App.DB.ExecuteAsync(command);
            return result == 0 ? ApplicationErrors.NoId.AsResult() : ActionResult.Success;
        }

        [GeneratedRegex("^[0-9a-zA-Z_]+$")]
        private static partial Regex MyRegex();
    }
}
