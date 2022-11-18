namespace com.etsoo.HtmlUtils
{
    /// <summary>
    /// HTML request service
    /// HTML 内容请求服务
    /// </summary>
    public class HtmlRequestService
    {
        /// <summary>
        /// Http Client
        /// HTTP 客户端
        /// </summary>
        protected readonly HttpClient client;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Http client</param>
        public HtmlRequestService(HttpClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Async get uri stream
        /// 异步获取网址数据流
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Stream</returns>
        public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken token = default)
        {
            return await client.GetStreamAsync(uri, token);
        }
    }
}
