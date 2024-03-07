using com.etsoo.Localization;
using com.etsoo.Utils;
using NUnit.Framework;

namespace Tests.Utils
{
    [TestFixture]
    public class LocalizationUtilTests
    {
        [Test]
        public void SetUtcKind_ChangedTest()
        {
            // Arrange
            var dt = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);

            // Act
            var result = SharedUtils.SetUtcKind(dt);

            // Assert
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void SetUtcKind_NoChangedTest()
        {
            // Arrange
            var dt = DateTime.Now;

            // Act
            var result = SharedUtils.SetUtcKind(dt);

            // Assert
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Local));
        }

        [Test]
        [SetCulture("zh-CN")]
        [SetUICulture("zh-CN")]
        public void GetTimeZone_Test()
        {
            // Correct
            var tz = LocalizationUtils.GetTimeZone("新西兰标准时间");

            Assert.That(tz.Id, Is.EqualTo("New Zealand Standard Time"));

            // Wrong
            tz = LocalizationUtils.GetTimeZone("China Time");
            Assert.That(tz, Is.EqualTo(TimeZoneInfo.Local));
        }

        [Test]
        public void JsMilisecondsToUTCTests()
        {
            // Arrange & Act
            // 2021/12/6 19:35:52 UTC, 2021/12/7 8:35:52 NZ time
            var result = SharedUtils.JsMilisecondsToUTC(1638819352807);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(result.Day, Is.EqualTo(6));
            });
        }

        [Test]
        public void GetRegionsByCurrencyTests()
        {
            var (region, culture) = LocalizationUtils.GetRegionsByCurrency("CNY").FirstOrDefault(item => item.Culture.Name.StartsWith("zh-"));
            Assert.Multiple(() =>
            {
                Assert.That(region?.TwoLetterISORegionName, Is.EqualTo("CN"));
                Assert.That(culture?.Name, Is.EqualTo("zh-Hans-CN"));
                Assert.That(LocalizationUtils.GetRegionsByCurrency("EUR").Count(), Is.GreaterThan(3));
            });
        }

        [Test]
        public void GetCulturesByCountryTests()
        {
            var cultures = LocalizationUtils.GetCulturesByCountry("SG");
            Assert.Multiple(() =>
            {
                Assert.That(cultures.Any(culture => culture.TwoLetterISOLanguageName.Equals("en")), Is.True);
                Assert.That(cultures.Count(), Is.EqualTo(4));
            });
        }

        [Test]
        public void GetCurrencyDataTests()
        {
            var data = LocalizationUtils.GetCurrencyData("CNY");
            Assert.Multiple(() =>
            {
                Assert.That(data?.Symbol, Is.EqualTo("¥"));
                Assert.That(data?.EnglishName, Is.EqualTo("Chinese Yuan"));
            });
        }
    }
}
