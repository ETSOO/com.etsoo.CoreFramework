using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Utility extensions
    /// 工具扩展
    /// </summary>
    internal static class UtilExtensions
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
        /// Database escape identifier
        /// 数据库转义标识符
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="database">Database name</param>
        /// <returns>Result</returns>
        public static string DbEscape(this string name, DatabaseName database)
        {
            // Multiple tables, follow SQL Server escape style
            if (name.Contains(" JOIN "))
            {
                return database switch
                {
                    DatabaseName.PostgreSQL or DatabaseName.SQLite => name.Replace("[", "\\\"").Replace("]", "\\\""),
                    _ => name
                };
            }

            return database switch
            {
                DatabaseName.PostgreSQL or DatabaseName.SQLite => $"\\\"{name}\\\"",
                DatabaseName.SQLServer => $"[{name.Replace("[", "[[").Replace("]", "]]")}]",
                _ => name
            };
        }

        /// <summary>
        /// Setup conditions
        /// 设置条件
        /// </summary>
        /// <param name="db">Database name</param>
        /// <returns>Result</returns>
        public static IEnumerable<DatabaseName> GetAllNames(this DatabaseName db)
        {
            foreach (DatabaseName item in Enum.GetValues(typeof(DatabaseName)))
            {
                if (db.HasFlag(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Setup conditions
        /// 设置条件
        /// </summary>
        /// <param name="db">Database name</param>
        /// <returns>Result</returns>
        public static Dictionary<DatabaseName, List<string>> SetupConditions(this DatabaseName db)
        {
            var conditions = new Dictionary<DatabaseName, List<string>>();

            foreach (var item in db.GetAllNames())
            {
                conditions[item] = new List<string>();
            }

            return conditions;
        }

        /// <summary>
        /// Setup JSON results
        /// 设置JSON结果
        /// </summary>
        /// <param name="db">Database name</param>
        /// <returns>Result</returns>
        public static Dictionary<DatabaseName, List<(string, string, string)>> SetupJsonResults(this DatabaseName db)
        {
            var conditions = new Dictionary<DatabaseName, List<(string, string, string)>>();

            foreach (var item in db.GetAllNames())
            {
                conditions[item] = new List<(string, string, string)>();
            }

            return conditions;
        }

        /// <summary>
        /// To select fields
        /// 转化为查询字段
        /// </summary>
        /// <param name="source">Source fields</param>
        /// <returns>Result</returns>
        public static string ToSelectFields(this List<(string, string, string)> source)
        {
            var items = source.Select(s =>
            {
                if (s.Item1.Equals(s.Item2)) return s.Item1;
                else return $"{s.Item1} AS {s.Item2}";
            });

            return string.Join(", ", items);
        }

        /// <summary>
        /// To JSON select fields
        /// 转化为JSON查询字段
        /// </summary>
        /// <param name="source">Source fields</param>
        /// <param name="db">Database type</param>
        /// <param name="isObject">Is object</param>
        /// <returns>Result</returns>
        public static string ToJsonSelectFields(this List<(string, string, string)> source, DatabaseName db, bool isObject)
        {
            var items = source.Select(s =>
            {
                if (db == DatabaseName.SQLServer)
                {
                    return s.Item2.Equals(s.Item3) ? s.Item3 : $"{s.Item2} AS {s.Item3}";
                }
                else
                {
                    return $"'{s.Item2}', {s.Item3}";
                }
            });

            var fields = string.Join(", ", items);

            // SQL Server
            if (db == DatabaseName.SQLServer)
            {
                return fields;
            }

            var sql = db switch
            {
                DatabaseName.PostgreSQL => $"json_build_object({fields})",
                DatabaseName.SQLite => $"json_object({fields})",
                _ => fields
            };

            if (!isObject)
            {
                sql = db switch
                {
                    DatabaseName.PostgreSQL => $"json_agg({sql})",
                    DatabaseName.SQLite => $"json_group_array({sql})",
                    _ => sql
                };
            }

            return sql;
        }

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
        /// Convert int? to "null" or "1"
        /// </summary>
        /// <param name="input">Input int</param>
        /// <returns>Result</returns>
        public static string ToIntCode(this int? input)
        {
            if (!input.HasValue)
                return "null";

            return input.ToString();
        }

        /// <summary>
        /// To specified case
        /// 转化为指定命名规则格式
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="namingPolicy">Naming policy</param>
        /// <returns>Result</returns>
        public static string ToCase(this string input, NamingPolicy? namingPolicy)
        {
            if (namingPolicy == null || namingPolicy == NamingPolicy.PascalCase)
                return input;

            return namingPolicy switch
            {
                NamingPolicy.SnakeCase => input.ToSnakeCase(),
                NamingPolicy.CamelCase => input.Substring(0, 1).ToLower() + input.Substring(1),
                _ => input
            };
        }

        /// <summary>
        /// Convert bool? to lowercase "null", "true" or "false"
        /// </summary>
        /// <param name="input">Input bool</param>
        /// <returns>Result</returns>
        public static string ToCode(this bool? input)
        {
            if (!input.HasValue)
                return "null";

            return input.Value.ToCode();
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

        /// <summary>
        /// To query sign
        /// 转化为查询符号
        /// </summary>
        /// <param name="sign">Query sign</param>
        /// <returns>Result</returns>
        public static string ToQuerySign(this SqlQuerySign sign)
        {
            return sign switch
            {
                SqlQuerySign.NotEqual => "<>",
                SqlQuerySign.Greater => ">",
                SqlQuerySign.GreaterOrEqual => ">=",
                SqlQuerySign.Less => "<",
                SqlQuerySign.LessOrEqual => "<=",
                SqlQuerySign.Like => "LIKE",
                SqlQuerySign.NotLike => "NOT LIKE",
                _ => "="
            };
        }

        /// <summary>
        /// To value code
        /// 转化为值代码
        /// </summary>
        /// <param name="valueCode">Value code</param>
        /// <param name="field">Current object field</param>
        /// <returns>Result</returns>
        public static string? ToValueCode(this string? valueCode, string field)
        {
            if (valueCode == null) return valueCode;

            if (valueCode == "{LIKE}")
            {
                return $"\"%\" + {field} + \"%\"";
            }
            else if (valueCode == "{LIKESTART}")
            {
                return $"\"%\" + {field}";
            }
            else if (valueCode == "{LIKEEND}")
            {
                return $"{field} + \"%\"";
            }
            else
            {
                return valueCode.Replace("{F}", field);
            }
        }
    }
}
