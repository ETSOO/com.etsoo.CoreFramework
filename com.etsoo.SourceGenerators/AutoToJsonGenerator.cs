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
    /// Auto ToJson generator
    /// Simple alternative to System.Text.Json.SourceGeneration
    /// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public class AutoToJsonGenerator : IIncrementalGenerator
    {
        // Get method name by type name
        private string GetMethod(string typeName)
        {
            if (typeName == "bool" || typeName == "boolean")
                return "WriteBoolean";

            return typeName.IsNumericType() ? "WriteNumber" : "WriteString";
        }

        private string GenerateBody(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, List<string> externalInheritances)
        {
            var body = new List<string>();

            var members = context.ParseMembers(compilation, tds, true, externalInheritances, out _);

            if (!context.CancellationToken.IsCancellationRequested)
            {
                foreach (var member in members)
                {
                    var (symbol, typeSymbol, nullable) = member;

                    // Ignore static field
                    if (symbol.IsStatic) continue;

                    // Attribute data
                    var attributeData = symbol.GetAttributeData("com.etsoo.Utils.Serialization.PIIAttribute");
                    if (attributeData != null) continue;

                    // Field name
                    var fieldName = symbol.Name;

                    // Is field or property
                    var isField = (symbol.Kind == SymbolKind.Field).ToCode();

                    // Is readonly
                    var isReadonly = typeSymbol.IsReadOnly.ToCode();

                    // Is nullable
                    var isNullable = nullable ? $"{fieldName} == null" : "false";

                    // Parameter name
                    var pName = "name" + fieldName;

                    // Item
                    var item = new StringBuilder();
                    if (nullable)
                    {
                        item.AppendLine($@"if({fieldName} == null)
                            w.WriteNull({pName});
                        else {{
                        ");
                    }

                    if (typeSymbol.IsSimpleType(true))
                    {
                        // Value field
                        var valueField = nullable && typeSymbol.IsValueType ? $"{fieldName}.Value" : fieldName;

                        // typeSymbol.ToString() = "int", typeSymbol.Name = "Int32"
                        var method = GetMethod(typeSymbol.ToString());
                        item.AppendLine($@"w.{method}({pName}, {valueField});");
                    }
                    else if (typeSymbol.TypeKind == TypeKind.Enum)
                    {
                        // Enum item type
                        var enumSymbol = (INamedTypeSymbol)typeSymbol;
                        var enumType = enumSymbol.EnumUnderlyingType?.Name ?? "Byte";
                        item.AppendLine($@"w.WriteNumber({pName}, ({enumType}){fieldName});");
                    }
                    else if (typeSymbol.TypeKind == TypeKind.Array || typeSymbol.IsList())
                    {
                        // Type
                        var itemTypeSymbol = typeSymbol.TypeKind == TypeKind.Array ? ((IArrayTypeSymbol)typeSymbol).ElementType : ((INamedTypeSymbol)typeSymbol).TypeArguments[0];
                        if (!itemTypeSymbol.IsSimpleType())
                        {
                            item.AppendLine($@"w.WritePropertyName({pName});");
                            item.AppendLine($@"await SharedUtils.JsonSerializeAsync(w, {fieldName}, options);");
                            //item.AppendLine($@"JsonSerializer.Serialize(w, {fieldName}, options);");
                        }
                        else
                        {
                            var method = GetMethod(itemTypeSymbol.ToString()) + "Value";
                            var itemName = "item" + fieldName;
                            item.AppendLine($@"w.WritePropertyName({pName});
                                w.WriteStartArray();

                                foreach (var {itemName} in {fieldName})
                                {{
                                    w.{method}({itemName});
                                }}

                                w.WriteEndArray();
                            ");
                        }
                    }
                    else
                    {
                        item.AppendLine($@"w.WritePropertyName({pName});");
                        item.AppendLine($@"await SharedUtils.JsonSerializeAsync(w, {fieldName}, options);");
                        //item.AppendLine($@"JsonSerializer.Serialize(w, {fieldName}, options);");
                    }

                    if (nullable)
                    {
                        item.AppendLine("}");
                    }

                    // Is writable
                    body.Add($@"
                        if (options.IsWritable({isNullable}, {isField}, {isReadonly}))
                        {{
                            var {pName} = options.ConvertName(""{fieldName}"");
                            if (fields == null || fields.Any(field => field.Equals(""{fieldName}"", StringComparison.OrdinalIgnoreCase) || field.Equals({pName})))
                            {{
                                {item}
                            }}
                        }}
                    ");
                }
            }

            return string.Join("\n", body);
        }

        private void GenerateCode(SourceProductionContext context, Compilation compilation, TypeDeclarationSyntax tds, Type _)
        {
            // Field symbol
            var symbol = compilation.ParseSyntaxNode<INamedTypeSymbol>(tds);
            if (symbol == null || context.CancellationToken.IsCancellationRequested)
                return;

            // Name space and class name
            var (ns, className) = (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);

            // Keyword
            var keyword = tds.Keyword.ToString();

            // Is Public
            var isPublic = tds.HasToken(SyntaxKind.PublicKeyword);

            // Is Virtual
            var virtualKeyword = tds.HasToken(SyntaxKind.SealedKeyword) || keyword.Equals("struct") ? "" : " virtual";

            // Has to call setup?
            var hasSetup = tds.Members
                .Where(member => member is MethodDeclarationSyntax)
                .Cast<MethodDeclarationSyntax>()
                .Any(m => m.Identifier.Text == "SetupAsync"
                    && m.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.AsyncKeyword))
                    && m.ParameterList.Parameters.Count == 0
                    && m.ReturnType.ToString() == "Task<bool>"
                    && m.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ProtectedKeyword)));

            // Inheritance
            var externals = new List<string>();

            // Body
            var body = GenerateBody(context, compilation, tds, externals);
            if (context.CancellationToken.IsCancellationRequested)
                return;

            externals.Add("com.etsoo.Utils.Serialization.IJsonSerialization");

            // Source code
            var source = $@"#nullable enable
                using com.etsoo.Utils;
                using com.etsoo.Utils.Serialization;
                using System;
                using System.Text.Json;
                using System.Threading.Tasks;

                namespace {ns}
                {{
                    {(isPublic ? "public" : "internal")} partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        /// <summary>
                        /// To Json
                        /// 转化为 Json
                        /// </summary>
                        /// <param name=""writer"">Writer</param>
                        /// <param name=""options"">Options</param>
                        /// <param name=""fields"">Fields allowed</param>
                        /// <returns>Task</returns>
                        public{virtualKeyword} async Task ToJsonAsync(System.Buffers.IBufferWriter<byte> writer, JsonSerializerOptions options, IEnumerable<string>? fields = null)
                        {{
                            {(hasSetup ? "if (!(await SetupAsync())) return;\n" : "")}
                            // Utf8JsonWriter
                            using var w = options.CreateJsonWriter(writer);

                            // Object start
                            w.WriteStartObject();

                            {body}

                            // Object end
                            w.WriteEndObject();

                            // Flush & dispose
                            await w.DisposeAsync();
                        }}
                    }}
                }}
            ";

            context.AddSource($"{ns}.{className}.AutoToJson.Generated.cs", SourceText.From(source, Encoding.UTF8));
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            var attributeType = typeof(AutoToJsonAttribute);
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
