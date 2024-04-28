using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Values;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
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
        /// Default device height
        /// 默认设备高度
        /// </summary>
        public const int DefaultDeviceHeight = 1080;

        /// <summary>
        /// Default device width
        /// 默认设备宽度
        /// </summary>
        public const int DefaultDeviceWidth = 1920;

        /// <summary>
        /// Default font size
        /// 默认字体大小
        /// </summary>
        public const double DefaultFontSize = 16;

        /// <summary>
        /// Create render device
        /// 创建渲染设备
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="fontSize">Default font size</param>
        /// <returns>Result</returns>
        public static DefaultRenderDevice CreateRenderDevice(int width = DefaultDeviceWidth, int height = DefaultDeviceHeight, double fontSize = DefaultFontSize)
        {
            return new DefaultRenderDevice
            {
                DeviceHeight = height,
                DeviceWidth = width,
                ViewPortHeight = height,
                ViewPortWidth = width,
                FontSize = fontSize
            };
        }

        /// <summary>
        /// Get double value
        /// 获取双精度值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <returns>Result</returns>
        public static double? GetDoubleValue(this ICssStyleDeclaration css, string name)
        {
            var value = css.GetProperty(name)?.RawValue;

            if (value is CssNumberValue n)
            {
                return n.Value;
            }

            return default;
        }

        /// <summary>
        /// Get float value
        /// 获取浮点值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <returns>Result</returns>
        public static float? GetFloatValue(this ICssStyleDeclaration css, string name)
        {
            var value = css.GetDoubleValue(name);
            if (value.HasValue)
            {
                return (float)value.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get integer value
        /// 获取整数值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <returns>Result</returns>
        public static int? GetIntValue(this ICssStyleDeclaration css, string name)
        {
            var value = css.GetProperty(name)?.RawValue;

            if (value is CssIntegerValue n)
            {
                return n.IntValue;
            }

            return default;
        }

        /// <summary>
        /// Get pixel value
        /// For computed current style, the value is in px
        /// 获取像素值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <returns>Result</returns>
        public static double? GetPixel(this ICssStyleDeclaration css, string name)
        {
            var value = css.GetProperty(name)?.RawValue;

            if (value is CssLengthValue l && l.Type == CssLengthValue.Unit.Px)
            {
                return l.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get pixel float value
        /// 获取像素浮点值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <param name="mode">Render mode</param>
        /// <returns>Result</returns>
        public static float? GetPixelF(this ICssStyleDeclaration css, string name)
        {
            var pixel = css.GetPixel(name);
            if (pixel == null) return null;
            return (float)pixel.Value;
        }

        /// <summary>
        /// Get point value
        /// 获取点值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <param name="mode">Render mode</param>
        /// <returns>Result</returns>
        public static double? GetPoint(this ICssStyleDeclaration css, string name)
        {
            var pixel = css.GetPixel(name);
            if (pixel == null) return null;
            return pixel.Value * 0.75;
        }

        /// <summary>
        /// Get point float value
        /// 获取点浮点值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <param name="mode">Render mode</param>
        /// <returns>Result</returns>
        public static float? GetPointF(this ICssStyleDeclaration css, string name)
        {
            var point = css.GetPoint(name);
            if (point == null) return null;
            return (float)point.Value;
        }

        /// <summary>
        /// Get image size
        /// 获取图片大小
        /// </summary>
        /// <param name="img">Image element</param>
        /// <returns>Result</returns>
        public static HtmlSize GetSize(this IHtmlImageElement img)
        {
            // Size, with property "width" and "height"
            double width = img.DisplayWidth;
            double height = img.DisplayHeight;

            // Style settings are in priority
            var size = GetStyleSize(img);
            if (size.Width > 0) width = size.Width;
            if (size.Height > 0) height = size.Height;

            if (width == 0)
            {
                width = 800;
            }

            return new HtmlSize { Width = width, Height = height };
        }

        /// <summary>
        /// Get element's style size
        /// 获取元素的样式大小
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Result</returns>
        public static HtmlSize GetStyleSize(this IElement element)
        {
            var css = element.ComputeCurrentStyle();

            var width = css.GetPixel(PropertyNames.Width);
            var height = css.GetPixel(PropertyNames.Height);

            return new HtmlSize { Width = width.GetValueOrDefault(), Height = height.GetValueOrDefault() };
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
        /// <param name="document">HTML Document</param>
        /// <param name="selector">Selector</param>
        /// <param name="action">Action</param>
        /// <returns>Document</returns>
        public static Task ManipulateElementsAsync(this IDocument document, string selector, Func<IHtmlElement, Task> action)
        {
            return ManipulateElementsAsync<IHtmlElement>(document, selector, action);
        }

        /// <summary>
        /// Manipulate HTML elements
        /// 操作HTML元素
        /// </summary>
        /// <param name="document">HTML Document</param>
        /// <param name="selector">Selector</param>
        /// <param name="action">Action</param>
        /// <returns>Document</returns>
        public static async Task ManipulateElementsAsync<T>(this IDocument document, string selector, Func<T, Task> action)
            where T : IHtmlElement
        {
            var elements = document.QuerySelectorAll<T>(selector);
            foreach (var element in elements)
            {
                await action(element);
            }
        }

        /// <summary>
        /// Parse table data
        /// 解析表格数据
        /// </summary>
        /// <typeparam name="T">Row data</typeparam>
        /// <param name="document">HTML Document</param>
        /// <param name="selector">Selector</param>
        /// <param name="creator">Row data creator</param>
        /// <param name="titleRowIndex">Title row index, -1 means no titles</param>
        /// <returns>Parsed data and continue or not</returns>
        public static List<T> ParseTable<T>(this IDocument document, string selector, Func<List<string>?, List<string>, int, (T?, bool)> creator, int titleRowIndex = 0)
        {
            var items = new List<T>();

            var element = document.QuerySelector(selector);

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
