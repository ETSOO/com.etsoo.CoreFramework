using com.etsoo.Database;
using com.etsoo.Database.Converters;
using System.Data;
using System.Globalization;

namespace Tests.Utils
{
    [TestClass]
    public class DatabaseUtilTests
    {
        private enum TestEnum
        {
            Monday,
            Tuesday,
            Sunday
        }

        private static IEnumerable<object[]> TypeToDbTypeBulkTestData
        {
            get
            {
                yield return new object[] { typeof(byte), DbType.Byte };
                yield return new object[] { typeof(sbyte), DbType.SByte };
                yield return new object[] { typeof(short), DbType.Int16 };
                yield return new object[] { typeof(ushort), DbType.UInt16 };
                yield return new object[] { typeof(bool?), DbType.Boolean };
                yield return new object[] { typeof(bool), DbType.Boolean };
                yield return new object[] { typeof(TimeSpan), DbType.Time };
                yield return new object[] { typeof(float), DbType.Single };
                yield return new object[] { typeof(TestEnum), null };
            }
        }

        [TestMethod]
        [DynamicData(nameof(TypeToDbTypeBulkTestData))]
        public void TypeToDbType_All_Test(Type type, DbType? dbType)
        {
            // Arrange & act
            var result = DatabaseUtils.TypeToDbType(type);

            // Assert
            Assert.AreEqual(dbType, result, $"{type.Name} is not converted with {dbType}");
        }

        private static IEnumerable<object[]> IsAnsiBulkTestData
        {
            get
            {
                yield return new object[] { "abc123", true };
                yield return new object[] { "abc,;-$%", true };
                yield return new object[] { "亿速ab", false };
            }
        }

        [TestMethod]
        [DynamicData(nameof(IsAnsiBulkTestData))]
        public void IsAnsi_Test(string input, bool isAnsi)
        {
            Assert.AreEqual(isAnsi, input.IsAnsi());
        }

        [TestMethod]
        public void ListItemsToJsonString_EmptyList_ReturnsEmptyArray()
        {
            // Arrange
            List<object> emptyList = [];
            DbType type = DbType.String;

            // Act
            string result = DatabaseUtils.ListItemsToJsonString(emptyList, type);

            // Assert
            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public void ListItemsToJsonString_SingleItem_ReturnsValidJsonArray()
        {
            // Arrange
            List<int> singleItemList = [42];
            DbType type = DbType.Int32;

            // Act
            var result = DatabaseUtils.ListItemsToJsonString(singleItemList, type);

            // Assert
            Assert.AreEqual("[42]", result);
        }

        [TestMethod]
        public void ListItemsToJsonString_MultipleItems_ReturnsValidJsonArray()
        {
            // Arrange
            List<string> stringList = ["apple", "orange", "banana"];
            DbType type = DbType.String;

            // Act
            var result = DatabaseUtils.ListItemsToJsonString(stringList, type);

            // Assert
            Assert.AreEqual("[\"apple\",\"orange\",\"banana\"]", result);
        }

        [TestMethod]
        public void DictionaryToJsonString_NullDictionary_ReturnsEmptyArray()
        {
            // Arrange
            Dictionary<int, Guid> emptyDictionary = [];
            DbType keyType = DbType.Int32;
            DbType valueType = DbType.Guid;

            // Act
            var result = DatabaseUtils.DictionaryToJsonString(emptyDictionary, keyType, valueType);

            // Assert
            Assert.AreEqual("[]", result);
        }

        [TestMethod]
        public void DictionaryToJsonString_IntegerStringDictionary_ReturnsValidJsonArray()
        {
            // Arrange
            Dictionary<int, string> intStringDictionary = new()
            {
                { 1, "One" },
                { 2, "Two" },
                { 3, "Three" }
            };

            DbType keyType = DbType.Int32;
            DbType valueType = DbType.String;

            // Act
            string result = DatabaseUtils.DictionaryToJsonString(intStringDictionary, keyType, valueType);

            // Assert
            Assert.AreEqual("[{\"key\":1,\"value\":\"One\"},{\"key\":2,\"value\":\"Two\"},{\"key\":3,\"value\":\"Three\"}]", result);
        }

        [TestMethod]
        public void GetTimeZone_Test()
        {
            // Arrange set culture for test
            CultureInfo.CurrentCulture = new CultureInfo("zh-CN");
            CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");

            // Correct
            var tz = TimeZoneUtils.GetTimeZone("Pacific/Auckland");
            Assert.AreEqual("New Zealand Standard Time", tz.Id);

            // Wrong
            tz = TimeZoneUtils.GetTimeZone("China Time");
            Assert.AreEqual(TimeZoneInfo.Local, tz);
        }
    }
}
