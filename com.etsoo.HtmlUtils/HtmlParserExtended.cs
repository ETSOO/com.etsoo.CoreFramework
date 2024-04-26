using AngleSharp;
using AngleSharp.Css;
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
        /// Create HTML parser with CSS and download support
        /// 创建带CSS和下载的HTML解析器
        /// </summary>
        /// <param name="root">Root path</param>
        /// <param name="width">Device width</param>
        /// <param name="height">Device height</param>
        /// <param name="fontSize">Font size</param>
        /// <returns>Result</returns>
        public static HtmlParserExtended CreateWithCssAndDownload(string root = "", int width = 1920, int height = 1080, double fontSize = 16)
        {
            var device = HtmlSharedUtils.CreateRenderDevice(width, height, fontSize);

            var config = Configuration.Default
                .WithCss()
                .With(new HtmlParserRequester(root))
                .With(new DefaultHttpRequester())
                .WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true })
            ;

            return new HtmlParserExtended(config, device);
        }

        /// <summary>
        /// Create document from URL with CSS and download support
        /// </summary>
        /// <param name="uri">URL</param>
        /// <param name="width">Device width</param>
        /// <param name="height">Device height</param>
        /// <param name="fontSize">Font size</param>
        /// <param name="cancellationToken">Cancellation</param>
        /// <returns>Result</returns>
        public static async Task<IDocument> CreateUrlDocumentAsync(string uri, int width = 1920, int height = 1080, double fontSize = 16, CancellationToken cancellationToken = default)
        {
            var device = HtmlSharedUtils.CreateRenderDevice(width, height, fontSize);

            var config = Configuration.Default
                .WithCss()
                .WithRenderDevice(device)
                .With(new DefaultHttpRequester())
                .WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true })
            ;

            var context = BrowsingContext.New(config);

            return await context.OpenAsync(uri, cancellationToken);
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
        /// <returns>Result</returns>
        public HtmlSize GetImageSize(IHtmlImageElement img)
        {
            // Size, with property "width" and "height"
            double width = img.DisplayWidth;
            double height = img.DisplayHeight;

            // Style settings are in priority
            var size = GetStyleSize(img);
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
        /// <returns>Result</returns>
        public HtmlSize GetStyleSize(IElement element)
        {
            var css = element.ComputeCurrentStyle();

            var width = css.GetPixel(PropertyNames.Width);
            var height = css.GetPixel(PropertyNames.Height);

            return new HtmlSize { Width = width.GetValueOrDefault(), Height = height.GetValueOrDefault() };
        }
    }
}
