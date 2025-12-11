using com.etsoo.Address.Validators;

namespace Tests.Address
{
    [TestClass]
    public class ValidatorTests
    {
        [TestMethod]
        public void ChinaPinValidatorFalseTests()
        {
            var validator = new ChinaPinValidator("430124198812081876");

            // Assert
            Assert.IsFalse(validator.Valid);
            Assert.IsFalse(validator.IsFemale);
            Assert.AreEqual(12, validator.Birthday?.Month);
            Assert.AreEqual(8, validator.Birthday?.Day);
        }

        [TestMethod]
        public void ChinaPinValidatorTrueTests()
        {
            var validator = new ChinaPinValidator("53010219200508011x");

            // Assert
            Assert.IsTrue(validator.Valid);

            if (validator.Valid)
            {
                Assert.AreEqual("53", validator.StateNum);
                Assert.AreEqual("5301", validator.CityNum);
                Assert.AreEqual("530102", validator.DistrictNum);
                Assert.IsFalse(validator.IsFemale);
                Assert.AreEqual(5, validator.Birthday?.Month);
            }
        }
    }
}
