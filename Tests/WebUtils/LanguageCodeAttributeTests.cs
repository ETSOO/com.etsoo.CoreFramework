using com.etsoo.WebUtils.Attributes;
using NUnit.Framework;

namespace Tests.WebUtils
{
    [TestFixture]
    public class LanguageCodeAttributeTests
    {
        [Test]
        public void IsValidTests()
        {
            var attribute = new LanguageCodeAttribute();
            Assert.IsTrue(attribute.IsValid("zh"));
            Assert.IsTrue(attribute.IsValid("zh-CN"));
            Assert.IsTrue(attribute.IsValid("zh-Hans"));
            Assert.IsTrue(attribute.IsValid("zh-Hans-CN"));
            Assert.IsTrue(attribute.IsValid("zh-Hans-HK"));
            Assert.IsTrue(attribute.IsValid("zh-Hant-HK"));
            Assert.IsTrue(attribute.IsValid("en"));
        }
    }
}
