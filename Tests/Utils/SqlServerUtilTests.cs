﻿using com.etsoo.Database;
using NUnit.Framework;
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
            var result = SqlServerUtils.GetSqlType(type);

            // Assert
            Assert.That(result, Is.EqualTo(sqlType), $"{type} is not converted with {sqlType}");
        }

        private static IEnumerable<TestCaseData> ToSqlDateTimeBulkTestData
        {
            get
            {
                yield return new TestCaseData("2009-04-03 15:41:27.370", 370);
                yield return new TestCaseData("2009-04-03 15:41:27.371", 370);
                yield return new TestCaseData("2009-04-03 15:41:27.372", 373);
                yield return new TestCaseData("2009-04-03 15:41:27.373", 373);
                yield return new TestCaseData("2009-04-03 15:41:27.374", 373);
                yield return new TestCaseData("2009-04-03 15:41:27.375", 377);
                yield return new TestCaseData("2009-04-03 15:41:27.376", 377);
                yield return new TestCaseData("2009-04-03 15:41:27.377", 377);
                yield return new TestCaseData("2009-04-03 15:41:27.378", 377);
                yield return new TestCaseData("2009-04-03 15:41:27.379", 380);
            }
        }

        [Test, TestCaseSource(nameof(ToSqlDateTimeBulkTestData))]
        public void ToSqlDateTime_All_Test(string input, int milliseconds)
        {
            // Arrange & act
            var date = DateTime.Parse(input).ToSqlDateTime();

            // Assert
            Assert.That(milliseconds, Is.EqualTo(date.Millisecond), "No match for milliseconds");
        }
    }
}
