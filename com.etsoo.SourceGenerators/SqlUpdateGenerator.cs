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
    /// SQL Update command generator
    /// SQL 更新命令生成器
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public class SqlUpdateGenerator : IIncrementalGenerator
    {
        private IEnumerable<string> GenerateBody(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, List<string> externalInheritances, string tableName, NamingPolicy? namingPlicy, DatabaseName database)
        {
            var body = new List<string>();
            var keys = new List<string>();

            // Avoid duplicate inheritances
            var list = externalInheritances.Count == 0 ? externalInheritances : new List<string>();

            var members = context.ParseMembers(compilation, tds, true, list, out _);
            if (!context.CancellationToken.IsCancellationRequested)
            {
                var propertyType = typeof(SqlColumnAttribute);
                var ignoreName = nameof(SqlColumnAttribute.Ignore);
                var columnNameField = nameof(SqlColumnAttribute.ColumnName);
                var keyName = nameof(SqlColumnAttribute.Key);
                var querySignName = nameof(SqlColumnAttribute.QuerySign);

                foreach (var member in members)
                {
                    var (symbol, typeSymbol, _) = member;

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

                    if (typeSymbol.TypeKind == TypeKind.Array || typeSymbol.IsList())
                    {
                        context.ReportDiagnostic(Diagnostic.Create("ETSG022", "SourceGenerators", $"Cannot support '{field}' of array type", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                        break;
                    }

                    // Key
                    var key = attributeData?.GetValue<bool?>(keyName) ?? false;

                    if (key)
                    {
                        // Query sign
                        var querySign = attributeData?.GetValue<SqlQuerySign>(querySignName) ?? SqlQuerySign.Equal;
                        var sign = querySign.ToQuerySign();

                        keys.Add($"{columnName} {sign} @{field}");
                    }
                    else
                    {
                        body.Add($@"
                            if (ChangedFields?.Contains(""{field}"", StringComparer.OrdinalIgnoreCase) is true)
                            {{
                                values.Add(""{columnName} = @{field}"");
                            }}
                        ");
                    }

                    body.Add($@"
                        parameters.Add(""{field}"", {field});
                    ");
                }
            }

            if (keys.Count == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create("ETSG023", "SourceGenerators", $"No keys (WHERE clause) for the update", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
            }

            body.Add($@"
                sql.Append(""Update {tableName.DbEscape(database)} SET {{0}} WHERE {string.Join(" AND ", keys)}"");
            ");

            return body;
        }

        private void GenerateCode(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = compilation.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (symbol == null || context.CancellationToken.IsCancellationRequested)
                return;

            // Attribute data
            var attributeData = symbol.GetAttributeData(attributeType.FullName);

            var tableName = attributeData?.GetValue<string?>(nameof(SqlUpdateCommandAttribute.TableName))!;
            if (string.IsNullOrEmpty(tableName))
            {
                context.ReportDiagnostic(Diagnostic.Create("ETSG021", "SourceGenerators", "Table name is required", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                return;
            }

            var database = (attributeData?.GetValue<DatabaseName>(nameof(SqlUpdateCommandAttribute.Database))).GetValueOrDefault();

            var namingPolicy = attributeData?.GetValue<NamingPolicy>(nameof(SqlUpdateCommandAttribute.NamingPolicy));

            var debug = attributeData?.GetValue<bool>(nameof(SqlUpdateCommandAttribute.Debug)) ?? false;

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
                bodies.Add(DatabaseName.SQLServer, GenerateBody(context, compilation, tds, externals, tableName, namingPolicy, DatabaseName.SQLServer));
            }
            if (database.HasFlag(DatabaseName.PostgreSQL))
            {
                bodies.Add(DatabaseName.PostgreSQL, GenerateBody(context, compilation, tds, externals, tableName, namingPolicy, DatabaseName.PostgreSQL));
            }
            if (database.HasFlag(DatabaseName.SQLite))
            {
                bodies.Add(DatabaseName.SQLite, GenerateBody(context, compilation, tds, externals, tableName, namingPolicy, DatabaseName.SQLite));
            }

            var body = bodies.Select((b, index) => @$"
                {(index > 0 ? "else " : "")}if(name == DatabaseName.{b.Key})
                {{
                    {string.Join("\n", b.Value)}
                }}
            ");

            externals.Add("ISqlUpdate");

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
                        /// Changed fields
                        /// 改变的字段
                        /// </summary>
                        public IEnumerable<string>? ChangedFields {{ get; set; }}

                        /// <summary>
                        /// Create SQL update command
                        /// 创建SQL更新命令
                        /// </summary>
                        /// <param name=""db"">Database</param>
                        /// <returns>Sql command text and parameters</returns>
                        public (string, IDbParameters) CreateSqlUpdate(IDatabase db)
                        {{
                            var parameters = new DbParameters();
                            var sql = new StringBuilder();
                            var values = new List<string>();

                            var name = db.Name;
                            {string.Join("\n", body)}
                            else
                            {{
                                throw new NotSupportedException($""Database {{name}} is not supported"");
                            }}

                            sql = sql.Replace(""{{0}}"", string.Join("", "", values));

                            {(debug ? "System.Diagnostics.Debug.WriteLine(sql);" : "")}

                            return (sql.ToString(), parameters);
                        }}

                        /// <summary>
                        /// Do SQL update
                        /// 执行SQL更新
                        /// </summary>
                        /// <param name=""db"">Database</param>
                        /// <param name=""callback"">Callback before execution</param>
                        /// <param name=""cancellationToken"">Cancellation token</param>
                        /// <returns>Rows affected</returns>
                        public Task<int> DoSqlUpdateAsync(IDatabase db, SqlCommandDelegate? callback = null, CancellationToken cancellationToken = default)
                        {{
                            var (sql, parameters) = CreateSqlUpdate(db);
                            callback?.Invoke(sql, parameters);
                            var command = db.CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
                            return db.ExecuteAsync(command);
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.SqlUpdate.Generated.cs", SourceText.From(source, Encoding.UTF8));
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            var attributeType = typeof(SqlUpdateCommandAttribute);
            var provider = context.CreateGeneratorProvider(attributeType);
            context.RegisterSourceOutput(provider, (context, source) =>
            {
                var (compilation, syntaxNodes) = source;
                foreach (var syntaxNode in syntaxNodes)
                {
                    if (syntaxNode == null) continue;
                    GenerateCode(context, compilation, syntaxNode, attributeType);
                }
            });
        }
    }
}
