namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Storage Interface, inherit for Local Storage, Network Storage and so on
    /// 存储接口，扩展用于本地存储和网络存储等
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Root path or bucket name
        /// 根路径或存储桶名称
        /// </summary>
        public string Root { get; }

        /// <summary>
        /// URL root
        /// URL根路径
        /// </summary>
        public string URLRoot { get; }

        /// <summary>
        /// Async delete file
        /// 异步删除文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Is deleted</returns>
        ValueTask<bool> DeleteAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async delete url file
        /// 异步删除URL文件
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>Is deleted</returns>
        ValueTask<bool> DeleteUrlAsync(string url, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async get write stream
        /// 异步获取写入流
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="writeCase">Write case</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        ValueTask<Stream?> GetWriteStreamAsync(string path, WriteCase writeCase = WriteCase.CreateNew, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Url address
        /// 获取URL地址
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>URL</returns>
        string GetUrl(string path);

        /// <summary>
        /// Async read file
        /// 异步读文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Stream</returns>
        ValueTask<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async write file
        /// 异步写文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="stream">Stream</param>
        /// <param name="writeCase">Write case</param>
        /// <param name="cancellationToken">Cancellation token</param>
        ValueTask<bool> WriteAsync(string path, Stream stream, WriteCase writeCase = WriteCase.CreateNew, CancellationToken cancellationToken = default);
    }
}