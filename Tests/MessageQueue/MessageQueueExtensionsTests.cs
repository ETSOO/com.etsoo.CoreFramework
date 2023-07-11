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
            Assert.AreEqual("""{"num":1,"bool":true}""", json);

            var message = await bytes.ToMessageAsync<SimpleData>(default);
            Assert.IsNotNull(message);
            Assert.AreEqual(data.Num, message?.Num);
            Assert.AreEqual(data.Bool, message?.Bool);
        }
    }
}
