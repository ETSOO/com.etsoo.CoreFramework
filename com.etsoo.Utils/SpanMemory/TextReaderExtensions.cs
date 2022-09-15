using System.Buffers;
using System.Text;

namespace com.etsoo.Utils.SpanMemory
{
    /// <summary>
    /// TextReader extensions
    /// TextReader 扩展
    /// </summary>
    public static class TextReaderExtensions
    {
        /// <summary>
        /// Bytes to read with TextReader
        /// </summary>
        public const int BytesToRead = 1024;

        /// <summary>
        /// Read all to stream
        /// 读取所有字节到写入流
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="stream">Stream</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Task</returns>
        public static async Task ReadAllBytesAsyn(this TextReader reader, Stream stream, Encoding? encoding = null)
        {
            // Default encoding
            encoding ??= Encoding.UTF8;

            // Memory for read
            Memory<char> memory = new char[BytesToRead];

            int read;
            while ((read = await reader.ReadBlockAsync(memory)) > 0)
            {
                // Memory
                Memory<byte> bytes = new byte[encoding.GetMaxByteCount(read)];

                // Write the chars to writer
                var bytesCount = encoding.GetBytes(memory.Span.Slice(0, read), bytes.Span);

                // Write bytes
                await stream.WriteAsync(bytes.Slice(0, bytesCount));

                // Make bytes written available
                await stream.FlushAsync();
            }
        }

        /// <summary>
        /// Read all to writer
        /// 读取所有字节到写入器
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="writer">Buffer writer, like SharedUtils.GetStream() or PipeWriter</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Task</returns>
        public static async Task ReadAllBytesAsyn(this TextReader reader, IBufferWriter<byte> writer, Encoding? encoding = null)
        {
            // Default encoding
            encoding ??= Encoding.UTF8;

            // Memory for read
            Memory<char> memory = new char[BytesToRead];

            int read;
            while ((read = await reader.ReadBlockAsync(memory)) > 0)
            {
                // Write the chars to writer
                encoding.GetBytes(memory.Span[..read], writer);
            }
        }

        /// <summary>
        /// Read all to writer
        /// 读取所有字节到写入器
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="maxLength">Max length</param>
        /// <returns>Task</returns>
        public static async Task<ReadOnlyMemory<char>> ReadAllCharsAsyn(this TextReader reader, int? maxLength = null)
        {
            var writer = maxLength.HasValue ? new ArrayBufferWriter<char>(maxLength.Value) : new ArrayBufferWriter<char>();

            // Memory for read
            var memory = writer.GetMemory(BytesToRead);

            int read;
            while ((read = await reader.ReadBlockAsync(memory)) > 0)
            {
                writer.Advance(read);
            }

            return writer.WrittenMemory;
        }
    }
}
