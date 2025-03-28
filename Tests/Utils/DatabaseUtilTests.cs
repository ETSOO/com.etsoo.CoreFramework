﻿using com.etsoo.Database;
using com.etsoo.Database.Converters;
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
            Assert.That(result, Is.EqualTo(dbType), $"{type.Name} is not converted with {dbType}");
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
            Assert.That(input.IsAnsi(), Is.EqualTo(isAnsi));
        }

        [Test]
        public void ListItemsToJsonString_EmptyList_ReturnsEmptyArray()
        {
            // Arrange
            List<object> emptyList = [];
            DbType type = DbType.String;

            // Act
            string result = DatabaseUtils.ListItemsToJsonString(emptyList, type);

            // Assert
            Assert.That(result, Is.EqualTo("[]"));
        }

        [Test]
        public void ListItemsToJsonString_SingleItem_ReturnsValidJsonArray()
        {
            // Arrange
            List<int> singleItemList = [42];
            DbType type = DbType.Int32;

            // Act
            var result = DatabaseUtils.ListItemsToJsonString(singleItemList, type);

            // Assert
            Assert.That(result, Is.EqualTo("[42]"));
        }

        [Test]
        public void ListItemsToJsonString_MultipleItems_ReturnsValidJsonArray()
        {
            // Arrange
            List<string> stringList = ["apple", "orange", "banana"];
            DbType type = DbType.String;

            // Act
            var result = DatabaseUtils.ListItemsToJsonString(stringList, type);

            // Assert
            Assert.That(result, Is.EqualTo("[\"apple\",\"orange\",\"banana\"]"));
        }

        [Test]
        public void DictionaryToJsonString_NullDictionary_ReturnsEmptyArray()
        {
            // Arrange
            Dictionary<int, Guid> emptyDictionary = [];
            DbType keyType = DbType.Int32;
            DbType valueType = DbType.Guid;

            // Act
            var result = DatabaseUtils.DictionaryToJsonString(emptyDictionary, keyType, valueType);

            // Assert
            Assert.That(result, Is.EqualTo("[]"));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo("[{\"key\":1,\"value\":\"One\"},{\"key\":2,\"value\":\"Two\"},{\"key\":3,\"value\":\"Three\"}]"));
        }

        [Test]
        [SetCulture("zh-CN")]
        [SetUICulture("zh-CN")]
        public void GetTimeZone_Test()
        {
            // Correct
            var tz = TimeZoneUtils.GetTimeZone("新西兰标准时间");

            Assert.That(tz.Id, Is.EqualTo("New Zealand Standard Time"));

            // Wrong
            tz = TimeZoneUtils.GetTimeZone("China Time");
            Assert.That(tz, Is.EqualTo(TimeZoneInfo.Local));
        }
    }
}
