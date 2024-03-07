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
                <ScanResult><![CDATA[条形码1]]></ScanResult>
                </ScanCodeInfo>
                </xml>"));

            // Act
            var dic = await XmlUtils.ParseXmlAsync(input);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(dic, Has.Count.EqualTo(7));
                Assert.That(dic["Event"], Is.EqualTo("scancode_push"));
                Assert.That(dic["ScanCodeInfo"], Is.EqualTo("<ScanType><![CDATA[qrcode]]></ScanType><ScanResult><![CDATA[条形码1]]></ScanResult>"));
            });
        }

        [Test]
        public void GetValueTests()
        {
            var dic = new Dictionary<string, string>
            {
                ["boolean"] = "true",
                ["int"] = "123",
                ["int1"] = ""
            };

            var v1 = XmlUtils.GetValue<bool>(dic, "boolean");
            Assert.That(v1, Is.EqualTo(true));

            var v2 = XmlUtils.GetValue<int>(dic, "int");
            Assert.That(v2, Is.EqualTo(123));

            var v3 = XmlUtils.GetValue<DateTime>(dic, "int1");
            Assert.That(v3, Is.Null);

            var v4 = XmlUtils.GetValue<DateTime>(dic, "nofield");
            Assert.That(v4, Is.Null);

            var v5 = XmlUtils.GetValue(dic, "boolean");
            Assert.That(v5, Is.EqualTo("true"));
        }

        [Test]
        public void GetListTests()
        {
            // Arrage & act
            var items = XmlUtils.GetList("<item><field1>1</field1><field2><![CDATA[a1]]></field2></item><item><field1>2</field1><field2><![CDATA[亿速]]></field2></item>");

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(items.Count(), Is.EqualTo(2));
                Assert.That(items.Any(item => item.ContainsKey("field2") && item["field2"] == "亿速"), Is.True);
            });
        }
    }
}
