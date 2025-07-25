using com.etsoo.WebUtils.Attributes;
using NUnit.Framework;

namespace Tests.WebUtils
{
    [TestFixture]
    internal class WechatIdAttributeTests
    {
        [Test]
        public void IsValidTests()
        {
            var attribute = new WechatIdAttribute();
            using (Assert.EnterMultipleScope())
            {
                Assert.That(attribute.IsValid("weixin_123"), Is.True);
                Assert.That(attribute.IsValid("123456"), Is.False);
            }
        }
    }
}
