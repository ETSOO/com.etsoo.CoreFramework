using com.etsoo.Utils;
using System.Text.Json;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue utilities
    /// 消息队列工具
    /// </summary>
    public static class MessageQueueUtils
    {
        /// <summary>
        /// Async model to JSON bytes
        /// 异步模型到 JSON 字节
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<ReadOnlyMemory<byte>> ToJsonBytesAsync<T>(T model, CancellationToken cancellationToken)
        {
            await using var stream = SharedUtils.GetStream();
            await JsonSerializer.SerializeAsync(stream, model, SharedUtils.JsonDefaultSerializerOptions, cancellationToken);
            return stream.ToArray().AsMemory();
        }

        /// <summary>
        /// Async JSON bytes to model
        /// 异步 JSON 字节转化为模型
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="bytes">JSON bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static async ValueTask<T?> FromJsonBytesAsync<T>(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken) where T : class
        {
            await using var stream = SharedUtils.GetStream(bytes.Span);
            return await JsonSerializer.DeserializeAsync<T>(stream, SharedUtils.JsonDefaultSerializerOptions, cancellationToken);
        }
    }
}
