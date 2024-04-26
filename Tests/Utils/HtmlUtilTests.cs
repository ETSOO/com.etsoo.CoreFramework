using AngleSharp.Css;
using AngleSharp.Dom;
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
            var parser = HtmlParserExtended.Create();
            var doc = await parser.ManipulateElementsAsync(stream, "span.eo-lock", async (IHtmlElement element) =>
            {
                if (element.TextContent == "Lock 2") element.TextContent = "Lock updated";
                count++;
                await Task.CompletedTask;
            });
            Assert.Multiple(() =>
            {
                Assert.That(count, Is.EqualTo(2));
                Assert.That(doc.Body?.InnerHtml, Is.EqualTo(htmlUpdated));
            });
        }

        [Test]
        public async Task ManipulateElementsAsyncGenericTests()
        {
            var html = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock 2</span></p>""";
            var htmlUpdated = """<p><span class="eo-lock" contenteditable="false">Lock 1</span></p><p><span class="eo-lock" contenteditable="false">Lock updated</span></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var count = 0;
            var parser = HtmlParserExtended.Create();
            var doc = await parser.ManipulateElementsAsync<IHtmlSpanElement>(stream, "span.eo-lock", async (element) =>
            {
                if (element.TextContent == "Lock 2") element.TextContent = "Lock updated";
                count++;
                await Task.CompletedTask;
            });
            Assert.Multiple(() =>
            {
                Assert.That(count, Is.EqualTo(2));
                Assert.That(doc.Body?.InnerHtml, Is.EqualTo(htmlUpdated));
            });
        }

        [Test]
        public async Task ExternalLinkLoadTests()
        {
            var html = """<html><head><title>External Link Test</title><link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC" crossorigin="anonymous" /></head><body><h1>Hello, world!</h1></body></html>""";
            await using var stream = SharedUtils.GetStream(html);
            var parser = HtmlParserExtended.CreateWithCssAndDownload();
            var doc = await parser.ParseDocumentAsync(stream, default);
            await doc.WaitForReadyAsync();
            var downloads = doc.GetDownloads();
            var link = doc.Head?.GetElementsByTagName("link").First() as IHtmlLinkElement;
            var h1 = doc.GetElementsByTagName("h1").First() as IHtmlHeadingElement;
            var h1Style = h1.ComputeCurrentStyle();
            Assert.Multiple(() =>
            {
                Assert.That(doc.StyleSheets.Length, Is.EqualTo(1));
                Assert.That(downloads.Count(), Is.EqualTo(1));
                Assert.That(h1Style.Count, Is.AtLeast(10));
                Assert.That(doc.Title, Is.EqualTo("External Link Test"));
                Assert.That(link, Is.Not.Null);
                Assert.That(h1, Is.Not.Null);
            });
        }

        [Test]
        public async Task ExternalLocalLinkLoadTests()
        {
            var html = """<html><head><link href="file:///D:/Etsoo/com.etsoo.EasyPdf/com.etsoo.EasyPdf.Tests/Resources/html/etsoo.css" rel="stylesheet" /></head><body></body></html>""";
            await using var stream = SharedUtils.GetStream(html);
            var parser = HtmlParserExtended.CreateWithCssAndDownload();
            var doc = await parser.ParseDocumentAsync(stream, default);
            await doc.WaitForReadyAsync();
            var downloads = doc.GetDownloads();
            var docStyle = doc.DocumentElement.ComputeCurrentStyle();
            Assert.Multiple(() =>
            {
                Assert.That(doc.StyleSheets.Length, Is.EqualTo(1));
                Assert.That(downloads.Count(), Is.EqualTo(1));
                Assert.That(docStyle.Count, Is.AtLeast(10));
            });
        }

        [Test]
        public async Task ExternalLocalAboutLinkLoadTests()
        {
            var html = """<html><head><link href="etsoo.css" rel="stylesheet" /></head><body></body></html>""";
            await using var stream = SharedUtils.GetStream(html);
            var parser = HtmlParserExtended.CreateWithCssAndDownload("D:\\Etsoo\\com.etsoo.EasyPdf\\com.etsoo.EasyPdf.Tests\\Resources\\html\\");
            var doc = await parser.ParseDocumentAsync(stream, default);
            await doc.WaitForReadyAsync();
            var downloads = doc.GetDownloads();
            var docStyle = doc.DocumentElement.ComputeCurrentStyle();
            Assert.Multiple(() =>
            {
                Assert.That(doc.StyleSheets.Length, Is.EqualTo(1));
                Assert.That(downloads.Count(), Is.EqualTo(1));
                Assert.That(docStyle.Count, Is.AtLeast(10));
            });
        }

        [Test]
        public async Task LoadExternalUrlTests()
        {
            var doc = await HtmlParserExtended.CreateUrlDocumentAsync("https://www.etsoo.com/");
            Assert.Multiple(() =>
            {
                Assert.That(doc.ContentType, Is.EqualTo("text/html"));
                Assert.That(doc.StyleSheets.Length, Is.AtLeast(1));
            });
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
            Assert.That(introduction, Is.EqualTo("This is a sample HTML string. It contains some content."));
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
            Assert.That(introduction, Is.EqualTo("This is a..."));
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
            Assert.That(introduction, Is.EqualTo("This is a sample..."));
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
            Assert.That(introduction, Is.EqualTo("...is a sample HTML..."));
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
            Assert.That(introduction, Is.EqualTo("这是样本HTML字符..."));
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
            Assert.That(introduction, Is.EqualTo("...ator服务器密码：$EL~10364A37B087CE71093647B200C6031415F9CF6A45F8E2C1AFFCD64D34C3CDADA7+CslGCkWvYrnoNBllHqBpw=="));
        }

        [Test]
        public async Task GetStyleSizeTests()
        {
            var html = """<p><img src="a.jpg" style="width: 10%; height: 20%"/><img src="b.png" height="240" style="width: 120px"/><img src="c.bmp" width="200" style="height: 100pt;"/></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var sizes = new List<HtmlSize>();
            var parser = HtmlParserExtended.CreateWithCss();
            var doc = await parser.ManipulateElementsAsync<IHtmlImageElement>(stream, "img", async (img) =>
            {
                var size = parser.GetImageSize(img);
                sizes.Add(size);
                await Task.CompletedTask;
            });

            Assert.Multiple(() =>
            {
                Assert.That(sizes, Has.Count.EqualTo(3));
                Assert.That(sizes[0].Width, Is.EqualTo(parser.RenderDevice.RenderWidth * 0.1));
                //Assert.That(sizes[0].Height, Is.EqualTo(parser.RenderDevice.RenderHeight * 0.2));
                Assert.That(sizes[1].Width, Is.EqualTo(120));
                Assert.That(sizes[1].Height, Is.EqualTo(240));
                Assert.That(sizes[2].Width, Is.EqualTo(200));
                Assert.That(Math.Round(sizes[2].Height, 2), Is.EqualTo(133.33));
            });
        }

        [Test]
        public async Task SizeUnitTests()
        {
            var html = """<style>img { width: 10%; height: 50em;}</style><p><img src="a.jpg"/></p>""";
            await using var stream = SharedUtils.GetStream(html);
            var parser = HtmlParserExtended.CreateWithCss();
            var doc = await parser.ParseDocumentAsync(stream, default);
            var img = doc.GetElementsByTagName("img").First() as IHtmlImageElement;
            var css = img.ComputeCurrentStyle();
            Assert.Multiple(() =>
            {
                var width = $"{parser.RenderDevice.DeviceWidth * 0.1}px";
                Assert.That(css.GetPropertyValue("width"), Is.EqualTo(width));
                Assert.That(css.GetPixel(PropertyNames.Width), Is.EqualTo(parser.RenderDevice.DeviceWidth * 0.1));
                Assert.That(css.GetPropertyValue("height"), Is.EqualTo("800px"));
                Assert.That(css.GetPixel(PropertyNames.Height), Is.EqualTo(800));
                Assert.That(css.GetPoint(PropertyNames.Height), Is.EqualTo(600));
            });
        }

        [Test]
        public void MakeStartHtmlTagTests()
        {
            var html = " Hello, world! ";
            var result = HtmlIOUtils.MakeStartHtmlTag(html);
            Assert.That(result, Is.EqualTo("<p>Hello, world!</p>"));
        }

        [Test]
        public void MakeStartHtmlTagWithTagsTests()
        {
            var html = " Hello, world! <p>Facilities Offered:</p><hr>";
            var result = HtmlIOUtils.MakeStartHtmlTag(html);
            Assert.That(result, Is.EqualTo("<p>Hello, world! </p><p>Facilities Offered:</p><hr>"));
        }

        [Test]
        public void ClearTagsTests()
        {
            var html = "<p><br></p><ul><li>Guest laundry facility</li></ul><p><br></p><p><br></p><p><br></p>";
            var result = HtmlIOUtils.ClearTags(html);
            Assert.That(result, Is.EqualTo("<ul><li>Guest laundry facility</li></ul>"));
        }
    }
}
