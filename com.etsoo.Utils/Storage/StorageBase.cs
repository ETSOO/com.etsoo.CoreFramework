
namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Storage Base
    /// 基础存储对象
    /// </summary>
    public abstract class StorageBase : IStorage
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
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="root">Root path or bucket name</param>
        /// <param name="urlRoot">URL root</param>
        public StorageBase(string root, string urlRoot)
        {
            Root = root;
            URLRoot = urlRoot;
        }

        /// <summary>
        /// Async delete url file
        /// 异步删除URL文件
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Is deleted</returns>
        public async ValueTask<bool> DeleteUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            var pos = url.IndexOf(URLRoot, StringComparison.OrdinalIgnoreCase);
            if (pos != -1)
            {
                var path = url[URLRoot.Length..];
                pos = path.IndexOf('?');
                if (pos != -1)
                    path = path[0..pos];

                return await DeleteAsync(path, cancellationToken);
            }

            return false;
        }

        /// <summary>
        /// Get Url address
        /// 获取URL地址
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>URL</returns>
        public string GetUrl(string path)
        {
            return URLRoot + path.Replace('\\', '/');
        }

        public abstract ValueTask<bool> DeleteAsync(string path, CancellationToken cancellationToken = default);
        public abstract ValueTask<Stream?> GetWriteStreamAsync(string path, WriteCase writeCase = WriteCase.CreateNew, CancellationToken cancellationToken = default);
        public abstract ValueTask<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default);
        public abstract ValueTask<bool> WriteAsync(string path, Stream stream, WriteCase writeCase = WriteCase.CreateNew, CancellationToken cancellationToken = default);
    }
}
