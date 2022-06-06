using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace com.etsoo.HTTP
{
    /// <summary>
    /// HTTP client two cases result
    /// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
    /// [MemberNotNull(nameof(field))], after call the method, the field will not be null
    /// [NotNullIfNotNull(nameof(field))], if field is not null, the indication will not be null
    /// HTTP客户端两种情况结果
    /// </summary>
    /// <typeparam name="D">Generic data type</typeparam>
    /// <typeparam name="E">Generic error type</typeparam>
    /// <param name="data">Data</param>
    /// <param name="Error">Error</param>
    public class HttpClientResult<D, E>
    {
        /// <summary>
        /// Is successful result
        /// </summary>
        [MemberNotNullWhen(true, nameof(Data))]
        [MemberNotNullWhen(false, nameof(Error))]
        public bool Success { get; }

        /// <summary>
        /// Data
        /// 数据
        /// </summary>
        public D? Data { get; }

        /// <summary>
        /// Error
        /// 错误
        /// </summary>
        public E? Error { get; }

        public HttpClientResult(D? data, E? error)
        {
            (Success, Data, Error) = (data != null, data, error);
        }
    }

    /// <summary>
    /// HTTP client service
    /// IHttpClientFactory in ASP.NET Core
    /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-6.0
    /// HTTP客户端服务抽象类
    /// </summary>
    public abstract class HttpClientService
    {
        /// <summary>
        /// Create Json StringContent
        /// PostAsJson / PostAsJsonAsync has no Content-Length header, may cause 412 (Precondition Failed)
        /// 创建Json字符串内容
        /// </summary>
        /// <typeparam name="T">Generic input type</typeparam>
        /// <param name="input">Input</param>
        /// <param name="serializerOptions">Options</param>
        /// <returns>Result</returns>
        public static StringContent CreateJsonStringContent<T>(T input, JsonSerializerOptions? serializerOptions = null)
        {
            var json = JsonSerializer.Serialize(input, serializerOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Http client
        /// HTTP客户端
        /// </summary>
        protected readonly HttpClient Client;

        /// <summary>
        /// Json serializer sending out options
        /// Json 序列化程序发送选项
        /// </summary>
        protected readonly JsonSerializerOptions OptionsOut;

        /// <summary>
        /// Json serializer options
        /// Json 序列化程序选项
        /// </summary>
        protected readonly JsonSerializerOptions Options;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Http client</param>
        public HttpClientService(HttpClient client)
        {
            Client = client;

            OptionsOut = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            Options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Delete data
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <returns>Result</returns>
        protected async Task<T?> DeleteAsync<T>(string requestUri)
        {
            using var response = await Client.DeleteAsync(requestUri);
            return await ResponseToAsync<T>(response);
        }

        /// <summary>
        /// Download to save stream
        /// 下载到保存的流
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="saveStream">Save stream</param>
        /// <returns>Filename</returns>
        protected async Task<string?> DownloadAsync(string requestUri, Stream saveStream)
        {
            // Get response
            using var response = await Client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);

            // Ensure success
            response.EnsureSuccessStatusCode();

            // Download stream
            await using var stream = await response.Content.ReadAsStreamAsync();
            await stream.CopyToAsync(saveStream);

            // https://stackoverflow.com/questions/12145390/how-to-set-downloading-file-name-in-asp-net-web-api
            // Filename
            return response.Content.Headers.ContentDisposition?.FileName;
        }

        /// <summary>
        /// Get data
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="serializerOptions">Serializer options</param>
        /// <returns>Result</returns>
        protected async Task<T?> GetAsync<T>(string requestUri, JsonSerializerOptions? serializerOptions = null)
        {
            return await Client.GetFromJsonAsync<T>(requestUri, serializerOptions ?? Options);
        }

        /// <summary>
        /// Get data to target stream
        /// 获取数据到目标流
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="stream">Target stream</param>
        /// <returns>Task</returns>
        protected async Task GetAsync(string requestUri, Stream stream)
        {
            using var response = await Client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
            await ResponseToAsync(response, stream);
        }

        /// <summary>
        /// Get data/error two conditions data
        /// 获取成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="dataField">Data unique field</param>
        /// <returns>Result</returns>
        protected async Task<HttpClientResult<D, E>> GetAsync<D, E>(string requestUri, string dataField)
        {
            // Get response
            using var response = await Client.GetAsync(requestUri);
            return await ResponseToAsync<D, E>(response, dataField);
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
        /// <returns>Result</returns>
        protected async Task<T?> PostAsync<D, T>(string requestUri, D data, JsonSerializerOptions? serializerOptions = null, bool contentLengthRequird = false)
        {
            using var response = contentLengthRequird
                ? await Client.PostAsync(requestUri, CreateJsonStringContent(data, serializerOptions))
                : await Client.PostAsJsonAsync(requestUri, data, serializerOptions ?? OptionsOut);
            return await ResponseToAsync<T>(response);
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
        /// <returns>Result</returns>
        protected async Task<HttpClientResult<D, E>> PostAsync<D, E>(string requestUri, D data, string dataField, JsonSerializerOptions? serializerOptions = null)
        {
            // Get response
            using var response = await Client.PostAsJsonAsync(requestUri, data, serializerOptions ?? OptionsOut);
            return await ResponseToAsync<D, E>(response, dataField);
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
        /// <returns>Result</returns>
        protected async Task<T?> PostFormAsync<D, T>(string requestUri, D data, Func<KeyValuePair<string, object>, KeyValuePair<string, string?>>? formatter = null)
        {
            // Deserialize
            var result = await SharedUtils.ObjectToDictionaryAsync(data);

            // Filter and format data
            IEnumerable<KeyValuePair<string, string?>> items;
            if (formatter == null)
            {
                items = result.Select(item => new KeyValuePair<string, string?>(item.Key, item.Value?.ToString())).Where(item => item.Value != null);
            }
            else
            {
                items = result.Select(item => formatter(item)).Where(item => item.Value != null);
            }

            // Return
            return await PostFormAsync<T>(requestUri, items as IEnumerable<KeyValuePair<string, string>>);
        }

        /// <summary>
        /// Post form data
        /// Post 表单数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request Uri</param>
        /// <param name="data">Directionry data</param>
        /// <returns>Result</returns>
        protected async Task<T?> PostFormAsync<T>(string requestUri, IEnumerable<KeyValuePair<string, string>> data)
        {
            using var response = await Client.PostAsync(requestUri, new FormUrlEncodedContent(data));
            return await ResponseToAsync<T>(response);
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
        /// <returns>Result</returns>
        protected async Task<T?> PutAsync<D, T>(string requestUri, D data, JsonSerializerOptions? serializerOptions = null)
        {
            using var response = await Client.PutAsJsonAsync(requestUri, data, serializerOptions ?? OptionsOut);
            return await ResponseToAsync<T>(response);
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
        /// <returns>Result</returns>
        protected async Task<HttpClientResult<D, E>> PutAsync<D, E>(string requestUri, D data, string dataField, JsonSerializerOptions? serializerOptions = null)
        {
            // Get response
            using var response = await Client.PutAsJsonAsync(requestUri, data, serializerOptions ?? OptionsOut);
            return await ResponseToAsync<D, E>(response, dataField);
        }

        /// <summary>
        /// Http Response to stream
        /// HTTP回应到流
        /// </summary>
        /// <param name="response">Http response</param>
        /// <param name="stream">Target stream</param>
        /// <returns>Result</returns>
        protected async Task ResponseToAsync(HttpResponseMessage response, Stream stream)
        {
            // Ensure success
            response.EnsureSuccessStatusCode();

            // Copy to stream
            await response.Content.CopyToAsync(stream);
        }

        /// <summary>
        /// Http Response to object
        /// HTTP回应转换为对象
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="response">Http response</param>
        /// <returns>Result</returns>
        protected async Task<T?> ResponseToAsync<T>(HttpResponseMessage response)
        {
            // Ensure success
            response.EnsureSuccessStatusCode();

            // Deserialize
            // Ignore case
            return await response.Content.ReadFromJsonAsync<T>(Options);
        }

        /// <summary>
        /// Http Response to data/error 2 conditions object
        /// HTTP回应转换为 数据/错误 两种情况对象
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="response">Http response</param>
        /// <param name="dataField">Data unique field</param>
        /// <returns>Result</returns>
        protected async Task<HttpClientResult<D, E>> ResponseToAsync<D, E>(HttpResponseMessage response, string dataField)
        {
            response.EnsureSuccessStatusCode();

            // Get stream
            await using var stream = await response.Content.ReadAsStreamAsync();

            // Test first 128-byte characters only
            Memory<byte> bytes = new byte[128];
            await stream.ReadAsync(bytes);

            var chars = Encoding.UTF8.GetString(bytes.Span);
            stream.Position = 0;

            if (Regex.IsMatch(chars, "[\\{\"\\s](" + dataField + ")[\"]?:", RegexOptions.Multiline))
            {
                var data = await JsonSerializer.DeserializeAsync<D>(stream, Options);
                return new HttpClientResult<D, E>(data, default);
            }
            else
            {
                var error = await JsonSerializer.DeserializeAsync<E>(stream, Options);
                return new HttpClientResult<D, E>(default, error);
            }
        }

        /// <summary>
        /// Send request
        /// 发送请求
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content</param>
        /// <param name="stream">Target stream</param>
        /// <param name="method">Method</param>
        /// <returns>Task</returns>
        protected async Task SendAsync(string requestUri, HttpContent content, Stream stream, HttpMethod? method = null)
        {
            var request = new HttpRequestMessage(method ?? HttpMethod.Post, requestUri);
            request.Content = content;

            var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            await ResponseToAsync(response, stream);
        }
    }
}
