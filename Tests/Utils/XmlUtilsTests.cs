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
            Assert.AreEqual(7, dic.Count);
            Assert.AreEqual("scancode_push", dic["Event"]);
            Assert.AreEqual("<ScanType><![CDATA[qrcode]]></ScanType><ScanResult><![CDATA[条形码1]]></ScanResult>", dic["ScanCodeInfo"]);
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
            Assert.AreEqual(true, v1);

            var v2 = XmlUtils.GetValue<int>(dic, "int");
            Assert.AreEqual(123, v2);

            var v3 = XmlUtils.GetValue<DateTime>(dic, "int1");
            Assert.IsNull(v3);

            var v4 = XmlUtils.GetValue<DateTime>(dic, "nofield");
            Assert.IsNull(v4);

            var v5 = XmlUtils.GetValue(dic, "boolean");
            Assert.AreEqual("true", v5);
        }

        [Test]
        public void GetListTests()
        {
            // Arrage & act
            var items = XmlUtils.GetList("<item><field1>1</field1><field2><![CDATA[a1]]></field2></item><item><field1>2</field1><field2><![CDATA[亿速]]></field2></item>");

            // Assert
            Assert.AreEqual(2, items.Count());
            Assert.IsTrue(items.Any(item => item.ContainsKey("field2") && item["field2"] == "亿速"));
        }
    }
}
