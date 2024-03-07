using com.etsoo.CoreFramework.Application;
using com.etsoo.Localization;
using NUnit.Framework;

namespace Tests.CoreFramework
{
    [TestFixture]
    public class ApplicationErrorsTests
    {
        /// <summary>
        /// Multiple cultures of the errors
        /// </summary>
        [Test]
        public void Errors_MultipleCultures()
        {
            // Arrange
            LocalizationUtils.SetCulture("zh-CN");

            // Action & assert
            // Resources depend on CurrentUICulture, not CurrentCulture
            Assert.That(ApplicationErrors.NoUserFound.Title, Is.EqualTo("找不到用户"));

            // Arrange
            LocalizationUtils.SetCulture("en-US");

            // Action & assert
            Assert.That(ApplicationErrors.NoUserFound.Title, Is.EqualTo("No User Found"));
        }
    }
}
