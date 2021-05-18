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
        public void IEnumerableToString_Test()
        {
            // Arrange
            var items = new int?[] { 1, 2, null, 3 };

            // Act
            var result = StringUtils.IEnumerableToString(items);

            // Assert
            Assert.IsTrue(result == "1,2,3");
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
            Assert.IsTrue(result1 == TestEnum.Friday);
            Assert.IsTrue(result2 == TestEnum.Friday);
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
        public void TryParse_Performance_Bulk(dynamic input)
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
            Assert.IsTrue(result == camel, $"{result} is not equal with {camel}");
        }

        [Test, TestCaseSource(nameof(PascalLinuxBulkTestData))]
        public void LinuxStyleToPascalCase_All_Test(string pascal, string camel)
        {
            // Arrange & act
            var result = StringUtils.LinuxStyleToPascalCase(camel).ToString();

            // Assert
            Assert.IsTrue(result == pascal, $"{result} is not equal with {pascal}");
        }

        [Test]
        public void IEnumerableToString_Int_Test()
        {
            // Arrange
            var items = new int?[] { 1, null, 2 };

            // Act
            var result = StringUtils.IEnumerableToString(items);

            // Assert
            Assert.IsTrue(result == "1,2");
        }

        [Test]
        public void DictionaryToString_Int_Test()
        {
            // Arrange
            var items = new Dictionary<int, bool> { { 1001, true }, { 1002, false } };

            // Act
            var result = StringUtils.DictionaryToString(items);

            // Assert
            Assert.IsTrue(result == "1001=True&1002=False");
        }

        [Test]
        public void AsEnumerable_Int_Test()
        {
            // Arrange
            var input = "1, a,3";

            // Act
            var items = StringUtils.AsEnumerable<int>(input);

            // Assert
            Assert.IsTrue(items.Count() == 2);
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
            Assert.IsTrue(items.Count() == 3);
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
            Assert.IsTrue(result.ToString() == "Hello");
        }
    }
}
