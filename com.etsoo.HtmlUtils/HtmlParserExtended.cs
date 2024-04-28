using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Dom;
using AngleSharp.Dom.Events;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;

namespace com.etsoo.HtmlUtils
{
    /// <summary>
    /// HTML parser extended
    /// 扩展的HTML解析器
    /// </summary>
    public static class HtmlParserExtended
    {
        /// <summary>
        /// Create HTML document from stream
        /// 从流创建HTML文档
        /// </summary>
        /// <param name="htmlStream">HTML stream</param>
        /// <param name="options">Parse options</param>
        /// <param name="device">Render device</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<IHtmlDocument> CreateAsync(Stream htmlStream, HtmlParserOptions options = new HtmlParserOptions(), IRenderDevice? device = null, CancellationToken cancellationToken = default)
        {
            device ??= HtmlSharedUtils.CreateRenderDevice();
            var config = Configuration.Default.WithRenderDevice(device);
            var parser = new HtmlParser(options, BrowsingContext.New(config));
            return await parser.ParseDocumentAsync(htmlStream, cancellationToken);
        }

        /// <summary>
        /// Create HTML document from URL
        /// 从URL创建HTML文档
        /// </summary>
        /// <param name="url">HTML URL</param>
        /// <param name="device">Render device</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<IDocument> CreateAsync(string url, IRenderDevice? device = null, CancellationToken cancellationToken = default)
        {
            device ??= HtmlSharedUtils.CreateRenderDevice();
            var config = Configuration.Default
                .WithRenderDevice(device)
                .WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = false });
            var context = BrowsingContext.New(config);
            return await context.OpenAsync(url, cancellationToken);
        }

        /// <summary>
        /// Create HTML document from stream with CSS support
        /// 从流创建带CSS支持的HTML文档
        /// </summary>
        /// <param name="htmlStream">HTML stream</param>
        /// <param name="options">Parse options</param>
        /// <param name="device">Render device</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<IHtmlDocument> CreateWithCssAsync(Stream htmlStream, HtmlParserOptions options = new HtmlParserOptions(), IRenderDevice? device = null, CancellationToken cancellationToken = default)
        {
            device ??= HtmlSharedUtils.CreateRenderDevice();
            var config = Configuration.Default.WithRenderDevice(device).WithCss();
            var parser = new HtmlParser(options, BrowsingContext.New(config));
            return await parser.ParseDocumentAsync(htmlStream, cancellationToken);
        }

        /// <summary>
        /// Create HTML document from stream with CSS and resource download support
        /// 从流创建带CSS和资源下载支持的HTML文档
        /// </summary>
        /// <param name="htmlStream">HTML stream</param>
        /// <param name="root">Root path for resource downloading</param>
        /// <param name="onRequested">On resource requested callback</param>
        /// <param name="options">Parse options</param>
        /// <param name="device">Render device</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<IHtmlDocument> CreateWithCssAndDownloadAsync(Stream htmlStream, string root = "", Action<RequestEvent>? onRequested = null, HtmlParserOptions options = new HtmlParserOptions(), IRenderDevice? device = null, CancellationToken cancellationToken = default)
        {
            var localRequester = new HtmlParserRequester(root);
            var defaultRequester = new DefaultHttpRequester();

            if (onRequested != null)
            {
                localRequester.AddEventListener(EventNames.Requested, (obj, env) =>
                {
                    onRequested((RequestEvent)env);
                });
                defaultRequester.AddEventListener(EventNames.Requested, (obj, env) =>
                {
                    onRequested((RequestEvent)env);
                });
            }

            device ??= HtmlSharedUtils.CreateRenderDevice();
            var config = Configuration.Default
                .WithRenderDevice(device)
                .WithCss()
                .With(localRequester)
                .With(defaultRequester)
                .WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true })
            ;
            var parser = new HtmlParser(options, BrowsingContext.New(config));

            return await parser.ParseDocumentAsync(htmlStream, cancellationToken);
        }

        /// <summary>
        /// Create HTML document from URL with CSS and resource download support
        /// 从URL创建带CSS和资源下载支持的HTML文档
        /// </summary>
        /// <param name="url">HTML URL</param>
        /// <param name="device">Render device</param>
        /// <param name="onRequested">On resource requested callback</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<IDocument> CreateWithCssAndDownloadAsync(string url, IRenderDevice? device = null, Action<RequestEvent>? onRequested = null, CancellationToken cancellationToken = default)
        {
            var defaultRequester = new DefaultHttpRequester();

            if (onRequested != null)
            {
                defaultRequester.AddEventListener(EventNames.Requested, (obj, env) =>
                {
                    onRequested((RequestEvent)env);
                });
            }

            device ??= HtmlSharedUtils.CreateRenderDevice();
            var config = Configuration.Default
                .WithRenderDevice(device)
                .WithCss()
                .With(defaultRequester)
                .WithDefaultLoader(new LoaderOptions { IsResourceLoadingEnabled = true })
            ;

            var context = BrowsingContext.New(config);
            return await context.OpenAsync(url, cancellationToken);
        }
    }
}
