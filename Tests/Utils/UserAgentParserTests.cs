using com.etsoo.Utils.Web;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Utils
{
    [TestFixture]
    public class UserAgentParserTests
    {
        private static IEnumerable<TestCaseData> ParseData
        {
            get
            {
                yield return new TestCaseData("Mozilla/5.0 (iPhone; CPU iPhone OS 5_1_1 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9B206 Safari/7534.48.3", false, true);
                yield return new TestCaseData("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36 OPR/38.0.2220.41", false, false);
                yield return new TestCaseData("Opera/9.60 (Windows NT 6.0; U; en) Presto/2.1.1", false, false);
                yield return new TestCaseData("PostmanRuntime (6.7.1)", false, false);
                yield return new TestCaseData("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)", true, false);
                yield return new TestCaseData("Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko", false, false);
            }
        }

        /// <summary>
        /// Bulk test to parse bool value
        /// </summary>
        /// <param name="userParser">User agent</param>
        /// <param name="isSpider">Is spider</param>
        /// <param name="isMobile">Is mobile</param>
        [Test, TestCaseSource(nameof(ParseData))]
        public void Parse_Bulk(string userAgent, bool isSpider, bool isMobile)
        {
            // Arrange & Act
            var parser = new UserAgentParser(userAgent);

            // Assert
            Assert.IsTrue(parser.IsSpider == isSpider);
            Assert.IsTrue(parser.IsMobile == isMobile);
        }

        /// <summary>
        /// Bulk test to parse bool value
        /// </summary>
        /// <param name="userParser">User agent</param>
        /// <param name="isSpider">Is spider</param>
        /// <param name="isMobile">Is mobile</param>
        [Test, TestCaseSource(nameof(ParseData))]
        public async Task ToJsonAsyn_Bulk(string userAgent, bool isSpider, bool isMobile)
        {
            // Arrange
            var parser = new UserAgentParser(userAgent);

            // Act
            var result = await parser.ToJsonAsync();

            // Assert
            if (isSpider)
                Assert.IsFalse(result.Contains("os"));
            else if (!string.IsNullOrEmpty(parser.ToString()))
                Assert.IsTrue(result.Contains("os"));
        }
    }
}
