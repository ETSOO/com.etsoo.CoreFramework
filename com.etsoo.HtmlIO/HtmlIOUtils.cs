using AngleSharp.Html.Dom;
using com.etsoo.HtmlUtils;
using com.etsoo.ImageUtils;
using com.etsoo.Utils;
using com.etsoo.Utils.Storage;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace com.etsoo.HtmlIO
{
    /// <summary>
    /// HTML IO Utilities
    /// </summary>
    public static class HtmlIOUtils
    {
        /// <summary>
        /// Clear unnecessary tags
        /// 清除不需要的标签
        /// </summary>
        /// <param name="content">Content</param>
        /// <returns>Result</returns>
        public static string ClearTags(string content)
        {
            return content.Replace("<p><br></p>", "");
        }

        /// <summary>
        /// Async format editor content
        /// 异步格式编辑器内容
        /// </summary>
        /// <param name="storage">Storage for images</param>
        /// <param name="path">Storage path</param>
        /// <param name="content">Raw content</param>
        /// <param name="logger">Logger</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<string?> FormatEditorContentAsync(IStorage storage, string path, string? content, ILogger? logger = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(content)) return null;

            // Clear tags
            content = ClearTags(content);

            // Start with HTML tag
            content = MakeStartHtmlTag(content);

            await using var stream = SharedUtils.GetStream(content);
            var doc = await HtmlSharedUtils.ManipulateElementsAsync<IHtmlImageElement>(stream, "img[src^='data:image/']", async (img) =>
            {
                var source = img.Source;
                if (string.IsNullOrEmpty(source)) return;

                try
                {
                    // Size
                    var width = img.DisplayWidth;
                    var height = img.DisplayHeight;

                    if (width == 0 && height == 0)
                    {
                        var (styleWidth, styleHeight) = HtmlSharedUtils.GetStyleSize(img);
                        if (styleWidth.HasValue && styleWidth.Value > 0) width = (int)styleWidth.Value;
                        if (styleHeight.HasValue && styleHeight.Value > 0) height = (int)styleHeight.Value;
                    }

                    if (width == 0 && height == 0)
                    {
                        // Make sure all implicit large pictures processed
                        width = HtmlSharedUtils.DefaultRenderDevice.DeviceWidth / 2;
                    }

                    var size = new Size(width, height);

                    await using var stream = SharedUtils.GetStream();
                    var extension = await ImageSharpUtils.CreateFromBase64StringAsync(source, size, stream, cancellationToken);
                    if (!extension.StartsWith(".")) extension = "." + extension;
                    var filePath = path + Path.GetRandomFileName() + extension;

                    var saveResult = await storage.WriteAsync(filePath, stream, WriteCase.CreateNew);
                    if (saveResult)
                    {
                        img.Source = storage.GetUrl(filePath);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "Image {source} failed", source);
                }
            }, true, cancellationToken);
            return doc.Body?.InnerHtml ?? content;
        }

        /// <summary>
        /// Make start HTML tag
        /// 添加开始 HTML 标签
        /// </summary>
        /// <param name="content">Raw content</param>
        /// <returns>Result</returns>
        public static string MakeStartHtmlTag(string content)
        {
            // Start with HTML tag
            content = content.Trim();

            if (!content.StartsWith('<'))
            {
                var index = content.IndexOf('<');
                if (index == -1) content = $"<p>{content}</p>";
                else
                {
                    content = $"<p>{content[0..index]}</p>{content[index..]}";
                }
            }

            return content;
        }
    }
}
