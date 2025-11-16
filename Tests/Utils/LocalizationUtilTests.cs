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
        [SetCulture("zh-CN")]
        [SetUICulture("zh-CN")]
        public void GetCulturesTests()
        {
            var cultures = LocalizationUtils.GetCultures(["zh-CN", "zh-Hans", "en-US", "ar"]);
            Assert.Multiple(() =>
            {
                Assert.That(cultures.Count(), Is.EqualTo(4));
                Assert.That(cultures.FirstOrDefault(c => c.Id.Equals("zh-CN"))?.Name, Is.EqualTo("中文 (中国)"));
                Assert.That(cultures.FirstOrDefault(c => c.Id.Equals("zh-Hans"))?.Name, Is.EqualTo("简体中文"));
                Assert.That(cultures.FirstOrDefault(c => c.Id.Equals("en-US"))?.Name, Is.EqualTo("英语 (美国)"));
                Assert.That(cultures.FirstOrDefault(c => c.Id.Equals("ar"))?.Name, Is.EqualTo("阿拉伯语"));
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
        [SetCulture("zh-CN")]
        [SetUICulture("zh-CN")]
        public void RegionChineseTest()
        {
            var regions = LocalizationUtils.GetAllRegions();
            var region = regions.FirstOrDefault(r => r.Key.Equals("US")).Value;
            Assert.Multiple(() =>
            {
                Assert.That(region?.Name, Is.EqualTo("美国"));
                Assert.That(region?.Currency.Name, Is.EqualTo("美元"));
            });
        }

        [Test]
        [SetCulture("zh-CN")]
        [SetUICulture("zh-CN")]
        public void CurrenciesByIdsTest()
        {
            var currencies = LocalizationUtils.GetAllRegions().GetCurrencies(["CNY", "USD", "NZD"]).ToArray();
            Assert.Multiple(() =>
            {
                Assert.That(currencies, Has.Length.EqualTo(3));
                Assert.That(currencies[2].Name, Is.EqualTo("新西兰元"));
            });
        }

        [Test]
        [SetCulture("zh-CN")]
        [SetUICulture("zh-CN")]
        public void RegionsByIdsTest()
        {
            var regions = LocalizationUtils.GetAllRegions().GetRegions(["CN", "US", "NZ"]).ToArray();
            Assert.Multiple(() =>
            {
                Assert.That(regions, Has.Length.EqualTo(3));
                Assert.That(regions[1].Name, Is.EqualTo("美国"));
            });
        }

        [Test]
        public void GetCurrencyDataNullTests()
        {
            var data = LocalizationUtils.GetCurrencyData("CNY1");
            Assert.That(data, Is.Null);
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
            using (Assert.EnterMultipleScope())
            {
                Assert.That(py1, Is.EqualTo("Chong Qing Ai Hao Zhen Hao Zhong"));
                Assert.That(py2, Is.EqualTo("Chong2 Qing4 Ai4 Hao4 Zhen1 Hao3 Zhong4"));
                Assert.That(py3, Is.EqualTo("CQAHZHZ"));
                Assert.That(py4, Is.EqualTo("Qing1 Dao3 Yi4 Su4 Si1 Wei2 Wang3 Luo4 Ke1 Ji4 You3 Xian4 Gong1 Si1"));
            }
        }

        [Test]
        public void GetPinyinNameTests()
        {
            // Arrange & Act
            var py1 = ChineseUtils.GetPinyin("尉迟敬德", true).ToPinyin(true);
            var py2 = ChineseUtils.GetPinyin("朴敬业", true).ToPinyin(true);
            var py3 = ChineseUtils.GetPinyin("肖赞长沙人是会长", true).ToPinyin(true);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(py1, Is.EqualTo("Yu4 Chi2 Jing4 De2"));
                Assert.That(py2, Is.EqualTo("Piao2 Jing4 Ye4"));
                Assert.That(py3, Is.EqualTo("Xiao1 Zan4 Chang2 Sha1 Ren2 Shi4 Hui4 Zhang3"));
            }
        }

        [Test]
        public void GetPinyinNameMixedTests()
        {
            // Arrange & Act
            var py1 = ChineseUtils.GetPinyin("肖赞Garry Xiao", true).ToPinyin(true);
            var py2 = ChineseUtils.GetPinyin("肖 赞", true).ToPinyin(true);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(py1, Is.EqualTo("Xiao1 Zan4"));
                Assert.That(py2, Is.EqualTo("Xiao1 Zan4"));
            }
        }

        [Test]
        public void ContainsChineseTests()
        {
            // Arrange & Act
            var result1 = LocalizationUtils.ContainsChinese("Hello, 世界!");
            var result2 = LocalizationUtils.ContainsChinese("Hello, World!");

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result1, Is.True);
                Assert.That(result2, Is.False);
            }
        }

        [Test]
        public void ContainsKoreanTests()
        {
            // Arrange & Act
            var result1 = LocalizationUtils.ContainsKorean("Hello, 세계!");
            var result2 = LocalizationUtils.ContainsKorean("Hello, 世界！");

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result1, Is.True);
                Assert.That(result2, Is.False);
            }
        }

        [Test]
        public void ContainsJapaneseTests()
        {
            // Arrange & Act
            var result1 = LocalizationUtils.ContainsJapanese("Hello, 世界!");
            var result2 = LocalizationUtils.ContainsJapanese("Hello, ワールド!");
            var result3 = LocalizationUtils.ContainsJapanese("Hello, 세계!");

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result1, Is.True);
                Assert.That(result2, Is.True);
                Assert.That(result3, Is.False);
            }
        }

        [Test]
        public void ParseNameTests()
        {
            // Arrange & Act
            var name1 = LocalizationUtils.ParseName("张伟");
            var name2 = LocalizationUtils.ParseName("李小龙");
            var name3 = LocalizationUtils.ParseName("王  芳");
            var name4 = LocalizationUtils.ParseName("John Smith");

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(name1.FamilyName, Is.EqualTo("张"));
                Assert.That(name1.GivenName, Is.EqualTo("伟"));
                Assert.That(name1.LatinFamilyName, Is.EqualTo("Zhang"));
                Assert.That(name1.LatinGivenName, Is.EqualTo("Wei"));

                Assert.That(name2.FamilyName, Is.EqualTo("李"));
                Assert.That(name2.GivenName, Is.EqualTo("小龙"));
                Assert.That(name2.LatinFamilyName, Is.EqualTo("Li"));
                Assert.That(name2.LatinGivenName, Is.EqualTo("Xiao Long"));

                Assert.That(name3.FamilyName, Is.EqualTo("王"));
                Assert.That(name3.GivenName, Is.EqualTo("芳"));
                Assert.That(name3.LatinFamilyName, Is.EqualTo("Wang"));
                Assert.That(name3.LatinGivenName, Is.EqualTo("Fang"));

                Assert.That(name4.FamilyName, Is.EqualTo("Smith"));
                Assert.That(name4.GivenName, Is.EqualTo("John"));
                Assert.That(name4.LatinFamilyName, Is.Null);
                Assert.That(name4.LatinGivenName, Is.Null);
            }
        }
    }
}
