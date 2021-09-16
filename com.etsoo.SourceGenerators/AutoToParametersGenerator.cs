using com.etsoo.SourceGenerators.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Auto AsParameters generator
    /// </summary>
    [Generator]
    public class AutoToParametersGenerator : ISourceGenerator
    {
        private IEnumerable<string> GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, bool ignoreNull, bool snakeCase, List<string> externalInheritances)
        {
            var body = new List<string>();

            var members = context.ParseMembers(tds, externalInheritances, out _);

            if(!context.CancellationToken.IsCancellationRequested)
            {
                var propertyType = typeof(PropertyAttribute);
                var arrayPropertyType = typeof(ArrayPropertyAttribute);
                var ignoreName = nameof(PropertyAttribute.Ignore);
                var isAnsiName = nameof(PropertyAttribute.IsAnsi);
                var fixedLengthName = nameof(PropertyAttribute.FixedLength);
                var lengthName = nameof(PropertyAttribute.Length);
                var nameField = nameof(PropertyAttribute.Name);
                var typeNameField = nameof(PropertyAttribute.TypeName);

                foreach (var member in members)
                {
                    var (symbol, typeSymbol, nullable) = member;

                    // Attribute data
                    var attributeData = symbol.GetAttributeData(propertyType.FullName);
                    
                    // Ignore it?
                    var ignore = attributeData?.GetValue<bool?>(ignoreName) ?? false;
                    if (ignore)
                        continue;

                    // Is ansi, not unicode
                    var isAnsi = attributeData?.GetValue<bool?>(isAnsiName);
                    var fixedLength = attributeData?.GetValue<bool?>(fixedLengthName);
                    var length = attributeData?.GetValue<int?>(lengthName);

                    // Object field name
                    var fieldName = symbol.Name;

                    // Parameter name
                    var name = attributeData?.GetValue<string>(nameField);
                    if (string.IsNullOrEmpty(name))
                    {
                        name = snakeCase ? fieldName.ToSnakeCase() : fieldName;
                    }

                    // Data type name
                    var typeName = attributeData?.GetValue<string>(typeNameField);

                    // Array data
                    var arrayData = symbol.GetAttributeData(arrayPropertyType.FullName);

                    // Line parts
                    string valuePart;

                    // Is dbstring
                    bool isDbString = false;

                    if (typeSymbol.IsSimpleType())
                    {
                        // Default type name from member's type
                        if (typeName == null || typeName == string.Empty)
                        {
                            typeName = typeSymbol.Name;
                        }

                        // String case
                        if (typeName.Equals("String"))
                        {
                            isDbString = true;
                            valuePart = $"{fieldName}.ToDbString({isAnsi.ToCode()}, {length.ToIntCode()}, {fixedLength.ToCode()})";
                        }
                        else
                        {
                            valuePart = fieldName;
                        }
                    }
                    else if (typeSymbol.TypeKind == TypeKind.Enum)
                    {
                        // Enum item type
                        if (typeName == null || typeName == string.Empty)
                        {
                            var enumSymbol = (INamedTypeSymbol)typeSymbol;
                            typeName = enumSymbol.EnumUnderlyingType?.Name;
                            if (typeName == null)
                                typeName = "Byte";
                        }

                        valuePart = $"({typeName}){fieldName}";
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
                        var itemDbType = GetDbType(itemTypeSymbol, isAnsi);

                        if (string.IsNullOrEmpty(typeName))
                        {
                            isDbString = true;

                            // Splitter
                            var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? ',');

                            // String
                            valuePart = $"(databaseName == \"SQLSERVER\" ? SqlServerUtils.ListToTVP({fieldName}, {itemDbType}, {length.ToIntCode()}) : StringUtils.IEnumerableToString({fieldName}, '{splitter}').ToDbString({isAnsi.ToCode()}, {length.ToIntCode()}, {fixedLength.ToCode()}))";
                        }
                        else
                        {
                            // SQL Server TVP
                            valuePart = $"SqlServerUtils.ListToIdRecords({fieldName}, {itemDbType}, {length.ToIntCode()}).AsTableValuedParameter(\"{typeName}\")";
                        }
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
                        var itemDbType = GetDbType(itemTypeSymbol, isAnsi);

                        if (string.IsNullOrEmpty(typeName))
                        {
                            isDbString = true;

                            // Splitter
                            var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? ',');

                            // String
                            valuePart = $"(databaseName == \"SQLSERVER\" ? SqlServerUtils.ListToTVP({fieldName}, {itemDbType}, {length.ToIntCode()}) : StringUtils.IEnumerableToString({fieldName}, '{splitter}').ToDbString({isAnsi.ToCode()}, {length.ToIntCode()}, {fixedLength.ToCode()}))";
                        }
                        else
                        {
                            // SQL Server TVP
                            valuePart = $"SqlServerUtils.ListToIdRecords({fieldName}, {itemDbType}, {length.ToIntCode()}).AsTableValuedParameter(\"{typeName}\")";
                        }
                    }
                    else if(typeSymbol.IsDictionary())
                    {
                        if (string.IsNullOrEmpty(typeName))
                        {
                            isDbString = true;

                            // Splitter
                            var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? '&');

                            // String
                            valuePart = $"StringUtils.DictionaryToString({fieldName}, '{splitter}', '=').ToDbString({isAnsi.ToCode()}, {length.ToIntCode()}, {fixedLength.ToCode()})";
                        }
                        else
                        {
                            var dicSymbol = (INamedTypeSymbol)typeSymbol;

                            // Key type
                            var keyType = dicSymbol.TypeArguments[0];

                            // Item value type
                            var itemType = dicSymbol.TypeArguments[1];

                            // SQL Server TVP
                            valuePart = $"SqlServerUtils.DictionaryToRecords({fieldName}, {keyType.Name.ToDbType()}, {itemType.Name.ToDbType()}).AsTableValuedParameter(\"{typeName}\")";
                        }
                    }
                    else
                    {
                        continue;
                    }

                    // Item
                    var itemCode = new StringBuilder();
                    if (ignoreNull && (nullable || typeSymbol.IsReferenceType))
                    {
                        // Ignore null field
                        itemCode.AppendLine($"if({fieldName} != null)");
                    }

                    if (isDbString)
                        itemCode.Append($"parameters.Add(\"{name}\", {valuePart})");
                    else
                        itemCode.Append($"parameters.Add(\"{name}\", {valuePart}, {typeName.ToDbType()})");

                    body.Add(itemCode.ToString());
                }
            }

            return body;
        }

        private string GetDbType(ITypeSymbol symbol, bool? isAnsi)
        {
            return isAnsi.GetValueOrDefault() ? "DbType.AnsiString" : symbol.Name.ToDbType();
        }

        private void GenerateCode(GeneratorExecutionContext context, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = context.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (symbol == null || context.CancellationToken.IsCancellationRequested)
                return;

            // Attribute data
            var attributeData = symbol.GetAttributeData(attributeType.FullName);

            // Ignore null
            var ignoreNull = attributeData?.GetValue<bool>(nameof(AutoToParametersAttribute.IgnoreNull));

            // Snake case
            var snakeCase = attributeData?.GetValue<bool>(nameof(AutoToParametersAttribute.SnakeCase));

            // Name space and class name
            var (ns, className) = (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);

            // Keyword
            var keyword = tds.Keyword.ToString();

            // Inheritance
            var externals = new List<string>();

            // Body
            var body = GenerateBody(context, tds, ignoreNull.GetValueOrDefault(), snakeCase.GetValueOrDefault(), externals);
            if (context.CancellationToken.IsCancellationRequested)
                return;

            externals.Add("com.etsoo.Utils.Serialization.IAutoParameters");

            // Source code
            var source = $@"#nullable enable
                using Dapper;
                using com.etsoo.Utils.Database;
                using com.etsoo.Utils.String;
                using System;
                using System.Data;

                namespace {ns}
                {{
                    public partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        /// <summary>
                        /// Data modal to Dapper parameters
                        /// 数据模型转化为Dapper参数
                        /// </summary>
                        /// <param name=""databaseName"">Database name, see com.etsoo.Utils.Database.Name</param>
                        /// <returns>Dynamic parameters</returns>
                        public DynamicParameters AsParameters(string? databaseName = null)
                        {{
                            var parameters = new DynamicParameters();

                            {string.Join(";\n", body)};

                            return parameters;
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.AutoToParameters.Generated.cs", SourceText.From(source, Encoding.UTF8));
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
            /*
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
            */

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(AutoToParametersAttribute)));
        }
    }
}
