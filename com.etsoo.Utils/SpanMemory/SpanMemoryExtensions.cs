using System.Buffers;
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
        /// Base64 char array to byte array
        /// Base64编码字符数组转化为字节数组
        /// </summary>
        /// <param name="chars">Base64 chars</param>
        /// <returns>Bytes</returns>
        public static ReadOnlySpan<byte> ToBase64Bytes(this ReadOnlySpan<char> chars)
        {
            Span<byte> bytes = new byte[chars.Length];
            Convert.TryFromBase64Chars(chars, bytes, out var bytesWritten);
            return bytes[..bytesWritten];
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
            var writer = new ArrayBufferWriter<byte>();
            encoding.GetBytes(chars, writer);
            return writer.WrittenSpan;
        }

        /// <summary>
        /// Char array to UTF8 byte array
        /// 字符数组转化为 UTF8 字节数组
        /// </summary>
        /// <param name="chars">Chars</param>
        /// <returns>Bytes</returns>
        public static ReadOnlyMemory<byte> ToEncodingBytes(this ReadOnlyMemory<char> chars)
        {
            return chars.ToEncodingBytes(Encoding.UTF8);
        }

        /// <summary>
        /// Char array to byte array
        /// 字符数组转化为字节数组
        /// </summary>
        /// <param name="chars">Chars</param>
        /// <returns>Bytes</returns>
        public static ReadOnlyMemory<byte> ToEncodingBytes(this ReadOnlyMemory<char> chars, Encoding encoding)
        {
            var writer = new ArrayBufferWriter<byte>();
            encoding.GetBytes(chars.Span, writer);
            return writer.WrittenMemory;
        }
    }
}
