using ZXing.Common;

namespace com.etsoo.ImageUtils.Barcode
{
    /// <summary>
    /// Barcode options
    /// 条形码选项
    /// </summary>
    public class BarcodeOptions : EncodingOptions
    {
        /// <summary>
        /// Background color
        /// 背景颜色
        /// </summary>
        public Color Background { get; set; } = Color.Transparent;

        /// <summary>
        /// Background color text
        /// 背景颜色文本
        /// </summary>
        public string BackgroundText
        {
            set
            {
                if (Color.TryParse(value, out var color))
                {
                    Background = color;
                }
            }
        }

        /// <summary>
        /// Foreground color
        /// 前景颜色
        /// </summary>
        public Color Foreground { get; set; } = Color.Black;

        /// <summary>
        /// Foreground color text
        /// 前景颜色文本
        /// </summary>
        public string ForegroundText
        {
            set
            {
                if (Color.TryParse(value, out var color))
                {
                    Foreground = color;
                }
            }
        }

        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        /// <see cref="ZXing.BarcodeFormat"/>
        public string? Type { get; set; }

        /// <summary>
        /// Content
        /// 内容
        /// </summary>
        public required string Content { get; set; }
    }
}
