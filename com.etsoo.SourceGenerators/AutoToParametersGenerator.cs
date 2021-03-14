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
    /// Auto AsParameters generator
    /// </summary>
    [Generator]
    public class AutoToParametersGenerator : ISourceGenerator
    {
        private IEnumerable<string> GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, bool ignoreNull, bool snakeCase)
        {
            var body = new List<string>();

            var members = context.ParseMembers(tds);

            if(!context.CancellationToken.IsCancellationRequested)
            {
                var propertyType = typeof(PropertyAttribute);
                var arrayPropertyType = typeof(ArrayPropertyAttribute);
                var isAnsiName = nameof(PropertyAttribute.IsAnsi);
                var nameField = nameof(PropertyAttribute.Name);
                var typeNameField = nameof(PropertyAttribute.TypeName);

                foreach (var member in members)
                {
                    var (symbol, _, typeSymbol, nullable) = member;

                    // Object field name
                    var fieldName = symbol.Name;

                    var attributeData = symbol.GetAttributeData(propertyType.FullName);
                    var arrayData = symbol.GetAttributeData(arrayPropertyType.FullName);

                    var isAnsi = attributeData?.GetValue<bool?>(isAnsiName) ?? false;

                    // Parameter name
                    var name = attributeData?.GetValue<string>(nameField);
                    if (string.IsNullOrEmpty(name))
                    {
                        name = snakeCase ? fieldName.ToSnakeCase() : fieldName;
                    }

                    // Data type name
                    var typeName = attributeData?.GetValue<string>(typeNameField);

                    // Line parts
                    string valuePart;

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
                            valuePart = $"{fieldName}.ToDbString({isAnsi.ToCode()})";
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

                        valuePart = $"({typeName.ToLower()}){fieldName}";
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

                        if (string.IsNullOrEmpty(typeName))
                        {
                            // Splitter
                            var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? ',');

                            // String
                            valuePart = $"StringUtil.IEnumerableToString({fieldName}, '{splitter}').ToDbString({isAnsi.ToCode()})";
                        }
                        else
                        {
                            // SQL Server TVP
                            valuePart = $"SqlServerUtil.ListToIdRecords({fieldName}, {itemTypeSymbol.Name.ToDbType()}).AsTableValuedParameter(\"{typeName}\")";
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

                        if (string.IsNullOrEmpty(typeName))
                        {
                            // Splitter
                            var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? ',');

                            // String
                            valuePart = $"StringUtil.IEnumerableToString({fieldName}, '{splitter}').ToDbString({isAnsi.ToCode()})";
                        }
                        else
                        {
                            // SQL Server TVP
                            valuePart = $"SqlServerUtil.ListToIdRecords({fieldName}, {itemTypeSymbol.Name.ToDbType()}).AsTableValuedParameter(\"{typeName}\")";
                        }
                    }
                    else if(typeSymbol.IsDictionary())
                    {
                        if (string.IsNullOrEmpty(typeName))
                        {
                            // Splitter
                            var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? '&');

                            // String
                            valuePart = $"StringUtil.DictionaryToString({fieldName}, '{splitter}', '=').ToDbString({isAnsi.ToCode()})";
                        }
                        else
                        {
                            var dicSymbol = (INamedTypeSymbol)typeSymbol;

                            // Key type
                            var keyType = dicSymbol.TypeArguments[0];

                            // Item value type
                            var itemType = dicSymbol.TypeArguments[1];

                            // SQL Server TVP
                            valuePart = $"SqlServerUtil.DictionaryToRecords({fieldName}, {keyType.Name.ToDbType()}, {itemType.Name.ToDbType()}).AsTableValuedParameter(\"{typeName}\")";
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
                    itemCode.Append($"parameters.Add(\"{name}\", {valuePart}, {typeName.ToDbType()})");

                    body.Add(itemCode.ToString());
                }
            }

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

            // Ignore null
            var ignoreNull = attributeData?.GetValue<bool>(nameof(AutoToParametersAttribute.IgnoreNull));

            // Snake case
            var snakeCase = attributeData?.GetValue<bool>(nameof(AutoToParametersAttribute.SnakeCase));

            // Name space and class name
            var (ns, className) = (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);

            // Keyword
            var keyword = tds.Keyword.ToString();

            // Body
            var body = GenerateBody(context, tds, ignoreNull.GetValueOrDefault(), snakeCase.GetValueOrDefault());
            if (context.CancellationToken.IsCancellationRequested)
                return;

            // Source code
            var source = $@"
                using Dapper;
                using com.etsoo.Utils.Database;
                using com.etsoo.Utils.String;
                using System;
                using System.Data;

                namespace {ns}
                {{
                    public partial {keyword} {className} : com.etsoo.Utils.Serialization.IAutoParameters
                    {{
                        /// <summary>
                        /// Data modal to Dapper parameters
                        /// 数据模型转化为Dapper参数
                        /// </summary>
                        /// <returns>Dynamic parameters</returns>
                        public DynamicParameters AsParameters()
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
            if (!(context.SyntaxReceiver is SyntaxReceiver syntaxReceiver))
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
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            */

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(AutoToParametersAttribute), SyntaxKind.PartialKeyword));
        }
    }
}
