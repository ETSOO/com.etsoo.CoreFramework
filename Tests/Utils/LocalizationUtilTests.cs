using com.etsoo.Utils.Localization;
using NUnit.Framework;
using System;

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
            Assert.IsTrue(result.Kind == DateTimeKind.Utc);
        }

        [Test]
        public void SetUtcKind_NoChangedTest()
        {
            // Arrange
            var dt = DateTime.Now;

            // Act
            var result = LocalizationUtils.SetUtcKind(dt);

            // Assert
            Assert.IsTrue(result.Kind == DateTimeKind.Local);
        }
    }
}
