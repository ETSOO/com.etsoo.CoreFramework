using AngleSharp.Html.Dom;
using com.etsoo.HtmlUtils;
using com.etsoo.ImageUtils;
using com.etsoo.Utils;
using com.etsoo.Utils.Storage;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace com.etsoo.HtmlIO
{
    /// <summary>
    /// HTML IO Utilities
    /// </summary>
    public static partial class HtmlIOUtils
    {
        /// <summary>
        /// Clear unnecessary tags
        /// 清除不需要的标签
        /// </summary>
        /// <param name="content">Content</param>
        /// <returns>Result</returns>
        public static string? ClearTags(string? content)
        {
            // Remove blanks
            content = content?.Trim();
            if (string.IsNullOrEmpty(content)) return null;

            // Remove empty style property inside tags
            content = StyleRegex().Replace(content, "$1");

            // Remove all "<p><br></p>"
            content = BrRegex().Replace(content, "");

            // Remove empty <p> tags
            content = PgRegex().Replace(content, "").Trim();

            // Return null if no content
            if (string.IsNullOrEmpty(content)) return null;

            // Supplement "<p>" for the first one
            var firstMatch = BlockRegex().Match(content);
            if (!firstMatch.Success)
            {
                content = $"<p>{content}</p>";
            }
            else if (firstMatch.Index > 0)
            {
                var prev = content[..firstMatch.Index];
                var next = content[firstMatch.Index..];
                content = $"<p>{prev}</p>{next}";
            }

            return content;
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
            // Clear tags
            content = ClearTags(content);

            if (string.IsNullOrEmpty(content)) return null;

            await using var stream = SharedUtils.GetStream(content);

            var doc = await HtmlParserExtended.CreateWithCssAsync(stream, cancellationToken: cancellationToken);

            await doc.ManipulateElementsAsync<IHtmlImageElement>("img[src^='data:image/']", async (img) =>
            {
                var source = img.Source;
                if (string.IsNullOrEmpty(source)) return;

                try
                {
                    // Size
                    var size = img.GetSize();
                    var sharpSize = new SixLabors.ImageSharp.Size((int)size.Width, (int)size.Height);

                    await using var stream = SharedUtils.GetStream();
                    var extension = await ImageSharpUtils.CreateFromBase64StringAsync(source, sharpSize, stream, cancellationToken);
                    if (!extension.StartsWith('.')) extension = "." + extension;
                    var filePath = path + Path.GetRandomFileName() + extension;

                    var saveResult = await storage.WriteAsync(filePath, stream, WriteCase.CreateNew, cancellationToken: cancellationToken);
                    if (saveResult)
                    {
                        img.Source = storage.GetUrl(filePath);
                    }
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "Image {source} failed", source);
                }
            });
            return doc.Body?.InnerHtml ?? content;
        }

        [GeneratedRegex(@"(<[^<>]+)\s+style\s*=\s*(['""])\2")]
        private static partial Regex StyleRegex();

        [GeneratedRegex(@"<p><br\/?><\/p>")]
        private static partial Regex BrRegex();

        [GeneratedRegex(@"<p><\/p>")]
        private static partial Regex PgRegex();

        [GeneratedRegex(@"<(p|div|h[1-6]|table|section|header|footer|article|nav|main|form|ul|ol|fieldset|blockquote|pre)[^>]*>")]
        private static partial Regex BlockRegex();
    }
}
