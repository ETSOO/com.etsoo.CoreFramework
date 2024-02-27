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
    /// SQL Insert command generator
    /// SQL 插入命令生成器
    /// </summary>
    [Generator]
    public class SqlInsertGenerator : ISourceGenerator
    {
        private IEnumerable<string> GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, List<string> externalInheritances, string tableName, string primaryKey, NamingPolicy? namingPlicy, DatabaseName database)
        {
            var body = new List<string>();
            var columns = new List<string>();
            var values = new List<string>();

            // Avoid duplicate inheritances
            var list = externalInheritances.Count == 0 ? externalInheritances : new List<string>();

            var pKey = primaryKey.ToCase(namingPlicy).DbEscape(database);
            string? idField = null;

            var members = context.ParseMembers(tds, true, list, out _);
            if (!context.CancellationToken.IsCancellationRequested)
            {
                var propertyType = typeof(SqlColumnAttribute);
                var ignoreName = nameof(SqlColumnAttribute.Ignore);
                var columnNameField = nameof(SqlColumnAttribute.ColumnName);
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

                    if (typeSymbol.TypeKind == TypeKind.Array || typeSymbol.IsList())
                    {
                        context.ReportDiagnostic(Diagnostic.Create("ETSG012", "SourceGenerators", $"Cannot support '{field}' of array type", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                        break;
                    }

                    // Value code
                    var valueCode = attributeData?.GetValue<string?>(valueCodeField);

                    if (columnName.Equals(pKey, StringComparison.OrdinalIgnoreCase))
                    {
                        idField = columnName;
                    }

                    columns.Add(columnName);
                    values.Add($"@{field}");
                    body.Add($@"parameters.Add(""{field}"", {valueCode ?? field});");
                }
            }

            var sql = new StringBuilder($@"INSERT INTO {tableName.DbEscape(database)} ({string.Join(", ", columns)})");
            if (database == DatabaseName.SQLServer)
            {
                sql.Append($@" OUTPUT inserted.{pKey} VALUES ({string.Join(", ", values)})");
            }
            else if (database == DatabaseName.MySQL)
            {
                sql.Append($@" VALUES ({string.Join(", ", values)}); {(idField == null ? "; SELECT LAST_INSERT_ID()" : $"SELECT {primaryKey} AS {pKey}")}");
            }
            else if (database == DatabaseName.PostgreSQL || database == DatabaseName.SQLite)
            {
                sql.Append($@" VALUES ({string.Join(", ", values)}) RETURNING {pKey}");
            }

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

            var tableName = attributeData?.GetValue<string?>(nameof(SqlInsertCommandAttribute.TableName))!;
            if (string.IsNullOrEmpty(tableName))
            {
                context.ReportDiagnostic(Diagnostic.Create("ETSG011", "SourceGenerators", "Table name is required", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                return;
            }

            var primaryKey = attributeData?.GetValue<string?>(nameof(SqlInsertCommandAttribute.PrimaryKey)) ?? "Id";

            var database = (attributeData?.GetValue<DatabaseName>(nameof(SqlInsertCommandAttribute.Database))).GetValueOrDefault();

            var namingPolicy = attributeData?.GetValue<NamingPolicy>(nameof(SqlInsertCommandAttribute.NamingPolicy));

            var debug = attributeData?.GetValue<bool>(nameof(SqlInsertCommandAttribute.Debug)) ?? false;

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
                bodies.Add(DatabaseName.SQLServer, GenerateBody(context, tds, externals, tableName, primaryKey, namingPolicy, DatabaseName.SQLServer));
            }
            if (database.HasFlag(DatabaseName.MySQL))
            {
                bodies.Add(DatabaseName.MySQL, GenerateBody(context, tds, externals, tableName, primaryKey, namingPolicy, DatabaseName.MySQL));
            }
            if (database.HasFlag(DatabaseName.PostgreSQL))
            {
                bodies.Add(DatabaseName.PostgreSQL, GenerateBody(context, tds, externals, tableName, primaryKey, namingPolicy, DatabaseName.PostgreSQL));
            }
            if (database.HasFlag(DatabaseName.SQLite))
            {
                bodies.Add(DatabaseName.SQLite, GenerateBody(context, tds, externals, tableName, primaryKey, namingPolicy, DatabaseName.SQLite));
            }

            var body = bodies.Select((b, index) => @$"
                {(index > 0 ? "else " : "")}if(name == DatabaseName.{b.Key})
                {{
                    {string.Join("\n", b.Value)}
                }}
            ");

            externals.Add("com.etsoo.CoreFramework.Models.ISqlInsert");

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
                        /// Create SQL insert command
                        /// 创建SQL插入命令
                        /// </summary>
                        /// <param name=""db"">Database</param>
                        /// <returns>Sql command text and parameters</returns>
                        public (string, IDbParameters) CreateSqlInsert(IDatabase db)
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
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.SqlInsert.Generated.cs", SourceText.From(source, Encoding.UTF8));
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
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(SqlInsertCommandAttribute)));
        }
    }
}
