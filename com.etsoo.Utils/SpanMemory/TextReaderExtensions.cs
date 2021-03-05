﻿using System;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="writer">Pipe writer</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Task</returns>
        public static async Task ReadAllBytesAsyn(this TextReader reader, PipeWriter writer, Encoding? encoding = null)
        {
            // Default encoding
            encoding ??= Encoding.UTF8;

            // Memory for read
            Memory<char> memory = new char[BytesToRead];

            int read;
            while ((read = await reader.ReadBlockAsync(memory)) > 0)
            {
                // Write the chars to writer
                encoding.GetBytes(memory.Span.Slice(0, read), writer);

                // Make bytes written available
                await writer.FlushAsync();
            }
        }

        /// <summary>
        /// Read all to writer
        /// 读取所有字节到写入器
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="maxLength">Max length</param>
        /// <returns>Task</returns>
        public static async Task<ReadOnlyMemory<char>> ReadAllCharsAsyn(this TextReader reader, int maxLength)
        {
            var writer = new StreamBufferWriter<char>(maxLength);

            // Memory for read
            var memory = writer.GetMemory(Math.Min(maxLength, BytesToRead));

            int read;
            while ((read = await reader.ReadBlockAsync(memory)) > 0)
            {
                writer.Advance(read);
            }

            return writer.AsMemory();
        }
    }
}