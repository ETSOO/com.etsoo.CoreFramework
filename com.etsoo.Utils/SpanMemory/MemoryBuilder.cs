using System;

namespace com.etsoo.Utils.SpanMemory
{
    /// <summary>
    /// Memory builder
    /// Memory 构造器
    /// </summary>
    /// <typeparam name="T">Generic memory item type</typeparam>
    public ref struct MemoryBuilder<T>
    {
        private readonly Memory<T> memory;
        private int pos;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="maxLength">Max length</param>
        public MemoryBuilder(int maxLength)
        {
            memory = new T[maxLength];
            pos = 0;
        }

        /// <summary>
        /// Append single item
        /// 追加单独项目
        /// </summary>
        /// <param name="item">Single item</param>
        public void Append(T item)
        {
            memory.Span[pos] = item;
            pos++;
        }

        /// <summary>
        /// Append new item
        /// 追加新项目
        /// </summary>
        /// <param name="item">New item</param>
        /// <param name="firstItemFormatter">First item formatter</param>
        public void Append(ReadOnlySpan<T> item, Func<T, T>? firstItemFormatter = null)
        {
            if (item.IsEmpty)
                return;

            item.CopyTo(memory.Span[pos..]);

            if (firstItemFormatter != null)
            {
                memory.Span[pos] = firstItemFormatter(memory.Span[pos]);
            }

            pos += item.Length;
        }

        /// <summary>
        /// As memory
        /// 获取为 Memory
        /// </summary>
        /// <returns>Span</returns>
        public ReadOnlyMemory<T> AsMemory()
        {
            return memory.Slice(0, pos);
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns>Result</returns>
        public override string ToString()
        {
            return AsMemory().ToString();
        }
    }
}
