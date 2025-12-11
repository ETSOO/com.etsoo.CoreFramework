using AngleSharp.Css;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using com.etsoo.HtmlIO;
using com.etsoo.HtmlUtils;
using com.etsoo.Utils;

namespace Tests.Utils
{
    [TestClass]
    public class HtmlUtilTests
    {
        [TestMethod]
        public async Task ManipulateElementsAsyncTests()
        {
            var html = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock 2</span></p>""";
            var htmlUpdated = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock updated</span></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var count = 0;
            var doc = await HtmlParserExtended.CreateAsync(stream);
            await doc.ManipulateElementsAsync("span.eo-lock", async (IHtmlElement element) =>
            {
                if (element.TextContent == "Lock 2") element.TextContent = "Lock updated";
                count++;
                await Task.CompletedTask;
            });
            Assert.AreEqual(2, count);
            Assert.AreEqual(htmlUpdated, doc.Body?.InnerHtml);
        }

        [TestMethod]
        public async Task ManipulateElementsAsyncGenericTests()
        {
            var html = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock 2</span></p>""";
            var htmlUpdated = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock updated</span></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var count = 0;
            var doc = await HtmlParserExtended.CreateAsync(stream);
            await doc.ManipulateElementsAsync<IHtmlSpanElement>("span.eo-lock", async (element) =>
            {
                if (element.TextContent == "Lock 2") element.TextContent = "Lock updated";
                count++;
                await Task.CompletedTask;
            });
            Assert.AreEqual(2, count);
            Assert.AreEqual(htmlUpdated, doc.Body?.InnerHtml);
        }

        [TestMethod]
        public async Task ExternalLinkLoadTests()
        {
            var html = """<html><head><title>External Link Test</title><link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous" /></head><body><h1>Hello, world!</h1></body></html>""";
            await using var stream = SharedUtils.GetStream(html);
            var doc = await HtmlParserExtended.CreateWithCssAndDownloadAsync(stream, "", (evt) =>
            {
                Assert.IsTrue(evt.Response?.Address.Href.Contains("bootstrap.min.css") ?? false);
                return Task.CompletedTask;
            });
            await doc.WaitForReadyAsync();
            var downloads = doc.GetDownloads();
            var link = doc.Head?.GetElementsByTagName("link").First() as IHtmlLinkElement;
            var h1 = doc.GetElementsByTagName("h1").First() as IHtmlHeadingElement;
            var h1Style = h1.ComputeCurrentStyle();
            Assert.AreEqual(1, doc.StyleSheets.Length);
            Assert.AreEqual(1, downloads.Count());
            Assert.IsGreaterThanOrEqualTo(10, h1Style.Length);
            Assert.AreEqual("External Link Test", doc.Title);
            Assert.IsNotNull(link);
            Assert.IsNotNull(h1);
        }

        [TestMethod]
        public async Task ExternalLocalLinkLoadTests()
        {
            var html = """<html><head><link href="file:///D:/Etsoo/com.etsoo.EasyPdf/com.etsoo.EasyPdf.Tests/Resources/html/etsoo.css" rel="stylesheet" /></head><body></body></html>""";
            await using var stream = SharedUtils.GetStream(html);
            var doc = await HtmlParserExtended.CreateWithCssAndDownloadAsync(stream);
            await doc.WaitForReadyAsync();
            var downloads = doc.GetDownloads();
            var docStyle = doc.DocumentElement.ComputeCurrentStyle();
            Assert.AreEqual(1, doc.StyleSheets.Length);
            Assert.AreEqual(1, downloads.Count());
            Assert.IsGreaterThanOrEqualTo(10, docStyle.Length);
        }

