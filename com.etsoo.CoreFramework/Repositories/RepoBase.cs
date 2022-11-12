using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Models;
using com.etsoo.CoreFramework.User;
using com.etsoo.Database;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.SpanMemory;
using com.etsoo.Utils.String;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Buffers;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Base repository
    /// 基础仓库
    /// </summary>
    /// <typeparam name="C">Generic database conneciton type</typeparam>
    public abstract partial class RepoBase<C> : IRepoBase
        where C : DbConnection
    {
        /// <summary>
        /// Current user
        /// 当前用户
        /// </summary>
        virtual protected IServiceUser? User { get; }

        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        virtual protected ICoreApplication<C> App { get; }

        /// <summary>
        /// Flag
        /// 标识
        /// </summary>
        public string Flag { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="flag">Flag</param>
        /// <param name="user">Current user</param>
        protected RepoBase(ICoreApplication<C> app, string flag, IServiceUser? user = null) => (App, Flag, User) = (app, flag, user);

        /// <summary>
        /// Create command, default parameters added
        /// 创建命令，附加默认参数
        /// </summary>
        /// <param name="name">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="type">Command type</param>
        /// <returns>Command</returns>
        protected CommandDefinition CreateCommand(string name, IDbParameters? parameters = null, CommandType? type = null)
        {
            type ??=  App.DB.SupportStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;

            // For stored procedure, remove null value parameters
            if (type == CommandType.StoredProcedure) parameters?.ClearNulls();

            return new CommandDefinition(name, parameters, commandType: type);
        }

        /// <summary>
        /// Add system parameters
        /// 添加系统参数
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public virtual void AddSystemParameters(IDbParameters parameters)
        {
            if (User == null)
            {
                // Make sure the repository initialized with valid user
                throw new UnauthorizedAccessException();
            }

            App.AddSystemParameters(User, parameters);
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
        public async Task<T> ExecuteScalarAsync<T>(CommandDefinition command)
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
            if (parameters is IModelParameters p)
            {
                return p.AsParameters(App);
            }

            return DatabaseUtils.FormatParameters(parameters) ?? new DbParameters();
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
                return GetCommandNameBase(parts[0].Split(new[] { ' ', '_' }, StringSplitOptions.RemoveEmptyEntries));
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
            var list = QueryAsListAsync<D>(command);
            return await list.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Async query command as object list
        /// 异步执行命令返回对象列表
        /// </summary>
        /// <typeparam name="D">Generic object type</typeparam>
        /// <param name="command">Command</param>
        /// <returns>Result</returns>
        public IAsyncEnumerable<D> QueryAsListAsync<D>(CommandDefinition command) where D : IDataReaderParser<D>
        {
            return App.DB.QuerySourceAsync<D>(command);
        }

        /// <summary>
        /// Async query command as action result
        /// 异步执行命令返回操作结果
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Action result</returns>
        public async ValueTask<ActionResult> QueryAsResultAsync(CommandDefinition command)
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
            });
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
            });
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
            response.ContentType = "application/json";

            // Write to
            if (collectionNames == null)
                await ReadToStreamAsync(command, response.BodyWriter);
            else
                await ReadToStreamAsync(command, response.BodyWriter, DataFormat.Json, collectionNames);
        }

        /// <summary>
        /// Quick read data
        /// 快速读取数据
        /// </summary>
        /// <typeparam name="E">Generic return type</typeparam>
        /// <returns>Result</returns>
        public async Task<E> QuickReadAsync<E>(string sql, IDbParameters? parameters = null)
        {
            var command = CreateCommand(sql, parameters, CommandType.Text);

            return await App.DB.WithConnection((connection) =>
            {
                return connection.QueryFirstAsync<E>(command);
            });
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
        /// <returns>Result</returns>
        public async ValueTask<(ActionResult Result, UpdateResultData<T>? Data)> InlineUpdateAsync<T, M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null)
            where T : struct
            where M : IdItem<T>, IUpdateModel
        {
            var (result, rows) = await InlineUpdateBaseAsync(model, configs, additionalPart, additionalParams);

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
        /// <returns>Result</returns>
        public async ValueTask<(ActionResult Result, UpdateResultData? Data)> InlineUpdateAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null)
            where M : IdItem, IUpdateModel
        {
            var (result, rows) = await InlineUpdateBaseAsync(model, configs, additionalPart, additionalParams);

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
        /// <returns>Result</returns>
        private async ValueTask<(ActionResult Result, int Rows)> InlineUpdateBaseAsync<M>(M model, QuickUpdateConfigs configs, string? additionalPart = null, Dictionary<string, object>? additionalParams = null)
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
                    var matchParts = matchField.Split(" AS ", StringSplitOptions.RemoveEmptyEntries);
                    string? alias = null;
                    if (matchParts.Length > 1)
                    {
                        matchField = matchParts[0].Trim();
                        alias = matchParts[1].Trim();
                    }
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
            configs.TableName ??= StringUtils.LinuxStyleToPascalCase(Flag).ToString();

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

            var sql = App.DB.GetUpdateCommand(configs.TableName, "t", fieldsSql);
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

            // When user authorized
            if (User != null)
                AddSystemParameters(parameters);

            var command = CreateCommand(sql.ToString(), parameters, CommandType.Text);

            try
            {
                var records = await ExecuteAsync(command);

                // Success
                return (ActionResult.Success, records);
            }
            catch (Exception ex)
            {
                throw new Exception("Command: " + sql, ex);
            }
        }

        [GeneratedRegex("^[0-9a-zA-Z_]+$")]
        private static partial Regex MyRegex();
    }
}
