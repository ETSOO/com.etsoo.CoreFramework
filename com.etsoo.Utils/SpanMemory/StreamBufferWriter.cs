using System;
using System.Buffers;

namespace com.etsoo.Utils.SpanMemory
{
    /// <summary>
    /// Stream buffer writer
    /// 流缓冲区写入器
    /// </summary>
    /// <typeparam name="T">Generic item type</typeparam>
    public class StreamBufferWriter<T> : IBufferWriter<T>
    {
        private readonly Memory<T> memory;
        private readonly int memoryLength;
        private int pos;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="maxLength">Max length</param>
        public StreamBufferWriter(int maxLength)
        {
            memory = new T[maxLength];
            memoryLength = maxLength;
            pos = 0;
        }

        public void Advance(int count)
        {
            pos += count;
        }

        private int GetSize(int sizeHint)
        {
            if (sizeHint < 1)
                return memoryLength;

            return Math.Min(sizeHint, memoryLength);
        }

        /// <summary>
        /// Get memoery
        /// </summary>
        /// <param name="sizeHint">Size to read</param>
        /// <returns>Memory</returns>
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            return memory.Slice(pos, GetSize(sizeHint));
        }

        /// <summary>
        /// Get span
        /// </summary>
        /// <param name="sizeHint">Size to read</param>
        /// <returns>Span</returns>
        public Span<T> GetSpan(int sizeHint = 0)
        {
            return memory.Span.Slice(pos, GetSize(sizeHint));
        }

        /// <summary>
        /// As memory
        /// </summary>
        /// <returns>Memory</returns>
        public ReadOnlyMemory<T> AsMemory()
        {
            return memory.Slice(0, pos);
        }
    }
}