        [TestMethod]
        public async Task ExternalLocalAboutLinkLoadTests()
        {
            var html = """<html><head><link href="etsoo.css" rel="stylesheet" /></head><body></body></html>""";
            await using var stream = SharedUtils.GetStream(html);
            var doc = await HtmlParserExtended.CreateWithCssAndDownloadAsync(stream, "D:\\Etsoo\\com.etsoo.EasyPdf\\com.etsoo.EasyPdf.Tests\\Resources\\html\\");
            await doc.WaitForReadyAsync();
            var downloads = doc.GetDownloads();
            var docStyle = doc.DocumentElement.ComputeCurrentStyle();
            Assert.AreEqual(1, doc.StyleSheets.Length);
            Assert.AreEqual(1, downloads.Count());
            Assert.IsGreaterThanOrEqualTo(10, docStyle.Length);
        }

        [TestMethod]
        public async Task LoadExternalUrlTests()
        {
            var doc = await HtmlParserExtended.CreateAsync("https://www.etsoo.com/");
            Assert.AreEqual("text/html", doc.ContentType);
            Assert.IsTrue(doc.Title?.Contains("亿速") ?? false);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public async Task GetStyleSizeTests()
        {
            var html = """<p><img src="a.jpg" style="width: 10%; height: 20%"/><img src="b.png" height="240" style="width: 120px"/><img src="c.bmp" width="200" style="height: 100pt;"/></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var sizes = new List<HtmlSize>();
            var doc = await HtmlParserExtended.CreateWithCssAsync(stream);
            await doc.ManipulateElementsAsync<IHtmlImageElement>("img", async (img) =>
            {
                var size = img.GetSize();
                sizes.Add(size);
                await Task.CompletedTask;
            });

            var width = HtmlSharedUtils.DefaultDeviceWidth;

            Assert.HasCount(3, sizes);
            Assert.AreEqual(width * 0.1, sizes[0].Width);
            Assert.AreEqual(width * 0.2, sizes[0].Height);
            Assert.AreEqual(120, sizes[1].Width);
            Assert.AreEqual(240, sizes[1].Height);
            Assert.AreEqual(200, sizes[2].Width);
            Assert.AreEqual(133.33, Math.Round(sizes[2].Height, 2));
        }

        [TestMethod]
        public async Task SizeUnitTests()
        {
            var html = """<style>img { width: 10%; height: 50em; opacity: 0.85}</style><p><img src="a.jpg"/></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var doc = await HtmlParserExtended.CreateWithCssAsync(stream);
            var img = doc.GetElementsByTagName("img").First() as IHtmlImageElement;
            var css = img.ComputeCurrentStyle();
            var width = HtmlSharedUtils.DefaultDeviceWidth;
            Assert.AreEqual(0.85f, css.GetFloatValue(PropertyNames.Opacity));
            Assert.AreEqual($"{width * 0.1}px", css.GetPropertyValue("width"));
            Assert.AreEqual(width * 0.1, css.GetPixel(PropertyNames.Width));
            Assert.AreEqual("800px", css.GetPropertyValue("height"));
            Assert.AreEqual(800, css.GetPixel(PropertyNames.Height));
            Assert.AreEqual(600, css.GetPoint(PropertyNames.Height));
        }

        [TestMethod]
        public void ClearTagsTests()
        {
            var html = " Hello, world! ";
            var result = HtmlIOUtils.ClearTags(html);
            Assert.AreEqual("<p>Hello, world!</p>", result);
        }

        [TestMethod]
        public void ClearTagsAddBlockTests()
        {
            var html = " <b>Hello, world! </b><p>Facilities Offered:</p><hr>";
            var result = HtmlIOUtils.ClearTags(html);
            Assert.AreEqual("<p><b>Hello, world! </b></p><p>Facilities Offered:</p><hr>", result);
        }

        [TestMethod]
        public void ClearTagsRemoveTests()
        {
            var html = "<p><br></p><ul><li>Guest laundry facility</li></ul><p><br></p><p><br></p><p><br></p>";
            var result = HtmlIOUtils.ClearTags(html);
            Assert.AreEqual("<ul><li>Guest laundry facility</li></ul>", result);
        }
    }
}
