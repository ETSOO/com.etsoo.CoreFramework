using System.Data;
using System.Text.RegularExpressions;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Database util
    /// 数据库工具
    /// </summary>
    public static class DatabaseUtils
    {
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
