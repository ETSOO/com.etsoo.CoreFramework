using Dapper;
using System.Data;
using System.Text.RegularExpressions;

namespace com.etsoo.Database
{
    /// <summary>
    /// Database util
    /// 数据库工具
    /// </summary>
    public static class DatabaseUtils
    {
        /// <summary>
        /// Is the string only ansii characters
        /// 字符串是唯一的ASCII字符吗
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        public static bool IsAnsi(this string input)
        {
            return !input.Any(c => c > 127);
        }

        /// <summary>
        /// To Dapper DbString
        /// 转化为 Dapper DbString
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="isAnsi">Is non-unicode</param>
        /// <param name="length">Length</param>
        /// <param name="fixedLength">Fixed length</param>
        /// <returns>DbString</returns>
        public static DbString? ToDbString(this string input, bool? isAnsi = null, int? length = null, bool? fixedLength = null)
        {
            // Remove unnecessary empty
            input = input.Trim();
            if (input == string.Empty) return null;

            return input.ToDbStringSafe(isAnsi, length, fixedLength);
        }

        /// <summary>
        /// To Dapper DbString without empty by any chance.
        /// 确保没有任何空白的转化为 Dapper DbString
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="isAnsi">Is non-unicode</param>
        /// <param name="length">Length</param>
        /// <param name="fixedLength">Fixed length</param>
        /// <returns>DbString</returns>
        public static DbString ToDbStringSafe(this string input, bool? isAnsi = null, int? length = null, bool? fixedLength = null)
        {
            return new DbString { IsAnsi = isAnsi ?? input.IsAnsi(), IsFixedLength = fixedLength.GetValueOrDefault(), Value = input, Length = length ?? input.Length };
        }

        /// <summary>
        /// Format parameters to Dapper parameters
        /// 将参数格式化为 Dapper 参数
        /// </summary>
        /// <param name="parameters">Dynamic parameters</param>
        /// <returns>Result</returns>
        public static DynamicParameters? FormatParameters(object? parameters)
        {
            if (parameters == null) return null;

            if (parameters is DynamicParameters dp)
            {
                return dp;
            }

            if (parameters is IAutoParameters ap)
            {
                return ap.AsParameters();
            }

            return new DynamicParameters(parameters);
        }

        /// <summary>
        /// Determine whether it is a safe SQL statement in order to avoid SQL injection
        /// 判断是否为安全SQL语句，防止注水攻击
        /// <remarks>确保不包括执行命令、换行符、分号(;)、米字符(*)、注释符(--)、引号('")、等于号(=)、系统命令(sp_, xp_)</remarks>
        /// </summary>
        /// <param name="input">要验证的SQL语句</param>
        /// <param name="maxLength">最大长度，默认值为1024</param>
        /// <returns>是否安全</returns>
        public static bool IsSafeSQLPart(string? input, int maxLength = 1024)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > maxLength) return false;

            if (!Regex.IsMatch(input, @"(\n|\r\n?|;|\*|--|'|""|=|sp_|xp_)"))
            {
                if (!Regex.IsMatch(input, @"(^|\s+)(exec|execute|select|insert|update|delete|union|join|create|alter|drop|rename|truncate|backup|restore)\s", RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Convert Type to DbType
        /// 转化 Type 到 DbType
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>DbType</returns>
        public static DbType? TypeToDbType(Type type)
        {
            // Considerate nullable type
            var nullableType = Nullable.GetUnderlyingType(type) ?? type;

            // Return
            return TypeToDbType(nullableType.Name);
        }

        /// <summary>
        /// Convert Type to DbType from name
        /// 从类型名称转化 Type 到 DbType
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <returns>DbType</returns>
        public static DbType? TypeToDbType(string typeName)
        {
            // Name not match case
            if (typeName.Equals("TimeSpan", StringComparison.OrdinalIgnoreCase))
            {
                return DbType.Time;
            }

            // Try to parse with name
            if (Enum.TryParse<DbType>(typeName, true, out var dbType))
            {
                return dbType;
            }

            // Default
            return null;
        }
    }
}
