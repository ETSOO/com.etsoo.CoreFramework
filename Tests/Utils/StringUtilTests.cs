using com.etsoo.Utils.String;
using System.Diagnostics;

namespace Tests.Utils
{
    [TestClass]
    public class StringUtilTests
    {
        private static IEnumerable<object[]> TryParseBoolBulkTestData
        {
            get
            {
                yield return new object[] { true, true };
                yield return new object[] { "true", true };
                yield return new object[] { "True", true };
                yield return new object[] { "TrUe", true };
                yield return new object[] { 1, true };
                yield return new object[] { "1", true };
                yield return new object[] { false, false };
                yield return new object[] { "false", false };
                yield return new object[] { "False", false };
                yield return new object[] { 0, false };
                yield return new object[] { "0", false };
                yield return new object[] { "abc", null };
                yield return new object[] { -1, null };
                yield return new object[] { null, null };
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
        [TestMethod]
        [DynamicData(nameof(TryParseBoolBulkTestData))]
        public void TryParse_Bool_Bulk(object input, bool? expectedResult)
        {
            // Arange

            // Act
            var result = StringUtils.TryParseObject<bool>(input);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        private static IEnumerable<object[]> FormatFileSizeTestData
        {
            get
            {
                yield return new object[] { 1551859712L, "1.45 GB", 2 };
                yield return new object[] { 1551859712L, "1.4 GB", 1 };
                yield return new object[] { 1125000L, "1.07 MB", 2 };
            }
        }

        [TestMethod]
        [DynamicData(nameof(FormatFileSizeTestData))]
        public void FormatFileSize_Test(long input, string expected, int fractionDigits)
        {
            var result = StringUtils.FormatFileSize(input, fractionDigits);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetLCSTests()
        {
            // Arrange
            var input1 = "广东省佛山市季华西路中国陶瓷总部基地中区E座";
            var input2 = "中国陶瓷产业总部基地中区-E座";

            // Act
            var result = StringUtils.GetLCS(input1, input2);

            // Assert
            Assert.AreEqual("总部基地中区", result.ToString());
        }

        [TestMethod]
        public void GetSamePartsTest()
        {
            // Arrange
            var input1 = "广东省佛山市季华西路中国陶瓷总部基地中区E座";
            var input2 = "中国陶瓷产业总部基地中区-E座";

            // Act
            var result = StringUtils.GetSameParts(input1, input2);

            // Assert
            var expected = new[] { "总部基地中区", "中国陶瓷", "E座" };
            CollectionAssert.AreEqual(expected, result.ToArray());
        }

        [TestMethod]
        public void GetSamePartsMinCharsTest()
        {
            // Arrange
            var input1 = "1abcdefh";
            var input2 = "ab123de4h";

            // Act
            var result = StringUtils.GetSameParts(input1, input2, 2);

            // Assert
            var expected = new[] { "ab", "de" };
            CollectionAssert.AreEqual(expected, result.ToArray());
        }

        [TestMethod]
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

        [TestMethod]
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
            result = StringUtils.HideEmail("xm@etsoo.com");

            // Assert 3
            Assert.AreEqual("x***@etsoo.com", result);
        }

        [TestMethod]
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
        [TestMethod]
        public void TryParseEnum()
        {
            // Arrange
            // Act
            var result1 = StringUtils.TryParse<TestEnum>("Friday");
            var result2 = StringUtils.TryParse<TestEnum>("4");

            Assert.AreEqual(TestEnum.Friday, result1);
            Assert.AreEqual(TestEnum.Friday, result2);
        }

        private static IEnumerable<object[]> TryParsePerformanceBulkTestData
        {
            get
            {
                yield return new object[] { true };
                yield return new object[] { "true" };
                yield return new object[] { 1 };
                yield return new object[] { "abc" };
                yield return new object[] { -1 };
            }
        }

        /// <summary>
        /// Performance test to parse bool value, 10K less than 100ms
        /// </summary>
        /// <param name="input">Input data</param>
        [TestMethod]
        [DynamicData(nameof(TryParsePerformanceBulkTestData))]
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

            Assert.IsLessThan(100, ms, $"{input}, {ms} is more than 100 ms");
        }

        private static IEnumerable<object[]> PascalLinuxBulkTestData
        {
            get
            {
                yield return new object[] { "A", "a" };
                yield return new object[] { "Name", "name" };
                yield return new object[] { "HelloWorld", "hello_world" };
                yield return new object[] { "YourCustomerId", "your_customer_id" };
            }
        }

        [TestMethod]
        [DynamicData(nameof(PascalLinuxBulkTestData))]
        public void PascalCaseToLinuxStyle_All_Test(string pascal, string camel)
        {
            // Arrange & act
            var result = StringUtils.PascalCaseToLinuxStyle(pascal).ToString();

            // Assert
            Assert.AreEqual(camel, result);
        }

        [TestMethod]
        [DynamicData(nameof(PascalLinuxBulkTestData))]
        public void LinuxStyleToPascalCase_All_Test(string pascal, string camel)
        {
            // Arrange & act
            var result = StringUtils.LinuxStyleToPascalCase(camel).ToString();

            // Assert
            Assert.AreEqual(pascal, result);
        }

        [TestMethod]
        public void LinuxStyleToPascalCase_Cameral_Test()
        {
            // Arrange & act
            var input = "yourName";
            var result = StringUtils.LinuxStyleToPascalCase(input).ToString();

            // Assert
            Assert.AreEqual("YourName", result);
        }

        [TestMethod]
        public void IEnumerableToString_Int_Test()
        {
            // Arrange
            var items = new int?[] { 1, null, 2 };

            // Act
            var result = StringUtils.IEnumerableToString(items);

            // Assert
            Assert.AreEqual("1,2", result);
        }

        [TestMethod]
        public void DictionaryToString_Int_Test()
        {
            // Arrange
            var items = new Dictionary<int, bool> { { 1001, true }, { 1002, false } };

            // Act
            var result = StringUtils.DictionaryToString(items);

            // Assert
            Assert.AreEqual("1001=True&1002=False", result);
        }

        [TestMethod]
        public void AsEnumerable_Int_Test()
        {
            // Arrange
            var input = "1, a,3";

            // Act
            var items = StringUtils.AsEnumerable<int>(input);

            Assert.AreEqual(2, items.Count());
            CollectionAssert.Contains(items.ToList(), 3);
        }

        [TestMethod]
        public void AsEnumerable_String_Test()
        {
            // Arrange
            var input = "1, a,3";

            // Act
            var items = StringUtils.AsEnumerable(input);

            Assert.AreEqual(3, items.Count());
            CollectionAssert.Contains(items.ToList(), "3");
        }

        [TestMethod]
        public void IsJsonTests()
        {
            Assert.IsFalse("1".IsJson());
            Assert.IsFalse("false".IsJson());
            Assert.IsTrue("{}".IsJson());
            Assert.IsTrue("{ \"bool\": true }".IsJson());
            Assert.IsTrue("[]".IsJson());
        }

        [TestMethod]
        public void ToPascalWord_Test()
        {
            // Arrange
            var input = "HELLO";

            // Act
            var result = input.AsSpan().ToPascalWord();

            // Assert
            Assert.AreEqual("Hello", result.ToString());
        }

        private static IEnumerable<object[]> RemoveNonLettersTestData
        {
            get
            {
                yield return new object[] { " 123 ", "123" };
                yield return new object[] { "0532-5557 9200", "053255579200" };
                yield return new object[] { "02,s*", "02s" };
            }
        }

        [TestMethod]
        [DynamicData(nameof(RemoveNonLettersTestData))]
        public void RemoveNonLetters_Test(string input, string result)
        {
            Assert.AreEqual(result, StringUtils.RemoveNonLetters(input));
        }

        [TestMethod]
        public void SplitIntGuid_TestEmptyCase()
        {
            // Arrange & act
            var (id, guid) = StringUtils.SplitIntGuid(null);
            Assert.IsNull(id);
            Assert.IsNull(guid);
        }

        [TestMethod]
        public void SplitIntGuid_TestNormalCase()
        {
            // Arrange & act
            var (id, guid) = StringUtils.SplitIntGuid("5|700fc07c-ce7c-4af0-aed3-c83a9d30f23d");
            Assert.AreEqual(5, id);
            Assert.AreEqual(Guid.Parse("700fc07c-ce7c-4af0-aed3-c83a9d30f23d"), guid);
        }

        [TestMethod]
        public void NumberToCharsAndCharsToNumberTests()
        {
            var num = 1638777042242;
            var chars = StringUtils.NumberToChars(num);
            Assert.AreEqual("QmpkdVgv", chars);
            Assert.AreEqual(num, StringUtils.CharsToNumber(chars));
        }

        [TestMethod]
        public void WriteJsonTests()
        {
            var json = StringUtils.WriteJson((writer) =>
            {
                writer.WritePropertyName("test");
                writer.WriteStartObject();
                writer.WriteString("Brand", "亿速");
                writer.WriteEndObject();
            });

            Assert.AreEqual("""{"test":{"Brand":"亿速"}}""", json);
        }

        [TestMethod]
        public void GetPrimitiveValueTests()
        {
            Assert.AreEqual(1.0, StringUtils.GetPrimitiveValue(1.0));

            var guid = Guid.NewGuid();
            Assert.AreEqual(guid, StringUtils.GetPrimitiveValue(guid));

            var dt = DateTime.Now;
            Assert.AreEqual(dt, StringUtils.GetPrimitiveValue(dt));

            Assert.AreEqual("https://etsoo.com/", StringUtils.GetPrimitiveValue(new Uri("https://etsoo.com/")));
        }
    }
}
