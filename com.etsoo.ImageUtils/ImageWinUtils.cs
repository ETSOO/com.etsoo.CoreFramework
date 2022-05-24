using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

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
                Param = pItems.ToArray()
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
        /// <param name="cropSource">Crop source or leave blank for the target, null means as exact size as possible with crop</param>
        /// <returns>Resized bitmap</returns>
        public static Bitmap? Resize(Stream source, Size targetSize, bool cropSource)
        {
            // Parse as image first
            using var image = Image.FromStream(source, false, false);

            // Ignore small size
            if (image.Width <= targetSize.Width && image.Height <= targetSize.Height)
                return null;

            // Continue with Bitmap processing
            using var bm = new Bitmap(image);
            return ResizeInternal(bm, targetSize, cropSource);
        }

        /// <summary>
        /// Resize
        /// 调整大小
        /// </summary>
        /// <param name="source">Source bitmap</param>
        /// <param name="targetSize">Target size</param>
        /// <param name="cropSource">Crop source or leave blank for the target, null means as exact size as possible with crop</param>
        /// <returns>Resized bitmap</returns>
        public static Bitmap? Resize(Bitmap source, Size targetSize, bool cropSource)
        {
            // Ignore small size
            if (source.Width <= targetSize.Width && source.Height <= targetSize.Height)
                return null;

            return ResizeInternal(source, targetSize, cropSource);
        }

        // Resize with size validated
        private static Bitmap ResizeInternal(Bitmap source, Size targetSize, bool cropSource)
        {
            // Source size
            var sw = source.Width;
            var sh = source.Height;

            // Target size
            var tw = targetSize.Width;
            var th = targetSize.Height;

            // Ratio
            var rw = (float)sw / tw;
            var rh = (float)sh / th;

            Rectangle destArea, sourceArea;

            if (cropSource)
            {
                // Crop content in center
                int x = 0, y = 0;
                if (rw > rh)
                {
                    var newSW = Convert.ToInt32(tw * rh);
                    x = (sw - newSW) / 2;
                    sw = newSW;
                }
                else
                {
                    var newSH = Convert.ToInt32(th * rw);
                    y = (sh - newSH) / 2;
                    sh = newSH;
                }

                // Avoid zoom out
                if (tw > sw)
                    tw = sw;
                if (th > sh)
                    th = sh;

                sourceArea = new Rectangle(x, y, sw, sh);
                destArea = new Rectangle(0, 0, tw, th);
            }
            else
            {
                // All content in center with target width or height
                if (rw > rh)
                {
                    // Reduce target height
                    th = Convert.ToInt32(sh / rw);
                }
                else
                {
                    // Reduce target width
                    tw = Convert.ToInt32(sw / rh);
                }

                sourceArea = new Rectangle(0, 0, sw, sh);
                destArea = new Rectangle(0, 0, tw, th);
            }

            return Resize(source, destArea, sourceArea);
        }

        /// <summary>
        /// Resize
        /// 调整大小
        /// </summary>
        /// <param name="source">Source bitmap</param>
        /// <param name="destArea">Destination area</param>
        /// <param name="sourceArea">Source area</param>
        /// <returns>Resized bitmap</returns>
        public static Bitmap Resize(Bitmap source, Rectangle destArea, Rectangle sourceArea)
        {
            // Target bitmap
            var resized = new Bitmap(destArea.Width, destArea.Height);

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
        public static bool SaveImage(System.Drawing.Image image, string path)
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
        public static bool SaveImage(System.Drawing.Image image, Stream stream, ImageFormat? format = null)
        {
            var codec = GetCodecInfo(format ?? image.RawFormat);
            if (codec == null)
                return false;

            image.Save(stream, codec, GetEncodeParameters());

            return true;
        }
    }
}
