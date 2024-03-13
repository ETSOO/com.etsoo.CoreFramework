using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;

namespace com.etsoo.HtmlUtils
{
    /// <summary>
    /// HTML shared utils
    /// HTML共享的工具
    /// </summary>
    public static partial class HtmlSharedUtils
    {
        /// <summary>
        /// Create render device
        /// 创建渲染设备
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="fontSize">Default font size</param>
        /// <returns>Result</returns>
        public static DefaultRenderDevice CreateRenderDevice(int width, int height, double fontSize = 16)
        {
            return new DefaultRenderDevice
            {
                DeviceHeight = height,
                DeviceWidth = width,
                ViewPortWidth = width,
                ViewPortHeight = height,
                FontSize = fontSize
            };
        }

        /// <summary>
        /// Create default browsing context
        /// 创建默认的浏览上下文
        /// </summary>
        /// <param name="supportCss">Does support CSS parse</param>
        /// <param name="width">Device width</param>
        /// <param name="height">Devie height</param>
        /// <param name="fontSize">Default font size</param>
        /// <returns>Parser & Render Device</returns>
        public static (IBrowsingContext Context, IRenderDevice RenderDevice) CreateDefaultContext(bool supportCss = true, int width = 1920, int height = 1080, double fontSize = 16)
        {
            var device = CreateRenderDevice(width, height, fontSize);

            if (supportCss)
            {
                var config = Configuration.Default
                    .WithRenderDevice(device)
                    .WithCss();

                return (BrowsingContext.New(config), device);
            }
            else
            {
                return (BrowsingContext.New(Configuration.Default.WithRenderDevice(device)), device);
            }
        }

        /// <summary>
        /// Create default parser with or without CSS support
        /// 创建默认的解析器，支持CSS或不支持
        /// </summary>
        /// <param name="supportCss">Does support CSS parse</param>
        /// <param name="width">Device width</param>
        /// <param name="height">Devie height</param>
        /// <param name="fontSize">Default font size</param>
        /// <returns>Parser & Render Device</returns>
        public static (HtmlParser Parser, IRenderDevice RenderDevice) CreateDefaultParser(bool supportCss = true, int width = 1920, int height = 1080, double fontSize = 16)
        {
            var (context, renderDevice) = CreateDefaultContext(supportCss, width, height, fontSize);
            return (new HtmlParser(new HtmlParserOptions(), context), renderDevice);
        }

        /// <summary>
        /// Remove all tags but keep its content
        /// 删除所有标签但是保留内容
        /// </summary>
        /// <param name="html">HTML string</param>
        /// <returns>Result</returns>
        public static string RemoveAllTags(string html)
        {
            return MyRegex2().Replace(html, string.Empty);
        }

        /// <summary>
        /// Remove line breaks
        /// 删除换行符
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Result</returns>
        public static string RemoveLineBreaks(string input)
        {
            return MyRegex3().Replace(input, string.Empty);
        }

        /// <summary>
        /// Remove script tags and its content
        /// 删除脚本标签和其内容
        /// </summary>
        /// <param name="html">HTML string</param>
        /// <returns>Result</returns>
        public static string RemoveScripts(string html)
        {
            return MyRegex().Replace(html, string.Empty);
        }

        /// <summary>
        /// Remove style tags and its content
        /// 删除样式标签和其内容
        /// </summary>
        /// <param name="html">HTML string</param>
        /// <returns>Result</returns>
        public static string RemoveStyles(string html)
        {
            return MyRegex1().Replace(html, string.Empty);
        }

        /// <summary>
        /// Get introduction
        /// 获取简介
        /// </summary>
        /// <param name="html">HTML string</param>
        /// <param name="maxChars">Max characters excluding lookupText length for the introduction</param>
        /// <param name="lookupText">Lookup text</param>
        /// <param name="isWord">Is English word style</param>
        /// <returns>Result</returns>
        public static string GetIntroduction(string html, int maxChars, string? lookupText = null, bool isWord = false)
        {
            // Remove scripts
            var scriptRemoved = RemoveScripts(html);

            // Remove styles
            var styleRemoved = RemoveStyles(scriptRemoved);

            // All tags
            var tagRemoved = RemoveAllTags(styleRemoved);

            // Remove line breaks
            var content = RemoveLineBreaks(tagRemoved);

            if (isWord)
            {
                // Add blanks
                content = MyRegex4().Replace(content, " ");
            }

            // Remove trailing blanks
            content = content.Trim();

            // Len
            var len = content.Length;

            // Auto increase
            if (lookupText != null)
            {
                maxChars += lookupText.Length;
            }

            if (maxChars >= len)
            {
                return content;
            }

            // Index
            var index = string.IsNullOrEmpty(lookupText) ? -1 : content.IndexOf(lookupText);
            int left, right;

            if (index == -1)
            {
                left = 0;
                right = maxChars;
            }
            else
            {
                left = index - (maxChars - lookupText!.Length) / 2;
                if (left < 0) left = 0;

                right = left+ maxChars;
                if (right > len) right = len;
            }

            if (isWord)
            {
                // Adjust right index for word case
                var wordIndex = content.IndexOf(" ", right - 1);
                if (wordIndex == -1)
                {
                    if (len - right < 10) right = len;
                }
                else
                {
                    right = wordIndex;
                }
            }

            var part = content[left..right];
            return (left > 0 ? "..." : string.Empty) + part + (right < len ? "..." : string.Empty);
        }

