﻿using com.etsoo.Utils.String;
using NUnit.Framework;
using System.Diagnostics;

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
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private static IEnumerable<TestCaseData> FormatFileSizeTestData
        {
            get
            {
                yield return new TestCaseData(1551859712, "1.45 GB", 2);
                yield return new TestCaseData(1551859712, "1.4 GB", 1);
                yield return new TestCaseData(1125000, "1.07 MB", 2);
            }
        }

        [TestCaseSource(nameof(FormatFileSizeTestData))]
        public void FormatFileSize_Test(long input, string expected, int fractionDigits)
        {
            var result = StringUtils.FormatFileSize(input, fractionDigits);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetLCSTests()
        {
            // Arrange
            var input1 = "广东省佛山市季华西路中国陶瓷总部基地中区E座";
            var input2 = "中国陶瓷产业总部基地中区-E座";

            // Act
            var result = StringUtils.GetLCS(input1, input2);

            // Assert
            Assert.That(result.ToString(), Is.EqualTo("总部基地中区"));
        }

        [Test]
        public void GetSamePartsTest()
        {
            // Arrange
            var input1 = "广东省佛山市季华西路中国陶瓷总部基地中区E座";
            var input2 = "中国陶瓷产业总部基地中区-E座";

            // Act
            var result = StringUtils.GetSameParts(input1, input2);

            // Assert
            var expected = new[] { "总部基地中区", "中国陶瓷", "E座" };
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetSamePartsMinCharsTest()
        {
            // Arrange
            var input1 = "1abcdefh";
            var input2 = "ab123de4h";

            // Act
            var result = StringUtils.GetSameParts(input1, input2, 2);

            // Assert
            var expected = new[] { "ab", "de" };
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void HideData_Test()
        {
            // Act 1
            var result = StringUtils.HideData("4000609917");

            // Assert 1
            Assert.That(result, Is.EqualTo("400***917"));

            // Act 2
            result = StringUtils.HideData("+8653255579200");

            // Assert 2
            Assert.That(result, Is.EqualTo("+865***9200"));
        }

        [Test]
        public void HideEmail_Test()
        {
            // Act 1
            var result = StringUtils.HideEmail("info@etsoo.com");

            // Assert 1
            Assert.That(result, Is.EqualTo("in***@etsoo.com"));

            // Act 2
            result = StringUtils.HideEmail("helloworld@etsoo.com");

            // Assert 2
            Assert.That(result, Is.EqualTo("hel***rld@etsoo.com"));

            // Act 3
            result = StringUtils.HideEmail("xm@etsoo.com");

            // Assert 3
            Assert.That(result, Is.EqualTo("x***@etsoo.com"));
        }

        [Test]
        public void IEnumerableToString_Test()
        {
            // Arrange
            var items = new int?[] { 1, 2, null, 3 };

            // Act
            var result = StringUtils.IEnumerableToString(items);

            // Assert
            Assert.That(result, Is.EqualTo("1,2,3"));
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

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result1, Is.EqualTo(TestEnum.Friday));
                Assert.That(result2, Is.EqualTo(TestEnum.Friday));
            });
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

            Assert.That(ms, Is.LessThan(100), $"{input}, {ms} is more than 100 ms");
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
            Assert.That(result, Is.EqualTo(camel));
        }

        [Test, TestCaseSource(nameof(PascalLinuxBulkTestData))]
        public void LinuxStyleToPascalCase_All_Test(string pascal, string camel)
        {
            // Arrange & act
            var result = StringUtils.LinuxStyleToPascalCase(camel).ToString();

            // Assert
            Assert.That(result, Is.EqualTo(pascal));
        }

        [Test]
        public void LinuxStyleToPascalCase_Cameral_Test()
        {
            // Arrange & act
            var input = "yourName";
            var result = StringUtils.LinuxStyleToPascalCase(input).ToString();

            // Assert
            Assert.That(result, Is.EqualTo("YourName"));
        }

        [Test]
        public void IEnumerableToString_Int_Test()
        {
            // Arrange
            var items = new int?[] { 1, null, 2 };

            // Act
            var result = StringUtils.IEnumerableToString(items);

            // Assert
            Assert.That(result, Is.EqualTo("1,2"));
        }

        [Test]
        public void DictionaryToString_Int_Test()
        {
            // Arrange
            var items = new Dictionary<int, bool> { { 1001, true }, { 1002, false } };

            // Act
            var result = StringUtils.DictionaryToString(items);

            // Assert
            Assert.That(result, Is.EqualTo("1001=True&1002=False"));
        }

        [Test]
        public void AsEnumerable_Int_Test()
        {
            // Arrange
            var input = "1, a,3";

            // Act
            var items = StringUtils.AsEnumerable<int>(input);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(items.Count(), Is.EqualTo(2));
                Assert.That(items, Does.Contain(3));
            });
        }

        [Test]
        public void AsEnumerable_String_Test()
        {
            // Arrange
            var input = "1, a,3";

            // Act
            var items = StringUtils.AsEnumerable(input);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(items.Count(), Is.EqualTo(3));
                Assert.That(items, Does.Contain("3"));
            });
        }

        [Test]
        public void IsJsonTests()
        {
            Assert.Multiple(() =>
            {
                Assert.That("1".IsJson(), Is.False);
                Assert.That("false".IsJson(), Is.False);
                Assert.That("{}".IsJson(), Is.True);
                Assert.That("{ \"bool\": true }".IsJson(), Is.True);
                Assert.That("[]".IsJson(), Is.True);
            });
        }

        [Test]
        public void ToPascalWord_Test()
        {
            // Arrange
            var input = "HELLO";

            // Act
            var result = input.AsSpan().ToPascalWord();

            // Assert
            Assert.That(result.ToString(), Is.EqualTo("Hello"));
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
            Assert.That(result, Is.EqualTo(StringUtils.RemoveNonLetters(input)));
        }

        [Test]
        public void SplitIntGuid_TestEmptyCase()
        {
            // Arrange & act
            var (id, guid) = StringUtils.SplitIntGuid(null);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(id, Is.Null);
                Assert.That(guid, Is.Null);
            });
        }

        [Test]
        public void SplitIntGuid_TestNormalCase()
        {
            // Arrange & act
            var (id, guid) = StringUtils.SplitIntGuid("5|700fc07c-ce7c-4af0-aed3-c83a9d30f23d");

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(id, Is.EqualTo(5));
                Assert.That(Guid.Parse("700fc07c-ce7c-4af0-aed3-c83a9d30f23d"), Is.EqualTo(guid));
            });
        }

        [Test]
        public void NumberToCharsAndCharsToNumberTests()
        {
            var num = 1638777042242;
            var chars = StringUtils.NumberToChars(num);
            Assert.Multiple(() =>
            {
                Assert.That(chars, Is.EqualTo("QmpkdVgv"));
                Assert.That(StringUtils.CharsToNumber(chars), Is.EqualTo(num));
            });
        }

        [Test]
        public void WriteJsonTests()
        {
            var json = StringUtils.WriteJson((writer) =>
            {
                writer.WritePropertyName("test");
                writer.WriteStartObject();
                writer.WriteString("Brand", "亿速");
                writer.WriteEndObject();
            });

            Assert.That(json, Is.EqualTo("""{"test":{"Brand":"亿速"}}"""));
        }

        [Test]
        public void GetPrimitiveValueTests()
        {
            Assert.Multiple(() =>
            {
                Assert.That(StringUtils.GetPrimitiveValue(1.0), Is.EqualTo(1.0));

                var guid = Guid.NewGuid();
                Assert.That(StringUtils.GetPrimitiveValue(guid), Is.EqualTo(guid));

                var dt = DateTime.Now;
                Assert.That(StringUtils.GetPrimitiveValue(dt), Is.EqualTo(dt));

                Assert.That(StringUtils.GetPrimitiveValue(new Uri("https://etsoo.com/")), Is.EqualTo("https://etsoo.com/"));
            });
        }
    }
}
