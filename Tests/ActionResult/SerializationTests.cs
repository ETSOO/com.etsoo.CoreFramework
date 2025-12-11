using com.etsoo.Utils;
using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace Tests.ActionResult
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
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
            Assert.Contains("secret", json);
            Assert.Contains("***", json);
            Assert.Contains("uShortValue", json);
        }

        [TestMethod]
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
            Assert.DoesNotContain("secret", json);
            Assert.Contains("uShortValue", json);
        }

        [TestMethod]
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
            await result.ToJsonAsync(stream, TestContext.CancellationToken);
            stream.TryGetBuffer(out var bytes);
            var json = Encoding.UTF8.GetString(bytes);

            // Assert
            Assert.AreEqual("{\"data\":{\"id\":1,\"enabled\":true,\"guid\":\"00000000-0000-0000-0000-000000000000\",\"creation\":\"2024-05-18T00:00:00+00:00\"},\"ok\":true,\"title\":\"Success\"}", json);
        }

        [TestMethod]
        public async Task StringIdDataResultTests()
        {
            var id = "ABC";
            var result = com.etsoo.Utils.Actions.ActionResult.Succeed(id);

            await using var stream = SharedUtils.GetStream();
            await result.ToJsonAsync(stream, TestContext.CancellationToken);
            stream.Position = 0;

            var dataResult = await JsonSerializer.DeserializeAsync(stream, CommonJsonSerializerContext.Default.ActionResultStringIdData, TestContext.CancellationToken);
            Assert.AreEqual(id, dataResult?.Data?.Id);
        }

        [TestMethod]
        public async Task IdMsgDataResultTests()
        {
            var id = 12345;
            var msg = "Hello, world!";
            var result = com.etsoo.Utils.Actions.ActionResult.Succeed(id, msg);

            await using var stream = SharedUtils.GetStream();
            await result.ToJsonAsync(stream, TestContext.CancellationToken);
            stream.Position = 0;

            var dataResult = await JsonSerializer.DeserializeAsync(stream, CommonJsonSerializerContext.Default.ActionResultIdMsgData, TestContext.CancellationToken);

            // Assert
            Assert.IsNotNull(dataResult);
            Assert.IsTrue(dataResult!.Ok);

            if (dataResult.Ok is true)
            {
                Assert.AreEqual(id, dataResult.Data.Id);
                Assert.AreEqual(msg, dataResult.Data.Msg);
            }

            var newResult = await dataResult.ToActionResultAsync(CommonJsonSerializerContext.Default.IdMsgData);
            Assert.AreEqual(id, newResult.Data.Get<int>(nameof(id)));
            Assert.AreEqual(msg, newResult.Data.Get(nameof(msg)));
        }

        public TestContext TestContext { get; set; }
    }
}
