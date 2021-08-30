using com.etsoo.Utils.Localization;
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
        public void GetTimeZone_Test()
        {
            // Correct
            var tz = LocalizationUtils.GetTimeZone("New Zealand Standard Time");

            Assert.AreEqual("新西兰标准时间", tz.StandardName);

            // Wrong
            tz = LocalizationUtils.GetTimeZone("China Time");
            Assert.AreEqual(TimeZoneInfo.Local, tz);
        }
    }
}
