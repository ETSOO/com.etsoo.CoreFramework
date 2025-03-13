using com.etsoo.SourceGenerators.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Auto DbDataReader initialization generator
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public class AutoDataReaderGenerator : IIncrementalGenerator
    {
        private (string, List<string>) GenerateBody(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, bool utcDateTime, List<string> externalInheritances, string ns)
        {
            var body = new List<string>();
            var fields = new List<string>();

            var members = context.ParseMembers(compilation, tds, false, externalInheritances, out bool isPositionalRecord);

            if (!context.CancellationToken.IsCancellationRequested)
            {
                var arrayPropertyType = typeof(ArrayPropertyAttribute);

                var columnType = typeof(SqlSelectColumnAttribute);
                var prefixName = nameof(SqlSelectColumnAttribute.Prefix);
                var functionName = nameof(SqlSelectColumnAttribute.Function);

                var propertyType = typeof(SqlColumnAttribute);
                var columnNameName = nameof(SqlColumnAttribute.ColumnName);
                var ignoreName = nameof(SqlColumnAttribute.Ignore);

                string? lastPrefix = null;

                foreach (var member in members)
                {
                    var (symbol, typeSymbol, nullable) = member;

                    // Ignore static field
                    if (symbol.IsStatic) continue;

                    // Property data
                    var propertyData = symbol.GetAttributeData(propertyType.FullName);

                    // Ignore it?
                    var ignore = propertyData?.GetValue<bool?>(ignoreName) ?? false;
                    if (ignore)
                        continue;

                    // Column attribute data
                    var columnAttributeData = symbol.GetAttributeData(columnType.FullName);

                    // Object field name
                    var fieldName = symbol.Name;

                    // Field type name
                    var typeName = typeSymbol.GetTypeName(nullable, ns);

                    // Attribute data
                    var attributeData = symbol.GetAttributeData(arrayPropertyType.FullName);

                    // Value part
                    string valuePart;

                    if (typeSymbol.IsSimpleType())
                    {
                        valuePart = $@"(await reader.GetValueAsync<{typeName}>(""{fieldName}"", names))";

                        if (utcDateTime && typeSymbol.Name == "DateTime")
                        {
                            valuePart = $"SharedUtils.SetUtcKind{valuePart}";
                        }
                        else if (typeSymbol.Name == "String" && !nullable)
                        {
                            valuePart += "!";
                        }
                    }
                    else if (typeSymbol.TypeKind == TypeKind.Enum)
                    {
                        // Enum item type
                        var enumSymbol = (INamedTypeSymbol)typeSymbol;
                        var enumType = enumSymbol.EnumUnderlyingType?.Name ?? "Byte";
                        valuePart = $@"({typeName})(await reader.GetValueAsync<{enumType}>(""{fieldName}"", names))";
                    }
                    else if (typeSymbol.TypeKind == TypeKind.Array)
                    {
                        // Array type
                        var arraySymbol = (IArrayTypeSymbol)typeSymbol;
                        var itemTypeSymbol = arraySymbol.ElementType;
                        if (!itemTypeSymbol.IsSimpleType())
                        {
                            continue;
                        }

                        // Splitter
                        var splitter = Extensions.CharToString(attributeData?.GetValue<char?>("Splitter") ?? ',');

                        var arrayType = itemTypeSymbol.Name;
                        if (arrayType.Equals("String"))
                            valuePart = $@"StringUtils.AsEnumerable(await reader.GetValueAsync<string>(""{fieldName}"", names), '{splitter}').ToArray()";
                        else
                            valuePart = $@"StringUtils.AsEnumerable<{arrayType}>(await reader.GetValueAsync<string>(""{fieldName}"", names), '{splitter}').ToArray()";
                    }
                    else if (typeSymbol.IsList())
                    {
                        // Item type
                        var listSymbol = (INamedTypeSymbol)typeSymbol;
                        var itemTypeSymbol = listSymbol.TypeArguments[0];
                        if (!itemTypeSymbol.IsSimpleType())
                        {
                            continue;
                        }

                        // Splitter
                        var splitter = Extensions.CharToString(attributeData?.GetValue<char?>("Splitter") ?? ',');

                        var listType = itemTypeSymbol.Name;
                        if (listType.Equals("String"))
                            valuePart = $@"StringUtils.AsEnumerable(await reader.GetValueAsync<string>(""{fieldName}"", names), '{splitter}').ToList()";
                        else
                            valuePart = $@"StringUtils.AsEnumerable<{listType}>(await reader.GetValueAsync<string>(""{fieldName}"", names), '{splitter}').ToList()";
                    }
                    else
                    {
                        continue;
                    }

                    var columnName = propertyData?.GetValue<string?>(columnNameName) ?? fieldName;

                    var prefix = columnAttributeData?.GetValue<string?>(prefixName);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        lastPrefix = prefix;
                    }

                    var function = columnAttributeData?.GetValue<string?>(functionName);

                    string oneField;
                    if (!string.IsNullOrEmpty(function))
                    {
                        oneField = $"{function} AS {columnName}";
                    }
                    else
                    {
                        var columnNameWithPrefix = string.IsNullOrEmpty(lastPrefix) ? columnName : $"{lastPrefix}.{columnName}";

                        if (columnName.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                        {
                            oneField = columnNameWithPrefix;
                        }
                        else
                        {
                            oneField = $"{columnNameWithPrefix} AS {fieldName}";
                        }
                    }

                    fields.Add(oneField);

                    if (isPositionalRecord)
                        body.Add($@"{fieldName}: {valuePart}");
                    else
                        body.Add($@"{fieldName} = {valuePart}");
                }
            }

            // Limitation: When inheritanted, please keep the same style
            // Define constructor for Positional Record
            if (isPositionalRecord)
                return ("(" + string.Join(",\n", body) + ")", fields);

            return ("{\n" + string.Join(",\n", body) + "\n}", fields);
        }

        private void GenerateCode(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = compilation.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (symbol == null || context.CancellationToken.IsCancellationRequested)
                return;

            // Attribute data
            var attributeData = symbol.GetAttributeData(attributeType.FullName);

            // Auto Utc datetime
            var utcDateTime = attributeData?.GetValue<bool>(nameof(AutoDataReaderGeneratorAttribute.UtcDateTime));

            // Name space and class name
            var (ns, className) = (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);

            // Keyword
            var keyword = tds.Keyword.ToString();

            // Is Public
            var isPublic = tds.HasToken(SyntaxKind.PublicKeyword);

            // Name
            var name = tds.Identifier.ToString();

            // Inheritance
            var externals = new List<string>();

            // Body
            var (body, fields) = GenerateBody(context, compilation, tds, utcDateTime.GetValueOrDefault(), externals, ns);
            if (context.CancellationToken.IsCancellationRequested)
                return;

            externals.Add($"com.etsoo.Database.IDataReaderParser<{name}>");

            // Source code
            var source = $@"#nullable enable
                using System;
                using System.Collections.Generic;
                using System.Data.Common;
                using System.Linq;
                using System.Runtime.CompilerServices;
                using System.Threading.Tasks;
                using com.etsoo.Database;
                using com.etsoo.Utils;
                using com.etsoo.Utils.String;

                namespace {ns}
                {{
                    {(isPublic ? "public" : "internal")} partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        /// <summary>
                        /// Parser inner fields
                        /// 解析器内部字段
                        /// </summary>
                        public static IEnumerable<string> ParserInnerFields => [ ""{string.Join("\", \"", fields)}"" ];

                        public static async IAsyncEnumerable<{name}> CreateAsync(DbDataReader reader, [EnumeratorCancellation] CancellationToken cancellationToken = default)
                        {{
                            // Column names
                            var names = reader.GetColumnNames().ToList();

                            while(await reader.ReadAsync(cancellationToken))
                            {{
                                yield return new {name} {body};
                            }}
                        }}

                        public static async Task<List<{name}>> CreateListAsync(DbDataReader reader, CancellationToken cancellationToken = default)
                        {{
                            // Column names
                            var names = reader.GetColumnNames().ToList();

                            var list = new List<{name}>();

                            while(await reader.ReadAsync(cancellationToken))
                            {{
                                list.Add(new {name} {body});
                            }}

                            return list;
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.DataReader.Generated.cs", SourceText.From(source, Encoding.UTF8));
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            var attributeType = typeof(AutoDataReaderGeneratorAttribute);
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
