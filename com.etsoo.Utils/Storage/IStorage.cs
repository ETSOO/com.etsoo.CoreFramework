using System.IO;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Storage
{
    /// <summary>
    /// Storage Interface, inherit for Local Storage, Network Storage and so on
    /// 存储接口，扩展用于本地存储和网络存储等
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Get write stream
        /// 获取写入流
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="writeCase">Write case</param>
        /// <returns>Stream</returns>
        Stream? GetWriteStream(string path, WriteCase writeCase = WriteCase.CreateNew);

        /// <summary>
        /// Read file
        /// 读文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>Stream</returns>
        Stream? Read(string path);

        /// <summary>
        /// Async write file
        /// 异步写文件
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="stream">Stream</param>
        /// <param name="writeCase">Write case</param>
        ValueTask<bool> WriteAsync(string path, Stream stream, WriteCase writeCase = WriteCase.CreateNew);
    }
}