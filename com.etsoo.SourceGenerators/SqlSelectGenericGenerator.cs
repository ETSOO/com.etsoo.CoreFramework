﻿using com.etsoo.SourceGenerators.Attributes;
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
    [Generator(LanguageNames.CSharp)]
    public class SqlSelectGenericGenerator : IIncrementalGenerator
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

            string? lastPrefix = null;

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
                if (!string.IsNullOrEmpty(prefix))
                {
                    lastPrefix = prefix;
                }

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
                        if (!string.IsNullOrEmpty(lastPrefix))
                        {
                            sqlName = $"{lastPrefix}.{sqlName}";
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

        private (Dictionary<DatabaseName, List<string>> Condtions, IEnumerable<string> Parameters) GenerateConditions(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, List<string> externalInheritances, NamingPolicy? namingPlicy, DatabaseName database, ref bool hasPagingData)
        {
            // Conditions
            var parameters = new List<string>();
            var conditions = database.SetupConditions();

            var members = context.ParseMembers(compilation, tds, true, externalInheritances, out _);
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

                    // Value code
                    var valueCode = attributeData?.GetValue<string?>(valueCodeField);
                    if (querySign == SqlQuerySign.Like) valueCode = "{LIKE}";
                    valueCode = valueCode.ToValueCode(field) ?? value;

                    parameters.Add($@"parameters.Add(""{field}"", {valueCode});");

                    foreach (var item in conditions)
                    {
                        // Table column name
                        var columnName = (attributeData?.GetValue<string?>(columnNameField) ?? field.ToCase(namingPlicy)).DbEscape(item.Key);

                        // Table column names
                        var columnNames = attributeData?.GetValues<string>(columnNamesField);

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
                                item.Value.Add($@"
                                if({field} != null)
                                {{
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
                                item.Value.Add($@"
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
            }

            return (conditions, parameters);
        }

        private void GenerateCode(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = compilation.ParseSyntaxNode<INamedTypeSymbol>(tds);
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
            var hasPagingData = false;
            var (conditions, parameters) = GenerateConditions(context, compilation, tds, externals, namingPolicy, database, ref hasPagingData);

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
                            /// <param name=""conditionDelegate"">Query condition delegate</param>
                            /// <returns>Sql command text and parameters</returns>
                            public (string, IDbParameters) CreateSqlSelect(IDatabase db, SqlConditionDelegate? conditionDelegate = null)
                            {{
                                var (sql, parameters) = obj.CreateCommand(db, allFields[db.Name], conditionDelegate);
                                return (sql.ToString(), parameters);
                            }}

                            /// <summary>
                            /// Do SQL select command
                            /// 执行 SQL选择命令
                            /// </summary>
                            /// <param name=""db"">Database</param>
                            /// <param name=""callback"">Callback before execution</param>
                            /// <param name=""conditionDelegate"">Query condition delegate</param>
                            /// <param name=""cancellationToken"">Cancellation token</param>
                            /// <returns>Result</returns>
                            public async Task<{fullName}[]> DoSqlSelectAsync(IDatabase db, SqlCommandDelegate? callback = null, SqlConditionDelegate? conditionDelegate = null, CancellationToken cancellationToken = default)
                            {{
                                var (sql, parameters) = CreateSqlSelect(db, conditionDelegate);
                                callback?.Invoke(sql, parameters);
                                var command = db.CreateCommand(sql, parameters, CommandType.Text, cancellationToken);
                                return {(isInherit ? $"await db.QueryListAsync<{fullName}>(command)" : $"(await db.QueryAsync<{fullName}>(command)).ToArray()")};
                            }}

                            /// <summary>
                            /// Create SQL select as JSON command
                            /// 创建 SQL选择为JSON命令
                            /// </summary>
                            /// <param name=""db"">Database</param>
                            /// <param name=""conditionDelegate"">Query condition delegate</param>
                            /// <returns>Sql command text and parameters</returns>
                            public (string, IDbParameters) CreateSqlSelectJson(IDatabase db, SqlConditionDelegate? conditionDelegate = null)
                            {{
                                var (sql, parameters) = obj.CreateCommand(db, allFields[db.Name], conditionDelegate);

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

                        {pagingDataPart}
                        private (StringBuilder, IDbParameters) CreateCommand(IDatabase db, string fields, SqlConditionDelegate? conditionDelegate)
                        {{
                            var parameters = new DbParameters();
                            var conditions = new List<string>();

                            // Keyset pagination
                            QueryPaging?.GetKeysetConditions(parameters, conditions);

                            var currentPage = QueryPaging?.CurrentPage ?? 0;
                            var pageSize = QueryPaging?.BatchSize ?? 0;

                            var sql = new StringBuilder(""SELECT "");

                            {string.Join("\n", parameters)}

                            var name = db.Name;
                            {string.Join("\n", body)}
                            else
                            {{
                                throw new NotSupportedException($""Database {{name}} is not supported"");
                            }}

                            sql.Append(fields);

                            sql.Append($"" FROM {{tableNames[db.Name]}}"");

                            // Condition delegate
                            conditionDelegate?.Invoke(conditions);

                            if (conditions.Count > 0)
                            {{
                                sql.Append("" WHERE "");
                                sql.Append(string.Join("" AND "", conditions));
                            }}

                            var orderBy = QueryPaging?.GetOrderCommand(db);
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

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            var attributeType = typeof(SqlSelectGenericCommandAttribute);
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
