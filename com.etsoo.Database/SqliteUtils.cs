using System.Text.RegularExpressions;

namespace com.etsoo.Database
{
    /// <summary>
    /// Sqlite utils
    /// Sqlite 工具类
    /// </summary>
    public static class SqliteUtils
    {
        /// <summary>
        /// To JSON command
        /// 转换为JSON命令
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="withoutArrayWrapper">Without array wrapper, like SQL Server 'FOR JSON PATH, WITHOUT_ARRAY_WRAPPER'</param>
        /// <returns>Command</returns>
        public static string ToJsonCommand(string fields, bool withoutArrayWrapper = false)
        {
            // (?<!) = Negative lookbehind, no '(' found before ')'
            var regex = new Regex(@"(?<!\([^\)]+)\s*,\s*", RegexOptions.Compiled | RegexOptions.Multiline);
            var regexItem = new Regex(@"(\s+AS\s+|\.)*([a-zA-Z_$][0-9a-zA-Z_$]*)$", RegexOptions.Compiled);

            var items = regex.Split(fields);
            var r = new List<string>();
            foreach (var item in items)
            {
                // Json fieldname
                var m = regexItem.Match(item);
                var v = m.Success && m.Groups.Count == 3 ? m.Groups[2].Value : "illegal";
                r.Add($"'{v}'");

                // Query
                if (m.Success && m.Value.Contains(" AS ", StringComparison.OrdinalIgnoreCase))
                {
                    var newItem = item.Replace(m.Value, string.Empty);
                    r.Add(newItem);
                }
                else
                {
                    r.Add(item);
                }
            }

            var command = $"json_object({string.Join(", ", r)})";

            if (withoutArrayWrapper)
                return command;
            else
                return $"json_group_array({command})";
        }
    }
}