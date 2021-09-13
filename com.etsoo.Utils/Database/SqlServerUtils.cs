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
        /// Convert DbType to SqlDbType
        /// 转化 DbType 为 SqlDbType
        /// </summary>
        /// <param name="type">DbType</param>
        /// <returns>SqlDbType</returns>
        public static SqlDbType DbTypeToSql(DbType type)
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
        /// Transform list to TVP
        /// 转化列表到TVP参数列表
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="field">TVP field name</param>
        /// <returns>TVP value</returns>
        public static object ListToTVP<T>(IEnumerable<T> ids, string field = "id")
        {
            // Type => SqlDbType, like Int
            var type = DbTypeToSql(DatabaseUtils.TypeToDbType(typeof(T)).GetValueOrDefault());

            // Parameter UDT name
            var udt = $"et_{type.ToString().ToLower()}_ids";

            return ListToIdRecords(ids, type, field).AsTableValuedParameter(udt);
        }

        /// <summary>
        /// Transform list to SqlDataRecord list
        /// 转化列表到TVP参数列表
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="type">Data type</param>
        /// <param name="field">TVP field name</param>
        /// <returns>TVP list</returns>
        public static IEnumerable<SqlDataRecord> ListToIdRecords(IEnumerable list, DbType type, string field = "id")
        {
            return ListToIdRecords(list, DbTypeToSql(type), field);
        }

        /// <summary>
        /// Transform list to SqlDataRecord list
        /// 转化列表到TVP参数列表
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="type">Data type</param>
        /// <param name="field">TVP field name</param>
        /// <returns>TVP list</returns>
        public static IEnumerable<SqlDataRecord> ListToIdRecords(IEnumerable list, SqlDbType type, string field = "id")
        {
            // SqlDataRecord definition
            SqlDataRecord sdr;
            if (type is SqlDbType.Char or SqlDbType.NChar or SqlDbType.VarChar or SqlDbType.NVarChar)
                sdr = new SqlDataRecord(new SqlMetaData(field, type, 50)); // 50 is the maximum length
            else
                sdr = new SqlDataRecord(new SqlMetaData(field, type));

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
        /// Transform dictionary to SqlDataRecord list
        /// 转化字典到TVP参数列表
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyType">Key data type</param>
        /// <param name="itemType">Item data type</param>
        /// <returns>TVP list</returns>
        public static IEnumerable<SqlDataRecord> DictionaryToRecords(IDictionary dic, DbType keyType, DbType itemType)
        {
            return DictionaryToRecords(dic, DbTypeToSql(keyType), DbTypeToSql(itemType));
        }

        /// <summary>
        /// Transform dictionary to SqlDataRecord list
        /// 转化字典到TVP参数列表
        /// </summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="keyType">Key data type</param>
        /// <param name="itemType">Item data type</param>
        /// <returns>TVP list</returns>
        public static IEnumerable<SqlDataRecord> DictionaryToRecords(IDictionary dic, SqlDbType keyType, SqlDbType itemType)
        {
            // SqlDataRecord definition
            var sdr = new SqlDataRecord(new SqlMetaData("key", keyType), new SqlMetaData("item", itemType));

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
