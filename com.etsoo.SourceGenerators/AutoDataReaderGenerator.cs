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
    [Generator]
    public class AutoDataReaderGenerator : ISourceGenerator
    {
        private string GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds)
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

                    // Field type name
                    string typeName = typeSymbol.Name + (nullable ? "?" : string.Empty);

                    // ArrayProperty attribute data
                    var arrayData = symbol.GetAttributeData(arrayPropertyType.FullName);

                    // Value part
                    string valuePart;

                    if (typeSymbol.IsSimpleType())
                    {
                        valuePart = $@"(await reader.GetValueAsync<{typeName}>(""{fieldName}"", names))";
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
                        if(arrayType.Equals("String"))
                            valuePart = $@"StringUtil.AsEnumerable(await reader.GetValueAsync<string>(""{fieldName}"", names), '{splitter}').ToArray()";
                        else
                            valuePart = $@"StringUtil.AsEnumerable<{arrayType}>(await reader.GetValueAsync<string>(""{fieldName}"", names), '{splitter}').ToArray()";
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
                            valuePart = $@"StringUtil.AsEnumerable(await reader.GetValueAsync<string>(""{fieldName}"", names), '{splitter}').ToList()";
                        else
                            valuePart = $@"StringUtil.AsEnumerable<{listType}>(await reader.GetValueAsync<string>(""{fieldName}"", names), '{splitter}').ToList()";
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

        private void GenerateCode(GeneratorExecutionContext context, TypeDeclarationSyntax tds, Type _)
        {
            // Field symbol
            var symbol = context.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (symbol == null || context.CancellationToken.IsCancellationRequested)
                return;

            // Name space and class name
            var (ns, className) = (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);

            // Keyword
            var keyword = tds.Keyword.ToString();

            // Name
            var name = tds.Identifier.ToString();

            // Source code
            var source = $@"#nullable enable
                using System;
                using System.Collections.Generic;
                using System.Data.Common;
                using System.Linq;
                using System.Threading.Tasks;
                using com.etsoo.Utils.Database;
                using com.etsoo.Utils.String;

                namespace {ns}
                {{
                    public partial {keyword} {className} : com.etsoo.Utils.Serialization.IDataReaderParser
                    {{
                        public static async Task<IEnumerable<{name}>> CreateAsync(Task<DbDataReader> readerTask)
                        {{
                            using var reader = await readerTask;
                            var list = await CreateAsync(reader);
                            return list;
                        }}

                        public static async Task<IEnumerable<{name}>> CreateAsync(DbDataReader reader)
                        {{
                            // Object list
                            var list = new List<{name}>();
                            
                            // Column names
                            var names = reader.GetColumnNames().ToList();

                            while(await reader.ReadAsync())
                            {{
                                list.Add(new {name} {{
                                    {GenerateBody(context, tds)}
                                }});
                            }}

                            return list;
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
/*            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }*/

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(AutoDataReaderGeneratorAttribute), SyntaxKind.PartialKeyword));
        }
    }
}
