using com.etsoo.Utils.Database;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace Tests.Utils
{
    [TestFixture]
    public class DatabaseUtilTests
    {
        private enum TestEnum
        {
            Monday
        }

        private static IEnumerable<TestCaseData> TypeToDbTypeBulkTestData
        {
            get
            {
                yield return new TestCaseData(typeof(byte), DbType.Byte);
                yield return new TestCaseData(typeof(sbyte), DbType.SByte);
                yield return new TestCaseData(typeof(short), DbType.Int16);
                yield return new TestCaseData(typeof(ushort), DbType.UInt16);
                yield return new TestCaseData(typeof(bool?), DbType.Boolean);
                yield return new TestCaseData(typeof(bool), DbType.Boolean);
                yield return new TestCaseData(typeof(TimeSpan), DbType.Time);
                yield return new TestCaseData(typeof(float), DbType.Single);
                yield return new TestCaseData(typeof(TestEnum), null);
            }
        }

        [Test, TestCaseSource(nameof(TypeToDbTypeBulkTestData))]
        public void TypeToDbType_All_Test(Type type, DbType? dbType)
        {
            // Arrange & act
            var result = DatabaseUtils.TypeToDbType(type);

            // Assert
            Assert.AreEqual(dbType, result, $"{type.Name} is not converted with {dbType}");
        }
    }
}
