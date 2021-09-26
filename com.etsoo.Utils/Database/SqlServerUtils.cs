using com.etsoo.Utils.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using System.Collections;
using System.Data;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// SQL Server utilities
    /// SQL Server 工具
    /// </summary>
    public static class SqlServerUtils
    {
        /// <summary>
        /// Get SqlDbType from DbType
        /// 从DbType获取SqlDbType
        /// </summary>
        /// <param name="type">DbType</param>
        /// <returns>SqlDbType</returns>
        public static SqlDbType GetSqlType(DbType type)
        {
            return type switch
            {
                DbType.VarNumeric               => SqlDbType.Decimal,
                DbType.UInt16 or DbType.SByte   => SqlDbType.SmallInt,
                DbType.UInt32                   => SqlDbType.Int,
                DbType.UInt64                   => SqlDbType.BigInt,
                _                               => new SqlParameter { DbType = type }.SqlDbType
            };
        }

        /// <summary>
        /// Get SqlDbType from type
        /// 从类型获取SqlDbType
        /// </summary>
        /// <param name="type">Data type</param>
        /// <returns>SqlDbType</returns>
        public static SqlDbType GetSqlType(Type type)
        {
            return GetSqlType(DatabaseUtils.TypeToDbType(type).GetValueOrDefault());
        }

        /// <summary>
        /// Transform list to TVP
        /// 转化列表到TVP参数列表
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="maxLength">Max length for char/byte related lists</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>TVP value</returns>
        public static object ListToTVP<T>(IEnumerable<T> list, long? maxLength = null, Func<SqlDbType, string>? tvpFunc = null)
        {
            return ListToTVP(list, DatabaseUtils.TypeToDbType(typeof(T)).GetValueOrDefault(), maxLength, tvpFunc);
        }

        /// <summary>
        /// Get type name
        /// 获取类型名称
        /// </summary>
        /// <param name="type">Sql type</param>
        /// <returns>Result</returns>
        public static string GetTypeName(SqlDbType type)
        {
            return type.ToString().ToLower();
        }

        /// <summary>
        /// Get list parts
        /// 获取列表部分
        /// </summary>
        /// <param name="type">Sql type</param>
        /// <returns>Parts</returns>
        public static string[] GetListParts(SqlDbType type)
        {
            return new[] { GetTypeName(type), "ids" };
        }

        /// <summary>
        /// Get list command
        /// 获取列表命令
        /// </summary>
        /// <param name="type">Sql type</param>
        /// <param name="builder">Builder</param>
        /// <returns>Command</returns>
        public static string GetListCommand(SqlDbType type, CommandBuilderDelegate builder)
        {
            return builder(CommandIdentifier.Type, GetListParts(type));
        }

        private static string DefaultListTvpFunc(SqlDbType type)
        {
            return string.Join('_', GetListParts(type).Prepend("et"));
        }

        /// <summary>
        /// Get dictionary parts
        /// 获取字典部分
        /// </summary>
        /// <param name="keyType">Sql type</param>
        /// <param name="valueType">Value type</param>
        /// <returns>Parts</returns>
        public static string[] GetDicParts(SqlDbType keyType, SqlDbType valueType)
        {
            // Simple case, string key, example: et_int_items
            if (IsLengthSpecifiedType(keyType))
                return new[] { GetTypeName(valueType), "items" };

            // Other cases, example: et_int_int_items
            return new[] { GetTypeName(keyType), GetTypeName(valueType), "items" };
        }

        /// <summary>
        /// Get dictionary command
        /// 获取字典命令
        /// </summary>
        /// <param name="keyType">Key type</param>
        /// <param name="type">Value type</param>
        /// <param name="builder">Builder</param>
        /// <returns>Command</returns>
        public static string GetDicCommand(SqlDbType keyType, SqlDbType valueType, CommandBuilderDelegate builder)
        {
            return builder(CommandIdentifier.Type, GetDicParts(keyType, valueType));
        }

        private static string DefaultDicTvpFunc(SqlDbType keyType, SqlDbType valueType)
        {
            return string.Join('_', GetDicParts(keyType, valueType).Prepend("et"));
        }

        /// <summary>
        /// Get Guid item parts
        /// 获取标识项目部分
        /// </summary>
        /// <returns>Parts</returns>
        public static string[] GetGuidItemParts()
        {
            return new[] { "guid", "items" };
        }

        /// <summary>
        /// Get Guid item command
        /// 获取字典命令
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <returns>Command</returns>
        public static string GetGuidItemCommand(CommandBuilderDelegate builder)
        {
            return builder(CommandIdentifier.Type, GetGuidItemParts());
        }

        private static string DefaultGuidItemTvpFunc()
        {
            return string.Join('_', GetGuidItemParts().Prepend("et"));
        }

        /// <summary>
        /// Transform list to TVP
        /// 转化列表到TVP参数列表
        /// </summary>
        /// <param name="ids">Id list</param>
        /// <param name="type">Data type</param>
        /// <param name="maxLength">Max length for char/byte related lists</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>TVP value</returns>
        public static object ListToTVP(IEnumerable ids, DbType type, long? maxLength = null, Func<SqlDbType, string>? tvpFunc = null)
        {
            var sqlType = GetSqlType(type);

            tvpFunc ??= DefaultListTvpFunc;

            return ListToIdRecords(ids, sqlType, maxLength).AsTableValuedParameter(tvpFunc(sqlType));
        }

        /// <summary>
        /// Transform list to SqlDataRecord list
        /// 转化列表到TVP参数列表
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="type">Data type</param>
        /// <param name="maxLength">Max length for char/byte related lists</param>
        /// <returns>TVP list</returns>
        public static IEnumerable<SqlDataRecord> ListToIdRecords(IEnumerable list, DbType type, long? maxLength = null)
        {
            return ListToIdRecords(list, GetSqlType(type), maxLength);
        }

        private static bool IsLengthSpecifiedType(SqlDbType type)
        {
            return type is SqlDbType.Binary or SqlDbType.VarBinary or SqlDbType.Image or SqlDbType.Text or SqlDbType.NText or SqlDbType.Char or SqlDbType.NChar or SqlDbType.VarChar or SqlDbType.NVarChar;
        }

        /// <summary>
        /// Transform list to SqlDataRecord list
        /// 转化列表到TVP参数列表
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="type">Data type</param>
        /// <param name="maxLength">Max length for char/byte related lists</param>
        /// <param name="field">TVP field name</param>
        /// <returns>TVP list</returns>
        public static IEnumerable<SqlDataRecord> ListToIdRecords(IEnumerable list, SqlDbType type, long? maxLength = null)
        {
            // SqlDataRecord definition
            var sdr = IsLengthSpecifiedType(type) ? new SqlDataRecord(new SqlMetaData("id", type, maxLength.GetValueOrDefault(50))) : new SqlDataRecord(new SqlMetaData("id", type));

            // List enumerator
            var enumerator = list.GetEnumerator();

            while (enumerator.MoveNext())
            {
                // Set value
                if (enumerator.Current == null)
                    sdr.SetDBNull(0);
                else
                    sdr.SetValue(0, enumerator.Current);

                // Yield return for the current item
                // Memory saving
                yield return sdr;
            }
        }

        /// <summary>
        /// Transform dictionary to TVP
        /// 转化字典到TVP参数列表
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyMaxLength">Char/byte key max length</param>
        /// <param name="valueMaxLength">Char/byte value max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>TVP value</returns>
        public static object DictionaryToTVP<K, V>(Dictionary<K, V> dic, long? keyMaxLength = null, long? valueMaxLength = null, Func<SqlDbType, SqlDbType, string>? tvpFunc = null)
            where K : struct
            where V : struct
        {
            var keyType = DatabaseUtils.TypeToDbType(typeof(K)).GetValueOrDefault();
            var valueType = DatabaseUtils.TypeToDbType(typeof(V)).GetValueOrDefault();
            return DictionaryToTVP(dic, keyType, valueType, keyMaxLength, valueMaxLength, tvpFunc);
        }

        /// <summary>
        /// Transform dictionary to TVP
        /// 转化字典到TVP参数列表
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyType">Key type</param>
        /// <param name="valueType">Value type</param>
        /// <param name="keyMaxLength">Char/byte key max length</param>
        /// <param name="valueMaxLength">Char/byte value max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>TVP value</returns>
        public static object DictionaryToTVP<K, V>(Dictionary<K, V> dic, DbType keyType, DbType valueType, long? keyMaxLength = null, long? valueMaxLength = null, Func<SqlDbType, SqlDbType, string>? tvpFunc = null)
            where K : struct
            where V : struct
        {
            var keySqlType = GetSqlType(keyType);
            var valueSqlType = GetSqlType(valueType);

            tvpFunc ??= DefaultDicTvpFunc;

            return DictionaryToRecords(dic, keySqlType, valueSqlType, keyMaxLength, valueMaxLength).AsTableValuedParameter(tvpFunc(keySqlType, valueSqlType));
        }

        /// <summary>
        /// Transform dictionary to SqlDataRecord list
        /// 转化字典到TVP参数列表
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyType">Key data type</param>
        /// <param name="itemType">Item data type</param>
        /// <param name="keyMaxLength">Char/byte key max length</param>
        /// <param name="valueMaxLength">Char/byte value max length</param>
        /// <returns>TVP list</returns>
        public static IEnumerable<SqlDataRecord> DictionaryToRecords(IDictionary dic, DbType keyType, DbType itemType, long? keyMaxLength = null, long? valueMaxLength = null)
        {
            return DictionaryToRecords(dic, GetSqlType(keyType), GetSqlType(itemType), keyMaxLength, valueMaxLength);
        }

        /// <summary>
        /// Transform dictionary to SqlDataRecord list
        /// 转化字典到TVP参数列表
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyType">Key data type</param>
        /// <param name="itemType">Item data type</param>
        /// <param name="keyMaxLength">Char/byte key max length</param>
        /// <param name="valueMaxLength">Char/byte value max length</param>
        /// <returns>TVP list</returns>
        public static IEnumerable<SqlDataRecord> DictionaryToRecords(IDictionary dic, SqlDbType keyType, SqlDbType itemType, long? keyMaxLength = null, long? valueMaxLength = null)
        {
            // SqlDataRecord definition
            var keyMeta = IsLengthSpecifiedType(keyType) ? new SqlMetaData("key", keyType, keyMaxLength.GetValueOrDefault(40)) : new SqlMetaData("key", keyType);
            var itemMeta = IsLengthSpecifiedType(itemType) ? new SqlMetaData("item", itemType, valueMaxLength.GetValueOrDefault(128)) : new SqlMetaData("item", itemType);
            var sdr = new SqlDataRecord(keyMeta, itemMeta);

            // List enumerator
            var enumerator = dic.GetEnumerator();

            while (enumerator.MoveNext())
            {
                // Ignore null values
                if (enumerator.Key == null || enumerator.Value == null)
                    yield break;

                // Set value
                sdr.SetValue(0, enumerator.Key);
                sdr.SetValue(1, enumerator.Value);

                // Yield return for the current item
                // Memory saving
                yield return sdr;
            }
        }

        /// <summary>
        /// Guid items to records
        /// 转化Guid项目到TVP参数列表
        /// </summary>
        /// <param name="items">Items</param>
        /// <returns>Result</returns>
        public static IEnumerable<SqlDataRecord> GuidItemToRecords(IEnumerable<GuidItem> items, long? maxLength = null)
        {
            // SqlDataRecord definition
            var sdr = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.UniqueIdentifier), new SqlMetaData("Item", SqlDbType.VarChar, maxLength ?? 128));

            foreach (var item in items)
            {
                // Set value
                sdr.SetValue(0, item.Id);
                sdr.SetValue(1, item.Item);

                // Yield return for the current item
                // Memory saving
                yield return sdr;
            }
        }

        /// <summary>
        /// Guid items to parameters
        /// 转换Guid项目为TVP参数
        /// </summary>
        /// <param name="items">Items</param>
        /// <param name="maxLength">Item max length</param>
        /// <param name="tvpFunc">TVP building function</param>
        /// <returns>Result</returns>
        public static object GuidItemsToParameter(IEnumerable<GuidItem> items, long? maxLength = null, Func<string>? tvpFunc = null)
        {
            tvpFunc ??= DefaultGuidItemTvpFunc;
            return GuidItemToRecords(items, maxLength).AsTableValuedParameter(tvpFunc());
        }

        /// <summary>
        /// Convert to SQL Server DateTime same accuracy
        /// https://stackoverflow.com/questions/715432/why-is-sql-server-losing-a-millisecond
        /// 转换为SQL Server DateTime 一样的精度
        /// </summary>
        /// <param name="input">Datetime</param>
        /// <returns>Result</returns>
        public static DateTime ToSqlDateTime(this DateTime input)
        {
            // First to 0.001 second
            input = input.AddTicks(-input.Ticks % 10000);

            // To 0.000, 0.003, 0.007
            var m = input.Millisecond % 10;
            if (m == 1 || m == 4 || m == 8)
                input = input.AddMilliseconds(-1);
            else if (m == 2 || m == 6 || m == 9)
                input = input.AddMilliseconds(1);
            else if (m == 5)
                input = input.AddMilliseconds(2);

            // Return
            return input;
        }
    }
}
