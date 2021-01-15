using System.Text.RegularExpressions;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Utility extensions
    /// 工具扩展
    /// </summary>
    public static class UtilExtensions
    {
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
        /// <returns>result</returns>
        public static string ToCode(this bool input)
        {
            return input.ToString().ToLower();
        }
    }
}
