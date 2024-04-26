using AngleSharp.Io;
using AngleSharp.Text;
using System.Net;

namespace com.etsoo.HtmlUtils
{
    /// <summary>
    /// HTML parser requester for local files
    /// HTML本地文件解析器请求器
    /// </summary>
    public class HtmlParserRequester : BaseRequester
    {
        private readonly string root;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="root">Root path</param>
        public HtmlParserRequester(string root = "")
        {
            this.root = root;
        }

        /// <summary>
        /// Supports protocol (file, about)
        /// 支持的协议
        /// </summary>
        /// <param name="protocol">Protocol</param>
        /// <returns>Result</returns>
        public override bool SupportsProtocol(string protocol)
        {
            return protocol.IsOneOf(ProtocolNames.File, "about");
        }

        protected override async Task<IResponse?> PerformRequestAsync(Request request, CancellationToken cancel)
        {
            var address = request.Address;

            // file:/// or blank:///
            var start = address.Protocol.Length + 3;
            var path = Path.GetFullPath(root + address.Href[start..]);

            if (Path.Exists(path))
            {
                // Keep the stream open
                var stream = new MemoryStream();

                // Copy the file to the stream
                await File.OpenRead(path).CopyToAsync(stream, cancel);
                stream.Position = 0;

                // Create the response
                var response = new DefaultResponse
                {
                    Content = stream,
                    StatusCode = HttpStatusCode.OK,
                    Address = address
                };

                return response;
            }

            return default;
        }
    }
}
