
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
            Root = root.TrimStart().TrimEnd(' ', '/', '\\');
            URLRoot = urlRoot.Trim();
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
        /// Format path
        /// 格式化路径
        /// </summary>
        /// <param name="path">Relative path</param>
        /// <returns>Result</returns>
        protected virtual string FormatPath(string path)
        {
            path = path.Replace('\\', '/');
            if (!path.StartsWith('/')) path = '/' + path;
            return path;
        }

        /// <summary>
        /// Get full path
        /// 获取完整路径
        /// </summary>
        /// <param name="path">Relative path</param>
        /// <returns>Result</returns>
        protected virtual string GetPath(string path)
        {
            return Root + FormatPath(path);
        }

        /// <summary>
        /// Get Url address
        /// 获取URL地址
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>URL</returns>
        public string GetUrl(string path)
        {
            return URLRoot + FormatPath(path);
        }

        public abstract ValueTask<bool> CopyAsync(string srcPath, string destPath, IDictionary<string, string>? tags = null, bool deleteSource = false, CancellationToken cancellationToken = default);
        public abstract ValueTask<bool> DeleteAsync(string path, CancellationToken cancellationToken = default);
        public abstract ValueTask<bool> DeleteFolderAsync(string path, bool recursive = false, CancellationToken cancellationToken = default);
        public abstract ValueTask<IEnumerable<StorageEntry>?> ListEntriesAsync(string path, CancellationToken cancellationToken = default);
        public abstract ValueTask<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default);
        public abstract ValueTask<Stream?> GetWriteStreamAsync(string path, WriteCase writeCase = WriteCase.CreateNew, CancellationToken cancellationToken = default);
        public abstract ValueTask<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default);
        public abstract ValueTask<IDictionary<string, string>?> ReadTagsAsync(string path, CancellationToken cancellationToken = default);
        public abstract ValueTask<bool> WriteAsync(string path, Stream stream, WriteCase writeCase = WriteCase.CreateNew, IDictionary<string, string>? tags = null, CancellationToken cancellationToken = default);
        public abstract ValueTask<bool> WriteTagsAsync(string path, IDictionary<string, string> tags, CancellationToken cancellationToken = default);
    }
}