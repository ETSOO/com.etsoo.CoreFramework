using com.etsoo.Address;
using com.etsoo.Utils.String;
using NUnit.Framework;

namespace Tests.Utils
{
    /// <summary>
    /// Address region tests
    /// </summary>
    internal class AddressRegionTests
    {
        [Test]
        public void Countries_GetById_Tests()
        {
            // Arrange & act
            var country = AddressRegion.GetById("CN");

            // Assert
            Assert.That(country?.Currency, Is.EqualTo("CNY"));
        }

        [Test]
        public void Countries_GetByIdd_Tests()
        {
            // Arrange & act
            var country = AddressRegion.GetByIdd("64");

            // Assert
            Assert.That(country?.Id, Is.EqualTo("NZ"));
        }

        private static IEnumerable<TestCaseData> CreatePhoneBulkTestData
        {
            get
            {
                yield return new TestCaseData("+8613832922812", "CN", "13832922812", true);
                yield return new TestCaseData("+8653255579200", "CN", "053255579200", false);
            }
        }

        [Test, TestCaseSource(nameof(CreatePhoneBulkTestData))]
        public void Countries_CreatePhone_BulkTests(string phoneNumber, string region, string formatedNumber, bool isMobile)
        {
            // Arrange & act
            var phone = AddressRegion.CreatePhone(phoneNumber);

            Assert.Multiple(() =>
            {
                Assert.That(phone?.Region, Is.EqualTo(region));
                Assert.That(phone?.PhoneNumber, Is.EqualTo(formatedNumber));
                Assert.That(phone?.IsMobile, Is.EqualTo(isMobile));
            });
        }

        [Test]
        public void Countries_CreatePhones_Test()
        {
            // Arrange
            var phoneNumbers = new List<string>
            {
                "+8613832922812", "+86532555792", "53255579200"
            };

            // Act
            var phones = AddressRegion.CreatePhones(phoneNumbers, "CN");

            // Assert
            Assert.That(phones.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CountryPhone_ToInternationalFormat_Tests()
        {
            // Arrange
            var phone = AddressRegion.CreatePhone("0210722065", "NZ");

            // Act 1
            var result1 = phone?.ToInternationalFormat();

            // Assert 1
            Assert.That(result1, Is.EqualTo("+64210722065"));

            // Act 2
            var result2 = phone?.ToInternationalFormat("00");

            // Assert 2
            Assert.That(result2, Is.EqualTo("0064210722065"));
        }

        [Test]
        public void Extensions_UniquePhones_Tests()
        {
            // Arrange
            var phones = AddressRegion.CreatePhones(["13853259135", "+64210722065", "+8613853259135"], "CN");

            // Act & assert
            Assert.That(phones.UniquePhones().Count(), Is.EqualTo(2));
        }

        [Test]
        public void Extensions_JoinAsString_Tests()
        {
            // Arrange
            var data = new Dictionary<string, string>
            {
                ["a"] = "1",
                ["b"] = "2"
            };

            // Act
            var result = data.JoinAsString(",", ";");

            // Assert
            Assert.That(result, Is.EqualTo("a,1;b,2;"));
        }

        [Test]
        public void Extensions_JoinAsQuery_Tests()
        {
            // Arrange
            var data = new Dictionary<string, string>
            {
                ["a"] = "1=2",
                ["b"] = "2&3"
            };

            // Act
            var result = data.JoinAsQuery();

            // Assert
            Assert.That(result, Is.EqualTo("a=1%3d2&b=2%263&"));
        }
    }
}
