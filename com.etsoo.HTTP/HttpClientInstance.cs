using System.Text.Json;

namespace com.etsoo.HTTP
{
    /// <summary>
    /// Http client instance for quick use
    /// HTTP 客户端实例用于快速使用
    /// </summary>
    public class HttpClientInstance : HttpClientService
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Http client</param>
        public HttpClientInstance(HttpClient client) : base(client)
        {
        }

        /// <summary>
        /// Delete data
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<T?> DeleteAsync<T>(string requestUri, CancellationToken cancellationToken = default)
        {
            return await base.DeleteAsync<T>(requestUri, cancellationToken);
        }

        /// <summary>
        /// Download to save stream
        /// 下载到保存的流
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Filename</returns>
        public new async Task<string> DownloadAsync(string requestUri, Stream saveStream, CancellationToken cancellationToken = default)
        {
            return await base.DownloadAsync(requestUri, saveStream, cancellationToken);
        }

        /// <summary>
        /// Get data
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="serializerOptions">Serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<T?> GetAsync<T>(string requestUri, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
        {
            return await base.GetAsync<T>(requestUri, serializerOptions, cancellationToken);
        }

        /// <summary>
        /// Get data to target stream
        /// 获取数据到目标流
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="stream">Target stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public new async Task GetAsync(string requestUri, Stream stream, CancellationToken cancellationToken = default)
        {
            await base.GetAsync(requestUri, stream, cancellationToken);
        }

        /// <summary>
        /// Get data/error two conditions data
        /// 获取成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<HttpClientResult<D, E>> GetAsync<D, E>(string requestUri, string dataField, CancellationToken cancellationToken = default)
        {
            return await base.GetAsync<D, E>(requestUri, dataField, cancellationToken);
        }

        /// <summary>
        /// Async get uri stream
        /// 异步获取网址数据流
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public new async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            return await base.GetStreamAsync(uri, cancellationToken);
        }

        /// <summary>
        /// Post Json data
        /// Post Json 数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="serializerOptions">Serializer options</param>
        /// <param name="contentLengthRequird">Content-Length required</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<T?> PostAsync<D, T>(string requestUri, D data, JsonSerializerOptions? serializerOptions = null, bool contentLengthRequird = false, CancellationToken cancellationToken = default)
        {
            return await base.PostAsync<D, T>(requestUri, data, serializerOptions, contentLengthRequird, cancellationToken);
        }

        /// <summary>
        /// Post Json data, data/error two conditions data
        /// Post Json数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="serializerOptions">Serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<HttpClientResult<D, E>> PostAsync<D, E>(string requestUri, D data, string dataField, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
        {
            return await base.PostAsync<D, E>(requestUri, data, dataField, serializerOptions, cancellationToken);
        }

        /// <summary>
        /// Post data, data/error two conditions data
        /// Post 数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<HttpClientResult<D, E>> PostAsync<D, E>(string requestUri, HttpContent content, string dataField, CancellationToken cancellationToken = default)
        {
            return await base.PostAsync<D, E>(requestUri, content, dataField, cancellationToken);
        }

        /// <summary>
        /// Post form data
        /// Post 表单数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request Uri</param>
        /// <param name="data">Data</param>
        /// <param name="formatter">Data formatter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<T?> PostFormAsync<D, T>(string requestUri, D data, Func<KeyValuePair<string, object>, KeyValuePair<string, string?>>? formatter = null, CancellationToken cancellationToken = default)
        {
            return await base.PostFormAsync<D, T>(requestUri, data, formatter, cancellationToken);
        }

        /// <summary>
        /// Post form data
        /// Post 表单数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request Uri</param>
        /// <param name="data">Directionry data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<T?> PostFormAsync<T>(string requestUri, IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken = default)
        {
            return await base.PostFormAsync<T>(requestUri, data, cancellationToken);
        }

        /// <summary>
        /// Put Json data
        /// Put Json 数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="serializerOptions">Serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<T?> PutAsync<D, T>(string requestUri, D data, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
        {
            return await PutAsync<D, T>(requestUri, data, serializerOptions, cancellationToken);
        }

        /// <summary>
        /// Put Json data, data/error two conditions data
        /// Put Json数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="serializerOptions">Serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<HttpClientResult<D, E>> PutAsync<D, E>(string requestUri, D data, string dataField, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
        {
            return await base.PutAsync<D, E>(requestUri, data, dataField, serializerOptions, cancellationToken);
        }

        /// <summary>
        /// Http Response to stream
        /// HTTP回应到流
        /// </summary>
        /// <param name="response">Http response</param>
        /// <param name="stream">Target stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task ResponseToAsync(HttpResponseMessage response, Stream stream, CancellationToken cancellationToken = default)
        {
            await base.ResponseToAsync(response, stream, cancellationToken);
        }

        /// <summary>
        /// Http Response to object
        /// HTTP回应转换为对象
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="response">Http response</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<T?> ResponseToAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            return await base.ResponseToAsync<T>(response, cancellationToken);
        }

        /// <summary>
        /// Http Response to data/error 2 conditions object
        /// HTTP回应转换为 数据/错误 两种情况对象
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="response">Http response</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public new async Task<HttpClientResult<D, E>> ResponseToAsync<D, E>(HttpResponseMessage response, string dataField, CancellationToken cancellationToken = default)
        {
            return await base.ResponseToAsync<D, E>(response, dataField, cancellationToken);
        }

        /// <summary>
        /// Send request
        /// 发送请求
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content</param>
        /// <param name="method">Method</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Json result</returns>
        public new async Task<T?> SendAsync<T>(string requestUri, HttpContent content, HttpMethod? method = null, CancellationToken cancellationToken = default)
        {
            return await SendAsync<T>(requestUri, content, method, cancellationToken);
        }

        /// <summary>
        /// Send request
        /// 发送请求
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content</param>
        /// <param name="stream">Target stream</param>
        /// <param name="method">Method</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public new async Task SendAsync(string requestUri, HttpContent content, Stream stream, HttpMethod? method = null, CancellationToken cancellationToken = default)
        {
            await SendAsync(requestUri, content, stream, method, cancellationToken);
        }
    }
}
