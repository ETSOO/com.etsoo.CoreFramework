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
    [Generator]
    public class SqlUpdateGenerator : ISourceGenerator
    {
        private IEnumerable<string> GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, List<string> externalInheritances, string tableName, NamingPolicy? namingPlicy, DatabaseName database)
        {
            var body = new List<string>();
            var keys = new List<string>();

            // Avoid duplicate inheritances
            var list = externalInheritances.Count == 0 ? externalInheritances : new List<string>();

            var members = context.ParseMembers(tds, true, list, out _);
            if (!context.CancellationToken.IsCancellationRequested)
            {
                var propertyType = typeof(SqlColumnAttribute);
                var ignoreName = nameof(SqlColumnAttribute.Ignore);
                var columnNameField = nameof(SqlColumnAttribute.ColumnName);
                var keyName = nameof(SqlColumnAttribute.Key);
                var querySignName = nameof(SqlColumnAttribute.QuerySign);

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

        private void GenerateCode(GeneratorExecutionContext context, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = context.ParseSyntaxNode<INamedTypeSymbol>(tds);
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

            externals.Add("com.etsoo.CoreFramework.Models.ISqlUpdate");

            // Source code
            var source = $@"#nullable enable
                using com.etsoo.Database;
                using System;
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

                            return (sql.ToString(), parameters);
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.SqlUpdate.Generated.cs", SourceText.From(source, Encoding.UTF8));
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
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(SqlUpdateCommandAttribute)));
        }
    }
}
