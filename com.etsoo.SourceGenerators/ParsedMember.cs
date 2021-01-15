using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Parsed member
    /// 解析的成员
    /// </summary>
    public class ParsedMember
    {
        /// <summary>
        /// Member symbol
        /// </summary>
        public ISymbol Symbol { get; }

        /// <summary>
        /// Type syntax
        /// </summary>
        public TypeSyntax Type { get; }

        /// <summary>
        /// Type symbol
        /// </summary>
        public ITypeSymbol TypeSymbol { get; }

        /// <summary>
        /// Nullable
        /// </summary>
        public bool Nullable { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbol">Member symbol</param>
        /// <param name="type">Type syntax</param>
        /// <param name="typeSymbol">Type symbol</param>
        /// <param name="nullable">Nullable</param>
        public ParsedMember(ISymbol symbol, TypeSyntax type, ITypeSymbol typeSymbol, bool nullable)
        {
            if (nullable)
            {
                var enumType = typeSymbol.GetEnumType();
                if (enumType != null)
                {
                    // Enum
                    typeSymbol = enumType;
                }
                else if (typeSymbol is INamedTypeSymbol nts && !nts.IsGenericType)
                {
                    // Get the original non-generic definition, like string? => string, int[]? => int[]
                    typeSymbol = typeSymbol.OriginalDefinition;
                }
            }

            (Symbol, Type, TypeSymbol, Nullable) = (symbol, type, typeSymbol, nullable);
        }

        /// <summary>
        /// Deconstruct
        /// </summary>
        /// <param name="symbol">Member symbol</param>
        /// <param name="type">Type syntax</param>
        /// <param name="typeSymbol">Type symbol</param>
        /// <param name="nullable">Nullable</param>
        public void Deconstruct(out ISymbol symbol, out TypeSyntax type, out ITypeSymbol typeSymbol, out bool nullable) => 
            (symbol, type, typeSymbol, nullable) = (Symbol, Type, TypeSymbol, Nullable);
    }
}
