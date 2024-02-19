using Microsoft.CodeAnalysis;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Parsed member
    /// 解析的成员
    /// </summary>
    internal class ParsedMember
    {
        /// <summary>
        /// Member symbol
        /// </summary>
        public ISymbol Symbol { get; }

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
        /// <param name="typeSymbol">Type symbol</param>
        /// <param name="nullable">Nullable</param>
        public ParsedMember(ISymbol symbol, ITypeSymbol typeSymbol)
        {
            var nullable = typeSymbol.IsNullable();
            if (nullable)
            {
                var enumType = typeSymbol.GetEnumType();
                if (enumType != null)
                {
                    // Enum
                    typeSymbol = enumType;
                }
                else if (typeSymbol is INamedTypeSymbol nts)
                {
                    if (!nts.IsGenericType)
                    {
                        // Get the original non-generic definition, like string? => string, int[]? => int[]
                        typeSymbol = typeSymbol.OriginalDefinition;
                    }
                    else if (nts.TypeKind == TypeKind.Struct && nts.TypeArguments.Length == 1)
                    {
                        // bool? => bool
                        typeSymbol = nts.TypeArguments[0];
                    }
                }
            }

            (Symbol, TypeSymbol, Nullable) = (symbol, typeSymbol, nullable);
        }

        /// <summary>
        /// Deconstruct
        /// </summary>
        /// <param name="symbol">Member symbol</param>
        /// <param name="type">Type syntax</param>
        /// <param name="typeSymbol">Type symbol</param>
        /// <param name="nullable">Nullable</param>
        public void Deconstruct(out ISymbol symbol, out ITypeSymbol typeSymbol, out bool nullable) =>
            (symbol, typeSymbol, nullable) = (Symbol, TypeSymbol, Nullable);
    }
}
