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
                Assert.That(validator.IsFemale, Is.False);
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
                Assert.That(validator.Valid, Is.True);

                if (validator.Valid)
                {
                    Assert.That(validator.StateNum, Is.EqualTo("53"));
                    Assert.That(validator.CityNum, Is.EqualTo("5301"));
                    Assert.That(validator.DistrictNum, Is.EqualTo("530102"));
                    Assert.That(validator.IsFemale, Is.False);
                    Assert.That(validator.Birthday?.Month, Is.EqualTo(5));
                }
            });
        }
    }
}
