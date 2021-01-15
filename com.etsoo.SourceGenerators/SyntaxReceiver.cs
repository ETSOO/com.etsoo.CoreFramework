using com.etsoo.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace com.etsoo.CoreFramework.SourceGenerators
{
    /// <summary>
    /// Syntax receiver
    /// </summary>
    public class SyntaxReceiver : ISyntaxReceiver
    {
        /// <summary>
        /// Applied structs
        /// </summary>
        public List<StructDeclarationSyntax> StructCandidates { get; } = new List<StructDeclarationSyntax>();

        /// <summary>
        /// Applied classes
        /// </summary>
        public List<ClassDeclarationSyntax> ClassCandidates { get; } = new List<ClassDeclarationSyntax>();

        /// <summary>
        /// Applied records
        /// </summary>
        public List<RecordDeclarationSyntax> RecordCandidates { get; } = new List<RecordDeclarationSyntax>();

        /// <summary>
        /// Filter attribute type
        /// </summary>
        public readonly Type AttributeType;

        private readonly SyntaxKind? KeywordKind;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="attributeType">Attribute type</param>
        /// <param name="keywordKind">Keyword kind required</param>
        public SyntaxReceiver(Type attributeType, SyntaxKind? keywordKind) => (AttributeType, KeywordKind) = (attributeType, keywordKind);

        /// <summary>
        /// Visit filter
        /// </summary>
        /// <param name="syntaxNode">Syntax node</param>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            try
            {
                // Business logic to decide what we're interested in goes here
                if (syntaxNode is StructDeclarationSyntax sds
                    && (!KeywordKind.HasValue || sds.HasToken(KeywordKind.Value))
                    && sds.AttributeLists.HasAttribute(AttributeType))
                {
                    // Partial Struct
                    StructCandidates.Add(sds);
                }
                else if (syntaxNode is ClassDeclarationSyntax cds
                    && (!KeywordKind.HasValue || cds.HasToken(KeywordKind.Value))
                    && cds.AttributeLists.HasAttribute(AttributeType))
                {
                    // Partial Class
                    ClassCandidates.Add(cds);
                }
                else if (syntaxNode is RecordDeclarationSyntax rds
                    && (!KeywordKind.HasValue || rds.HasToken(KeywordKind.Value))
                    && rds.AttributeLists.HasAttribute(AttributeType))
                {
                    // Partial Record
                    RecordCandidates.Add(rds);
                }
            }
            catch
            {

            }
        }
    }
}
