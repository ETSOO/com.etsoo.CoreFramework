using com.etsoo.Localization;
using com.etsoo.Utils;
using System.Globalization;

namespace Tests.Utils
{
    [TestClass]
    public class LocalizationUtilTests
    {
        [TestMethod]
        public void SetUtcKind_ChangedTest()
        {
            // Arrange
            var dt = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);

            // Act
            var result = SharedUtils.SetUtcKind(dt);

            // Assert
            Assert.AreEqual(DateTimeKind.Utc, result.Kind);
        }

        [TestMethod]
        public void SetUtcKind_NoChangedTest()
        {
            // Arrange
            var dt = DateTime.Now;

            // Act
            var result = SharedUtils.SetUtcKind(dt);

            // Assert
            Assert.AreEqual(DateTimeKind.Local, result.Kind);
        }

        [TestMethod]
        public void JsMilisecondsToUTCTests()
        {
            // Arrange & Act
            // 2021/12/6 19:35:52 UTC, 2021/12/7 8:35:52 NZ time
            var result = SharedUtils.JsMilisecondsToUTC(1638819352807);

            // Assert
            Assert.AreEqual(DateTimeKind.Utc, result.Kind);
            Assert.AreEqual(6, result.Day);
        }

        [TestMethod]
        public void GetRegionsByCurrencyTests()
        {
            var (region, culture) = LocalizationUtils.GetRegionsByCurrency("CNY").FirstOrDefault(item => item.Culture.Name.StartsWith("zh-"));
            Assert.AreEqual("CN", region?.TwoLetterISORegionName);
            Assert.AreEqual("zh-Hans-CN", culture?.Name);
            Assert.IsGreaterThan(3, LocalizationUtils.GetRegionsByCurrency("EUR").Count());
        }

        [TestMethod]
        public void GetCulturesTests()
        {
            CultureInfo.CurrentCulture = new CultureInfo("zh-CN");
            CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
            var cultures = LocalizationUtils.GetCultures(["zh-CN", "zh-Hans", "en-US", "ar"]);
            Assert.AreEqual(4, cultures.Count());
            Assert.AreEqual("中文 (中国)", cultures.FirstOrDefault(c => c.Id.Equals("zh-CN"))?.Name);
            Assert.AreEqual("简体中文", cultures.FirstOrDefault(c => c.Id.Equals("zh-Hans"))?.Name);
            Assert.AreEqual("英语 (美国)", cultures.FirstOrDefault(c => c.Id.Equals("en-US"))?.Name);
            Assert.AreEqual("阿拉伯语", cultures.FirstOrDefault(c => c.Id.Equals("ar"))?.Name);
        }

        [TestMethod]
        public void GetCulturesByCountryTests()
        {
            var cultures = LocalizationUtils.GetCulturesByCountry("SG");
            Assert.IsTrue(cultures.Any(culture => culture.TwoLetterISOLanguageName.Equals("en")));
            Assert.AreEqual(4, cultures.Count());
        }

        [TestMethod]
        public void GetCurrencyDataTests()
        {
            var data = LocalizationUtils.GetCurrencyData("CNY");
            Assert.AreEqual("¥", data?.Symbol);
            Assert.AreEqual("Chinese Yuan", data?.EnglishName);
        }

        [TestMethod]
        public void RegionChineseTest()
        {
            CultureInfo.CurrentCulture = new CultureInfo("zh-CN");
            CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
            var regions = LocalizationUtils.GetAllRegions();
            var region = regions.FirstOrDefault(r => r.Key.Equals("US")).Value;
            Assert.AreEqual("美国", region?.Name);
            Assert.AreEqual("美元", region?.Currency.Name);
        }

        [TestMethod]
        public void CurrenciesByIdsTest()
        {
            CultureInfo.CurrentCulture = new CultureInfo("zh-CN");
            CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
            var currencies = LocalizationUtils.GetAllRegions().GetCurrencies(["CNY", "USD", "NZD"]).ToArray();
            Assert.HasCount(3, currencies);
            Assert.AreEqual("新西兰元", currencies[2].Name);
        }

        [TestMethod]
        public void RegionsByIdsTest()
        {
            CultureInfo.CurrentCulture = new CultureInfo("zh-CN");
            CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
            var regions = LocalizationUtils.GetAllRegions().GetRegions(["CN", "US", "NZ"]).ToArray();
            Assert.HasCount(3, regions);
            Assert.AreEqual("美国", regions[1].Name);
        }

