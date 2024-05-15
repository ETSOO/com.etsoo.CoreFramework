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
    /// SQL Delete command generator
    /// SQL 删除命令生成器
    /// </summary>
    [Generator]
    public class SqlDeleteGenerator : ISourceGenerator
    {
        private IEnumerable<string> GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, List<string> externalInheritances, string tableName, NamingPolicy? namingPlicy, DatabaseName database)
        {
            var body = new List<string>();

            // Avoid duplicate inheritances
            var list = externalInheritances.Count == 0 ? externalInheritances : new List<string>();

            // Conditions
            var conditions = new List<string>();

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

                foreach (var member in members)
                {
                    var (symbol, typeSymbol, nullable) = member;

                    // Ignore static field
                    if (symbol.IsStatic) continue;

                    // Attribute data
                    var attributeData = symbol.GetAttributeData(propertyType.FullName);

                    // Ignore it?
                    var ignore = attributeData?.GetValue<bool?>(ignoreName) ?? false;
                    if (ignore)
                        continue;

                    // Field name
                    var field = symbol.Name;

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
                    var cvalueSource = $"@{field}";
                    var cvalue = cvalueSource;
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

                    body.Add($@"parameters.Add(""{field}"", {valueCode});");

                    if (columnNames?.Any() is true)
                    {
                        var columnNamesSql = string.Join(" OR ", columnNames.Select(c => $"{c.ToCase(namingPlicy).DbEscape(database)} {sign} {cvalue}"));
                        if (nullable)
                        {
                            if (keepNull)
                            {
                                conditions.Add($@"(({cvalueSource} IS NULL AND {columnName} IS NULL) OR {columnNamesSql})");
                            }
                            else
                            {
                                conditions.Add($@"({cvalueSource} IS NULL OR {columnNamesSql})");
                            }
                        }
                        else
                        {
                            conditions.Add($@"({columnNamesSql})");
                        }
                    }
                    else
                    {
                        if (nullable)
                        {
                            if (keepNull)
                            {
                                conditions.Add($@"(({cvalueSource} IS NULL AND {columnName} IS NULL) OR {columnName} {sign} {cvalue})");
                            }
                            else
                            {
                                conditions.Add($@"({cvalueSource} IS NULL OR {columnName} {sign} {cvalue})");
                            }
                        }
                        else
                        {
                            conditions.Add($@"{columnName} {sign} {cvalue}");
                        }
                    }
                }
            }

            if (conditions.Count == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create("ETSG003", "SourceGenerators", $"No conditions for delete", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                throw new Exception("No conditions for delete");
            }

            var sql = new StringBuilder($@"DELETE FROM {tableName.DbEscape(database)} WHERE {string.Join(" AND ", conditions)}");
            body.Add($@"sql = ""{sql}"";");

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

            var tableName = attributeData?.GetValue<string?>(nameof(SqlDeleteCommandAttribute.TableName))!;
            if (string.IsNullOrEmpty(tableName))
            {
                context.ReportDiagnostic(Diagnostic.Create("ETSG001", "SourceGenerators", "Table name is required", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                return;
            }

            var database = (attributeData?.GetValue<DatabaseName>(nameof(SqlDeleteCommandAttribute.Database))).GetValueOrDefault();

            var namingPolicy = attributeData?.GetValue<NamingPolicy>(nameof(SqlDeleteCommandAttribute.NamingPolicy));

            var debug = attributeData?.GetValue<bool>(nameof(SqlDeleteCommandAttribute.Debug)) ?? false;

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
            if (database.HasFlag(DatabaseName.SQLServer))
            {
                bodies.Add(DatabaseName.SQLServer, GenerateBody(context, tds, externals, tableName, namingPolicy, DatabaseName.SQLServer));
            }
            if (database.HasFlag(DatabaseName.MySQL))
            {
                bodies.Add(DatabaseName.MySQL, GenerateBody(context, tds, externals, tableName, namingPolicy, DatabaseName.MySQL));
            }
            if (database.HasFlag(DatabaseName.PostgreSQL))
            {
                bodies.Add(DatabaseName.PostgreSQL, GenerateBody(context, tds, externals, tableName, namingPolicy, DatabaseName.PostgreSQL));
            }
            if (database.HasFlag(DatabaseName.SQLite))
            {
                bodies.Add(DatabaseName.SQLite, GenerateBody(context, tds, externals, tableName, namingPolicy, DatabaseName.SQLite));
            }

            var body = bodies.Select((b, index) => @$"
                {(index > 0 ? "else " : "")}if(name == DatabaseName.{b.Key})
                {{
                    {string.Join("\n", b.Value)}
                }}
            ");

            externals.Add("ISqlDelete");

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
                        /// <summary>
                        /// Create SQL delete command
                        /// 创建SQL删除命令
                        /// </summary>
                        /// <param name=""db"">Database</param>
                        /// <returns>Sql command text and parameters</returns>
                        public (string, IDbParameters) CreateSqlDelete(IDatabase db)
                        {{
                            var parameters = new DbParameters();
                            string sql;

                            var name = db.Name;
                            {string.Join("\n", body)}
                            else
                            {{
                                throw new NotSupportedException($""Database {{name}} is not supported"");
                            }}

                            {(debug ? "System.Diagnostics.Debug.WriteLine(sql);" : "")}

                            return (sql, parameters);
                        }}

                        /// <summary>
                        /// Do SQL delete
                        /// 执行SQL删除
                        /// </summary>
                        /// <param name=""db"">Database</param>
                        /// <param name=""callback"">Callback before execution</param>
                        /// <param name=""cancellationToken"">Cancellation token</param>
                        /// <returns>Rows affected</returns>
                        public Task<int> DoSqlDeleteAsync(IDatabase db, SqlCommandDelegate? callback = null, CancellationToken cancellationToken = default)
                        {{
                            var (sql, parameters) = CreateSqlDelete(db);
                            callback?.Invoke(sql, parameters);
                            var command = db.CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
                            return db.ExecuteAsync(command);
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.SqlDelete.Generated.cs", SourceText.From(source, Encoding.UTF8));
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
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(SqlDeleteCommandAttribute)));
        }
    }
}
