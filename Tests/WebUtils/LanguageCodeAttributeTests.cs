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
            Assert.Multiple(() =>
            {
                Assert.That(attribute.IsValid("zh"), Is.True);
                Assert.That(attribute.IsValid("zh-CN"), Is.True);
                Assert.That(attribute.IsValid("zh-Hans"), Is.True);
                Assert.That(attribute.IsValid("zh-Hans-CN"), Is.True);
                Assert.That(attribute.IsValid("zh-Hans-HK"), Is.True);
                Assert.That(attribute.IsValid("zh-Hant-HK"), Is.True);
                Assert.That(attribute.IsValid("en"), Is.True);
            });
        }
    }
}
