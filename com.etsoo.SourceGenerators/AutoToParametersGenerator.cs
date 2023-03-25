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
    /// Auto AsParameters generator
    /// </summary>
    [Generator]
    public class AutoToParametersGenerator : ISourceGenerator
    {
        private IEnumerable<string> GenerateBody(GeneratorExecutionContext context, TypeDeclarationSyntax tds, bool ignoreNull, bool snakeCase, List<string> externalInheritances)
        {
            var body = new List<string>();

            var members = context.ParseMembers(tds, true, externalInheritances, out _);

            if (!context.CancellationToken.IsCancellationRequested)
            {
                var propertyType = typeof(PropertyAttribute);
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

                    // StringLengthAttribute, avoid duplicate set
                    var stringLengthAttribute = symbol.GetAttributeData("System.ComponentModel.DataAnnotations.StringLengthAttribute");
                    var stringLength = stringLengthAttribute?.GetValue<int?>("MaximumLength");

                    // Is ansi, not unicode
                    var isAnsi = attributeData?.GetValue<bool?>(isAnsiName);
                    var fixedLength = attributeData?.GetValue<bool?>(fixedLengthName);
                    var length = attributeData?.GetValue<int?>(lengthName) ?? stringLength;

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
                            typeName = enumSymbol.EnumUnderlyingType?.Name ?? "Byte";
                        }

                        valuePart = $"({typeName}){fieldName}";
                    }
                    else if (typeName == "json")
                    {
                        isDbString = true;
                        valuePart = $"JsonSerializer.Serialize({fieldName}, com.etsoo.Utils.SharedUtils.JsonDefaultSerializerOptions).ToDbString({isAnsi.ToCode()}, {length.ToIntCode()}, {fixedLength.ToCode()})";
                    }
                    else if (typeSymbol.TypeKind == TypeKind.Array)
                    {
                        // Array type
                        var arraySymbol = (IArrayTypeSymbol)typeSymbol;
                        var itemTypeSymbol = arraySymbol.ElementType;
                        if (itemTypeSymbol.IsSimpleType())
                        {
                            var itemDbType = GetDbType(itemTypeSymbol, isAnsi);

                            isDbString = true;

                            if (string.IsNullOrEmpty(typeName))
                            {
                                // String
                                valuePart = $"app.DB.ListToParameter({fieldName}, {itemDbType}, {length.ToIntCode()}, (type) => SqlServerUtils.GetListCommand(type, app.BuildCommandName))";
                            }
                            else
                            {
                                // SQL Server TVP
                                valuePart = $"SqlServerUtils.ListToIdRecords({fieldName}, {itemDbType}, {length.ToIntCode()}).AsTableValuedParameter(\"{typeName}\")";
                            }
                        }
                        else if (itemTypeSymbol.InheritedFrom("com.etsoo.Database.ISqlServerDataRecord"))
                        {
                            isDbString = true;
                            valuePart = $"{fieldName}.ToTVP()";
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (typeSymbol.IsList())
                    {
                        // Item type
                        var listSymbol = (INamedTypeSymbol)typeSymbol;
                        var itemTypeSymbol = listSymbol.TypeArguments[0];
                        if (itemTypeSymbol.IsSimpleType())
                        {
                            var itemDbType = GetDbType(itemTypeSymbol, isAnsi);

                            isDbString = true;

                            if (string.IsNullOrEmpty(typeName))
                            {
                                // String
                                valuePart = $"app.DB.ListToParameter({fieldName}, {itemDbType}, {length.ToIntCode()}, (type) => SqlServerUtils.GetListCommand(type, app.BuildCommandName))";
                            }
                            else
                            {
                                // SQL Server TVP
                                valuePart = $"SqlServerUtils.ListToIdRecords({fieldName}, {itemDbType}, {length.ToIntCode()}).AsTableValuedParameter(\"{typeName}\")";
                            }
                        }
                        else if (itemTypeSymbol.InheritedFrom("com.etsoo.Database.ISqlServerDataRecord"))
                        {
                            isDbString = true;
                            valuePart = $"{fieldName}.ToTVP()";
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (typeSymbol.IsDictionary())
                    {
                        var dicSymbol = (INamedTypeSymbol)typeSymbol;

                        // Key type
                        var keyType = dicSymbol.TypeArguments[0];
                        var keyDbType = keyType.Name.ToDbType();

                        // Item value type
                        var itemType = dicSymbol.TypeArguments[1];
                        var itemDbType = GetDbType(itemType, isAnsi);

                        isDbString = true;

                        if (string.IsNullOrEmpty(typeName))
                        {
                            // String
                            valuePart = $"app.DB.DictionaryToTVP({fieldName}, {keyDbType}, {itemDbType}, null, {length.ToIntCode()}, (keyType, valueType) => SqlServerUtils.GetDicCommand(keyType, valueType, app.BuildCommandName))";
                        }
                        else
                        {
                            // SQL Server TVP
                            valuePart = $"SqlServerUtils.DictionaryToRecords({fieldName}, {keyDbType}, {itemDbType}).AsTableValuedParameter(\"{typeName}\")";
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
                        itemCode.AppendLine($"if({fieldName} == null) {{ parameters.Add(\"{name}\", null); }} else ");
                    }

                    if (isDbString)
                        itemCode.Append($"parameters.Add(\"{name}\", {valuePart})");
                    else
                        itemCode.Append($"app.DB.AddParameter(parameters, \"{name}\", {valuePart}, {typeName.ToDbType()})");

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

            // Has to call setup?
            var hasSetup = tds.Members
                .Where(member => member is MethodDeclarationSyntax)
                .Cast<MethodDeclarationSyntax>()
                .Any(m => m.Identifier.Text == "Setup"
                    && m.ParameterList.Parameters.Count == 1
                    && m.ParameterList.Parameters[0].Type?.ToString() == "IDbParameters"
                    && m.ReturnType.ToString() == "bool"
                    && m.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)));

            // Is Public
            var isPublic = tds.HasToken(SyntaxKind.PublicKeyword);

            // Is Virtual
            var virtualKeyword = tds.HasToken(SyntaxKind.SealedKeyword) || keyword.Equals("struct") ? "" : " virtual";

            // Inheritance
            var externals = new List<string>();

            // Body
            var body = GenerateBody(context, tds, ignoreNull.GetValueOrDefault(), snakeCase.GetValueOrDefault(), externals);
            if (context.CancellationToken.IsCancellationRequested)
                return;

            externals.Add("com.etsoo.CoreFramework.Models.IModelParameters");

            // Source code
            var source = $@"#nullable enable
                using Dapper;
                using com.etsoo.Database;
                using com.etsoo.Utils.String;
                using com.etsoo.CoreFramework.Application;
                using System;
                using System.Data;
                using System.Text.Json;

                namespace {ns}
                {{
                    {(isPublic ? "public" : "internal")} partial {keyword} {className} : {string.Join(", ", externals)}
                    {{
                        /// <summary>
                        /// Data modal to Dapper parameters
                        /// 数据模型转化为Dapper参数
                        /// </summary>
                        /// <param name=""app"">Application</param>
                        /// <returns>Dynamic parameters</returns>
                        public{virtualKeyword} IDbParameters AsParameters(ICoreApplicationBase app)
                        {{
                            var parameters = new DbParameters();
                            {(hasSetup ? "if (!Setup(parameters)) return parameters;\n" : "")};

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
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            // Register a factory that can create our custom syntax receiver
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(typeof(AutoToParametersAttribute)));
        }
    }
}
