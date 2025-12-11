using com.etsoo.CoreFramework.Authentication;

namespace Tests.Utils
{
    [TestClass]
    public class UtilTests
    {
        [TestMethod]
        public void ValueToEnumTests()
        {
            var item = (UserRole)0;
            Assert.IsFalse(com.etsoo.Utils.SharedUtils.EnumIsDefined(item));

            item = (UserRole)5;
            Assert.IsTrue(com.etsoo.Utils.SharedUtils.EnumIsDefined(item));
        }
    }
}
