using com.etsoo.Address.Validators;
using NUnit.Framework;

namespace Tests.Address
{
    [TestFixture]
    internal class ValidatorTests
    {
        [Test]
        public void ChinaPinValidatorFalseTests()
        {
            var validator = new ChinaPinValidator("430124198812081876");
            Assert.IsFalse(validator.Valid);
            Assert.AreEqual("M", validator.Gender);
            Assert.AreEqual(12, validator.Birthday?.Month);
            Assert.AreEqual(8, validator.Birthday?.Day);
        }

        [Test]
        public void ChinaPinValidatorTrueTests()
        {
            var validator = new ChinaPinValidator("53010219200508011x");
            Assert.IsTrue(validator.Valid);
            Assert.AreEqual("M", validator.Gender);
            Assert.AreEqual(5, validator.Birthday?.Month);
        }
    }
}
