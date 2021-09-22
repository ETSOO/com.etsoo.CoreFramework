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
    /// Auto DbDataReader initialization generator
    /// </summary>
    [Generator]
    public class AutoDataReaderGenerator : ISourceGenerator
    {
        private string GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, bool utcDateTime, List<string> externalInheritances)
        {
            var body = new List<string>();

            var members = context.ParseMembers(tds, false, externalInheritances, out bool isPositionalRecord);

            if (!context.CancellationToken.IsCancellationRequested)
            {
                var arrayPropertyType = typeof(ArrayPropertyAttribute);

                foreach (var member in members)
                {
                    var (symbol, typeSymbol, nullable) = member;

                    // Object field name
                    var fieldName = symbol.Name;

                    // Field type name
                    string typeName = typeSymbol.Name + (nullable ? "?" : string.Empty);

                    // ArrayProperty attribute data
                    var arrayData = symbol.GetAttributeData(arrayPropertyType.FullName);

                    // Value part
                    string valuePart;

                    if (typeSymbol.IsSimpleType())
                    {
                        valuePart = $@"(await reader.GetValueAsync<{typeName}>(""{fieldName}"", names))";

                        if (utcDateTime && typeSymbol.Name == "DateTime")
                        {
                            valuePart = $"LocalizationUtils.SetUtcKind{valuePart}";
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
                        var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? ',');

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
                        var splitter = Extensions.CharToString(arrayData?.GetValue<char?>("Splitter") ?? ',');

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

        private void GenerateCode(GeneratorExecutionContext context, TypeDeclarationSyntax tds, Type attributeType)
        {
            // Field symbol
            var symbol = context.ParseSyntaxNode<INamedTypeSymbol>(tds);
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

            // Name
            var name = tds.Identifier.ToString();

            // Inheritance
            var externals = new List<string>();

            // Body
            var body = GenerateBody(context, tds, utcDateTime.GetValueOrDefault(), externals);
            if (context.CancellationToken.IsCancellationRequested)
                return;

            externals.Add($"com.etsoo.Utils.Serialization.IDataReaderParser<{name}>");

            // Source code
            var source = $@"#nullable enable
                using System;
                using System.Collections.Generic;
                using System.Data.Common;
                using System.Linq;
                using System.Threading.Tasks;
                using com.etsoo.Utils.Database;
                using com.etsoo.Utils.String;
                using com.etsoo.Utils.Localization;

                namespace {ns}
                {{
                    public partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        public static async IAsyncEnumerable<{name}> CreateAsync(DbDataReader reader)
                        {{
                            // Column names
                            var names = reader.GetColumnNames().ToList();

                            while(await reader.ReadAsync())
                            {{
                                yield return new {name} {body};
                            }}
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.DataReader.Generated.cs", SourceText.From(source, Encoding.UTF8));
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
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(AutoDataReaderGeneratorAttribute)));
        }
    }
}
