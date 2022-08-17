using System.Buffers;
using System.Text.Json;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Json serialization interface
    /// Json 序列化接口
    /// </summary>
    public interface IJsonSerialization
    {
        /// <summary>
        /// To Json
        /// 转化为 Json
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="options">Options</param>
        /// <param name="fields">Fields allowed</param>
        Task ToJsonAsync(IBufferWriter<byte> writer, JsonSerializerOptions options, IEnumerable<string>? fields = null);
    }
}