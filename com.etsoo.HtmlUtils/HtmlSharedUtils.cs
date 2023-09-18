using AngleSharp;
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
        /// Create default CSS parser
        /// 创建默认 CSS 解析器
        /// </summary>
        /// <returns></returns>
        public static HtmlParser CreateDefaultCssParser()
        {
            var config = Configuration.Default.WithCss();

            /*
                .WithRenderDevice(new DefaultRenderDevice
                {
                    DeviceHeight = 768,
                    DeviceWidth = 1024,
                });
            */

            var context = BrowsingContext.New(config);

            return new HtmlParser(new HtmlParserOptions(), context);
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
        /// Manipulate HTML elements
        /// 操作HTML元素
        /// </summary>
        /// <param name="stream">HTML stream</param>
        /// <param name="selector">Selector</param>
        /// <param name="action">Action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Document</returns>
        public static Task<IHtmlDocument> ManipulateElementsAsync(Stream stream, string selector, Func<IHtmlElement, Task> action, CancellationToken cancellationToken = default)
        {
            return ManipulateElementsAsync(stream, selector, action, false, cancellationToken);
        }

        /// <summary>
        /// Manipulate HTML elements with CSS
        /// 操作HTML元素并解析样式
        /// </summary>
        /// <param name="stream">HTML stream</param>
        /// <param name="selector">Selector</param>
        /// <param name="action">Action</param>
        /// <param name="parseStyle">Parse style or not</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Document</returns>
        public static Task<IHtmlDocument> ManipulateElementsAsync(Stream stream, string selector, Func<IHtmlElement, Task> action, bool parseStyle, CancellationToken cancellationToken = default)
        {
            return ManipulateElementsAsync<IHtmlElement>(stream, selector, action, parseStyle, cancellationToken);
        }

        /// <summary>
        /// Manipulate HTML elements
        /// 操作HTML元素
        /// </summary>
        /// <param name="stream">HTML stream</param>
        /// <param name="selector">Selector</param>
        /// <param name="action">Action</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Document</returns>
        public static Task<IHtmlDocument> ManipulateElementsAsync<T>(Stream stream, string selector, Func<T, Task> action, CancellationToken cancellationToken = default)
            where T : IHtmlElement
        {
            return ManipulateElementsAsync(stream, selector, action, false, cancellationToken);
        }

        /// <summary>
        /// Manipulate HTML elements
        /// 操作HTML元素
        /// </summary>
        /// <param name="stream">HTML stream</param>
        /// <param name="selector">Selector</param>
        /// <param name="action">Action</param>
        /// <param name="parseStyle">Parse style or not</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Document</returns>
        public async static Task<IHtmlDocument> ManipulateElementsAsync<T>(Stream stream, string selector, Func<T, Task> action, bool parseStyle, CancellationToken cancellationToken = default)
            where T : IHtmlElement
        {
            var parser = parseStyle ? CreateDefaultCssParser() : new HtmlParser();

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
