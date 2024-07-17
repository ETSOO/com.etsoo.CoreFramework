namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Local Storage
    /// 本地存储
    /// </summary>
    public class LocalStorage : StorageBase
    {
        public LocalStorage(string root, string urlRoot) : base(root, urlRoot)
        {
        }

        public LocalStorage(StorageOptions options) : base(options.Root, options.URLRoot)
        {
        }

        /// <summary>
        /// Get file info
        /// 获取文件信息
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>File info</returns>
        protected FileInfo GetFileInfo(string path)
        {
            return new FileInfo(Root + path);
        }

        /// <summary>
        /// Async delete file
        /// 异步删除文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public override async ValueTask<bool> DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            var fi = GetFileInfo(path);
            if (fi.Exists)
            {
                return await Task.Run(() =>
                {
                    fi.Delete();
                    return true;
                }, cancellationToken);
            }

            return false;
        }

        /// <summary>
        /// Async get write stream
        /// 异步获取写入流
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="writeCase">Write case</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public override async ValueTask<Stream?> GetWriteStreamAsync(string path, WriteCase writeCase = WriteCase.CreateNew, CancellationToken cancellationToken = default)
        {
            var fi = GetFileInfo(path);
            if (
                (writeCase == WriteCase.CreateNew && fi.Exists)
                    ||
                (writeCase == WriteCase.Appending && !fi.Exists)
            )
            {
                return null;
            }

            return await Task.Run(() =>
            {
                if (writeCase == WriteCase.CreateOrOverwrite && fi.Exists)
                {
                    // Delete existing file
                    fi.Delete();
                }

                // Create directory
                if (fi.Directory != null && !fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }

                // Current file stream
                return fi.Exists ? fi.OpenWrite() : fi.Create();
            }, cancellationToken);
        }

        /// <summary>
        /// Async read file
        /// 异步读文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stream</returns>
        public override async ValueTask<Stream?> ReadAsync(string path, CancellationToken cancellationToken = default)
        {
            var fi = GetFileInfo(path);
            if (!fi.Exists)
                return null;

            return await Task.Run(fi.OpenRead, cancellationToken);
        }

        /// <summary>
        /// Async write file
        /// 异步写文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="stream">Stream</param>
        /// <param name="writeCase">Write case</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public override async ValueTask<bool> WriteAsync(string path, Stream stream, WriteCase writeCase = WriteCase.CreateNew, CancellationToken cancellationToken = default)
        {
            // Current file stream
            await using var fileStream = await GetWriteStreamAsync(path, writeCase, cancellationToken);
            if (fileStream == null) return false;

            // Reset stream current position
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            // Copy the stream to the file stream
            await stream.CopyToAsync(fileStream, cancellationToken);

            // Flush and close
            await fileStream.FlushAsync(cancellationToken);

            // Close the stream explicitly
            fileStream.Close();
            await fileStream.DisposeAsync();

            // Return
            return true;
        }
    }
}
