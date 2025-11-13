using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
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
    /// HTTP客户端服务类
    /// </summary>
    public class HttpClientService
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
        [RequiresDynamicCode("CreateJsonStringContent 'input' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("CreateJsonStringContent 'input' may require dynamic access otherwise can break functionality when trimming application code")]
        public static StringContent CreateJsonStringContent<T>(T input, JsonSerializerOptions? serializerOptions = null)
        {
            var json = JsonSerializer.Serialize(input, serializerOptions);
            return CreateJsonStringContent(json);
        }

        /// <summary>
        /// Create Json StringContent
        /// PostAsJson / PostAsJsonAsync has no Content-Length header, may cause 412 (Precondition Failed)
        /// 创建Json字符串内容
        /// </summary>
        /// <typeparam name="T">Generic input type</typeparam>
        /// <param name="input">Input</param>
        /// <param name="typeInfo">Json type info</param>
        /// <returns>Result</returns>
        public static StringContent CreateJsonStringContent<T>(T input, JsonTypeInfo<T> typeInfo)
        {
            var json = JsonSerializer.Serialize(input, typeInfo);
            return CreateJsonStringContent(json);
        }

        /// <summary>
        /// Create Json StringContent
        /// 创建Json字符串内容
        /// </summary>
        /// <param name="json">Json string</param>
        /// <returns>String content</returns>
        public static StringContent CreateJsonStringContent(string json)
        {
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Http client
        /// HTTP客户端
        /// </summary>
        public readonly HttpClient Client;

        /// <summary>
        /// Json serializer sending out options
        /// Json 序列化程序发送选项
        /// </summary>
        public readonly JsonSerializerOptions OptionsOut;

        /// <summary>
        /// Json serializer options
        /// Json 序列化程序选项
        /// </summary>
        public readonly JsonSerializerOptions Options;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">Http client</param>
        public HttpClientService(HttpClient client)
        {
            Client = client;

            OptionsOut = SharedUtils.JsonDefaultSerializerOptions;

            Options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// Delete data
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("DeleteAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("DeleteAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<T?> DeleteAsync<T>(string requestUri, CancellationToken cancellationToken = default)
        {
            using var response = await Client.DeleteAsync(requestUri, cancellationToken);
            return await ResponseToAsync<T>(response, cancellationToken);
        }

        /// <summary>
        /// Delete data
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<T?> DeleteAsync<T>(string requestUri, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default)
        {
            using var response = await Client.DeleteAsync(requestUri, cancellationToken);
            return await ResponseToAsync(response, typeInfo, cancellationToken);
        }

        /// <summary>
        /// Download to save stream
        /// 下载到保存的流
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="saveStream">Save stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Filename</returns>
        public async Task<string> DownloadAsync(string requestUri, Stream saveStream, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            // Ensure success
            response.EnsureSuccessStatusCode();

            // Download stream
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await stream.CopyToAsync(saveStream, cancellationToken);

            // https://stackoverflow.com/questions/12145390/how-to-set-downloading-file-name-in-asp-net-web-api
            // Filename
            var headers = response.Content.Headers;
            return headers.ContentDisposition?.FileName ?? Path.GetFileNameWithoutExtension(requestUri) + MimeTypeMap.TryGetExtension(headers.ContentType?.MediaType) ?? string.Empty;
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
        [RequiresDynamicCode("GetAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("GetAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<T?> GetAsync<T>(string requestUri, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
        {
            return await Client.GetFromJsonAsync<T>(requestUri, serializerOptions ?? Options, cancellationToken);
        }

        /// <summary>
        /// Get data
        /// 获取数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<T?> GetAsync<T>(string requestUri, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default)
        {
            return await Client.GetFromJsonAsync(requestUri, typeInfo, cancellationToken);
        }

        /// <summary>
        /// Get data to target stream
        /// 获取数据到目标流
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="stream">Target stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public async Task GetAsync(string requestUri, Stream stream, CancellationToken cancellationToken = default)
        {
            using var response = await Client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await ResponseToAsync(response, stream, cancellationToken);
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
        [RequiresDynamicCode("GetAsync 'D' and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("GetAsync 'D' and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<HttpClientResult<D, E>> GetAsync<D, E>(string requestUri, string dataField, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.GetAsync(requestUri, cancellationToken);
            return await ResponseToAsync<D, E>(response, dataField, cancellationToken);
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
        public async Task<HttpClientResult<D, E>> GetAsync<D, E>(string requestUri, string dataField, JsonTypeInfo<D> dataTypeInfo, JsonTypeInfo<E> errorTypeInfo, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.GetAsync(requestUri, cancellationToken);
            return await ResponseToAsync(response, dataField, dataTypeInfo, errorTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Async get uri stream
        /// 异步获取网址数据流
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            return await Client.GetStreamAsync(uri, cancellationToken);
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
        [RequiresDynamicCode("PostAsync 'D' and 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("PostAsync 'D' and 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<T?> PostAsync<D, T>(string requestUri, D data, JsonSerializerOptions? serializerOptions = null, bool contentLengthRequird = false, CancellationToken cancellationToken = default)
        {
            using var response = contentLengthRequird
                ? await Client.PostAsync(requestUri, CreateJsonStringContent(data, serializerOptions ?? OptionsOut), cancellationToken)
                : await Client.PostAsJsonAsync(requestUri, data, serializerOptions ?? OptionsOut, cancellationToken);
            return await ResponseToAsync<T>(response, cancellationToken);
        }

        /// <summary>
        /// Post Json data
        /// Post Json 数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="contentLengthRequird">Content-Length required</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<T?> PostAsync<D, T>(string requestUri, D data, JsonTypeInfo<D> dataTypeInfo, JsonTypeInfo<T> resultTypeInfo, bool contentLengthRequird = false, CancellationToken cancellationToken = default)
        {
            using var response = contentLengthRequird
                ? await Client.PostAsync(requestUri, CreateJsonStringContent(data, dataTypeInfo), cancellationToken)
                : await Client.PostAsJsonAsync(requestUri, data, dataTypeInfo, cancellationToken);
            return await ResponseToAsync(response, resultTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Post Json data, data/error two conditions data
        /// Post Json数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="T">Generic input data type</typeparam>
        /// <typeparam name="D">Generic result data type</typeparam>
        /// <typeparam name="E">Generic result error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="serializerOptions">Serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("PostAsync 'T', 'D', and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("PostAsync 'T', 'D', and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<HttpClientResult<D, E>> PostAsync<T, D, E>(string requestUri, T data, string dataField, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.PostAsJsonAsync(requestUri, data, serializerOptions ?? OptionsOut, cancellationToken);
            return await ResponseToAsync<D, E>(response, dataField, cancellationToken);
        }

        /// <summary>
        /// Post Json data, data/error two conditions data
        /// Post Json数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="T">Generic input data type</typeparam>
        /// <typeparam name="D">Generic result data type</typeparam>
        /// <typeparam name="E">Generic result error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="inputTypeInfo">Input data json type info</param>
        /// <param name="dataTypeInfo">Result data json type info</param>
        /// <param name="errorTypeInfo">Result error json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<HttpClientResult<D, E>> PostAsync<T, D, E>(string requestUri, T data, string dataField, JsonTypeInfo<T> inputTypeInfo, JsonTypeInfo<D> dataTypeInfo, JsonTypeInfo<E> errorTypeInfo, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.PostAsJsonAsync(requestUri, data, inputTypeInfo, cancellationToken);
            return await ResponseToAsync(response, dataField, dataTypeInfo, errorTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Post data, data/error two conditions data
        /// Post 数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="D">Generic result data type</typeparam>
        /// <typeparam name="E">Generic result error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("PostAsync 'D', and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("PostAsync 'D', and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<HttpClientResult<D, E>> PostAsync<D, E>(string requestUri, HttpContent content, string dataField, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.PostAsync(requestUri, content, cancellationToken);
            return await ResponseToAsync<D, E>(response, dataField, cancellationToken);
        }

        /// <summary>
        /// Post data, data/error two conditions data
        /// Post 数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="D">Generic result data type</typeparam>
        /// <typeparam name="E">Generic result error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="dataTypeInfo">Result data json type info</param>
        /// <param name="errorTypeInfo">Result error json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<HttpClientResult<D, E>> PostAsync<D, E>(string requestUri, HttpContent content, string dataField, JsonTypeInfo<D> dataTypeInfo, JsonTypeInfo<E> errorTypeInfo, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.PostAsync(requestUri, content, cancellationToken);
            return await ResponseToAsync(response, dataField, dataTypeInfo, errorTypeInfo, cancellationToken);
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
        [RequiresDynamicCode("PostFormAsync 'D', and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("PostFormAsync 'D', and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<T?> PostFormAsync<D, T>(string requestUri, D data, Func<KeyValuePair<string, object>, KeyValuePair<string, string?>>? formatter = null, CancellationToken cancellationToken = default)
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
            return await PostFormAsync<T>(requestUri, items as IEnumerable<KeyValuePair<string, string>>, cancellationToken);
        }

        /// <summary>
        /// Post form data
        /// Post 表单数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request Uri</param>
        /// <param name="data">Data</param>
        /// <param name="dataTypeInfo">Input data json type info</param>
        /// <param name="resultTypeInfo">Result data json type info</param>
        /// <param name="formatter">Data formatter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<T?> PostFormAsync<D, T>(string requestUri, D data, JsonTypeInfo<D> dataTypeInfo, JsonTypeInfo<T> resultTypeInfo, Func<KeyValuePair<string, object>, KeyValuePair<string, string?>>? formatter = null, CancellationToken cancellationToken = default)
        {
            // Deserialize
            var result = await SharedUtils.ObjectToDictionaryAsync(data, dataTypeInfo, cancellationToken);

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
            return await PostFormAsync<T>(requestUri, items as IEnumerable<KeyValuePair<string, string>>, resultTypeInfo, cancellationToken);
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
        [RequiresDynamicCode("PostFormAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("PostFormAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<T?> PostFormAsync<T>(string requestUri, IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken = default)
        {
            using var response = await Client.PostAsync(requestUri, new FormUrlEncodedContent(data), cancellationToken);
            return await ResponseToAsync<T>(response, cancellationToken);
        }

        /// <summary>
        /// Post form data
        /// Post 表单数据
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request Uri</param>
        /// <param name="data">Directionry data</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<T?> PostFormAsync<T>(string requestUri, IEnumerable<KeyValuePair<string, string>> data, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default)
        {
            using var response = await Client.PostAsync(requestUri, new FormUrlEncodedContent(data), cancellationToken);
            return await ResponseToAsync(response, typeInfo, cancellationToken);
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
        [RequiresDynamicCode("PutAsync 'D' and 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("PutAsync 'D' and 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<T?> PutAsync<D, T>(string requestUri, D data, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
        {
            using var response = await Client.PutAsJsonAsync(requestUri, data, serializerOptions ?? OptionsOut, cancellationToken);
            return await ResponseToAsync<T>(response, cancellationToken);
        }

        /// <summary>
        /// Put Json data
        /// Put Json 数据
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="dataTypeInfo">Input data json type info</param>
        /// <param name="resultTypeInfo">Result data json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<T?> PutAsync<D, T>(string requestUri, D data, JsonTypeInfo<D> dataTypeInfo, JsonTypeInfo<T> resultTypeInfo, CancellationToken cancellationToken = default)
        {
            using var response = await Client.PutAsJsonAsync(requestUri, data, dataTypeInfo, cancellationToken);
            return await ResponseToAsync(response, resultTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Put Json data, data/error two conditions data
        /// Put Json数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="T">Generic input data type</typeparam>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="serializerOptions">Serializer options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("PutAsync 'T', 'D', and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("PutAsync 'T', 'D', and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<HttpClientResult<D, E>> PutAsync<T, D, E>(string requestUri, T data, string dataField, JsonSerializerOptions? serializerOptions = null, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.PutAsJsonAsync(requestUri, data, serializerOptions ?? OptionsOut, cancellationToken);
            return await ResponseToAsync<D, E>(response, dataField, cancellationToken);
        }

        /// <summary>
        /// Put Json data, data/error two conditions data
        /// Put Json数据，成功/失败两种情况的数据
        /// </summary>
        /// <typeparam name="T">Generic input data type</typeparam>
        /// <typeparam name="D">Generic result data type</typeparam>
        /// <typeparam name="E">Generic result error type</typeparam>
        /// <param name="requestUri">Request uri</param>
        /// <param name="data">Data</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="inputTypeInfo">Input data json type info</param>
        /// <param name="dataTypeInfo">Result data json type info</param>
        /// <param name="errorTypeInfo">Result error json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<HttpClientResult<D, E>> PutAsync<T, D, E>(string requestUri, T data, string dataField, JsonTypeInfo<T> inputTypeInfo, JsonTypeInfo<D> dataTypeInfo, JsonTypeInfo<E> errorTypeInfo, CancellationToken cancellationToken = default)
        {
            // Get response
            using var response = await Client.PutAsJsonAsync(requestUri, data, inputTypeInfo, cancellationToken);
            return await ResponseToAsync(response, dataField, dataTypeInfo, errorTypeInfo, cancellationToken);
        }

        /// <summary>
        /// Http Response to stream
        /// HTTP回应到流
        /// </summary>
        /// <param name="response">Http response</param>
        /// <param name="stream">Target stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task ResponseToAsync(HttpResponseMessage response, Stream stream, CancellationToken cancellationToken = default)
        {
            // Ensure success
            response.EnsureSuccessStatusCode();

            // Copy to stream
            await response.Content.CopyToAsync(stream, cancellationToken);
        }

        /// <summary>
        /// Http Response to object
        /// HTTP回应转换为对象
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="response">Http response</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("ResponseToAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("ResponseToAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<T?> ResponseToAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            // Ensure success
            response.EnsureSuccessStatusCode();

            // Deserialize
            // Ignore case
            return await response.Content.ReadFromJsonAsync<T>(Options, cancellationToken);
        }

        /// <summary>
        /// Http Response to object
        /// HTTP回应转换为对象
        /// </summary>
        /// <typeparam name="T">Generic result type</typeparam>
        /// <param name="response">Http response</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<T?> ResponseToAsync<T>(HttpResponseMessage response, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default)
        {
            // Ensure success
            response.EnsureSuccessStatusCode();

            // Deserialize
            // Ignore case
            return await response.Content.ReadFromJsonAsync(typeInfo, cancellationToken);
        }

        private async ValueTask<bool> IsStreamJsonDataAsync(Stream stream, string dataField, CancellationToken cancellationToken)
        {
            // Read first bytes
            var len = Math.Min(dataField.Length + 64, stream.Length);

            Memory<byte> bytes = new byte[len];
            await stream.ReadExactlyAsync(bytes, cancellationToken);

            // Characters
            var chars = Encoding.UTF8.GetString(bytes.Span);

            // Back to the beginning
            stream.Position = 0;

            return Regex.IsMatch(chars, "[\\{\"\\s](" + dataField + ")[\"]?:", RegexOptions.Multiline);
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
        [RequiresDynamicCode("ResponseToAsync 'D' and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("ResponseToAsync 'D' and 'E' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<HttpClientResult<D, E>> ResponseToAsync<D, E>(HttpResponseMessage response, string dataField, CancellationToken cancellationToken = default)
        {
            response.EnsureSuccessStatusCode();

            // Get stream
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            // Check
            if (await IsStreamJsonDataAsync(stream, dataField, cancellationToken))
            {
                var data = await JsonSerializer.DeserializeAsync<D>(stream, Options, cancellationToken);
                return new HttpClientResult<D, E>(data, default);
            }
            else
            {
                var error = await JsonSerializer.DeserializeAsync<E>(stream, Options, cancellationToken);
                return new HttpClientResult<D, E>(default, error);
            }
        }

        /// <summary>
        /// Http Response to data/error 2 conditions object
        /// HTTP回应转换为 数据/错误 两种情况对象
        /// </summary>
        /// <typeparam name="D">Generic data type</typeparam>
        /// <typeparam name="E">Generic error type</typeparam>
        /// <param name="response">Http response</param>
        /// <param name="dataField">Data unique field</param>
        /// <param name="dataTypeInfo">Data json type info</param>
        /// <param name="errorTypeInfo">Error json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public async Task<HttpClientResult<D, E>> ResponseToAsync<D, E>(HttpResponseMessage response, string dataField, JsonTypeInfo<D> dataTypeInfo, JsonTypeInfo<E> errorTypeInfo, CancellationToken cancellationToken = default)
        {
            response.EnsureSuccessStatusCode();

            // Get stream
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            // Check
            if (await IsStreamJsonDataAsync(stream, dataField, cancellationToken))
            {
                var data = await JsonSerializer.DeserializeAsync(stream, dataTypeInfo, cancellationToken);
                return new HttpClientResult<D, E>(data, default);
            }
            else
            {
                var error = await JsonSerializer.DeserializeAsync(stream, errorTypeInfo, cancellationToken);
                return new HttpClientResult<D, E>(default, error);
            }
        }

        /// <summary>
        /// Send request with result
        /// 发送请求请获取结果
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content</param>
        /// <param name="method">Method</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Json result</returns>
        [RequiresDynamicCode("SendAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("SendAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public async Task<T?> SendAsync<T>(string requestUri, HttpContent content, HttpMethod? method = null, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(method ?? HttpMethod.Post, requestUri)
            {
                Content = content
            };

            var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return await ResponseToAsync<T>(response, cancellationToken);
        }

        /// <summary>
        /// Send request with result
        /// 发送请求请获取结果
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content</param>
        /// <param name="typeInfo">Result json type info</param>
        /// <param name="method">Method</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Json result</returns>
        public async Task<T?> SendAsync<T>(string requestUri, HttpContent content, JsonTypeInfo<T> typeInfo, HttpMethod? method = null, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(method ?? HttpMethod.Post, requestUri)
            {
                Content = content
            };

            var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return await ResponseToAsync(response, typeInfo, cancellationToken);
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
        public async Task SendAsync(string requestUri, HttpContent content, Stream stream, HttpMethod? method = null, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(method ?? HttpMethod.Post, requestUri)
            {
                Content = content
            };

            var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await ResponseToAsync(response, stream, cancellationToken);
        }
    }
}
