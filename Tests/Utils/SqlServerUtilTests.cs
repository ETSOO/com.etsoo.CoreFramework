using com.etsoo.Utils.Database;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;

namespace Tests.Utils
{
    [TestFixture]
    public class SqlServerUtilTests
    {
        private static IEnumerable<TestCaseData> DbTypeToSqlBulkTestData
        {
            get
            {
                yield return new TestCaseData(DbType.Byte, SqlDbType.TinyInt);
                yield return new TestCaseData(DbType.AnsiString, SqlDbType.VarChar);
                yield return new TestCaseData(DbType.AnsiStringFixedLength, SqlDbType.Char);
                yield return new TestCaseData(DbType.String, SqlDbType.NVarChar);
                yield return new TestCaseData(DbType.StringFixedLength, SqlDbType.NChar);
                yield return new TestCaseData(DbType.Byte, SqlDbType.TinyInt);
                yield return new TestCaseData(DbType.Currency, SqlDbType.Money);
                yield return new TestCaseData(DbType.VarNumeric, SqlDbType.Decimal);
                yield return new TestCaseData(DbType.SByte, SqlDbType.SmallInt);
                yield return new TestCaseData(DbType.Object, SqlDbType.Variant);
                yield return new TestCaseData(DbType.Int16, SqlDbType.SmallInt);
                yield return new TestCaseData(DbType.Int64, SqlDbType.BigInt);
                yield return new TestCaseData(DbType.UInt64, SqlDbType.BigInt);
                yield return new TestCaseData(DbType.Date, SqlDbType.Date);
                yield return new TestCaseData(DbType.Time, SqlDbType.Time);
                yield return new TestCaseData(DbType.Guid, SqlDbType.UniqueIdentifier);
            }
        }

        [Test, TestCaseSource(nameof(DbTypeToSqlBulkTestData))]
        public void DbTypeToSql_All_Test(DbType type, SqlDbType sqlType)
        {
            // Arrange & act
            var result = SqlServerUtils.DbTypeToSql(type);

            // Assert
            Assert.IsTrue(result == sqlType, $"{type} is not converted with {sqlType}");
        }
    }
}
