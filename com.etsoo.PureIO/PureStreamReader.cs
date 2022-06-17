using System.Buffers;

namespace com.etsoo.PureIO
{
    /// <summary>
    /// Pure stream reader for file bytes parser
    /// 纯流阅读器用于文件字节解析
    /// </summary>
    public class PureStreamReader : IDisposable, IAsyncDisposable
    {
        public const char CarriageReturn = '\r';
        public const byte CarriageReturnByte = 13;

        public const char LineFeed = '\n';
        public const byte LineFeedByte = 10;

        private const int DefaultBufferSize = 1024;
        private const int MinimumBufferSize = 8;

        /// <summary>
        /// Base stream
        /// 基础流对象
        /// </summary>
        public Stream BaseStream { get; init; }

        /// <summary>
        /// true if the current stream position is at the end of the stream; otherwise false
        /// 如果当前流位置在流的末尾，则为真； 否则为假
        /// </summary>
        public bool EndOfStream { get; private set; }

        private readonly bool leaveOpen;
        private readonly Memory<byte> bufferBytes;
        private int lastPos = -1;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="stream">Base stream</param>
        /// <param name="bufferSize">Buffer size when reading</param>
        /// <param name="leaveOpen">Leave open</param>
        public PureStreamReader(Stream stream, int bufferSize = DefaultBufferSize, bool leaveOpen = false)
        {
            BaseStream = stream;

            if (bufferSize < MinimumBufferSize) bufferSize = MinimumBufferSize;
            bufferBytes = new byte[bufferSize];

            this.leaveOpen = leaveOpen;
        }

        private ReadOnlySpan<byte> ReadBuffer()
        {
            var span = bufferBytes.Span;

            if (lastPos == -1)
            {
                var count = BaseStream.Read(span);
                if (count == 0)
                {
                    EndOfStream = true;
                    return Array.Empty<byte>();
                }

                lastPos = 0;
                return count < span.Length ? span[..count] : span;
            }
            else
            {
                return span[lastPos..];
            }
        }

        private ReadOnlySpan<byte> ReadBufferLine(out bool success)
        {
            // Read buffer, make sure one byte at least is ready
            var span = ReadBuffer();
            if (EndOfStream)
            {
                success = false;
                return span;
            }

            // Search directly
            lastPos = span.IndexOfAny(LineFeedByte, CarriageReturnByte);
            if (lastPos > -1)
            {
                var result = span[..lastPos];

                // Is return?
                var isReturn = span[lastPos] == CarriageReturnByte;

                // Move forward
                lastPos++;

                // r, n or rn, consider rn case
                if (isReturn)
                {
                    if (lastPos < span.Length)
                    {
                        if (span[lastPos] == LineFeedByte)
                        {
                            lastPos++;
                        }
                    }
                    else
                    {
                        // Continue reading buffer
                        lastPos = -1;
                        span = ReadBuffer();
                        if (span[0] == LineFeedByte)
                        {
                            // Jump to next byte
                            lastPos = 1;
                        }
                    }
                }

                if (lastPos == span.Length)
                {
                    // Reset when now is in the end
                    lastPos = -1;
                }

                success = true;
                return result;
            }

            success = false;
            return span;
        }

        private ReadOnlySpan<byte> ReadBufferTo(byte target, out bool success)
        {
            // Read buffer, make sure one byte at least is ready
            var span = ReadBuffer();
            if (EndOfStream)
            {
                success = false;
                return span;
            }

            // Search directly
            lastPos = span.IndexOf(target);
            if (lastPos > -1)
            {
                var result = span[..lastPos];

                // Move forward
                lastPos++;

                if (lastPos == span.Length)
                {
                    // Reset when now is in the end
                    lastPos = -1;
                }

                success = true;
                return result;
            }

            success = false;
            return span;
        }