        /// <summary>
        /// Get element's style size
        /// 获取元素的样式大小
        /// </summary>
        /// <param name="element">Element</param>
        /// <param name="dimensions">Device's dimension</param>
        /// <param name="currentView">Current view or static style</param>
        /// <returns>Result</returns>
        public static HtmlSize GetStyleSize(this IElement element, IRenderDimensions dimension, bool currentView = false)
        {
            var style = currentView ? element.ComputeCurrentStyle() : element.GetStyle();
            if (style == null) return new HtmlSize();

            var width = style.GetProperty(PropertyNames.Width).RawValue?.AsPx(dimension, RenderMode.Horizontal);
            var height = style.GetProperty(PropertyNames.Height).RawValue?.AsPx(dimension, RenderMode.Vertical);

            return new HtmlSize { Width = width.GetValueOrDefault(), Height = height.GetValueOrDefault() };
        }

        /// <summary>
        /// Get image size
        /// 获取图片大小
        /// </summary>
        /// <param name="img">Image element</param>
        /// <param name="dimensions">Device's dimension</param>
        /// <param name="currentView">Current view or static style</param>
        /// <returns>Result</returns>
        public static HtmlSize GetSize(this IHtmlImageElement img, IRenderDimensions dimension, bool currentView = false)
        {
            // Size, with property "width" and "height"
            double width = img.DisplayWidth;
            double height = img.DisplayHeight;

            // Style settings are in priority
            var size = GetStyleSize(img, dimension, currentView);
            if (size.Width > 0) width = size.Width;
            if (size.Height > 0) height = size.Height;

            if (width == 0)
            {
                // Make sure all implicit large pictures processed
                width = dimension.RenderWidth / 2;
            }

            if (height == 0)
            {
                height = dimension.RenderHeight / 2;
            }

            return new HtmlSize { Width = width, Height = height };
        }

        /// <summary>
        /// Manipulate HTML elements
        /// 操作HTML元素
        /// </summary>
        /// <param name="parser">HTML parser</param>
        /// <param name="stream">HTML stream</param>
        /// <param name="selector">Selector</param>
        /// <param name="action">Action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Document</returns>
        public static Task<IHtmlDocument> ManipulateElementsAsync(this HtmlParser parser, Stream stream, string selector, Func<IHtmlElement, Task> action, CancellationToken cancellationToken = default)
        {
            return ManipulateElementsAsync<IHtmlElement>(parser, stream, selector, action, cancellationToken);
        }

        /// <summary>
        /// Manipulate HTML elements
        /// 操作HTML元素
        /// </summary>
        /// <param name="parser">HTML parser</param>
        /// <param name="stream">HTML stream</param>
        /// <param name="selector">Selector</param>
        /// <param name="action">Action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Document</returns>
        public async static Task<IHtmlDocument> ManipulateElementsAsync<T>(this HtmlParser parser, Stream stream, string selector, Func<T, Task> action, CancellationToken cancellationToken = default)
            where T : IHtmlElement
        {
            var doc = await parser.ParseDocumentAsync(stream, cancellationToken);
            var elements = doc.QuerySelectorAll<T>(selector);
            foreach (var element in elements)
            {
                await action(element);
            }

            return doc;
        }

        /// <summary>
        /// Parse table data
        /// 解析表格数据
        /// </summary>
        /// <typeparam name="T">Row data</typeparam>
        /// <param name="stream">HTML stream</param>
        /// <param name="selector">Table selector</param>
        /// <param name="creator">Row data creator</param>
        /// <param name="titleRowIndex">Title row index, -1 means no titles</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Parsed data and continue or not</returns>
        public async static Task<List<T>> ParseTable<T>(Stream stream, string selector, Func<List<string>?, List<string>, int, (T?, bool)> creator, int titleRowIndex = 0, CancellationToken cancellationToken = default)
        {
            var items = new List<T>();

            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(stream, cancellationToken);
            var element = doc.QuerySelector(selector);

            var startIndex = titleRowIndex + 1;
            if (element is IHtmlTableElement table && table.Rows.Length > startIndex)
            {
                var titles = titleRowIndex < 0 ? null : table.Rows[titleRowIndex].Cells.Select(cell => cell.Text().Trim()).ToList();
                for (var i = startIndex; i < table.Rows.Length; i++)
                {
                    // Row data
                    var data = table.Rows[i].Cells.Select(cell => cell.Text().Trim()).ToList();

                    // Create item
                    // Also could break when pass the index
                    var (item, continueProcess) = creator(titles, data, i - startIndex);

                    // Add to the collection
                    if (item != null)
                        items.Add(item);

                    // Break
                    if (!continueProcess) break;
                }
            }

            return items;
        }

        [GeneratedRegex("<script.*?</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
        private static partial Regex MyRegex();

        [GeneratedRegex("<style.*?</style>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
        private static partial Regex MyRegex1();

        [GeneratedRegex("<.*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
        private static partial Regex MyRegex2();

        [GeneratedRegex("\r\n?|\n", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
        private static partial Regex MyRegex3();

        [GeneratedRegex("(?<=[.\\?!])(?=[^\\s])")]
        private static partial Regex MyRegex4();
    }
}
