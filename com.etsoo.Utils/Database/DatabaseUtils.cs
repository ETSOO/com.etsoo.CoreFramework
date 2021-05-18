using System;
using System.Data;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// Database util
    /// 数据库工具
    /// </summary>
    public static class DatabaseUtils
    {
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
