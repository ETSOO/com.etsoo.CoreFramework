using com.etsoo.Utils;
using NUnit.Framework;

namespace Tests.Utils
{
    [TestFixture]
    public class SharedUtilTests
    {
        [Test]
        public void GetAccordingValueTests()
        {
            var fields = new List<string> { "日期", "美元", "欧元", "日元", "港元", "英镑", "林吉特", "卢布", "澳元", "加元", "新西兰元", "新加坡元", "瑞士法郎", "兰特", "韩元", "迪拉姆", "里亚尔", "福林", "兹罗提", "丹麦克朗", "瑞典克朗", "挪威克朗", "里拉", "比索", "泰铢" };
            var values = new List<string> { "2022-02-24", "632.8", "715.14", "5.5079", "81.079", "856.99", "66.144", "1283.55", "457.16", "496.96", "428.31", "470.01", "689.73", "239.05", "18889.0", "58.039", "59.287", "5046.66", "64.149", "104.02", "148.4", "140.58", "218.486", "319.98", "509.91" };
            var value = SharedUtils.GetAccordingValue<decimal>(fields, values, "港元", 4);
            Assert.AreEqual(81.079, value);
        }
    }
}
