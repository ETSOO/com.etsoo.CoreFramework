using com.etsoo.SourceGenerators.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// SQL Select command generator
    /// SQL 选择命令生成器
    /// </summary>
    [Generator]
    public class SqlSelectGenerator : ISourceGenerator
    {
        private IEnumerable<string> GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, List<string> externalInheritances, string tableName, NamingPolicy? namingPlicy, DatabaseName database, NamingPolicy? jsonNamingPolicy, ref bool hasPagingData)
        {
            var body = new List<string>();

            // Avoid duplicate inheritances
            var list = externalInheritances.Count == 0 ? externalInheritances : new List<string>();

            var members = context.ParseMembers(tds, true, list, out _);
            if (!context.CancellationToken.IsCancellationRequested)
            {
                var propertyType = typeof(SqlColumnAttribute);
                var ignoreName = nameof(SqlColumnAttribute.Ignore);
                var columnNameField = nameof(SqlColumnAttribute.ColumnName);
                var columnNamesField = nameof(SqlColumnAttribute.ColumnNames);
                var keepNullName = nameof(SqlColumnAttribute.KeepNull);
                var querySignName = nameof(SqlColumnAttribute.QuerySign);
                var valueCodeField = nameof(SqlColumnAttribute.ValueCode);

                var columnType = typeof(SqlSelectColumnAttribute);
                var prefixName = nameof(SqlSelectColumnAttribute.Prefix);

                string? lastPrefix = null;

                foreach (var member in members)
                {
                    var (symbol, typeSymbol, nullable) = member;

                    // Ignore static field
                    if (symbol.IsStatic) continue;

                    // Field name
                    var field = symbol.Name;

                    // Paging data
                    if (field.Equals("QueryPaging", StringComparison.OrdinalIgnoreCase))
                    {
                        hasPagingData = true;
                        continue;
                    }

                    // Attribute data
                    var attributeData = symbol.GetAttributeData(propertyType.FullName);

                    // Ignore it?
                    var ignore = attributeData?.GetValue<bool?>(ignoreName) ?? false;
                    if (ignore)
                        continue;

                    // Column attribute data
                    var columnAttributeData = symbol.GetAttributeData(columnType.FullName);

                    // Prefix
                    var prefix = columnAttributeData?.GetValue<string?>(prefixName);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        lastPrefix = prefix;
                    }

                    // Table column name
                    var columnName = (attributeData?.GetValue<string?>(columnNameField) ?? field.ToCase(namingPlicy)).DbEscape(database);

                    // Table column names
                    var columnNames = attributeData?.GetValues<string>(columnNamesField);

                    // Keep null
                    var keepNull = attributeData?.GetValue<bool?>(keepNullName) ?? false;

                    // Query sign
                    var querySign = attributeData?.GetValue<SqlQuerySign>(querySignName) ?? SqlQuerySign.Equal;
                    var sign = querySign.ToQuerySign();

                    var value = field;
                    var cvalue = $"@{field}";
                    if (typeSymbol.TypeKind == TypeKind.Array || typeSymbol.IsList())
                    {
                        // Type
                        var itemTypeSymbol = typeSymbol.TypeKind == TypeKind.Array ? ((IArrayTypeSymbol)typeSymbol).ElementType : ((INamedTypeSymbol)typeSymbol).TypeArguments[0];
                        if (itemTypeSymbol.IsSimpleType(true))
                        {
                            // Ignore the value
                            value = "null";

                            // Sign
                            sign = querySign == SqlQuerySign.NotEqual ? "NOT IN" : "IN";

                            if (itemTypeSymbol.ToString().IsNumericType())
                            {
                                cvalue = $"DatabaseUtils.ToInClause({field})";
                            }
                            else
                            {
                                cvalue = $"DatabaseUtils.ToStringInClause({field})";
                            }

                            cvalue = $"\" + {cvalue} + \"";
                        }
                        else
                        {
                            context.ReportDiagnostic(Diagnostic.Create("ETSG002", "SourceGenerators", $"'{field}' is not a simple array", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                            break;
                        }
                    }

                    // Value code
                    var valueCode = attributeData?.GetValue<string?>(valueCodeField);
                    if (querySign == SqlQuerySign.Like) valueCode = "{LIKE}";
                    valueCode = valueCode.ToValueCode(field) ?? value;

                    if (columnNames?.Any() is true)
                    {
                        var columnNamesSql = string.Join(" OR ", columnNames.Select(c =>
                        {
                            c = c.ToCase(namingPlicy).DbEscape(database);

                            // Add prefix
                            if (!string.IsNullOrEmpty(lastPrefix))
                            {
                                c = $"{lastPrefix}.{c}";
                            }

                            return $"{c} {sign} {cvalue}";
                        }));

                        if (nullable)
                        {
                            body.Add($@"
                            if({field} != null)
                            {{
                                parameters.Add(""{field}"", {valueCode});
                                conditions.Add(""({columnNamesSql})"");
                            }}
                        {(keepNull ? $@"
                            else
                            {{
                                conditions.Add(""{columnName} IS NULL"");
                            }}
                        " : "")}
                        ");
                        }
                        else
                        {
                            body.Add($@"
                            parameters.Add(""{field}"", {valueCode});
                            conditions.Add(""({columnNamesSql})"");
                        ");
                        }
                    }
                    else
                    {
                        // Add prefix
                        if (!string.IsNullOrEmpty(lastPrefix))
                        {
                            columnName = $"{lastPrefix}.{columnName}";
                        }

                        if (nullable)
                        {
                            body.Add($@"
                            if({field} != null)
                            {{
                                parameters.Add(""{field}"", {valueCode});
                                conditions.Add(""{columnName} {sign} {cvalue}"");
                            }}
                        {(keepNull ? $@"
                            else
                            {{
                                conditions.Add(""{columnName} IS NULL"");
                            }}
                        " : "")}
                        ");
                        }
                        else
                        {
                            body.Add($@"
                            parameters.Add(""{field}"", {valueCode});
                            conditions.Add(""{columnName} {sign} {cvalue}"");
                        ");
                        }
                    }
                }
            }

            var topPart = database == DatabaseName.SQLServer ? $@"
                if (currentPage == 0 && pageSize > 0)
                {{ 
                    sql.Append(""TOP "");
                    sql.Append(pageSize);
                    sql.Append("" "");
                }}
            " : "";

            body.Add($@"
                sql.Append(""SELECT "");{topPart}
                sql.Append(db.JoinJsonFields(fields, mappings, {(namingPlicy.HasValue ? $"NamingPolicy.{namingPlicy}" : "null")}, {(jsonNamingPolicy.HasValue ? $"NamingPolicy.{jsonNamingPolicy}" : "null")}));
                sql.Append("" FROM {tableName.DbEscape(database)}"");
            ");

            return body;
        }

        private void GenerateCode(GeneratorExecutionContext context, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = context.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (symbol == null || context.CancellationToken.IsCancellationRequested)
                return;

            // Attribute data
            var attributeData = symbol.GetAttributeData(attributeType.FullName);

            var tableName = attributeData?.GetValue<string?>(nameof(SqlSelectCommandAttribute.TableName))!;
            if (string.IsNullOrEmpty(tableName))
            {
                context.ReportDiagnostic(Diagnostic.Create("ETSG031", "SourceGenerators", "Table name is required", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                return;
            }

            var database = (attributeData?.GetValue<DatabaseName>(nameof(SqlSelectCommandAttribute.Database))).GetValueOrDefault();

            var namingPolicy = attributeData?.GetValue<NamingPolicy>(nameof(SqlSelectCommandAttribute.NamingPolicy));

            var jsonNamingPolicy = attributeData?.GetValue<NamingPolicy>(nameof(SqlSelectCommandAttribute.JsonNamingPolicy));

            var isObject = attributeData?.GetValue<bool>(nameof(SqlSelectCommandAttribute.IsObject)) ?? false;

            // Name space and class name
            var (ns, className) = (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);

            // Keyword
            var keyword = tds.Keyword.ToString();

            // Is Public
            var isPublic = tds.HasToken(SyntaxKind.PublicKeyword);

            // Inheritance
            var externals = new List<string>();

            // Generate body
            var bodies = new Dictionary<DatabaseName, IEnumerable<string>>();
            var hasPagingData = false;
            if (database.HasFlag(DatabaseName.SQLServer))
            {
                bodies.Add(DatabaseName.SQLServer, GenerateBody(context, tds, externals, tableName, namingPolicy, DatabaseName.SQLServer, jsonNamingPolicy, ref hasPagingData));
            }
            if (database.HasFlag(DatabaseName.MySQL))
            {
                bodies.Add(DatabaseName.MySQL, GenerateBody(context, tds, externals, tableName, namingPolicy, DatabaseName.MySQL, jsonNamingPolicy, ref hasPagingData));
            }
            if (database.HasFlag(DatabaseName.PostgreSQL))
            {
                bodies.Add(DatabaseName.PostgreSQL, GenerateBody(context, tds, externals, tableName, namingPolicy, DatabaseName.PostgreSQL, jsonNamingPolicy, ref hasPagingData));
            }
            if (database.HasFlag(DatabaseName.SQLite))
            {
                bodies.Add(DatabaseName.SQLite, GenerateBody(context, tds, externals, tableName, namingPolicy, DatabaseName.SQLite, jsonNamingPolicy, ref hasPagingData));
            }

            var body = bodies.Select((b, index) => @$"
                {(index > 0 ? "else " : "")}if(name == DatabaseName.{b.Key})
                {{
                    {string.Join("\n", b.Value)}
                }}
            ");

            externals.Add("ISqlSelect");

            var pagingDataPart = hasPagingData ? "" : $@"
                        /// <summary>
                        /// Query paging data
                        /// 查询分页数据
                        /// </summary>
                        public QueryPagingData? QueryPaging {{ get; set; }}

            ";

            // Source code
            var source = $@"#nullable enable
                using com.etsoo.Database;
                using System;
                using System.Data;
                using System.Text;

                namespace {ns}
                {{
                    {(isPublic ? "public" : "internal")} partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        {pagingDataPart}
                        private (StringBuilder, IDbParameters, Dictionary<string, string>) CreateCommand(IDatabase db, IEnumerable<string> fields, SqlConditionDelegate? conditionDelegate)
                        {{
                            var parameters = new DbParameters();
                            var conditions = new List<string>();
                            var sql = new StringBuilder();

                            var currentPage = QueryPaging?.CurrentPage ?? 0;
                            var pageSize = QueryPaging?.BatchSize ?? 0;

                            var mappings = new Dictionary<string, string>();

                            var name = db.Name;
                            {string.Join("\n", body)}
                            else
                            {{
                                throw new NotSupportedException($""Database {{name}} is not supported"");
                            }}

                            // Condition delegate
                            conditionDelegate?.Invoke(conditions);

                            if (conditions.Count > 0)
                            {{
                                sql.Append("" WHERE "");
                                sql.Append(string.Join("" AND "", conditions));
                            }}

                            var orderBy = QueryPaging?.GetOrderCommand();
                            if (!String.IsNullOrEmpty(orderBy))
                            {{ 
                                sql.Append("" "");
                                sql.Append(orderBy);
                            }}

                            if (pageSize > 0 && (currentPage > 0 || db.Name != DatabaseName.SQLServer))
                            {{
                                sql.Append("" "");
                                sql.Append(db.QueryLimit(pageSize, currentPage));
                            }}

                            return (sql, parameters, mappings);
                        }}

                        /// <summary>
                        /// Create SQL select command
                        /// 创建SQL选择命令
                        /// </summary>
                        /// <param name=""db"">Database</param>
                        /// <param name=""fields"">Fields to select</param>
                        /// <param name=""conditionDelegate"">Query condition delegate</param>
                        /// <returns>Sql command text and parameters</returns>
                        public (string, IDbParameters) CreateSqlSelect(IDatabase db, IEnumerable<string> fields, SqlConditionDelegate? conditionDelegate = null)
                        {{
                            var (sql, parameters, _) = CreateCommand(db, fields, conditionDelegate);
                            return (sql.ToString(), parameters);
                        }}

                        /// <summary>
                        /// Create SQL select as JSON command
                        /// 创建SQL选择为JSON命令
                        /// </summary>
                        /// <param name=""db"">Database</param>
                        /// <param name=""fields"">Fields to select</param>
                        /// <param name=""mappingDelegate"">Query fields mapping delegate</param>
                        /// <param name=""conditionDelegate"">Query condition delegate</param>
                        /// <returns>Sql command text and parameters</returns>
                        public (string, IDbParameters) CreateSqlSelectJson(IDatabase db, IEnumerable<string> fields, SqlMappingDelegate? mappingDelegate = null, SqlConditionDelegate? conditionDelegate = null)
                        {{
                            var (sql, parameters, mappings) = CreateCommand(db, fields, conditionDelegate);

                            // Mapping
                            mappingDelegate?.Invoke(mappings);

                            // Sub query, otherwise Sqlite 'order by' fails
                            sql.Insert(0, $""SELECT {{db.JoinJsonFields(mappings, {isObject.ToCode()})}} FROM ("");
                            sql.Append("")"");

                            if (db.Name == DatabaseName.SQLServer)
                            {{
                                // SQL Server sub query needs a name 't'
                                sql.Append("" t FOR JSON PATH{(isObject ? ", WITHOUT_ARRAY_WRAPPER" : "")}"");
                            }}

                            return (sql.ToString(), parameters);
                        }}

                        /// <summary>
                        /// Do SQL select
                        /// 执行SQL选择
                        /// </summary>
                        /// <typeparam name=""D"">Generic selected data type</typeparam>
                        /// <param name=""db"">Database</param>
                        /// <param name=""callback"">Callback before execution</param>
                        /// <param name=""conditionDelegate"">Query condition delegate</param>
                        /// <param name=""cancellationToken"">Cancellation token</param>
                        /// <returns>Result</returns>
                        public Task<D[]> DoSqlSelectAsync<D>(IDatabase db, SqlCommandDelegate? callback = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default) where D : IDataReaderParser<D>
                        {{
                            var (sql, parameters) = CreateSqlSelect(db, D.ParserInnerFields, conditionDelegate);
                            callback?.Invoke(sql, parameters);
                            var command = db.CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
                            return db.QueryListAsync<D>(command);
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.SqlSelect.Generated.cs", SourceText.From(source, Encoding.UTF8));
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // The generator infrastructure will create a receiver and populate it
            // We can retrieve the populated instance via the context
            if (context.SyntaxReceiver is not SyntaxReceiver syntaxReceiver)
            {
                return;
            }

            // Records
            foreach (var rds in syntaxReceiver.RecordCandidates)
            {
                GenerateCode(context, rds, syntaxReceiver.AttributeType);
            }

            // Structs
            foreach (var sds in syntaxReceiver.StructCandidates)
            {
                GenerateCode(context, sds, syntaxReceiver.AttributeType);
            }

            // Classes
            foreach (var cds in syntaxReceiver.ClassCandidates)
            {
                GenerateCode(context, cds, syntaxReceiver.AttributeType);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(SqlSelectCommandAttribute)));
        }
    }
}
