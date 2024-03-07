using com.etsoo.Utils;
using NUnit.Framework;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace Tests.ActionResult
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void SeriliazationTest()
        {
            // Arrange
            var modal = new UserModel
            {
                Id = 1001,
                Name = "Admin 1",
                Status = UserModel.StatusEnum.Deleted,
                UShortValue = 1,
                Secret = "Password"
            };

            // Act
            var json = JsonSerializer.Serialize(modal, SharedUtils.JsonDefaultSerializerOptions);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(json, Does.Contain("secret"));
                Assert.That(json, Does.Contain("***"));
                Assert.That(json, Does.Contain("uShortValue"));
            });
        }

        [Test]
        public async Task ToJsonTest()
        {
            // Arrange
            var modal = new UserModel
            {
                Id = 1001,
                Name = "Admin 1",
                Status = UserModel.StatusEnum.Deleted,
                UShortValue = 1,
                Secret = "Password"
            };

            // Act
            var writer = new ArrayBufferWriter<byte>();
            await modal.ToJsonAsync(writer, SharedUtils.JsonDefaultSerializerOptions);

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(json, Does.Not.Contain("secret"));
                Assert.That(json, Does.Contain("uShortValue"));
            });
        }
    }
}
