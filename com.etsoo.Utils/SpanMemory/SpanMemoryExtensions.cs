using System;
using System.Text;

namespace com.etsoo.Utils.SpanMemory
{
    /// <summary>
    /// Span & Memory extensions
    /// Span & Memory 扩展
    /// </summary>
    public static class SpanMemoryExtensions
    {
        /// <summary>
        /// Is all chars match the predicate
        /// 是否所有字符都与谓词匹配
        /// </summary>
        /// <param name="chars">Chars</param>
        /// <param name="predicate">Predicate</param>
        /// <returns>Is all passed</returns>
        public static bool All(this ReadOnlySpan<char> chars, Func<char, bool> predicate)
        {
            foreach (var c in chars)
            {
                if (!predicate(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Char array to UTF8 byte array
        /// 字符数组转化为 UTF8 字节数组
        /// </summary>
        /// <param name="chars">Chars</param>
        /// <returns>Bytes</returns>
        public static ReadOnlySpan<byte> ToEncodingBytes(this ReadOnlySpan<char> chars)
        {
            return chars.ToEncodingBytes(Encoding.UTF8);
        }

        /// <summary>
        /// Char array to byte array
        /// 字符数组转化为字节数组
        /// </summary>
        /// <param name="chars">Chars</param>
        /// <returns>Bytes</returns>
        public static ReadOnlySpan<byte> ToEncodingBytes(this ReadOnlySpan<char> chars, Encoding encoding)
        {
            Span<byte> bytes = new byte[encoding.GetMaxByteCount(chars.Length)];

            var bytesCount = encoding.GetBytes(chars, bytes);

            return bytes[0..bytesCount];
        }

        /// <summary>
        /// Char array to UTF8 byte array
        /// 字符数组转化为 UTF8 字节数组
        /// </summary>
        /// <param name="chars">Chars</param>
        /// <returns>Bytes</returns>
        public static Memory<byte> ToEncodingBytes(this ReadOnlyMemory<char> chars)
        {
            return chars.ToEncodingBytes(Encoding.UTF8);
        }

        /// <summary>
        /// Char array to byte array
        /// 字符数组转化为字节数组
        /// </summary>
        /// <param name="chars">Chars</param>
        /// <returns>Bytes</returns>
        public static Memory<byte> ToEncodingBytes(this ReadOnlyMemory<char> chars, Encoding encoding)
        {
            Memory<byte> bytes = new byte[encoding.GetMaxByteCount(chars.Length)];

            var bytesCount = encoding.GetBytes(chars.Span, bytes.Span);

            return bytes[0..bytesCount];
        }
    }
}
