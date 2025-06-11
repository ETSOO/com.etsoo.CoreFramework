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
            Assert.That(com.etsoo.Utils.SharedUtils.EnumIsDefined(item), Is.False);

            item = (UserRole)5;
            Assert.That(com.etsoo.Utils.SharedUtils.EnumIsDefined(item), Is.True);
        }
    }
}
