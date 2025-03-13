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
    /// Auto Dictionary initialization generator
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public class AutoDictionaryGenerator : IIncrementalGenerator
    {
        private string GenerateBody(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, bool snakeCase, List<string> externalInheritances, string ns)
        {
            var body = new List<string>();

            var members = context.ParseMembers(compilation, tds, false, externalInheritances, out bool isPositionalRecord);

            if (!context.CancellationToken.IsCancellationRequested)
            {
                var arrayPropertyType = typeof(ArrayPropertyAttribute);

                foreach (var member in members)
                {
                    var (symbol, typeSymbol, nullable) = member;

                    // Ignore static field
                    if (symbol.IsStatic) continue;

                    // Object field name
                    var fieldName = symbol.Name;

                    // Data field name
                    var dataFieldName = snakeCase ? fieldName.ToSnakeCase() : fieldName;

                    // Field type name
                    var typeName = typeSymbol.GetTypeName(nullable, ns);

                    // Attribute data
                    var attributeData = symbol.GetAttributeData(arrayPropertyType.FullName);

                    // Value part
                    string valuePart;

                    if (typeSymbol.IsSimpleType())
                    {
                        valuePart = $@"dic.GetExact<{typeName}>(""{dataFieldName}"")";

                        if (typeSymbol.Name == "String" && !nullable)
                        {
                            valuePart += "!";
                        }
                    }
                    else if (typeSymbol.TypeKind == TypeKind.Enum)
                    {
                        // Enum item type
                        var enumSymbol = (INamedTypeSymbol)typeSymbol;
                        var enumType = enumSymbol.EnumUnderlyingType?.Name ?? "Byte";
                        valuePart = $@"({typeName})dic.GetExact<{enumType}>(""{dataFieldName}"")";
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
                            valuePart = $@"StringUtils.AsEnumerable(dic.GetExact<string?>(""{dataFieldName}""), '{splitter}').ToArray()";
                        else
                            valuePart = $@"StringUtils.AsEnumerable<{arrayType}>(dic.GetExact<string?>(""{dataFieldName}""), '{splitter}').ToArray()";
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
                            valuePart = $@"StringUtils.AsEnumerable(dic.GetExact<string?>(""{dataFieldName}""), '{splitter}').ToList()";
                        else
                            valuePart = $@"StringUtils.AsEnumerable<{listType}>(dic.GetExact<string?>(""{dataFieldName}""), '{splitter}').ToList()";
                    }
                    else
                    {
                        continue;
                    }

                    if (isPositionalRecord)
                        body.Add($@"{fieldName}: {valuePart}");
                    else
                        body.Add($@"{fieldName} = {valuePart}");
                }
            }

            // Limitation: When inheritanted, please keep the same style
            // Define constructor for Positional Record
            if (isPositionalRecord)
                return "(" + string.Join(",\n", body) + ")";

            return "{\n" + string.Join(",\n", body) + "\n}";
        }

        private void GenerateCode(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = compilation.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (symbol == null || context.CancellationToken.IsCancellationRequested)
                return;

            // Attribute data
            var attributeData = symbol.GetAttributeData(attributeType.FullName);

            // Snake case
            var snakeCase = attributeData?.GetValue<bool>(nameof(AutoToParametersAttribute.SnakeCase));

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
            var body = GenerateBody(context, compilation, tds, snakeCase.GetValueOrDefault(), externals, ns);
            if (context.CancellationToken.IsCancellationRequested)
                return;

            externals.Add($"com.etsoo.Utils.Serialization.IDictionaryParser<{name}>");

            // Source code
            var source = $@"#nullable enable
                using com.etsoo.Utils.String;
                using System;
                using System.Collections.Generic;
                using System.Linq;

                namespace {ns}
                {{
                    {(isPublic ? "public" : "internal")} partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        public static {name} Create(StringKeyDictionaryObject dic)
                        {{
                            return new {name} {body};
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.Dictionary.Generated.cs", SourceText.From(source, Encoding.UTF8));
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            /*
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
            */

            var attributeType = typeof(AutoDictionaryGeneratorAttribute);
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
