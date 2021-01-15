using com.etsoo.SourceGenerators;
using com.etsoo.SourceGenerators.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.etsoo.CoreFramework.SourceGenerators
{
    /// <summary>
    /// Auto Dictionary initialization generator
    /// </summary>
    [Generator]
    public class AutoDictionaryGenerator : ISourceGenerator
    {
        private string GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, bool snakeCase)
        {
            var body = new List<string>();

            var members = context.ParseMembers(tds);

            if (!context.CancellationToken.IsCancellationRequested)
            {
                var arrayPropertyType = typeof(ArrayPropertyAttribute);

                foreach (var member in members)
                {
                    var (symbol, _, typeSymbol, nullable) = member;

                    // Object field name
                    var fieldName = symbol.Name;

                    // Data field name
                    var dataFieldName = snakeCase ? fieldName.ToSnakeCase() : fieldName;

                    // Field type name
                    var typeName = typeSymbol.Name + (nullable ? "?" : string.Empty);

                    // ArrayProperty attribute data
                    var arrayData = symbol.GetAttributeData(arrayPropertyType.FullName);

                    // Value part
                    string valuePart;

                    if (typeSymbol.IsSimpleType())
                    {
                        valuePart = $@"dic.GetExact<{typeName}>(""{dataFieldName}"")";
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
                        var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? ',');

                        var arrayType = itemTypeSymbol.Name;
                        if(arrayType.Equals("String"))
                            valuePart = $@"StringUtil.AsEnumerable(dic.GetExact<string?>(""{dataFieldName}""), '{splitter}').ToArray()";
                        else
                            valuePart = $@"StringUtil.AsEnumerable<{arrayType}>(dic.GetExact<string?>(""{dataFieldName}""), '{splitter}').ToArray()";
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
                        var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? ',');

                        var listType = itemTypeSymbol.Name;
                        if(listType.Equals("String"))
                            valuePart = $@"StringUtil.AsEnumerable(dic.GetExact<string?>(""{dataFieldName}""), '{splitter}').ToList()";
                        else
                            valuePart = $@"StringUtil.AsEnumerable<{listType}>(dic.GetExact<string?>(""{dataFieldName}""), '{splitter}').ToList()";
                    }
                    else
                    {
                        continue;
                    }

                    body.Add($@"{fieldName} = {valuePart}");
                }
            }

            return string.Join(",\n", body);
        }

        private void GenerateCode(GeneratorExecutionContext context, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = context.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (context.CancellationToken.IsCancellationRequested)
                return;

            // Attribute data
            var attributeData = symbol.GetAttributeData(attributeType.FullName);

            // Snake case
            var snakeCase = attributeData.GetValue<bool>(nameof(AutoToParametersAttribute.SnakeCase));

            // Name space and class name
            var (ns, className) = (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);

            // Keyword
            var keyword = tds.Keyword.ToString();

            // Name
            var name = tds.Identifier.ToString();

            // Source code
            var source = $@"#nullable enable
                using com.etsoo.Utils.String;
                using System;
                using System.Collections.Generic;
                using System.Linq;

                namespace {ns}
                {{
                    public partial {keyword} {className} : com.etsoo.Utils.Serialization.IDictionaryParser
                    {{
                        public static {name} Create(StringKeyDictionaryObject dic)
                        {{
                            return new {name} {{
                                {GenerateBody(context, tds, snakeCase)}
                            }};
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.Dictionary.Generated.cs", SourceText.From(source, Encoding.UTF8));
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // The generator infrastructure will create a receiver and populate it
            // We can retrieve the populated instance via the context
            if (!(context.SyntaxReceiver is SyntaxReceiver syntaxReceiver))
            {
                return;
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

            // Records
            foreach (var rds in syntaxReceiver.RecordCandidates)
            {
                GenerateCode(context, rds, syntaxReceiver.AttributeType);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
/*            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }*/

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(AutoDictionaryGeneratorAttribute), SyntaxKind.PartialKeyword));
        }
    }
}
