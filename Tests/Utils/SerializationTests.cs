using com.etsoo.Utils.Serialization;
using NUnit.Framework;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tests.Utils
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public async Task ToJsonTest()
        {
            // Arrange
            var modal = new UserModel
            {
                Id = 1001,
                Name = "Admin 1",
                Status = UserModel.StatusEnum.Deleted,
                Friends = new int[] { 1, 2, 3 },
                Valid = true,
                Keys = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } }
            };

            // Act
            var writer = new ArrayBufferWriter<byte>();
            await modal.ToJsonAsync(writer, new JsonSerializerOptions(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            // Assert
            Assert.IsTrue(json.Contains("keys"));
        }

        [Test]
        public void GetPropertyCaseInsensitiveTests()
        {
            var json = """{"contactTemplate": "abc"}""";
            var doc = JsonDocument.Parse(json);
            var template = doc.RootElement.GetPropertyCaseInsensitive("ContactTemplate");
            Assert.AreEqual("abc", template?.GetString());
        }
    }
}
