using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace com.etsoo.HtmlUtils
{
    /// <summary>
    /// HTML shared utils
    /// HTML共享的工具
    /// </summary>
    public static class HtmlSharedUtils
    {
        /// <summary>
        /// HTTP client
        /// HTTP 客户端
        /// </summary>
        public static readonly HttpClient client = new();

        /// <summary>
        /// Async get uri stream
        /// 异步获取网址数据流
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns>Stream</returns>
        public async static Task<Stream> GetStreamAsync(Uri uri)
        {
            return await client.GetStreamAsync(uri);
        }

        /// <summary>
        /// Async get uri stream
        /// 异步获取网址数据流
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Stream</returns>
        public async static Task<Stream> GetStreamAsync(Uri uri, CancellationToken token)
        {
            return await client.GetStreamAsync(uri, token);
        }

        /// <summary>
        /// Parse table data
        /// 解析表格数据
        /// </summary>
        /// <typeparam name="T">Row data</typeparam>
        /// <param name="stream">HTML stream</param>
        /// <param name="selector">Table selector</param>
        /// <param name="creator">Row data creator</param>
        /// <param name="titleRowIndex">Title row index, -1 means no titles</param>
        /// <returns>Parsed data and continue or not</returns>
        public async static Task<List<T>> ParseTable<T>(Stream stream, string selector, Func<List<string>?, List<string>, int, (T?, bool)> creator, int titleRowIndex = 0)
        {
            var items = new List<T>();

            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(stream);
            var element = doc.QuerySelector(selector);

            var startIndex = titleRowIndex + 1;
            if (element is IHtmlTableElement table && table.Rows.Length > startIndex)
            {
                var titles = titleRowIndex < 0 ? null : table.Rows[titleRowIndex].Cells.Select(cell => cell.Text().Trim()).ToList();
                for (var i = startIndex; i < table.Rows.Length; i++)
                {
                    // Row data
                    var data = table.Rows[i].Cells.Select(cell => cell.Text().Trim()).ToList();

                    // Create item
                    // Also could break when pass the index
                    var (item, continueProcess) = creator(titles, data, i - startIndex);

                    // Add to the collection
                    if (item != null)
                        items.Add(item);

                    // Break
                    if (!continueProcess) break;
                }
            }

            return items;
        }
    }
}
