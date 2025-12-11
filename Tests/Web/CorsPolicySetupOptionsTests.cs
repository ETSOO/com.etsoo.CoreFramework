using com.etsoo.Web;

namespace Tests.Web
{
    [TestClass]
    public class CorsPolicySetupOptionsTests
    {
        [TestMethod]
        public void CheckTests()
        {
            // Arrange
            var options = new CorsPolicySetupOptions(new[] { "https://*.etsoo.com", "https://192.168.1.*:*", "http://etsoo.cn:*" }, true);

            // Act & assert
            Assert.IsTrue(options.Check("http://localhost:1234"));
            Assert.IsTrue(options.Check("https://localhost"));
            Assert.IsTrue(options.Check("https://cn.etsoo.com"));
            Assert.IsTrue(options.Check("https://192.168.1.199:1234"));
            Assert.IsTrue(options.Check("http://etsoo.cn"));
            Assert.IsTrue(options.Check("http://etsoo.cn:124"));

            Assert.IsFalse(options.Check("http://cn.etsoo.com"));
            Assert.IsFalse(options.Check("https://cn.etsoo.com:1234"));
        }
    }
}
