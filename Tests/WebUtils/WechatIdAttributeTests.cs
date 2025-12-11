using com.etsoo.WebUtils.Attributes;

namespace Tests.WebUtils
{
    [TestClass]
    public class WechatIdAttributeTests
    {
        [TestMethod]
        public void IsValidTests()
        {
            var attribute = new WechatIdAttribute();
            Assert.IsTrue(attribute.IsValid("weixin_123"));
            Assert.IsFalse(attribute.IsValid("123456"));
        }
    }
}
