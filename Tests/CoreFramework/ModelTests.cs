using com.etsoo.CoreFramework.Models;
using NUnit.Framework;

namespace Tests.CoreFramework
{
    [TestFixture]
    public class ModelTests
    {
        [Test]
        public void LoginIdRQTest()
        {
            var loginIdRQ = new LoginIdRQ
            {
                DeviceId = "X01/cDAy!10395F7FDE101D118CE02CEFA26B699926FE5E2F86F67E3841C1B538BD030962E5ZubItmil/PX4GNPXtOE0vl3S8g1Jgx8aISwsdbHEbE71+IOf1Udtd6xD4nw93Fdc",
                Id = "019862b4a71672968de589405bd96f634b6e6cb4a811935f0cde67ae8239af35afyhUCWj2ZrRJ18oyOz9khHGw+oORxl4RWZiOcCOviFVA=",
                Region = "CN"
            };

            Assert.That(loginIdRQ.Validate(), Is.Null);
        }
    }
}
