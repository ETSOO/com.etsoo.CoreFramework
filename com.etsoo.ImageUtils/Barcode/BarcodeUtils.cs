using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;

namespace com.etsoo.ImageUtils.Barcode
{
    /// <summary>
    /// Barcode utils
    /// 条形码工具
    /// </summary>
    public static class BarcodeUtils
    {
        private static BarcodeFormat ParseFormat(string? type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                type = type.Trim().ToUpper();

                if (Enum.TryParse<BarcodeFormat>(type, out var format))
                {
                    return format;
                }
                else
                {
                    var names = Enum.GetNames<BarcodeFormat>();
                    var name = names.FirstOrDefault(name => name.Replace("_", "").Equals(type));
                    if (name != null)
                    {
                        return Enum.Parse<BarcodeFormat>(name);
                    }
                }
            }

            return BarcodeFormat.CODE_128;
        }

        /// <summary>
        /// Create barcode as Base64 string
        /// 将条形码创建为 Base64 字符串
        /// </summary>
        /// <param name="options"Options>Options</param>
        /// <returns>String</returns>
        public static string Create(BarcodeOptions options)
        {
            var writer = new ZXing.ImageSharp.BarcodeWriter<Rgba32>
            {
                Format = ParseFormat(options.Type),
                Options = options,
                Renderer = new ZXing.ImageSharp.Rendering.ImageSharpRenderer<Rgba32>
                {
                    Background = options.Background,
                    Foreground = options.Foreground
                }
            };

            using var image = writer.Write(options.Content);
            return image.ToBase64String(PngFormat.Instance);
        }

        /// <summary>
        /// Async create barcode
        /// 异步创建条形码
        /// </summary>
        /// <param name="stream">Stream to save the barcode</param>
        /// <param name="options">Options</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Task</returns>
        public static async Task CreateAsync(Stream stream, BarcodeOptions options, CancellationToken token = default)
        {
            var writer = new ZXing.ImageSharp.BarcodeWriter<Rgba32>
            {
                Format = ParseFormat(options.Type),
                Options = options,
                Renderer = new ZXing.ImageSharp.Rendering.ImageSharpRenderer<Rgba32>
                {
                    Background = options.Background,
                    Foreground = options.Foreground
                }
            };

            using var image = writer.Write(options.Content);
            await image.SaveAsPngAsync(stream, token);
        }
    }
}
