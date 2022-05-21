using com.etsoo.Localization;
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
            var result = LocalizationUtils.SetUtcKind(dt);

            // Assert
            Assert.AreEqual(DateTimeKind.Utc, result.Kind);
        }

        [Test]
        public void SetUtcKind_NoChangedTest()
        {
            // Arrange
            var dt = DateTime.Now;

            // Act
            var result = LocalizationUtils.SetUtcKind(dt);

            // Assert
            Assert.AreEqual(DateTimeKind.Local, result.Kind);
        }

        [Test]
        [SetCulture("zh-CN")]
        [SetUICulture("zh-CN")]
        public void GetTimeZone_Test()
        {
            // Correct
            var tz = LocalizationUtils.GetTimeZone("新西兰标准时间");

            Assert.AreEqual("New Zealand Standard Time", tz.StandardName);

            // Wrong
            tz = LocalizationUtils.GetTimeZone("China Time");
            Assert.AreEqual(TimeZoneInfo.Local, tz);
        }

        [Test]
        public void JsMilisecondsToUTCTests()
        {
            // Arrange & Act
            // 2021/12/6 19:35:52 UTC, 2021/12/7 8:35:52 NZ time
            var result = LocalizationUtils.JsMilisecondsToUTC(1638819352807);

            // Assert
            Assert.AreEqual(DateTimeKind.Utc, result.Kind);
            Assert.AreEqual(6, result.Day);
        }
    }
}
