using com.etsoo.Utils;

namespace Tests.Utils
{
    [TestClass]
    public class DebouncerTests
    {
        [TestMethod]
        public async Task Debounce_Should_Invoke_Action_After_Delay()
        {
            var tcs = new TaskCompletionSource<string>();

            using var debouncer = new Debouncer<string>(
                (key, _) =>
                {
                    tcs.TrySetResult(key);
                    return Task.CompletedTask;
                },
                scanInterval: 50);

            debouncer.Debounce("A", TimeSpan.FromMilliseconds(100));

            var completed = await Task.WhenAny(
                tcs.Task,
                Task.Delay(TimeSpan.FromSeconds(2)));

            Assert.AreEqual(tcs.Task, completed);
            Assert.AreEqual("A", await tcs.Task);
        }
    }
}
