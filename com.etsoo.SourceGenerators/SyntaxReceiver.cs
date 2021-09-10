using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace com.etsoo.SourceGenerators
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

        // Limited kinds
        private readonly List<SyntaxKind> Kinds = new() { SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword };

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="attributeType">Attribute type</param>
        public SyntaxReceiver(Type attributeType)
        {
            AttributeType = attributeType;
        }

        /// <summary>
        /// Visit filter
        /// </summary>
        /// <param name="syntaxNode">Syntax node</param>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            try
            {
                // Check token first
                if (!syntaxNode.HasTokens(Kinds)) return;

                // Business logic to decide what we're interested in goes here
                if (syntaxNode is RecordDeclarationSyntax rds
                    && rds.AttributeLists.HasAttribute(AttributeType))
                {
                    // Partial Record
                    RecordCandidates.Add(rds);
                }
                else if (syntaxNode is StructDeclarationSyntax sds
                    && sds.AttributeLists.HasAttribute(AttributeType))
                {
                    // Partial Struct
                    StructCandidates.Add(sds);
                }
                else if (syntaxNode is ClassDeclarationSyntax cds
                    && cds.AttributeLists.HasAttribute(AttributeType))
                {
                    // Partial Class
                    ClassCandidates.Add(cds);
                }
            }
            catch
            {

            }
        }
    }
}
