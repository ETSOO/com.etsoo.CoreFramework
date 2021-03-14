using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Utility extensions
    /// 工具扩展
    /// </summary>
    public static class UtilExtensions
    {
        private static readonly List<string> numericTypes = new() {
            "byte", 
            "sbyte",
            "short",
            "ushort",
            "int",
            "uint",
            "long",
            "ulong",
            "float",
            "double",
            "decimal"
        };

        /// <summary>
        /// Convert Pascal case name to snake case name
        /// com.etsoo.CoreFramework.Utils.StringUtil.PascalCaseToLinuxStyle
        /// </summary>
        /// <param name="name">Input name</param>
        /// <returns>Snake case name</returns>
        public static string ToSnakeCase(this string name)
        {
            return Regex.Replace(name, "([A-Z])", m => "_" + char.ToLower(m.Value[0]), RegexOptions.Compiled).TrimStart('_');
        }

        /// <summary>
        /// Convert bool to lowercase "true" or "false"
        /// </summary>
        /// <param name="input">Input bool</param>
        /// <returns>Result</returns>
        public static string ToCode(this bool input)
        {
            return input.ToString().ToLower();
        }

        /// <summary>
        /// Is numeric type
        /// </summary>
        /// <param name="type">Type name</param>
        /// <returns>Result</returns>
        public static bool IsNumericType(this string type)
        {
            return numericTypes.Contains(type);
        }

        /// <summary>
        /// Convert Type to DbType from name
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <returns>DbType</returns>
        public static string ToDbType(this string? typeName)
        {
            if (typeName == null || typeName == string.Empty)
                return "null";

            // Name not match case
            if (typeName.Equals("TimeSpan", StringComparison.OrdinalIgnoreCase))
            {
                return "DbType.Time";
            }

            // Try to parse with name
            if (Enum.TryParse<DbType>(typeName, true, out var dbType))
            {
                return "DbType." + dbType.ToString();
            }

            // Default
            return "null";
        }
    }
}
