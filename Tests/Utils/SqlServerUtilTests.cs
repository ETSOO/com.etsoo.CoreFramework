using com.etsoo.Database;
using System.Data;

namespace Tests.Utils
{
    [TestClass]
    public class SqlServerUtilTests
    {
        private static IEnumerable<object[]> DbTypeToSqlBulkTestData
        {
            get
            {
                yield return new object[] { DbType.Byte, SqlDbType.TinyInt };
                yield return new object[] { DbType.AnsiString, SqlDbType.VarChar };
                yield return new object[] { DbType.AnsiStringFixedLength, SqlDbType.Char };
                yield return new object[] { DbType.String, SqlDbType.NVarChar };
                yield return new object[] { DbType.StringFixedLength, SqlDbType.NChar };
                yield return new object[] { DbType.Byte, SqlDbType.TinyInt };
                yield return new object[] { DbType.Currency, SqlDbType.Money };
                yield return new object[] { DbType.VarNumeric, SqlDbType.Decimal };
                yield return new object[] { DbType.SByte, SqlDbType.SmallInt };
                yield return new object[] { DbType.Object, SqlDbType.Variant };
                yield return new object[] { DbType.Int16, SqlDbType.SmallInt };
                yield return new object[] { DbType.Int64, SqlDbType.BigInt };
                yield return new object[] { DbType.UInt64, SqlDbType.BigInt };
                yield return new object[] { DbType.Date, SqlDbType.Date };
                yield return new object[] { DbType.Time, SqlDbType.Time };
                yield return new object[] { DbType.Guid, SqlDbType.UniqueIdentifier };
            }
        }

        [TestMethod]
        [DynamicData(nameof(DbTypeToSqlBulkTestData))]
        public void DbTypeToSql_All_Test(DbType type, SqlDbType sqlType)
        {
            // Arrange & act
            var result = SqlServerUtils.GetSqlType(type);

            // Assert
            Assert.AreEqual(sqlType, result, $"{type} is not converted with {sqlType}");
        }

        private static IEnumerable<object[]> ToSqlDateTimeBulkTestData
        {
            get
            {
                yield return new object[] { "2009-04-03 15:41:27.370", 370 };
                yield return new object[] { "2009-04-03 15:41:27.371", 370 };
                yield return new object[] { "2009-04-03 15:41:27.372", 373 };
                yield return new object[] { "2009-04-03 15:41:27.373", 373 };
                yield return new object[] { "2009-04-03 15:41:27.374", 373 };
                yield return new object[] { "2009-04-03 15:41:27.375", 377 };
                yield return new object[] { "2009-04-03 15:41:27.376", 377 };
                yield return new object[] { "2009-04-03 15:41:27.377", 377 };
                yield return new object[] { "2009-04-03 15:41:27.378", 377 };
                yield return new object[] { "2009-04-03 15:41:27.379", 380 };
            }
        }

        [TestMethod]
        [DynamicData(nameof(ToSqlDateTimeBulkTestData))]
        public void ToSqlDateTime_All_Test(string input, int milliseconds)
        {
            // Arrange & act
            var date = DateTime.Parse(input).ToSqlDateTime();

            // Assert
            Assert.AreEqual(milliseconds, date.Millisecond, "No match for milliseconds");
        }
    }
}
