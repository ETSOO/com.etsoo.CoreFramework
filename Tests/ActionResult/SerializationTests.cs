using com.etsoo.Utils.Serialization;
using NUnit.Framework;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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
            var json = JsonSerializer.Serialize(modal, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { SerializationExtensions.PIIAttributeMaskingPolicy }
                }
            });

            // Assert
            Assert.IsFalse(json.Contains("secret"));
            Assert.IsFalse(json.Contains("uShortValue"));
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
            await modal.ToJsonAsync(writer, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            // Assert
            Assert.IsFalse(json.Contains("secret"));
            Assert.IsFalse(json.Contains("uShortValue"));
        }
    }
}
