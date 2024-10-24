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

        [Test]
        public void GetPinyinTests()
        {
            // Arrange
            var pinyin = ChineseUtils.GetPinyin("重庆爱好真好重");

            // Act
            var py1 = pinyin.ToPinyin();
            var py2 = pinyin.ToPinyin(true);
            var py3 = pinyin.ToInitials();
            var py4 = ChineseUtils.GetPinyin("青岛亿速思维网络科技有限公司").ToPinyin(true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(py1, Is.EqualTo("Chong Qing Ai Hao Zhen Hao Zhong"));
                Assert.That(py2, Is.EqualTo("Chong2 Qing4 Ai4 Hao4 Zhen1 Hao3 Zhong4"));
                Assert.That(py3, Is.EqualTo("CQAHZHZ"));
                Assert.That(py4, Is.EqualTo("Qing1 Dao3 Yi4 Su4 Si1 Wei2 Wang3 Luo4 Ke1 Ji4 You3 Xian4 Gong1 Si1"));
            });
        }

        [Test]
        public void GetPinyinNameTests()
        {
            // Arrange & Act
            var py1 = ChineseUtils.GetPinyin("尉迟敬德", true).ToPinyin(true);
            var py2 = ChineseUtils.GetPinyin("朴敬业", true).ToPinyin(true);
            var py3 = ChineseUtils.GetPinyin("肖赞长沙人是会长", true).ToPinyin(true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(py1, Is.EqualTo("Yu4 Chi2 Jing4 De2"));
                Assert.That(py2, Is.EqualTo("Piao2 Jing4 Ye4"));
                Assert.That(py3, Is.EqualTo("Xiao1 Zan4 Chang2 Sha1 Ren2 Shi4 Hui4 Zhang3"));
            });
        }
    }
}