        [TestMethod]
        public void GetCurrencyDataNullTests()
        {
            var data = LocalizationUtils.GetCurrencyData("CNY1");
            Assert.IsNull(data);
        }

        [TestMethod]
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
            Assert.AreEqual("Chong Qing Ai Hao Zhen Hao Zhong", py1);
            Assert.AreEqual("Chong2 Qing4 Ai4 Hao4 Zhen1 Hao3 Zhong4", py2);
            Assert.AreEqual("CQAHZHZ", py3);
            Assert.AreEqual("Qing1 Dao3 Yi4 Su4 Si1 Wei2 Wang3 Luo4 Ke1 Ji4 You3 Xian4 Gong1 Si1", py4);
        }

        [TestMethod]
        public void GetPinyinNameTests()
        {
            // Arrange & Act
            var py1 = ChineseUtils.GetPinyin("尉迟敬德", true).ToPinyin(true);
            var py2 = ChineseUtils.GetPinyin("朴敬业", true).ToPinyin(true);
            var py3 = ChineseUtils.GetPinyin("肖赞长沙人是会长", true).ToPinyin(true);

            // Assert
            Assert.AreEqual("Yu4 Chi2 Jing4 De2", py1);
            Assert.AreEqual("Piao2 Jing4 Ye4", py2);
            Assert.AreEqual("Xiao1 Zan4 Chang2 Sha1 Ren2 Shi4 Hui4 Zhang3", py3);
        }

        [TestMethod]
        public void GetPinyinNameMixedTests()
        {
            // Arrange & Act
            var py1 = ChineseUtils.GetPinyin("肖赞Garry Xiao", true).ToPinyin(true);
            var py2 = ChineseUtils.GetPinyin("肖 赞", true).ToPinyin(true);

            // Assert
            Assert.AreEqual("Xiao1 Zan4", py1);
            Assert.AreEqual("Xiao1 Zan4", py2);
        }

        [TestMethod]
        public void ContainsChineseTests()
        {
            // Arrange & Act
            var result1 = LocalizationUtils.ContainsChinese("Hello, 世界!");
            var result2 = LocalizationUtils.ContainsChinese("Hello, World!");

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void ContainsKoreanTests()
        {
            // Arrange & Act
            var result1 = LocalizationUtils.ContainsKorean("Hello, 세계!");
            var result2 = LocalizationUtils.ContainsKorean("Hello, 世界！");

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void ContainsJapaneseTests()
        {
            // Arrange & Act
            var result1 = LocalizationUtils.ContainsJapanese("Hello, 世界!");
            var result2 = LocalizationUtils.ContainsJapanese("Hello, ワールド!");
            var result3 = LocalizationUtils.ContainsJapanese("Hello, 세계!");

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsFalse(result3);
        }

        [TestMethod]
        public void ParseNameTests()
        {
            // Arrange & Act
            var name1 = LocalizationUtils.ParseName("张伟");
            var name2 = LocalizationUtils.ParseName("李小龙");
            var name3 = LocalizationUtils.ParseName("王  芳");
            var name4 = LocalizationUtils.ParseName("John Smith");

            Assert.AreEqual("ZW", name1.PinyinInitials);
            Assert.AreEqual("张", name1.FamilyName);
            Assert.AreEqual("伟", name1.GivenName);
            Assert.AreEqual("Zhang", name1.LatinFamilyName);
            Assert.AreEqual("Wei", name1.LatinGivenName);

            Assert.AreEqual("LXL", name2.PinyinInitials);
            Assert.AreEqual("李", name2.FamilyName);
            Assert.AreEqual("小龙", name2.GivenName);
            Assert.AreEqual("Li", name2.LatinFamilyName);
            Assert.AreEqual("Xiao Long", name2.LatinGivenName);

            Assert.AreEqual("WF", name3.PinyinInitials);
            Assert.AreEqual("王", name3.FamilyName);
            Assert.AreEqual("芳", name3.GivenName);
            Assert.AreEqual("Wang", name3.LatinFamilyName);
            Assert.AreEqual("Fang", name3.LatinGivenName);

            Assert.IsNull(name4.PinyinInitials);
            Assert.AreEqual("Smith", name4.FamilyName);
            Assert.AreEqual("John", name4.GivenName);
            Assert.IsNull(name4.LatinFamilyName);
            Assert.IsNull(name4.LatinGivenName);
        }
    }
}
