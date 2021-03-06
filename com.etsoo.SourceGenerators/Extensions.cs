﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Source generators extension
    /// 源生成器扩展
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Is SyntaxList has the specific attribute
        /// SyntaxList 是否具有特定属性
        /// </summary>
        /// <param name="lists">SyntaxList</param>
        /// <param name="attributeType">Attribute type</param>
        /// <returns>Result</returns>
        public static bool HasAttribute(this SyntaxList<AttributeListSyntax> lists, Type attributeType)
        {
            return HasAttribute(lists, attributeType.Namespace, attributeType.Name);
        }

        /// <summary>
        /// Is SyntaxList has the specific attribute
        /// SyntaxList 是否具有特定属性
        /// </summary>
        /// <param name="lists">SyntaxList</param>
        /// <param name="ns">Attriubte class namespace</param>
        /// <param name="className">Attribute class name</param>
        /// <returns>Result</returns>
        public static bool HasAttribute(this SyntaxList<AttributeListSyntax> lists, string ns, string className)
        {
            return lists.Any(list => list.Attributes.Any(a => {
                // Name match
                var name = a.Name.ToString();
                if (name.Equals(className) || (name + "Attribute").Equals(className))
                {
                    // Namespace match
                    if (a.GetUsingDirectiveSyntax(ns) != null)
                    {
                        return true;
                    }
                }

                return false;
            }));
        }

        /// <summary>
        /// Convert char to string
        /// 转化 char 为字符串
        /// </summary>
        /// <param name="c">Char</param>
        /// <returns>Result</returns>
        public static string CharToString(char c)
        {
            if (c == '\r')
                return "\\r";
            if (c == '\n')
                return "\\n";
            if (c == '\t')
                return "\\t";
            if (c == '\b')
                return "\\b";
            if (c == '\'')
                return "\\'";

            return c.ToString();
        }

        /// <summary>
        /// Get SyntaxNode type UsingDirectiveSyntax
        /// 获取 SyntaxNode 类型的引用
        /// </summary>
        /// <param name="sn">SyntaxNode</param>
        /// <param name="ns">Name space</param>
        /// <returns>UsingDirectiveSyntax</returns>
        public static UsingDirectiveSyntax GetUsingDirectiveSyntax(this SyntaxNode sn, string ns)
        {
            return sn.SyntaxTree.GetReference(sn).SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().FirstOrDefault((s) => s.Name.ToString().Equals(ns));
        }

        /// <summary>
        /// Test SyntaxNode has the specific kind of token
        /// 测试 SyntaxNode 是否具有特定类型的令牌
        /// </summary>
        /// <param name="sn">SyntaxNode</param>
        /// <param name="kind">Syntax kind</param>
        /// <returns>Result</returns>
        public static bool HasToken(this SyntaxNode sn, SyntaxKind kind)
        {
            return sn.ChildTokens().Any(token => token.IsKind(kind));
        }

        /// <summary>
        /// Get parent SyntaxNode
        /// 获取父 SyntaxNode
        /// </summary>
        /// <param name="compilation">Compilation</param>
        /// <param name="sn">SyntaxNode</param>
        /// <returns>Symbol</returns>
        public static T? GetParentSyntaxNode<T>(this SyntaxNode sn)
        {
            while (sn != null)
            {
                if (sn is T nds)
                {
                    return nds;
                }

                if (sn.Parent == null)
                    break;

                sn = sn.Parent;
            }

            return default;
        }

        /// <summary>
        /// Get attribute data
        /// 获取属性数据
        /// </summary>
        /// <param name="symbol">Symbol</param>
        /// <param name="type">Type full name</param>
        /// <returns>Attribute data</returns>
        public static AttributeData? GetAttributeData(this ISymbol symbol, string type)
        {
            return symbol.GetAttributes().SingleOrDefault(a => {
                if (a.AttributeClass != null)
                {
                    return a.AttributeClass.ToDisplayString().Equals(type);
                }

                return false;
            });
        }

        /// <summary>
        /// Get attribute data value
        /// 获取属性数据字段值
        /// </summary>
        /// <typeparam name="T">Generic value type</typeparam>
        /// <param name="data">Attribute data</param>
        /// <param name="name">Field name</param>
        /// <returns>Field value</returns>
        public static T? GetValue<T>(this AttributeData data, string name)
        {
            var kv = data.NamedArguments.FirstOrDefault(a => a.Key.Equals(name));
            var value = kv.Value.Value;
            // kv == default(KeyValuePair<string, TypedConstant>)
            if (kv.Key == null && data.AttributeConstructor != null)
            {
                var parameters = data.AttributeConstructor.Parameters;
                for (var p = 0; p < parameters.Length; p++)
                {
                    if (parameters[p].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        value = data.ConstructorArguments[p].Value;
                        break;
                    }
                }
            }

            if(value == null)
            {
                return default;
            }

            return (T?)value;
        }

        /// <summary>
        /// Parse SyntaxNode
        /// 解析 SyntaxNode
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="sn">SyntaxNode</param>
        /// <returns>Symbol</returns>
        public static T? ParseSyntaxNode<T>(this GeneratorExecutionContext context, SyntaxNode sn) where T : ISymbol
        {
            var model = context.Compilation.GetSemanticModel(sn.SyntaxTree);
            return (T?)model.GetDeclaredSymbol(sn);
        }

        /// <summary>
        /// Parse TypeSyntax
        /// 解析 TypeSyntax
        /// </summary>
        /// <param name="ts">TypeSyntax</param>
        /// <returns>(Nullable, Non nullable TypeSyntax)</returns>
        public static (bool nullable, TypeSyntax ts) IsNullable(this TypeSyntax ts)
        {
            if (ts is NullableTypeSyntax np)
            {
                return (true, np.ElementType);
            }

            return (false, ts);
        }

        /// <summary>
        /// Parse SyntaxNode with name space and class name
        /// 解析 SyntaxNode 为命名空间和类名
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="sn">SyntaxNode</param>
        /// <returns>Name space and class name</returns>
        public static (string nameSpace, string className) ParseSyntaxNodeNames(this GeneratorExecutionContext context, SyntaxNode sn)
        {
            var symbol = context.ParseSyntaxNode<ISymbol>(sn)!;
            return (symbol.ContainingNamespace.ToDisplayString(), symbol.Name);
        }

        /// <summary>
        /// Parse SyntaxNode with name space and class name
        /// 解析 SyntaxNode 为命名空间和类名
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="td">TypeDeclarationSyntax</param>
        /// <returns>Public properties and fields</returns>
        public static IEnumerable<ParsedMember> ParseMembers(this GeneratorExecutionContext context, TypeDeclarationSyntax tds)
        {
            var items = new List<ParsedMember>();

            foreach (var member in tds.Members)
            {
                // Only public
                if (!member.HasToken(SyntaxKind.PublicKeyword))
                {
                    continue;
                }

                if (member is PropertyDeclarationSyntax p)
                {
                    // Property
                    // Symbol
                    var pSymbol = context.ParseSyntaxNode<IPropertySymbol>(p);
                    if(pSymbol != null)
                    {
                        var (pNullable, type) = p.Type.IsNullable();
                        items.Add(new ParsedMember(pSymbol, type, pSymbol.Type, pNullable));
                    }
                }
                else if (member is FieldDeclarationSyntax f)
                {
                    // Field
                    foreach (var variable in f.Declaration.Variables)
                    {
                        // Symbol
                        var fSymbol = context.ParseSyntaxNode<IFieldSymbol>(variable);
                        if (fSymbol != null)
                        {
                            var (fNullable, fType) = f.Declaration.Type.IsNullable();

                            items.Add(new ParsedMember(fSymbol, fType, fSymbol.Type, fNullable));
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            return items;
        }

        /// <summary>
        /// Is nullable type
        /// 是否为可为空类型
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Nullable or not</returns>
        public static bool IsNullable(this ITypeSymbol type)
        {
            return type.NullableAnnotation == NullableAnnotation.Annotated;
        }

        /// <summary>
        /// Get Enum type
        /// 获取枚举类型
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Enum type</returns>
        public static ITypeSymbol? GetEnumType(this ITypeSymbol type)
        {
            if(type.TypeKind == TypeKind.Enum)
            {
                return type;
            }

            // Nullabel case
            // Enum? is a struct
            if (type.TypeKind == TypeKind.Struct
                && type is INamedTypeSymbol nts
                && nts.TypeArguments.Length == 1
                && nts.TypeArguments[0].TypeKind == TypeKind.Enum)
            {
                return nts.TypeArguments[0];
            }

            return default;
        }

        /// <summary>
        /// Is Enum type
        /// 是否为枚举类型
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Enum or not</returns>
        public static bool IsEnum(this ITypeSymbol type)
        {
            return type.GetEnumType() != null;
        }

        /// <summary>
        /// Is list
        /// 是否为列表
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>List or not</returns>
        public static bool IsList(this ITypeSymbol type)
        {
            if (type.TypeKind == TypeKind.Class
                && type is INamedTypeSymbol nts
                && nts.IsGenericType
                && nts.TypeArguments.Length == 1
            )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Is dictionary
        /// 是否为字典
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>List or not</returns>
        public static bool IsDictionary(this ITypeSymbol type)
        {
            if (type.TypeKind == TypeKind.Class
                && type is INamedTypeSymbol nts
                && nts.IsGenericType
                && nts.TypeArguments.Length == 2
            )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Is simple DB type
        /// 是否为简单的数据库类型
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Simple DB type or not</returns>
        public static bool IsSimpleType(this ITypeSymbol type)
        {
            var typeName = type.Name;

            // Other case
            if (typeName == "TimeSpan")
            {
                return true;
            }

            // Try to parse with name
            return Enum.TryParse<DbType>(typeName, false, out _);
        }
    }
}
