using com.etsoo.Utils;
using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
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

        [Test]
        public async Task ToJsonTest2()
        {
            // Arrange
            var result = new com.etsoo.Utils.Actions.ActionResult
            {
                Ok = true,
                Title = "Success",
                Data = new StringKeyDictionaryObject
                {
                    { "id", 1L },
                    { "enabled", true },
                    { "guid", Guid.Empty },
                    { "creation", DateTimeOffset.Parse("2024/05/18 00:00:00+00:00") }
                }
            };

            // Act
            await using var stream = SharedUtils.GetStream();
            await result.ToJsonAsync(stream);
            stream.TryGetBuffer(out var bytes);
            var json = Encoding.UTF8.GetString(bytes);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(json, Is.EqualTo("{\"data\":{\"id\":1,\"enabled\":true,\"guid\":\"00000000-0000-0000-0000-000000000000\",\"creation\":\"2024-05-18T00:00:00+00:00\"},\"ok\":true,\"title\":\"Success\"}"));
            });
        }

        [Test]
        public async Task StringIdDataResultTests()
        {
            var id = "ABC";
            var result = com.etsoo.Utils.Actions.ActionResult.Succeed(id);

            await using var stream = SharedUtils.GetStream();
            await result.ToJsonAsync(stream);
            stream.Position = 0;

            var dataResult = await JsonSerializer.DeserializeAsync(stream, CommonJsonSerializerContext.Default.ActionResultStringIdData);
            Assert.That(dataResult?.Data.Id, Is.EqualTo(id));
        }

        [Test]
        public async Task IdMsgDataResultTests()
        {
            var id = 12345;
            var msg = "Hello, world!";
            var result = com.etsoo.Utils.Actions.ActionResult.Succeed(id, msg);

            await using var stream = SharedUtils.GetStream();
            await result.ToJsonAsync(stream);
            stream.Position = 0;

            var dataResult = await JsonSerializer.DeserializeAsync(stream, CommonJsonSerializerContext.Default.ActionResultIdMsgData);
            Assert.Multiple(() =>
            {
                Assert.That(dataResult?.Data.Id, Is.EqualTo(id));
                Assert.That(dataResult?.Data.Msg, Is.EqualTo(msg));
            });
        }
    }
}
