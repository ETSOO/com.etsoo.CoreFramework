using com.etsoo.Utils;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

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
                        if (Configuration.Default.ImageFormatsManager.TryFindFormatByMimeType(mime, out var foundFormat))
                        {
                            format = foundFormat;
                        }
                    }
                }
            }

            format ??= PngFormat.Instance;

            await using var ms = SharedUtils.GetStream(Convert.FromBase64String(input.Trim()));

            using var image = await Image.LoadAsync(ms, token);

            await image.SaveAsync(stream, format, token);

            return format.FileExtensions.First();
        }

        /// <summary>
        /// Async resize image stream keeping original ratio
        /// 异步调整图像流大小，保持原始比例
        /// </summary>
        /// <param name="imageStream">Image stream to resize</param>
        /// <param name="width">Target width</param>
        /// <param name="height">Target height</param>
        /// <param name="targetStream">Target stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public static async Task ResizeImageStreamAsync(Stream imageStream, int? width, int? height, Stream targetStream, CancellationToken cancellationToken = default)
        {
            var w = width.GetValueOrDefault();
            var h = height.GetValueOrDefault();
            if (w < 10 && h < 10)
            {
                throw new ArgumentException("Both width and height are invalid");
            }

            // Source image
            using var image = Image.Load(imageStream);

            var sourceSize = image.Size;

            int targetWidth, targetHeight;
            if (w >= 10)
            {
                if (sourceSize.Width < w)
                {
                    await imageStream.CopyToAsync(targetStream, cancellationToken);
                    return;
                }

                targetWidth = w;
                targetHeight = w * sourceSize.Height / sourceSize.Width;
            }
            else
            {
                if (sourceSize.Height < h)
                {
                    await imageStream.CopyToAsync(targetStream, cancellationToken);
                    return;
                }

                targetHeight = h;
                targetWidth = h * sourceSize.Width / sourceSize.Height;
            }

            // Image format
            var format = image.Metadata.DecodedImageFormat ?? JpegFormat.Instance;

            image.Mutate(x => x.Resize(targetWidth, targetHeight));

            // Save
            await image.SaveAsync(targetStream, format, cancellationToken);
        }

        /// <summary>
        /// Async resize image stream
        /// 异步调整图像流大小
        /// </summary>
        /// <param name="imageStream">Image stream to resize</param>
        /// <param name="targetSize">Target size</param>
        /// <param name="targetStream">Target stream</param>
        /// <param name="cropSource">Crop source or leave blank for the target</param>
        /// <param name="blankColor">Blank area color</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public static async Task ResizeImageStreamAsync(Stream imageStream, Size targetSize, Stream targetStream, bool cropSource = true, Color? blankColor = null, CancellationToken cancellationToken = default)
        {
            // Source image
            using var image = Image.Load(imageStream);

            var sourceSize = image.Size;
            if (sourceSize.Width < targetSize.Width && sourceSize.Height < targetSize.Height)
            {
                await imageStream.CopyToAsync(targetStream, cancellationToken);
                return;
            }

            // Size calculation
            var (source, target, isResizing) = ImageShared.Calculate(sourceSize.Width, sourceSize.Height, targetSize.Width, targetSize.Height, cropSource);

            // Image format
            var format = image.Metadata.DecodedImageFormat ?? JpegFormat.Instance;

            if (isResizing)
            {
                // Simple mode
                image.Mutate(x => x.Resize(targetSize));
                await image.SaveAsync(targetStream, format, cancellationToken);
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
                await newImage.SaveAsync(targetStream, format, cancellationToken);
            }
        }
    }
}
