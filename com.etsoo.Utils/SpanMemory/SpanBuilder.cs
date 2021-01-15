using System;

namespace com.etsoo.Utils.SpanMemory
{
    /// <summary>
    /// Span builder
    /// Span 构造器
    /// </summary>
    /// <typeparam name="T">Generic span item type</typeparam>
    public ref struct SpanBuilder<T>
    {
        private readonly Span<T> span;
        private int pos;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="maxLength">Max length</param>
        public SpanBuilder(int maxLength)
        {
            span = new T[maxLength];
            pos = 0;
        }

        /// <summary>
        /// Append single item
        /// 追加单独项目
        /// </summary>
        /// <param name="item">Single item</param>
        public void Append(T item)
        {
            span[pos] = item;
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

            item.CopyTo(span[pos..]);

            if (firstItemFormatter != null)
            {
                span[pos] = firstItemFormatter(span[pos]);
            }

            pos += item.Length;
        }

        /// <summary>
        /// As span
        /// 获取为 Span
        /// </summary>
        /// <returns>Span</returns>
        public ReadOnlySpan<T> AsSpan()
        {
            return span.Slice(0, pos);
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns>Result</returns>
        public override string ToString()
        {
            return AsSpan().ToString();
        }
    }
}
