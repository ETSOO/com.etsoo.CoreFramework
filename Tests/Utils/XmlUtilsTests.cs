using com.etsoo.Utils;
using NUnit.Framework;
using System.Text;

namespace Tests.Utils
{
    [TestFixture]
    public class XmlUtilsTests
    {
        [Test]
        public async Task ParseXmlAsyncTests()
        {
            // Arrange
            var input = new MemoryStream(Encoding.UTF8.GetBytes(@"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
                <FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
                <CreateTime>1408090502</CreateTime>
                <MsgType><![CDATA[event]]></MsgType>
                <Event><![CDATA[scancode_push]]></Event>
                <EventKey><![CDATA[6]]></EventKey>
                <ScanCodeInfo><ScanType><![CDATA[qrcode]]></ScanType>
                <ScanResult><![CDATA[1]]></ScanResult>
                </ScanCodeInfo>
                </xml>"));

            // Act
            var dic = await XmlUtils.ParseXmlAsync(input);

            // Assert
            Assert.AreEqual(7, dic.Count);
            Assert.AreEqual("scancode_push", dic["Event"]);
            Assert.AreEqual("<ScanType><![CDATA[qrcode]]></ScanType><ScanResult><![CDATA[1]]></ScanResult>", dic["ScanCodeInfo"]);
        }
    }
}
