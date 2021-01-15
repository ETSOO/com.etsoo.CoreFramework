using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace com.etsoo.Utils.Database
{
    /// <summary>
    /// SQL Server utilities
    /// SQL Server 工具
    /// </summary>
    public static class SqlServerUtil
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
            var sdr = new SqlDataRecord(new SqlMetaData(field, type));

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
    }
}
