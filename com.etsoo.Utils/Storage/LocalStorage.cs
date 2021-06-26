using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

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
        /// <param name="root">Root</param>
        /// <param name="urlRoot">URL root</param>
        public LocalStorage(string root, string urlRoot)
        {
            Root = root;
            URLRoot = urlRoot;
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        public LocalStorage(IConfigurationSection section) : this(
            section.GetValue<string>("Root"),
            section.GetValue<string>("URLRoot")
        )
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
        /// Delete file
        /// 删除文件
        /// </summary>
        /// <param name="path">Path</param>
        public void Delete(string path)
        {
            var fi = GetFileInfo(path);
            if (fi.Exists)
                fi.Delete();
        }

        /// <summary>
        /// Get write stream
        /// 获取写入流
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="writeCase">Write case</param>
        /// <returns>Stream</returns>
        public Stream? GetWriteStream(string path, WriteCase writeCase = WriteCase.CreateNew)
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
            return fi.Exists ? fi.OpenWrite() : fi.Create();
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
        /// Read file
        /// 读文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Stream</returns>
        public Stream? Read(string path)
        {
            var fi = GetFileInfo(path);
            if (!fi.Exists)
                return null;

            // Returns a read-only FileStream object with the FileShare mode set to Read
            return fi.OpenRead();
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
            var fileStream = fi.Exists ? fi.OpenWrite() : fi.Create();

            // Copy the stream to the file stream
            await stream.CopyToAsync(fileStream);

            // Return
            return true;
        }
    }
}
