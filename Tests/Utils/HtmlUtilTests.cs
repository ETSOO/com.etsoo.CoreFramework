using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Values;
using AngleSharp.Html.Dom;
using com.etsoo.HtmlIO;
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
            var html = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock 2</span></p>""";
            var htmlUpdated = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock updated</span></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var count = 0;
            var doc = await HtmlSharedUtils.ManipulateElementsAsync(stream, "span.eo-lock", async (IHtmlElement element) =>
            {
                if (element.TextContent == "Lock 2") element.TextContent = "Lock updated";
                count++;
                await Task.CompletedTask;
            });
            Assert.AreEqual(count, 2);
            Assert.AreEqual(htmlUpdated, doc.Body?.InnerHtml);
        }

        [Test]
        public async Task ManipulateElementsAsyncGenericTests()
        {
            var html = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock 2</span></p>""";
            var htmlUpdated = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock updated</span></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var count = 0;
            var doc = await HtmlSharedUtils.ManipulateElementsAsync<IHtmlSpanElement>(stream, "span.eo-lock", async (element) =>
            {
                if (element.TextContent == "Lock 2") element.TextContent = "Lock updated";
                count++;
                await Task.CompletedTask;
            });
            Assert.AreEqual(count, 2);
            Assert.AreEqual(htmlUpdated, doc.Body?.InnerHtml);
        }

        [Test]
        public async Task ManipulateElementsWithStyleAsyncGenericTests()
        {
            var html = """<p><img src="a.jpg" style="width: 10%;"/><img src="b.png" style="width: 120px"/><img src="c.bmp" style="width: 100pt;"/></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var widths = new List<double>();
            var device = HtmlSharedUtils.DefaultRenderDevice;
            var doc = await HtmlSharedUtils.ManipulateElementsAsync<IHtmlImageElement>(stream, "img", async (img) =>
            {
                var style = img.GetStyle();
                if (Length.TryParse(style.GetWidth(), out var width))
                {
                    widths.Add(width.AsPx(device, RenderMode.Horizontal));
                }

                await Task.CompletedTask;
            }, true);

            Assert.AreEqual(3, widths.Count);
            Assert.AreEqual(device.RenderWidth * 0.1, widths[0]);
            Assert.AreEqual(120, widths[1]);
            Assert.AreEqual(133.33, Math.Round(widths[2], 2));
        }

        [Test]
        public void GetIntroduction_NoLookupText_ReturnsIntroductionWithinMaxChars()
        {
            // Arrange
            string html = "<script>var a = 1;</script><p>This is a sample HTML string.</p><p>It contains some content.</p><style>body{}</style>";
            ushort maxChars = 55;

            // Act
            string introduction = HtmlSharedUtils.GetIntroduction(html, maxChars, null, true);

            // Assert
            Assert.AreEqual("This is a sample HTML string. It contains some content.", introduction);
        }

        [Test]
        public void GetIntroduction_NoLookupText_ContentExceedsMaxChars_ReturnsTrimmedIntroduction()
        {
            // Arrange
            string html = "<p>This is a sample HTML string.</p><p>It contains some content.</p>";
            ushort maxChars = 10;

            // Act
            string introduction = HtmlSharedUtils.GetIntroduction(html, maxChars, null, true);

            // Assert
            Assert.AreEqual("This is a...", introduction);
        }

        [Test]
        public void GetIntroduction_LookupTextFound_LessCharsThanMaxChars_ReturnsLookupText()
        {
            // Arrange
            string html = "<p>This is a sample HTML string.</p><p>It contains some content.</p>";
            ushort maxChars = 12;
            string lookupText = "is a";

            // Act
            string introduction = HtmlSharedUtils.GetIntroduction(html, maxChars, lookupText, true);

            // Assert
            Assert.AreEqual("This is a sample...", introduction);
        }

        [Test]
        public void GetIntroduction_LookupTextFound_MoreCharsThanMaxChars_ReturnsLookupText()
        {
            // Arrange
            string html = "<p>This is a sample HTML string.</p><p>It contains some content.</p>";
            ushort maxChars = 10;
            string lookupText = "sample";

            // Act
            string introduction = HtmlSharedUtils.GetIntroduction(html, maxChars, lookupText, true);

            // Assert
            Assert.AreEqual("...is a sample HTML...", introduction);
        }

        [Test]
        public void GetIntroductionChinese_LookupTextFound_MoreCharsThanMaxChars_ReturnsLookupText()
        {
            // Arrange
            string html = "<p>这是样本HTML字符串。</p><p>考虑字符串合并</p>";
            ushort maxChars = 8;
            string lookupText = "样本";

            // Act
            string introduction = HtmlSharedUtils.GetIntroduction(html, maxChars, lookupText);

            // Assert
            Assert.AreEqual("这是样本HTML字符...", introduction);
        }

        [Test]
        public void GetIntroduction_RealCase_ReturnsLookupText()
        {
            // Arrange
            string html = """服务器地址：qd.etsoo.com<p>服务器用户名：administrator</p><p>服务器密码：<span class="eo-lock" contenteditable="false">$EL~10364A37B087CE71093647B200C6031415F9CF6A45F8E2C1AFFCD64D34C3CDADA7+CslGCkWvYrnoNBllHqBpw==</span></p>""";
            ushort maxChars = 20;
            string lookupText = "$EL~10364A37B087CE71093647B200C6031415F9CF6A45F8E2C1AFFCD64D34C3CDADA7+CslGCkWvYrnoNBllHqBpw==";

            // Act
            string introduction = HtmlSharedUtils.GetIntroduction(html, maxChars, lookupText);

            // Assert
            Assert.AreEqual("...ator服务器密码：$EL~10364A37B087CE71093647B200C6031415F9CF6A45F8E2C1AFFCD64D34C3CDADA7+CslGCkWvYrnoNBllHqBpw==", introduction);
        }

        [Test]
        public async Task GetStyleSizeTests()
        {
            var html = """<p><img src="a.jpg" style="width: 10%; height: 20%"/><img src="b.png" style="width: 120px"/><img src="c.bmp" style="height: 100pt;"/></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var sizes = new List<(double? width, double? height)>();
            var device = HtmlSharedUtils.DefaultRenderDevice;
            var doc = await HtmlSharedUtils.ManipulateElementsAsync<IHtmlImageElement>(stream, "img", async (img) =>
            {
                sizes.Add(HtmlSharedUtils.GetStyleSize(img, device));
                await Task.CompletedTask;
            }, true);

            Assert.AreEqual(3, sizes.Count);
            Assert.AreEqual((device.RenderWidth * 0.1, device.RenderHeight * 0.2), sizes[0]);
            Assert.AreEqual(120, sizes[1].width);
            Assert.IsNull(sizes[1].height);
            Assert.IsNull(sizes[2].width);
            Assert.AreEqual(133.33, Math.Round(sizes[2].height.GetValueOrDefault(), 2));
        }

        [Test]
        public void MakeStartHtmlTagTests()
        {
            var html = " Hello, world! ";
            var result = HtmlIOUtils.MakeStartHtmlTag(html);
            Assert.AreEqual("<p>Hello, world!</p>", result);
        }

        [Test]
        public void ClearTagsTests()
        {
            var html = "<p><br></p><ul><li>Guest laundry facility</li></ul><p><br></p><p><br></p><p><br></p>";
            var result = HtmlIOUtils.ClearTags(html);
            Assert.AreEqual("<ul><li>Guest laundry facility</li></ul>", result);
        }
    }
}
