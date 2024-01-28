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

            // Assert
            Assert.IsTrue(json.Contains("secret"));
            Assert.IsTrue(json.Contains("***"));
            Assert.IsTrue(json.Contains("uShortValue"));
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

            // Assert
            Assert.IsFalse(json.Contains("secret"));
            Assert.IsTrue(json.Contains("uShortValue"));
        }
    }
}