        private ReadOnlySpan<byte> ReadBufferTo(ReadOnlySpan<byte> targets, out bool success, bool ignoreConsecutives = true)
        {
            // Read buffer, make sure one byte at least is ready
            var span = ReadBuffer();
            if (EndOfStream)
            {
                success = false;
                return span;
            }

            // Search directly
            lastPos = span.IndexOfAny(targets);
            if (lastPos > -1)
            {
                var result = span[..lastPos];

                // Move forward
                lastPos++;

                // Ignore all consecutive targets
                // 忽略所有连续目标
                if (ignoreConsecutives)
                {
                    while (!EndOfStream)
                    {
                        if (lastPos >= span.Length)
                        {
                            lastPos = -1;
                        }

                        if (lastPos == -1)
                        {
                            span = ReadBuffer();
                            if (EndOfStream) break;
                        }

                        var b = span[lastPos];
                        if (targets.Contains(b))
                        {
                            lastPos++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (lastPos == span.Length)
                {
                    // Reset when now is in the end
                    lastPos = -1;
                }

                success = true;
                return result;
            }

            success = false;
            return span;
        }

        /// <summary>
        /// Returns the next available byte but does not pop it
        /// 返回下一个可用字节但不使用它
        /// </summary>
        /// <returns>Result</returns>
        public byte? Peek()
        {
            // Read buffer
            var span = ReadBuffer();
            if (span.Length == 0) return null;
            return span[0];
        }

        /// <summary>
        /// Read one byte
        /// 读取一个字节
        /// </summary>
        /// <returns>Result</returns>
        public byte? ReadByte()
        {
            // Read buffer
            var span = ReadBuffer();

            if (span.Length == 0) return null;
            lastPos++;
            return span[0];
        }

        /// <summary>
        /// Read line
        /// 读取行
        /// </summary>
        /// <param name="maxReading">Max reading bytes, default is 100K</param>
        /// <returns>Bytes</returns>
        public ReadOnlySpan<byte> ReadLine(int maxReading = DefaultBufferSize * 100)
        {
            // First try with reading from buffer success
            var span = ReadBufferLine(out var success);
            if (success) return span;

            var writer = new ArrayBufferWriter<byte>();
            do
            {
                // Accumulate reading
                maxReading -= span.Length;
                if (maxReading <= 0) break;

                // Write current content
                writer.Write(span);

                // Continue reading
                span = ReadBufferLine(out var moreTry);
                if (moreTry)
                {
                    writer.Write(span);
                    break;
                }
            }
            while (!EndOfStream);

            return writer.WrittenSpan;
        }

        /// <summary>
        /// Read all bytes until to the target byte, after reading will move over the target byte.
        /// 读取所有字节直到目标字节，读取后将移过目标字节
        /// </summary>
        /// <param name="target">Target byte</param>
        /// <returns>Result</returns>
        public ReadOnlySpan<byte> ReadTo(byte target)
        {
            // First try with reading from buffer success
            var span = ReadBufferTo(target, out var success);
            if (success) return span;

            var writer = new ArrayBufferWriter<byte>();
            do
            {
                // Write current content
                writer.Write(span);

                // Continue reading
                span = ReadBufferTo(target, out var moreTry);
                if (moreTry)
                {
                    writer.Write(span);
                    break;
                }
            }
            while (!EndOfStream);

            return writer.WrittenSpan;
        }

        /// <summary>
        /// Read all bytes until to the target bytes, after reading will move over the target bytes.
        /// 读取所有字节直到目标字节，读取后将移过目标字节
        /// </summary>
        /// <param name="targets">Target bytes</param>
        /// <param name="ignoreConsecutives">Ignore all consecutive targets</param>
        /// <returns>Result</returns>
        public ReadOnlySpan<byte> ReadTo(ReadOnlySpan<byte> targets, bool ignoreConsecutives = true)
        {
            // First try with reading from buffer success
            var span = ReadBufferTo(targets, out var success, ignoreConsecutives);
            if (success) return span;

            var writer = new ArrayBufferWriter<byte>();
            do
            {
                // Write current content
                writer.Write(span);

                // Continue reading
                span = ReadBufferTo(targets, out var moreTry, ignoreConsecutives);
                if (moreTry)
                {
                    writer.Write(span);
                    break;
                }
            }
            while (!EndOfStream);

            return writer.WrittenSpan;
        }

        public async ValueTask DisposeAsync()
        {
            if (!leaveOpen)
            {
                await BaseStream.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            if (!leaveOpen)
            {
                BaseStream.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}