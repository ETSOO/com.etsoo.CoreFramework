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
    /// SQL Select generic return type command generator
    /// SQL 选择通用返回类型命令生成器
    /// </summary>
    [Generator]
    public class SqlSelectGenericGenerator : ISourceGenerator
    {
        private Dictionary<DatabaseName, List<(string, string, string)>> GenerateResultCode(INamedTypeSymbol smbol, DatabaseName database, NamingPolicy? namingPlicy, NamingPolicy? jsonNamingPolicy)
        {
            var codeLines = database.SetupJsonResults();

            var members = smbol.ParseMembers(true, new List<string>(), out _);

            var propertyType = typeof(SqlColumnAttribute);
            var ignoreName = nameof(SqlColumnAttribute.Ignore);
            var columnNameField = nameof(SqlColumnAttribute.ColumnName);

            var columnType = typeof(SqlSelectColumnAttribute);
            var prefixName = nameof(SqlSelectColumnAttribute.Prefix);
            var functionName = nameof(SqlSelectColumnAttribute.Function);
            var asNameName = nameof(SqlSelectColumnAttribute.AsName);
            var jsonNameName = nameof(SqlSelectColumnAttribute.JsonName);

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

                // Column attribute data
                var columnAttributeData = symbol.GetAttributeData(columnType.FullName);

                // Prefix
                var prefix = columnAttributeData?.GetValue<string?>(prefixName);

                // Function
                var function = columnAttributeData?.GetValue<string?>(functionName);

                // 'AS' name
                var asName = columnAttributeData?.GetValue<string?>(asNameName);

                // JSON name
                var jsonName = columnAttributeData?.GetValue<string?>(jsonNameName);

                // Field name
                var field = symbol.Name;

                if (typeSymbol.TypeKind == TypeKind.Array || typeSymbol.IsList())
                {
                    throw new NotSupportedException($"ETSG042 - Cannot support '{field}' of array type");
                }

                foreach (var item in codeLines)
                {
                    // Table column name
                    var columnName = attributeData?.GetValue<string?>(columnNameField) ?? field.ToCase(namingPlicy);
                    asName ??= columnName;

                    // JSON name
                    jsonName ??= asName.ToCase(jsonNamingPolicy);

                    // Name in the SQL command
                    string sqlName;

                    // 'function' may support multiple databases with format: SQLServer:**^SQLite:**
                    // Put the default one without database in the first position
                    if (!string.IsNullOrEmpty(function))
                    {
                        var parts = function!.Split('^');
                        var match = $"{item.Key}:";
                        var part = parts.FirstOrDefault(p => p.StartsWith(match))?.Substring(match.Length) ?? parts[0];
                        sqlName = part.Replace("{F}", columnName);
                    }
                    else
                    {
                        sqlName = columnName.DbEscape(item.Key);

                        // Add prefix
                        if (!string.IsNullOrEmpty(prefix))
                        {
                            sqlName = $"{prefix}.{sqlName}";
                        }
                    }

                    item.Value.Add((sqlName, asName, jsonName));
                }
            }

            // Sort by item 2 ('AS' name)
            foreach (var db in codeLines)
            {
                db.Value.Sort((a, b) => string.Compare(a.Item2, b.Item2));
            }

            return codeLines;
        }

        private (Dictionary<DatabaseName, List<string>> Condtions, IEnumerable<string> Parameters) GenerateConditions(GeneratorExecutionContext context, TypeDeclarationSyntax tds, List<string> externalInheritances, NamingPolicy? namingPlicy, DatabaseName database)
        {
            // Conditions
            var parameters = new List<string>();
            var conditions = database.SetupConditions();

            var members = context.ParseMembers(tds, true, externalInheritances, out _);
            if (!context.CancellationToken.IsCancellationRequested)
            {
                var propertyType = typeof(SqlColumnAttribute);
                var ignoreName = nameof(SqlColumnAttribute.Ignore);
                var columnNameField = nameof(SqlColumnAttribute.ColumnName);
                var keepNullName = nameof(SqlColumnAttribute.KeepNull);
                var querySignName = nameof(SqlColumnAttribute.QuerySign);

                var columnType = typeof(SqlSelectColumnAttribute);
                var prefixName = nameof(SqlSelectColumnAttribute.Prefix);

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

                    // Column attribute data
                    var columnAttributeData = symbol.GetAttributeData(columnType.FullName);

                    // Prefix
                    var prefix = columnAttributeData?.GetValue<string?>(prefixName);

                    // Field name
                    var field = symbol.Name;

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
                            context.ReportDiagnostic(Diagnostic.Create("ETSG043", "SourceGenerators", $"'{field}' is not a simple array", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                            break;
                        }
                    }

                    parameters.Add($@"parameters.Add(""{field}"", {value});");

                    foreach (var item in conditions)
                    {
                        // Table column name
                        var columnName = (attributeData?.GetValue<string?>(columnNameField) ?? field.ToCase(namingPlicy)).DbEscape(item.Key);

                        // Add prefix
                        if (!string.IsNullOrEmpty(prefix))
                        {
                            columnName = $"{prefix}.{columnName}";
                        }

                        if (nullable)
                        {
                            item.Value.Add($@"
                            if({field} != null)
                            {{
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
                            item.Value.Add($@"
                            conditions.Add(""{columnName} {sign} {cvalue}"");
                        ");
                        }
                    }
                }
            }

            return (conditions, parameters);
        }

        private void GenerateCode(GeneratorExecutionContext context, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = context.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (symbol == null || context.CancellationToken.IsCancellationRequested)
                return;

            // Attribute data
            var attributeData = symbol.GetAttributeData(attributeType.FullName);

            var tableName = attributeData?.GetValue<string?>(nameof(SqlSelectGenericCommandAttribute.TableName))!;
            if (string.IsNullOrEmpty(tableName))
            {
                context.ReportDiagnostic(Diagnostic.Create("ETSG041", "SourceGenerators", "Table name is required", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, true));
                return;
            }

            var database = (attributeData?.GetValue<DatabaseName>(nameof(SqlSelectGenericCommandAttribute.Database))).GetValueOrDefault();

            var namingPolicy = attributeData?.GetValue<NamingPolicy>(nameof(SqlSelectGenericCommandAttribute.NamingPolicy));

            var jsonNamingPolicy = attributeData?.GetValue<NamingPolicy>(nameof(SqlSelectGenericCommandAttribute.JsonNamingPolicy));

            // Results
            var results = symbol.GetAllAttributeData(typeof(SqlSelectResultAttribute).FullName);

            // Name space and class name
            var (ns, className) = (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);

            // Keyword
            var keyword = tds.Keyword.ToString();

            // Is Public
            var isPublic = tds.HasToken(SyntaxKind.PublicKeyword);

            // Inheritance
            var externals = new List<string>
            {
                "ISqlSelectGeneric"
            };

            // Table names
            var tableNames = database.GetAllNames()
                .Select(d => new KeyValuePair<DatabaseName, string>(d, tableName.DbEscape(d)))
                .ToDictionary(x => x.Key, x => x.Value);

            var tableNamesDeclaration = tableNames.Select(item => $"[DatabaseName.{item.Key}] = \"{item.Value}\"");

            // Conditions
            var (conditions, parameters) = GenerateConditions(context, tds, externals, namingPolicy, database);

            var body = conditions.Select((b, index) => @$"
                {(index > 0 ? "else " : "")}if(name == DatabaseName.{b.Key})
                {{
                    {string.Join("\n", b.Value)}
                }}
            ");

            var dataReaderName = typeof(AutoDataReaderGeneratorAttribute).FullName;

            var instances = new List<(string Name, string ClassName, string Interface)>();

            foreach (var result in results)
            {
                var type = result.GetValue<INamedTypeSymbol>("Type");
                if (type == null) continue;

                var typeAttributeData = type.GetAttributeData(dataReaderName);
                var fullName = type.GetFullName();
                var isInherit = typeAttributeData != null || type.InheritedFrom("com.etsoo.Database.IDataReaderParser<TSelf>");

                var propertyName = result.GetValue<string>("PropertyName") ?? type.Name;
                var propertyClassName = $"{propertyName}Result";
                var propertyInterface = $"ISqlSelectResult<{fullName}>";
                var isObject = result.GetValue<bool>("IsObject");

                instances.Add((propertyName, propertyClassName, propertyInterface));

                var resultCode = GenerateResultCode(type, database, namingPolicy, jsonNamingPolicy);

                var fields = resultCode.Select(item => $"[DatabaseName.{item.Key}] = \"{item.Value.ToSelectFields()}\"");
                var jsonFields = resultCode.Select(item => $"[DatabaseName.{item.Key}] = \"{item.Value.ToJsonSelectFields(item.Key, isObject)}\"");

                // Source code
                var resultSource = $@"#nullable enable
                using com.etsoo.Database;
                using System;
                using System.Data;
                using System.Text;

                namespace {ns}
                {{
                    {(isPublic ? "public" : "internal")} partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        private class {propertyClassName} : {propertyInterface}
                        {{
                            private readonly static Dictionary<DatabaseName, string> allFields = new()
                            {{
                                {string.Join(",\n", fields)}
                            }};

                            private readonly static Dictionary<DatabaseName, string> allJsonFields = new()
                            {{
                                {string.Join(",\n", jsonFields)}
                            }};

                            private readonly {className} obj;
                            public {propertyClassName}({className} obj)
                            {{
                                this.obj = obj;
                            }}

                            /// <summary>
                            /// Create SQL select command
                            /// 创建 SQL选择命令
                            /// </summary>
                            /// <param name=""db"">Database</param>
                            /// <returns>Sql command text and parameters</returns>
                            public (string, IDbParameters) CreateSqlSelect(IDatabase db)
                            {{
                                var (sql, parameters) = obj.CreateCommand(db, allFields[db.Name]);
                                return (sql.ToString(), parameters);
                            }}

                            /// <summary>
                            /// Do SQL select command
                            /// 执行 SQL选择命令
                            /// </summary>
                            /// <param name=""db"">Database</param>
                            /// <param name=""cancellationToken"">Cancellation token</param>
                            /// <returns>Result</returns>
                            public async Task<{fullName}[]> DoSqlSelectAsync(IDatabase db, CancellationToken cancellationToken = default)
                            {{
                                var (sql, parameters) = CreateSqlSelect(db);
                                var command = db.CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
                                return {(isInherit ? $"await db.QueryListAsync<{fullName}>(command)" : $"(await db.QueryAsync<{fullName}>(command)).ToArray()")};
                            }}

                            /// <summary>
                            /// Create SQL select as JSON command
                            /// 创建 SQL选择为JSON命令
                            /// </summary>
                            /// <param name=""db"">Database</param>
                            /// <returns>Sql command text and parameters</returns>
                            public (string, IDbParameters) CreateSqlSelectJson(IDatabase db)
                            {{
                                var (sql, parameters) = obj.CreateCommand(db, allFields[db.Name]);

                                // Sub query, otherwise Sqlite 'order by' fails
                                sql.Insert(0, $""SELECT {{allJsonFields[db.Name]}} FROM ("");
                                sql.Append("")"");

                                if (db.Name == DatabaseName.SQLServer)
                                {{
                                    // SQL Server sub query needs a name 't'
                                    sql.Append("" t FOR JSON PATH{(isObject ? ", WITHOUT_ARRAY_WRAPPER" : "")}"");
                                }}

                                return (sql.ToString(), parameters);
                            }}
                        }}
                    }}
                }}
                ";

                context.AddSource($"{ns}.{className}.SqlGenericSelect.{propertyName}.Generated.cs", SourceText.From(resultSource, Encoding.UTF8));
            }

            // Results interface
            var resultInterface = instances.Select(item => $"{item.Interface} {item.Name} {{ get; }}");
            var resultClass = instances.Select(item => $"public {item.Interface} {item.Name} => new {item.ClassName}(obj);");

            // Source code
            var source = $@"#nullable enable
                using com.etsoo.Database;
                using System;
                using System.Text;

                namespace {ns}
                {{
                    {(isPublic ? "public" : "internal")} partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        public interface IDefaultClass
                        {{
                            {string.Join("\n", resultInterface)}
                        }}

                        private class DefaultClass : IDefaultClass
                        {{
                            private readonly {className} obj;

                            public DefaultClass({className} obj)
                            {{
                                this.obj = obj;
                            }}

                            {string.Join("\n", resultClass)}
                        }}

                        private readonly static Dictionary<DatabaseName, string> tableNames = new()
                        {{
                            {string.Join(",\n", tableNamesDeclaration)}
                        }};

                        /// <summary>
                        /// All data results
                        /// 所有数据结果
                        /// </summary>
                        public IDefaultClass Default => new DefaultClass(this);

                        /// <summary>
                        /// Query paging data
                        /// 查询分页数据
                        /// </summary>
                        public QueryData? QueryPaging {{ get; set; }}

                        private (StringBuilder, IDbParameters) CreateCommand(IDatabase db, string fields)
                        {{
                            var parameters = new DbParameters();
                            var conditions = new List<string>();
                            var sql = new StringBuilder(""SELECT "");

                            var currentPage = QueryPaging?.CurrentPage ?? 0;
                            var pageSize = QueryPaging?.BatchSize ?? 0;

                            {string.Join("\n", parameters)}

                            var name = db.Name;
                            {string.Join("\n", body)}
                            else
                            {{
                                throw new NotSupportedException($""Database {{name}} is not supported"");
                            }}

                            sql.Append(fields);

                            sql.Append($"" FROM {{tableNames[db.Name]}}"");

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

                            if (pageSize > 0 && currentPage == 0 && db.Name == DatabaseName.SQLServer)
                            {{
                                sql.Insert(7, $""TOP {{pageSize}} "");
                            }}
                            else if (pageSize > 0 && (currentPage > 0 || db.Name != DatabaseName.SQLServer))
                            {{
                                sql.Append("" "");
                                sql.Append(db.QueryLimit(pageSize, currentPage));
                            }}

                            return (sql, parameters);
                        }}
                    }}
                }}
                ";

            context.AddSource($"{ns}.{className}.SqlGenericSelect.Generated.cs", SourceText.From(source, Encoding.UTF8));
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
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(SqlSelectGenericCommandAttribute)));
        }
    }
}
