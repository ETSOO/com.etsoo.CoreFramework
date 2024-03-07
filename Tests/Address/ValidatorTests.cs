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
            Assert.Multiple(() =>
            {
                Assert.That(validator.Valid, Is.False);
                Assert.That(validator.Gender, Is.EqualTo("M"));
                Assert.That(validator.Birthday?.Month, Is.EqualTo(12));
                Assert.That(validator.Birthday?.Day, Is.EqualTo(8));
            });
        }

        [Test]
        public void ChinaPinValidatorTrueTests()
        {
            var validator = new ChinaPinValidator("53010219200508011x");
            Assert.Multiple(() =>
            {
                Assert.That(validator.Valid);
                Assert.That(validator.Gender, Is.EqualTo("M"));
                Assert.That(validator.Birthday?.Month, Is.EqualTo(5));
            });
        }
    }
}
