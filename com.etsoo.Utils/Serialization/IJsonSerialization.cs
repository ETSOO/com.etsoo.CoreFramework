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
        Task ToJsonAsync(IBufferWriter<byte> writer, JsonSerializerOptions options);
    }
}