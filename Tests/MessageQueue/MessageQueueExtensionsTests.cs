using com.etsoo.CoreFramework.Models;
using com.etsoo.MessageQueue;

namespace Tests.MessageQueue
{
    [TestClass]
    public class MessageQueueExtensionsTests
    {
        [TestMethod]
        public async Task BytesTransformationTest()
        {
            var data = new SimpleData { Num = 1, Bool = true };
            var bytes = await MessageQueueUtils.ToJsonBytesAsync(data, default);
            var json = bytes.ToJsonString();
            Assert.AreEqual("""{"num":1,"bool":true}""", json);

            var message = await bytes.ToMessageAsync<SimpleData>(default);

            Assert.IsNotNull(message);
            Assert.AreEqual(data.Num, message?.Num);
            Assert.AreEqual(data.Bool, message?.Bool);
        }

        [TestMethod]
        public async Task MessageQueueUtilsTests()
        {
            var data = new AuthRequest
            {
                AppId = 1,
                AppKey = Guid.NewGuid().ToString(),
                RedirectUri = new Uri("http://localhost"),
                ResponseType = "code",
                Scope = "core super app1",
                State = "state"
            };

            var bytes = await MessageQueueUtils.ToJsonBytesAsync(data, ModelJsonSerializerContext.Default.AuthRequest);
            var message = await MessageQueueUtils.FromJsonBytesAsync(bytes, ModelJsonSerializerContext.Default.AuthRequest);

            Assert.IsNotNull(message);
            Assert.AreEqual(data.AppId, message?.AppId);
            Assert.AreEqual(data.AppKey, message?.AppKey);
            Assert.AreEqual(data.RedirectUri, message?.RedirectUri);
        }
    }
}
