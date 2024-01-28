using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Local Storage
    /// 本地存储
    /// </summary>
    public class LocalStorage : IStorage
    {
        /// <summary>
        /// Root
        /// 根目录
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
        /// <param name="section">Configuration section</param>
        [RequiresUnreferencedCode("LocalStorage constructor AOT configuration issue")]
        [RequiresDynamicCode("LocalStorage constructor AOT configuration issue")]
        public LocalStorage(IConfigurationSection section)
            : this(section.Get<LocalStorageSettings>())
        {

        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="settings">Settings</param>
        public LocalStorage(LocalStorageSettings? settings)
        {
            Root = settings?.Root ?? string.Empty;
            URLRoot = settings?.URLRoot ?? string.Empty;
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
        public async ValueTask DeleteAsync(string path)
        {
            var fi = GetFileInfo(path);
            if (fi.Exists)
            {
                await Task.Run(() =>
                {
                    fi.Delete();
                });
            }
        }

        /// <summary>
        /// Async delete url file
        /// 异步删除URL文件
        /// </summary>
        /// <param name="url">URL</param>
        public async ValueTask DeleteUrlAsync(string url)
        {
            var pos = url.IndexOf(URLRoot, StringComparison.OrdinalIgnoreCase);
            if (pos != -1)
            {
                var path = url[URLRoot.Length..];
                pos = path.IndexOf('?');
                if (pos != -1)
                    path = path[0..pos];

                await DeleteAsync(path);
            }
        }

        /// <summary>
        /// Async get write stream
        /// 异步获取写入流
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="writeCase">Write case</param>
        /// <returns>Stream</returns>
        public async ValueTask<Stream?> GetWriteStreamAsync(string path, WriteCase writeCase = WriteCase.CreateNew)
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
            return await Task.Run(() =>
            {
                return fi.Exists ? fi.OpenWrite() : fi.Create();
            });
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

        /// <summary>
        /// Async read file
        /// 异步读文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Stream</returns>
        public async ValueTask<Stream?> ReadAsync(string path)
        {
            var fi = GetFileInfo(path);
            if (!fi.Exists)
                return null;

            return await Task.Run(fi.OpenRead);
        }

        /// <summary>
        /// Async write file
        /// 异步写文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="stream">Stream</param>
        /// <param name="writeCase">Write case</param>
        public async ValueTask<bool> WriteAsync(string path, Stream stream, WriteCase writeCase = WriteCase.CreateNew)
        {
            var fi = GetFileInfo(path);
            if (
                (writeCase == WriteCase.CreateNew && fi.Exists)
                    ||
                (writeCase == WriteCase.Appending && !fi.Exists)
            )
            {
                return false;
            }

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
            await using var fileStream = fi.Exists ? fi.OpenWrite() : fi.Create();

            // Reset stream current position
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            // Copy the stream to the file stream
            await stream.CopyToAsync(fileStream);

            // Flush and close
            await fileStream.FlushAsync();

            // Close the stream explicitly
            fileStream.Close();

            // Return
            return true;
        }
    }
}
