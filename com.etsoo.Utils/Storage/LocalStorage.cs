using com.etsoo.Utils.Serialization;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Local Storage
    /// 本地存储
    /// </summary>
    public class LocalStorage : StorageBase
    {
        const string dbFile = "_e_i_o.db";
        const string fsTable = "CREATE TABLE IF NOT EXISTS fs (path TEXT PRIMARY KEY, tags TEXT)";

        public LocalStorage(string root, string urlRoot) : base(root, urlRoot)
        {
        }

        public LocalStorage(StorageOptions options) : this(options.Root, options.URLRoot)
        {
        }

        /// <summary>
        /// Get Sqlite connection
        /// 获取 Sqlite 链接
        /// </summary>
        /// <returns>Result</returns>
        protected SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={Root}\\{dbFile};Cache=Shared;Mode=ReadWriteCreate;");
        }

        /// <summary>
        /// Get file info
        /// 获取文件信息
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>File info</returns>
        protected FileInfo GetFileInfo(string path)
        {
            if (path.Contains(dbFile, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid path", nameof(path));

            return new FileInfo(GetPath(path));
        }

        /// <summary>
        /// Async copy file
        /// 异步复制文件
        /// </summary>
        /// <param name="srcPath">Source path</param>
        /// <param name="destPath">Destination path</param>
        /// <param name="tags">Tags to override</param>
        /// <param name="deleteSource">Is delete the source path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public override async ValueTask<bool> CopyAsync(string srcPath, string destPath, IDictionary<string, string>? tags = null, bool deleteSource = false, CancellationToken cancellationToken = default)
        {
            var fi = GetFileInfo(srcPath);
            if (fi.Exists)
            {
                if (deleteSource)
                {
                    fi.MoveTo(GetPath(destPath), true);
                }
                else
                {
                    fi.CopyTo(GetPath(destPath), true);
                }

                if (tags != null)
                {
                    await WriteTagsAsync(destPath, tags, cancellationToken);
                }
            }

            return false;
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
        /// Async delete folder
        /// 异步删除目录
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="recursive">Recursive</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public override async ValueTask<bool> DeleteFolderAsync(string path, bool recursive = false, CancellationToken cancellationToken = default)
        {
            var folder = new DirectoryInfo(GetPath(path));
            if (folder.Exists)
            {
                return await Task.Run(() =>
                {
                    folder.Delete(recursive);
                    return true;
                }, cancellationToken);
            }

            return false;
        }

        /// <summary>
        /// Async check file exists
        /// 异步检查文件是否存在
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public override async ValueTask<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return File.Exists(GetPath(path));
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
        /// Async list entries under the path
        /// 异步列出路径下的条目
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public override async ValueTask<IEnumerable<StorageEntry>?> ListEntriesAsync(string path, CancellationToken cancellationToken = default)
        {
            path = FormatPath(path);

            var directory = new DirectoryInfo(GetPath(path));
            if (!directory.Exists)
                return null;

            return await Task.Run(() =>
            {
                var entries = new List<StorageEntry>();

                var directories = directory.GetDirectories();
                entries.AddRange(directories.Select(d => new StorageEntry { Name = d.Name, IsFile = false, CreationTime = d.CreationTime, LastWriteTime = d.LastWriteTime }));

                var files = directory.GetFiles();
                if (string.IsNullOrEmpty(path) || path == "/")
                {
                    files = files.Where(f => f.Name != dbFile).ToArray();
                }
                entries.AddRange(files.Select(f => new StorageEntry { Name = f.Name, IsFile = true, Size = f.Length, CreationTime = f.CreationTime, LastWriteTime = f.LastWriteTime }));

                return entries;
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
        /// Async read tags
        /// 异步读取标签
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public override async ValueTask<IDictionary<string, string>?> ReadTagsAsync(string path, CancellationToken cancellationToken = default)
        {
            path = FormatPath(path);

            await using var connection = GetConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.CommandText = $"{fsTable}; SELECT tags FROM fs WHERE path = @path";
            command.Parameters.AddWithValue("@path", path);

            var tags = await command.ExecuteScalarAsync(cancellationToken);
            await connection.CloseAsync();

            if (tags == null || tags == DBNull.Value)
            {
                return null;
            }

            return JsonSerializer.Deserialize((string)tags, CommonJsonSerializerContext.Default.IDictionaryStringString);
        }

        /// <summary>
        /// Async write file
        /// 异步写文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="stream">Stream</param>
        /// <param name="writeCase">Write case</param>
        /// <param name="tags">Tags</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public override async ValueTask<bool> WriteAsync(string path, Stream stream, WriteCase writeCase = WriteCase.CreateNew, IDictionary<string, string>? tags = null, CancellationToken cancellationToken = default)
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

            if (tags != null)
            {
                await WriteTagsAsync(path, tags, cancellationToken);
            }

            // Return
            return true;
        }

        /// <summary>
        /// Async write tags
        /// 异步写入标签
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="tags">Tags</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public override async ValueTask<bool> WriteTagsAsync(string path, IDictionary<string, string> tags, CancellationToken cancellationToken = default)
        {
            path = FormatPath(path);

            await using var connection = GetConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            if (tags.Any())
            {
                var json = JsonSerializer.Serialize(tags, CommonJsonSerializerContext.Default.IDictionaryStringString);

                command.CommandText = $"{fsTable}; INSERT OR REPLACE INTO fs (path, tags) VALUES (@path, @tags)";
                command.Parameters.AddWithValue("@path", path);
                command.Parameters.AddWithValue("@tags", json);
            }
            else
            {
                command.CommandText = $"{fsTable}; DELETE FROM fs WHERE path = @path";
                command.Parameters.AddWithValue("@path", path);
            }

            await command.ExecuteNonQueryAsync(cancellationToken);

            await connection.CloseAsync();

            return true;
        }
    }
}
