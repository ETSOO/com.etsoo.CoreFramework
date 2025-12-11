using com.etsoo.CoreFramework.Application;
using com.etsoo.Localization;

namespace Tests.CoreFramework
{
    [TestClass]
    public class ApplicationErrorsTests
    {
        /// <summary>
        /// Multiple cultures of the errors
        /// </summary>
        [TestMethod]
        public void Errors_MultipleCultures()
        {
            // Arrange
            LocalizationUtils.SetCulture("zh-CN");

            // Action & assert
            // Resources depend on CurrentUICulture, not CurrentCulture
            Assert.AreEqual("找不到用户", ApplicationErrors.NoUserFound.Title);

            // Arrange
            LocalizationUtils.SetCulture("en-US");

            // Action & assert
            Assert.AreEqual("No User Found", ApplicationErrors.NoUserFound.Title);
        }
    }
}
