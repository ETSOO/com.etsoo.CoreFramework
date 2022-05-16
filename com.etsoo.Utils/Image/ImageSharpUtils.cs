using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;

namespace com.etsoo.Utils.Image
{
    /// <summary>
    /// ImageSharp Utils
    /// https://docs.sixlabors.com/
    /// ImageShart 图片工具类
    /// </summary>
    public static class ImageSharpUtils
    {
        /// <summary>
        /// Create image from Base64 string and save to the stream
        /// 从 Base64 字符串创建图像并保存到流
        /// </summary>
        /// <param name="input">Input Base64 string</param>
        /// <param name="stream">Stream</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Extension</returns>
        public static async Task<string> CreateFromBase64StringAsync(string input, Stream stream, CancellationToken token = default)
        {
            // Format
            IImageFormat? format = null;

            // Parse base64 string
            var index = input.IndexOf(',');
            if (index is not -1)
            {
                // Schema
                var schema = input[..index];

                // Truncate Base64 data
                input = input[(index + 1)..];

                // data index
                index = schema.IndexOf("data:", StringComparison.OrdinalIgnoreCase);
                if (index is not -1)
                {
                    // 5 is length of "data:"
                    index += 5;

                    var endIndex = schema.IndexOf(';', index + 1);
                    if (endIndex is not -1)
                    {
                        // MIME like image/png
                        var mime = schema[index..endIndex].Trim();

                        // Find format
                        format = Configuration.Default.ImageFormatsManager.FindFormatByMimeType(mime);
                    }
                }
            }

            format ??= PngFormat.Instance;

            using var ms = SharedUtils.GetStream(Convert.FromBase64String(input.Trim()));

            using var image = await SixLabors.ImageSharp.Image.LoadAsync(ms, token);

            await image.SaveAsync(stream, format, token);

            return format.FileExtensions.First();
        }
    }
}
