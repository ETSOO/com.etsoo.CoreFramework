using com.etsoo.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

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
        [RequiresDynamicCode("ToJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("ToJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task<ReadOnlyMemory<byte>> ToJsonBytesAsync<T>(T model, CancellationToken cancellationToken = default)
        {
            return await ToJsonBytesAsync(model, SharedUtils.JsonDefaultSerializerOptions, cancellationToken);
        }

        /// <summary>
        /// Async model to JSON bytes
        /// 异步模型到 JSON 字节
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="options">Json options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("ToJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("ToJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task<ReadOnlyMemory<byte>> ToJsonBytesAsync<T>(T model, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        {
            await using var stream = SharedUtils.GetStream();
            await JsonSerializer.SerializeAsync(stream, model, options, cancellationToken);

            if (stream.TryGetBuffer(out var buffer))
            {
                return buffer.AsMemory();
            }
            else
            {
                return stream.ToArray().AsMemory();
            }
        }

        /// <summary>
        /// Async model to JSON bytes
        /// 异步模型到 JSON 字节
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<ReadOnlyMemory<byte>> ToJsonBytesAsync<T>(T model, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default)
        {
            await using var stream = SharedUtils.GetStream();
            await JsonSerializer.SerializeAsync(stream, model, typeInfo, cancellationToken);
            if (stream.TryGetBuffer(out var buffer))
            {
                return buffer.AsMemory();
            }
            else
            {
                return stream.ToArray().AsMemory();
            }
        }

        /// <summary>
        /// Async JSON bytes to model
        /// 异步 JSON 字节转化为模型
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="bytes">JSON bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        [RequiresDynamicCode("FromJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("FromJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async ValueTask<T?> FromJsonBytesAsync<T>(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default) where T : class
        {
            return await FromJsonBytesAsync<T>(bytes, SharedUtils.JsonDefaultSerializerOptions, cancellationToken);
        }

        /// <summary>
        /// Async JSON bytes to model
        /// 异步 JSON 字节转化为模型
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="bytes">JSON bytes</param>
        /// <param name="options">Json options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        [RequiresDynamicCode("FromJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("FromJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async ValueTask<T?> FromJsonBytesAsync<T>(ReadOnlyMemory<byte> bytes, JsonSerializerOptions options, CancellationToken cancellationToken = default) where T : class
        {
            await using var stream = SharedUtils.GetStream(bytes.Span);
            return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
        }

        /// <summary>
        /// Async JSON bytes to model
        /// 异步 JSON 字节转化为模型
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="bytes">JSON bytes</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static async ValueTask<T?> FromJsonBytesAsync<T>(ReadOnlyMemory<byte> bytes, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default) where T : class
        {
            await using var stream = SharedUtils.GetStream(bytes.Span);
            return await JsonSerializer.DeserializeAsync(stream, typeInfo, cancellationToken);
        }
    }
}
