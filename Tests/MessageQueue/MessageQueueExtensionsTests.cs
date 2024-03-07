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
    }
}
