using com.etsoo.CoreFramework.Models;
using com.etsoo.MessageQueue;
using NUnit.Framework;

namespace Tests.MessageQueue
{
    [TestFixture]
    internal class MessageQueueExtensionsTests
    {
        [Test]
        public async Task BytesTransformationTest()
        {
            var data = new SimpleData { Num = 1, Bool = true };
            var bytes = await MessageQueueUtils.ToJsonBytesAsync(data, default);
            var json = bytes.ToJsonString();
            Assert.That(json, Is.EqualTo("""{"num":1,"bool":true}"""));

            var message = await bytes.ToMessageAsync<SimpleData>(default);
            Assert.Multiple(() =>
            {
                Assert.That(message, Is.Not.Null);
                Assert.That(message?.Num, Is.EqualTo(data.Num));
                Assert.That(message?.Bool, Is.EqualTo(data.Bool));
            });
        }

        [Test]
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
            Assert.Multiple(() =>
            {
                Assert.That(message, Is.Not.Null);
                Assert.That(message?.AppId, Is.EqualTo(data.AppId));
                Assert.That(message?.AppKey, Is.EqualTo(data.AppKey));
                Assert.That(message?.RedirectUri, Is.EqualTo(data.RedirectUri));
            });
        }
    }
}
