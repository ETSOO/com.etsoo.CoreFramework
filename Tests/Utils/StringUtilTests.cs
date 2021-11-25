using com.etsoo.Utils.String;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tests.Utils
{
    [TestFixture]
    public class StringUtilTests
    {
        private static IEnumerable<TestCaseData> TryParseBoolBulkTestData
        {
            get
            {
                yield return new TestCaseData(true, true);
                yield return new TestCaseData("true", true);
                yield return new TestCaseData("True", true);
                yield return new TestCaseData("TrUe", true);
                yield return new TestCaseData(1, true);
                yield return new TestCaseData("1", true);
                yield return new TestCaseData(false, false);
                yield return new TestCaseData("false", false);
                yield return new TestCaseData("False", false);
                yield return new TestCaseData(0, false);
                yield return new TestCaseData("0", false);
                yield return new TestCaseData("abc", null);
                yield return new TestCaseData(-1, null);
                yield return new TestCaseData(null, null);
            }
        }

        private enum TestEnum
        {
            Mondy,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
            Sunday
        }

        /// <summary>
        /// Bulk test to parse bool value
        /// </summary>
        /// <param name="input">Input data</param>
        /// <param name="expectedResult">Expected result</param>
        [Test, TestCaseSource(nameof(TryParseBoolBulkTestData))]
        public void TryParse_Bool_Bulk(dynamic input, bool? expectedResult)
        {
            // Arange

            // Act
            var result = StringUtils.TryParseObject<bool>(input);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void HideData_Test()
        {
            // Act 1
            var result = StringUtils.HideData("4000609917");

            // Assert 1
            Assert.AreEqual("400***917", result);

            // Act 2
            result = StringUtils.HideData("+8653255579200");

            // Assert 2
            Assert.AreEqual("+865***9200", result);
        }

        [Test]
        public void HideEmail_Test()
        {
            // Act 1
            var result = StringUtils.HideEmail("info@etsoo.com");

            // Assert 1
            Assert.AreEqual("in***@etsoo.com", result);

            // Act 2
            result = StringUtils.HideEmail("helloworld@etsoo.com");

            // Assert 2
            Assert.AreEqual("hel***rld@etsoo.com", result);

            // Act 3
            result = StringUtils.HideEmail("a@etsoo.com");

            // Assert 3
            Assert.AreEqual("***@etsoo.com", result);
        }

        [Test]
        public void IEnumerableToString_Test()
        {
            // Arrange
            var items = new int?[] { 1, 2, null, 3 };

            // Act
            var result = StringUtils.IEnumerableToString(items);

            // Assert
            Assert.AreEqual("1,2,3", result);
        }

        /// <summary>
        /// Enum parse test
        /// </summary>
        [Test]
        public void TryParseEnum()
        {
            // Arrange
            // Act
            var result1 = StringUtils.TryParse<TestEnum>("Friday");
            var result2 = StringUtils.TryParse<TestEnum>("4");

            // Assert
            Assert.AreEqual(TestEnum.Friday, result1);
            Assert.AreEqual(TestEnum.Friday, result2);
        }

        private static IEnumerable<TestCaseData> TryParsePerformanceBulkTestData
        {
            get
            {
                yield return new TestCaseData(true);
                yield return new TestCaseData("true");
                yield return new TestCaseData(1);
                yield return new TestCaseData("abc");
                yield return new TestCaseData(-1);
            }
        }

        /// <summary>
        /// Performance test to parse bool value, 10K less than 100ms
        /// </summary>
        /// <param name="input">Input data</param>
        [Test, TestCaseSource(nameof(TryParsePerformanceBulkTestData))]
        public void TryParse_Performance_Bulk(object input)
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < 10000; i++)
            {
                StringUtils.TryParseObject<bool>(input);
            }

            sw.Stop();

            var ms = sw.ElapsedMilliseconds;

            Assert.IsTrue(ms < 100, $"{input}, {ms} is more than 100 ms");
        }

        private static IEnumerable<TestCaseData> PascalLinuxBulkTestData
        {
            get
            {
                yield return new TestCaseData("A", "a");
                yield return new TestCaseData("Name", "name");
                yield return new TestCaseData("HelloWorld", "hello_world");
                yield return new TestCaseData("YourCustomerId", "your_customer_id");
            }
        }

        [Test, TestCaseSource(nameof(PascalLinuxBulkTestData))]
        public void PascalCaseToLinuxStyle_All_Test(string pascal, string camel)
        {
            // Arrange & act
            var result = StringUtils.PascalCaseToLinuxStyle(pascal).ToString();

            // Assert
            Assert.AreEqual(camel, result);
        }

        [Test, TestCaseSource(nameof(PascalLinuxBulkTestData))]
        public void LinuxStyleToPascalCase_All_Test(string pascal, string camel)
        {
            // Arrange & act
            var result = StringUtils.LinuxStyleToPascalCase(camel).ToString();

            // Assert
            Assert.AreEqual(pascal, result);
        }

        [Test]
        public void IEnumerableToString_Int_Test()
        {
            // Arrange
            var items = new int?[] { 1, null, 2 };

            // Act
            var result = StringUtils.IEnumerableToString(items);

            // Assert
            Assert.AreEqual("1,2", result);
        }

        [Test]
        public void DictionaryToString_Int_Test()
        {
            // Arrange
            var items = new Dictionary<int, bool> { { 1001, true }, { 1002, false } };

            // Act
            var result = StringUtils.DictionaryToString(items);

            // Assert
            Assert.AreEqual("1001=True&1002=False", result);
        }

        [Test]
        public void AsEnumerable_Int_Test()
        {
            // Arrange
            var input = "1, a,3";

            // Act
            var items = StringUtils.AsEnumerable<int>(input);

            // Assert
            Assert.AreEqual(2, items.Count());
            Assert.IsTrue(items.Contains(3));
        }

        [Test]
        public void AsEnumerable_String_Test()
        {
            // Arrange
            var input = "1, a,3";

            // Act
            var items = StringUtils.AsEnumerable(input);

            // Assert
            Assert.AreEqual(3, items.Count());
            Assert.IsTrue(items.Contains("3"));
        }

        [Test]
        public void ToPascalWord_Test()
        {
            // Arrange
            var input = "HELLO";

            // Act
            var result = input.AsSpan().ToPascalWord();

            // Assert
            Assert.AreEqual("Hello", result.ToString());
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

        private static IEnumerable<TestCaseData> RemoveNonLettersTestData
        {
            get
            {
                yield return new TestCaseData(" 123 ", "123");
                yield return new TestCaseData("0532-5557 9200", "053255579200");
                yield return new TestCaseData("02,s*", "02s");
            }
        }

        [Test, TestCaseSource(nameof(RemoveNonLettersTestData))]
        public void RemoveNonLetters_Test(string input, string result)
        {
            Assert.AreEqual(StringUtils.RemoveNonLetters(input), result);
        }

        [Test]
        public void SplitIntGuid_TestEmptyCase()
        {
            // Arrange & act
            var (id, guid) = StringUtils.SplitIntGuid(null);

            // Assert
            Assert.IsNull(id);
            Assert.IsNull(guid);
        }

        [Test]
        public void SplitIntGuid_TestNormalCase()
        {
            // Arrange & act
            var (id, guid) = StringUtils.SplitIntGuid("5|700fc07c-ce7c-4af0-aed3-c83a9d30f23d");

            // Assert
            Assert.AreEqual(id, 5);
            Assert.AreEqual(guid, Guid.Parse("700fc07c-ce7c-4af0-aed3-c83a9d30f23d"));
        }
    }
}
