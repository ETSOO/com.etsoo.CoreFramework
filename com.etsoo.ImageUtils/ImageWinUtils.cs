using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace com.etsoo.ImageUtils
{
    /// <summary>
    /// Windows image shared utilites
    /// Windows下图片共享工具
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class ImageWinUtils
    {
        /// <summary>
        /// Get Codec info
        /// 获取编号信息
        /// </summary>
        /// <param name="path">Image file path</param>
        /// <returns>Codec info</returns>
        public static ImageCodecInfo? GetCodecInfo(string path)
        {
            // Get the file's extension
            var ext = Path.GetExtension(path);

            // No extension
            if (string.IsNullOrEmpty(ext))
                return null;

            // To lower case
            ext = "*" + ext.ToUpper();

            // Find the supported format
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(item => item.FilenameExtension != null && item.FilenameExtension.Split(';').Contains(ext));
        }

        /// <summary>
        /// Get Codec info
        /// 获取编号信息
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>Codec info</returns>
        public static ImageCodecInfo? GetCodecInfo(ImageFormat? format)
        {
            if (format == null)
                return null;

            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(item => item.FormatID == format.Guid);
        }

        /// <summary>
        /// Get encoder parameters
        /// 获取编码器参数
        /// </summary>
        /// <param name="quality">Quality</param>
        /// <param name="colorDepth">Color depth</param>
        /// <param name="compression">Compression</param>
        /// <returns>Encoder parameters</returns>
        public static EncoderParameters GetEncodeParameters(int quality = 100, int? colorDepth = null, long? compression = null)
        {
            // Quality
            var pItems = new List<EncoderParameter>
            {
                new(Encoder.Quality, quality)
            };

            if (colorDepth != null)
            {
                pItems.Add(new(Encoder.ColorDepth, colorDepth.Value));
            }

            if (compression != null)
            {
                pItems.Add(new(Encoder.Compression, compression.Value));
            }

            // Return
            return new EncoderParameters
            {
                Param = [.. pItems]
            };
        }

        /// <summary>
        /// Get image format
        /// 获取图片格式
        /// </summary>
        /// <param name="path">Image file path</param>
        /// <returns>Format</returns>
        public static ImageFormat? GetFormat(string path)
        {
            var encoder = GetCodecInfo(path);
            if (encoder == null)
                return null;

            // Return
            return new ImageFormat(encoder.FormatID);
        }

        /// <summary>
        /// Resize
        /// 调整大小
        /// </summary>
        /// <param name="source">Source stream</param>
        /// <param name="targetSize">Target size</param>
        /// <param name="cropSource">Crop source or leave blank for the target</param>
        /// <param name="blankColor">Blank area color</param>
        /// <returns>Resized bitmap</returns>
        public static Bitmap? Resize(Stream source, Size targetSize, bool cropSource = true, Color? blankColor = null)
        {
            // Parse as image first
            using var image = Image.FromStream(source, false, false);

            // Ignore small size
            if (image.Width <= targetSize.Width && image.Height <= targetSize.Height)
                return null;

            // Continue with Bitmap processing
            using var bm = new Bitmap(image);
            return ResizeInternal(bm, targetSize, cropSource, blankColor);
        }

        /// <summary>
        /// Resize
        /// 调整大小
        /// </summary>
        /// <param name="source">Source bitmap</param>
        /// <param name="targetSize">Target size</param>
        /// <param name="cropSource">Crop source or leave blank for the target</param>
        /// <param name="blankColor">Blank area color</param>
        /// <returns>Resized bitmap</returns>
        public static Bitmap? Resize(Bitmap source, Size targetSize, bool cropSource = true, Color? blankColor = null)
        {
            // Ignore small size
            if (source.Width <= targetSize.Width && source.Height <= targetSize.Height)
                return null;

            return ResizeInternal(source, targetSize, cropSource, blankColor);
        }

        // Resize with size validated
        private static Bitmap ResizeInternal(Bitmap sourceBM, Size targetSize, bool cropSource, Color? blankColor)
        {
            var (source, target, _) = ImageShared.Calculate(sourceBM.Width, sourceBM.Height, targetSize.Width, targetSize.Height, cropSource);

            var destArea = new Rectangle(source.X, source.Y, source.Width, source.Height);
            var sourceArea = new Rectangle(target.X, target.Y, target.Width, target.Height);

            return Resize(sourceBM, destArea, sourceArea, blankColor);
        }

        /// <summary>
        /// Resize
        /// 调整大小
        /// </summary>
        /// <param name="source">Source bitmap</param>
        /// <param name="destArea">Destination area</param>
        /// <param name="sourceArea">Source area</param>
        /// <param name="blankColor">Blank area color</param>
        /// <returns>Resized bitmap</returns>
        public static Bitmap Resize(Bitmap source, Rectangle destArea, Rectangle sourceArea, Color? blankColor = null)
        {
            // Target bitmap
            var resized = new Bitmap(destArea.Width, destArea.Height);

            // Keep transparent
            if (blankColor.HasValue) resized.MakeTransparent(blankColor.Value);
            else resized.MakeTransparent();

            // Target graphics
            using var g = Graphics.FromImage(resized);

            // Quality control
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.CompositingMode = CompositingMode.SourceCopy;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Draw the source to target
            g.DrawImage(source, destArea, sourceArea.X, sourceArea.Y, sourceArea.Width, sourceArea.Height, GraphicsUnit.Pixel);

            // Return
            return resized;
        }

        /// <summary>
        /// Save image to path
        /// 保存图片到路径
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="path">Path</param>
        /// <returns>Result</returns>
        public static bool SaveImage(Image image, string path)
        {
            var codec = GetCodecInfo(path);
            if (codec == null)
                return false;

            image.Save(path, codec, GetEncodeParameters());

            return true;
        }

        /// <summary>
        /// Save image to stream
        /// 保持图片到流
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="stream">Stream</param>
        /// <param name="format">Format</param>
        /// <returns>Result</returns>
        public static bool SaveImage(Image image, Stream stream, ImageFormat? format = null)
        {
            var codec = GetCodecInfo(format ?? image.RawFormat);
            if (codec == null)
                return false;

            image.Save(stream, codec, GetEncodeParameters());

            return true;
        }
    }
}