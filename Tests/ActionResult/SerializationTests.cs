using NUnit.Framework;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tests.ActionResult
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public async Task ToJson_Test()
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
    }
}
