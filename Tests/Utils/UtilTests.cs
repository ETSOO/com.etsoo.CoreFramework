using com.etsoo.CoreFramework.Authentication;
using NUnit.Framework;

namespace Tests.Utils
{
    [TestFixture]
    public class UtilTests
    {
        [Test]
        public void ValueToEnumTests()
        {
            var item = (UserRole)0;
            Assert.IsFalse(com.etsoo.Utils.SharedUtils.EnumIsDefined(item));

            item = (UserRole)3;
            Assert.IsTrue(com.etsoo.Utils.SharedUtils.EnumIsDefined(item));
        }
    }
}
