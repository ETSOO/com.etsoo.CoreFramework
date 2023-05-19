using AngleSharp.Html.Dom;
using com.etsoo.HtmlUtils;
using com.etsoo.Utils;
using NUnit.Framework;

namespace Tests.Utils
{
    internal class HtmlUtilTests
    {
        [Test]
        public async Task ManipulateElementsAsyncTests()
        {
            await using var stream = SharedUtils.GetStream("""<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock 2</span></p>""");
            var count = 0;
            await HtmlSharedUtils.ManipulateElementsAsync(stream, "span.eo-lock", async (IHtmlElement element) =>
            {
                count++;
                await Task.CompletedTask;
            });
            Assert.AreEqual(count, 2);
        }
    }
}
