using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;

namespace com.etsoo.HtmlUtils
{
    /// <summary>
    /// HTML parser extended
    /// 扩展的HTML解析器
    /// </summary>
    public class HtmlParserExtended : HtmlParser
    {
        /// <summary>
        /// Create HTML parser
        /// 创建HTML解析器
        /// </summary>
        /// <param name="width">Device width</param>
        /// <param name="height">Device height</param>
        /// <param name="fontSize">Font size</param>
        /// <returns>Result</returns>
        public static HtmlParserExtended Create(int width = 1920, int height = 1080, double fontSize = 16)
        {
            var device = HtmlSharedUtils.CreateRenderDevice(width, height, fontSize);
            return new HtmlParserExtended(Configuration.Default, device);
        }

        /// <summary>
        /// Create HTML parser with CSS
        /// 创建带CSS的HTML解析器
        /// </summary>
        /// <param name="width">Device width</param>
        /// <param name="height">Device height</param>
        /// <param name="fontSize">Font size</param>
        /// <returns>Result</returns>
        public static HtmlParserExtended CreateWithCss(int width = 1920, int height = 1080, double fontSize = 16)
        {
            var device = HtmlSharedUtils.CreateRenderDevice(width, height, fontSize);
            var config = Configuration.Default.WithCss();
            return new HtmlParserExtended(config, device);
        }

        /// <summary>
        /// Create HTML parser with CSS and download
        /// 创建带CSS和下载的HTML解析器
        /// </summary>
        /// <param name="width">Device width</param>
        /// <param name="height">Device height</param>
        /// <param name="fontSize">Font size</param>
        /// <returns>Result</returns>
        public static HtmlParserExtended CreateWithCssAndDownload(int width = 1920, int height = 1080, double fontSize = 16)
        {
            var device = HtmlSharedUtils.CreateRenderDevice(width, height, fontSize);

            var config = Configuration.Default
                .WithCss()
                .WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true })
            ;

            return new HtmlParserExtended(config, device);
        }

        /// <summary>
        /// Render device
        /// 渲染设备
        /// </summary>
        public readonly IRenderDevice RenderDevice;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="renderDevice">Render device</param>
        /// <param name="options">Parse options</param>
        public HtmlParserExtended(IConfiguration config, IRenderDevice renderDevice, HtmlParserOptions options = new HtmlParserOptions())
            : base(options, BrowsingContext.New(config.WithRenderDevice(renderDevice)))
        {
            RenderDevice = renderDevice;
        }

        /// <summary>
        /// Get image size
        /// 获取图片大小
        /// </summary>
        /// <param name="img">Image element</param>
        /// <param name="currentView">Current view or static style</param>
        /// <returns>Result</returns>
        public HtmlSize GetImageSize(IHtmlImageElement img, bool currentView = false)
        {
            // Size, with property "width" and "height"
            double width = img.DisplayWidth;
            double height = img.DisplayHeight;

            // Style settings are in priority
            var size = GetStyleSize(img, currentView);
            if (size.Width > 0) width = size.Width;
            if (size.Height > 0) height = size.Height;

            if (width == 0)
            {
                // Make sure all implicit large pictures processed
                width = RenderDevice.RenderWidth / 2;
            }

            if (height == 0)
            {
                height = RenderDevice.RenderHeight / 2;
            }

            return new HtmlSize { Width = width, Height = height };
        }

        /// <summary>
        /// Get element's style size
        /// 获取元素的样式大小
        /// </summary>
        /// <param name="element">Element</param>
        /// <param name="currentView">Current view or static style</param>
        /// <returns>Result</returns>
        public HtmlSize GetStyleSize(IElement element, bool currentView = false)
        {
            var style = currentView ? element.ComputeCurrentStyle() : element.GetStyle();
            if (style == null) return new HtmlSize();

            var width = GetPixel(style, PropertyNames.Width);
            var height = GetPixel(style, PropertyNames.Height);

            return new HtmlSize { Width = width.GetValueOrDefault(), Height = height.GetValueOrDefault() };
        }

        private RenderMode NameToRenderMode(string name)
        {
            if (name == PropertyNames.Width
                || name == PropertyNames.MaxWidth
                || name == PropertyNames.MinWidth
                || name == PropertyNames.Left
                || name == PropertyNames.Right
                || name == PropertyNames.MarginLeft
                || name == PropertyNames.MarginRight
                || name == PropertyNames.PaddingLeft
                || name == PropertyNames.PaddingRight
                || name.EndsWith("-x")) return RenderMode.Horizontal;

            return RenderMode.Vertical;
        }

        /// <summary>
        /// Get pixel value
        /// 获取像素值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <param name="mode">Render mode</param>
        /// <returns>Result</returns>
        public double? GetPixel(ICssStyleDeclaration css, string name, RenderMode mode = RenderMode.Undefined)
        {
            var value = css.GetProperty(name)?.RawValue;
            if (value == null) return null;

            if (mode == RenderMode.Undefined)
            {
                mode = NameToRenderMode(name);
            }

            return value.AsPx(RenderDevice, mode);
        }

        /// <summary>
        /// Get pixel float value
        /// 获取像素浮点值
        /// </summary>
        /// <param name="css">Css declaration</param>
        /// <param name="name">Property name</param>
        /// <param name="mode">Render mode</param>
        /// <returns>Result</returns>
        public float? GetPixelF(ICssStyleDeclaration css, string name, RenderMode mode = RenderMode.Undefined)
        {
            var pixel = GetPixel(css, name, mode);
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
        public double? GetPoint(ICssStyleDeclaration css, string name, RenderMode mode = RenderMode.Undefined)
        {
            var pixel = GetPixel(css, name, mode);
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
        public float? GetPointF(ICssStyleDeclaration css, string name, RenderMode mode = RenderMode.Undefined)
        {
            var point = GetPoint(css, name, mode);
            if (point == null) return null;
            return (float)point.Value;
        }
    }
}
