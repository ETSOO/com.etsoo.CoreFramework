using com.etsoo.CoreFramework.Database;
using com.etsoo.Utils.Database;
using com.etsoo.Utils.SpanMemory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
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
            var writer = new StreamBufferWriter<byte>(2048);
            await modal.ToJsonAsync(writer, new JsonSerializerOptions(JsonSerializerDefaults.Web) { IgnoreNullValues = true });

            var json = Encoding.UTF8.GetString(writer.AsMemory().ToArray());

            // Assert
            Assert.IsTrue(json.Contains("keys"));
        }
    }
}
