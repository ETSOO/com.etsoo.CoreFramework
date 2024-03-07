using com.etsoo.Web;
using NUnit.Framework;

namespace Tests.Web
{
    [TestFixture]
    public class CorsPolicySetupOptionsTests
    {
        [Test]
        public void CheckTests()
        {
            // Arrange
            var options = new CorsPolicySetupOptions(new[] { "https://*.etsoo.com", "https://192.168.1.*:*", "http://etsoo.cn:*" }, true);

            Assert.Multiple(() =>
            {
                // Act & assert
                Assert.That(options.Check("http://localhost:1234"), Is.True);
                Assert.That(options.Check("https://localhost"), Is.True);
                Assert.That(options.Check("https://cn.etsoo.com"), Is.True);
                Assert.That(options.Check("https://192.168.1.199:1234"), Is.True);
                Assert.That(options.Check("http://etsoo.cn"), Is.True);
                Assert.That(options.Check("http://etsoo.cn:124"), Is.True);

                Assert.That(options.Check("http://cn.etsoo.com"), Is.False);
                Assert.That(options.Check("https://cn.etsoo.com:1234"), Is.False);
            });
        }
    }
}
