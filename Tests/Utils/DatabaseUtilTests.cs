using com.etsoo.Database;
using NUnit.Framework;
using System.Data;

namespace Tests.Utils
{
    [TestFixture]
    public class DatabaseUtilTests
    {
        private enum TestEnum
        {
            Monday,
            Tuesday,
            Sunday
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

        private static IEnumerable<TestCaseData> IsAnsiBulkTestData
        {
            get
            {
                yield return new TestCaseData("abc123", true);
                yield return new TestCaseData("abc,;-$%", true);
                yield return new TestCaseData("亿速ab", false);
            }
        }

        [Test, TestCaseSource(nameof(IsAnsiBulkTestData))]
        public void IsAnsi_Test(string input, bool isAnsi)
        {
            Assert.AreEqual(isAnsi, input.IsAnsi());
        }

        [Test]
        public void JoinParametersTests()
        {
            var result = DatabaseUtils.JoinParameters(nameof(TestEnum.Monday), nameof(TestEnum.Tuesday), nameof(TestEnum.Sunday));
            Assert.AreEqual("@Monday, @Tuesday, @Sunday", result);
        }
    }
}
