using com.etsoo.Utils;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace com.etsoo.ImageUtils
{
    /// <summary>
    /// ImageSharp Utils
    /// https://docs.sixlabors.com/
    /// ImageShart 图片工具类
    /// </summary>
    public static class ImageSharpUtils
    {
        /// <summary>
        /// Create stream from Base64 string
        /// 从 Base64 字符串创建流
        /// </summary>
        /// <param name="input">Input Base64 string</param>
        /// <param name="format">Image format</param>
        /// <returns>Stream</returns>
        public static Stream CreateStreamFromBase64String(string input, out IImageFormat? format)
        {
            // Format
            format = null;

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
                        if (Configuration.Default.ImageFormatsManager.TryFindFormatByMimeType(mime, out var foundFormat))
                        {
                            format = foundFormat;
                        }
                    }
                }
            }

            return SharedUtils.GetStream(Convert.FromBase64String(input.Trim()));
        }

        /// <summary>
        /// Create image from Base64 string and save to the stream
        /// 从 Base64 字符串创建图像并保存到流
        /// </summary>
        /// <param name="input">Input Base64 string</param>
        /// <param name="targetSize">Resizing target size</param>
        /// <param name="targetStream">Stream for saving</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Extension and size in pixel</returns>
        public static async Task<string> CreateFromBase64StringAsync(string input, Size targetSize, Stream targetStream, CancellationToken cancellationToken = default)
        {
            // Parse
            await using var stream = CreateStreamFromBase64String(input, out var format);

            // Resize
            format = await ResizeImageStreamAsync(stream, targetSize, targetStream, format, cancellationToken);

            return format.FileExtensions.First();
        }

        /// <summary>
        /// Async resize image stream keeping original ratio
        /// 异步调整图像流大小，保持原始比例
        /// </summary>
        /// <param name="imageStream">Image stream to resize</param>
        /// <param name="targetSize">Target size</param>
        /// <param name="targetStream">Target stream</param>
        /// <param name="defaultFormat">Image format</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Image format</returns>
        public static async Task<IImageFormat> ResizeImageStreamAsync(Stream imageStream, Size targetSize, Stream targetStream, IImageFormat? defaultFormat = null, CancellationToken cancellationToken = default)
        {
            var width = targetSize.Width;
            var height = targetSize.Height;
            if (width < 10 && height < 10)
            {
                throw new ArgumentException("Both width and height less than 10px are invalid");
            }

            // Source image
            using var image = Image.Load(imageStream);

            // Image format
            var format = defaultFormat ?? image.Metadata.DecodedImageFormat ?? JpegFormat.Instance;

            var sourceSize = image.Size;

            int targetWidth = 0, targetHeight = 0;
            if (width >= 10 && sourceSize.Width >= width)
            {
                targetWidth = width;
                targetHeight = width * sourceSize.Height / sourceSize.Width;
            }
            else if (sourceSize.Height >= height)
            {
                targetHeight = height;
                targetWidth = height * sourceSize.Width / sourceSize.Height;
            }

            if (targetWidth > 0 && targetHeight > 0)
            {
                image.Mutate(x => x.Resize(targetWidth, targetHeight));
            }

            // Save
            await image.SaveAsync(targetStream, format, cancellationToken);

            return format;
        }

        /// <summary>
        /// Async resize image stream with adjustment
        /// 异步调整图像流大小
        /// </summary>
        /// <param name="imageStream">Image stream to resize</param>
        /// <param name="targetSize">Target size</param>
        /// <param name="targetStream">Target stream</param>
        /// <param name="cropSource">Crop source or leave blank for the target</param>
        /// <param name="blankColor">Blank area color</param>
        /// <param name="defaultFormat">Image format</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public static async Task<IImageFormat> ResizeImageStreamAsync(Stream imageStream, Size targetSize, Stream targetStream, bool cropSource = true, Color? blankColor = null, IImageFormat? defaultFormat = null, CancellationToken cancellationToken = default)
        {
            // Source image
            using var image = Image.Load(imageStream);

            // Image format
            var format = defaultFormat ?? image.Metadata.DecodedImageFormat ?? JpegFormat.Instance;

            var sourceSize = image.Size;
            if (sourceSize.Width < targetSize.Width && sourceSize.Height < targetSize.Height)
            {
                // Ignore
            }
            else
            {
                // Size calculation
                var (source, target, isResizing) = ImageShared.Calculate(sourceSize.Width, sourceSize.Height, targetSize.Width, targetSize.Height, cropSource);

                if (isResizing)
                {
                    // Simple mode
                    image.Mutate(x => x.Resize(targetSize));

                }
                else
                {
                    if (cropSource)
                    {
                        var sourceRect = new Rectangle(source.X, source.Y, source.Width, source.Height);
                        image.Mutate(x => x.Crop(sourceRect).Resize(target.Width, target.Height));
                    }
                    else
                    {
                        image.Mutate(x => x.Resize(target.Width, target.Height));
                    }

                    var newImage = new Image<Rgba32>(targetSize.Width, targetSize.Height);
                    newImage.Mutate(x => x
                        .BackgroundColor(blankColor.GetValueOrDefault(Color.Transparent))
                        .DrawImage(image, new Point(target.X, target.Y), 1)
                    );
                }
            }

            await image.SaveAsync(targetStream, format, cancellationToken);

            return format;
        }
    }
}
