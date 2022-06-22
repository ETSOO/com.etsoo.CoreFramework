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

        /// <summary>
        /// '\r'
        /// </summary>
        public const byte CarriageReturnByte = 13;

        public const char LineFeed = '\n';

        /// <summary>
        /// '\n'
        /// </summary>
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
        private readonly int bufferSize;
        private readonly Memory<byte> bufferBytes;
        private int lastCount = -1;
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
            this.bufferSize = bufferSize;
            bufferBytes = new byte[bufferSize];

            this.leaveOpen = leaveOpen;
        }

        private ReadOnlySpan<byte> ReadBuffer(bool singleByte = false)
        {
            var span = bufferBytes.Span;

            // No reading or at the ending
            if (lastPos == -1 || lastPos == lastCount)
            {
                // Locate to the start
                lastPos = 0;

                lastCount = BaseStream.Read(span);
                if (lastCount == 0)
                {
                    // Reading completed
                    EndOfStream = true;
                    return Array.Empty<byte>();
                }
            }

            // Avoid zero bytes
            return singleByte ? span[lastPos..(lastPos + 1)] : span[lastPos..lastCount];
        }

        private async ValueTask<ReadOnlyMemory<byte>> ReadBufferAsync()
        {
            // No reading or at the ending
            if (lastPos == -1 || lastPos == lastCount)
            {
                // Locate to the start
                lastPos = 0;

                lastCount = await BaseStream.ReadAsync(bufferBytes);
                if (lastCount == 0)
                {
                    // Reading completed
                    EndOfStream = true;
                    return Array.Empty<byte>();
                }
            }

            // Avoid zero bytes
            return bufferBytes[lastPos..lastCount];
        }

        private async ValueTask<ReadOnlyMemory<byte>> BackwardReadBufferAsync()
        {
            // No reading or at the beginning
            if (lastPos == -1 || lastPos == 0)
            {
                var streamPos = BaseStream.Position;
                if (streamPos == 0)
                {
                    return Array.Empty<byte>();
                }

                var offset = Math.Min(bufferSize, streamPos);

                // Backward
                BaseStream.Seek(-offset, SeekOrigin.Current);

                // Read forward
                lastCount = await BaseStream.ReadAsync(bufferBytes);

                if (lastCount == 0)
                {
                    // Reading completed
                    EndOfStream = true;
                    return Array.Empty<byte>();
                }
                else if (lastCount < offset)
                {
                    // May not possible to happen, retry
                    BaseStream.Position = streamPos;
                    lastPos = -1;
                    return await BackwardReadBufferAsync();
                }

                // Backward
                BaseStream.Seek(-offset, SeekOrigin.Current);

                // Locate to the end
                lastPos = lastCount;
            }

            // Avoid zero bytes
            return bufferBytes[..lastPos];
        }

        private ReadOnlySpan<byte> ReadBufferLine(out bool success)
        {
            // Read buffer, make sure one byte at least is ready
            var span = ReadBuffer();
            if (EndOfStream)
            {
                success = true;
                return span;
            }

            // Search directly
            var pos = span.IndexOfAny(LineFeedByte, CarriageReturnByte);
            if (pos > -1)
            {
                var result = span[..pos];

                // Move forward
                pos++;

                // Only support /r or /n, not /r/n
                lastPos += pos;

                success = true;
                return result;
            }
            else
            {
                lastPos = -1;
            }

            success = false;
            return span;
        }

        private async ValueTask<(bool, ReadOnlyMemory<byte>)> ReadBufferLineAsync()
        {
            // Read buffer, make sure one byte at least is ready
            var memory = await ReadBufferAsync();
            if (EndOfStream)
            {
                return (true, memory);
            }

            // Search directly
            var pos = memory.Span.IndexOfAny(LineFeedByte, CarriageReturnByte);
            if (pos > -1)
            {
                var result = memory[..pos];

                // Move forward
                pos++;

                // Only support /r or /n, not /r/n
                lastPos += pos;

                return (true, result);
            }
            else
            {
                lastPos = -1;
            }

            return (false, memory);
        }

        private async ValueTask<(bool, ReadOnlyMemory<byte>)> BackwardReadBufferLineAsync()
        {
            // Read buffer, make sure one byte at least is ready
            var memory = await BackwardReadBufferAsync();
            if (memory.IsEmpty)
            {
                return (true, memory);
            }

            // Search directly
            var pos = memory.Span.LastIndexOfAny(LineFeedByte, CarriageReturnByte);
            if (pos > -1)
            {
                var result = memory[(pos + 1)..];

                // Move backward
                // pos--;

                // Only support /r or /n, not /r/n
                lastPos = pos;

                return (true, result);
            }
            else
            {
                lastPos = -1;
            }

            return (false, memory);
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
            var pos = span.IndexOf(target);
            if (pos > -1)
            {
                var result = span[..pos];

                // Move forward
                pos++;

                // Update global pos
                lastPos += pos;

                success = true;
                return result;
            }
            else
            {
                lastPos = -1;
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
            var pos = span.IndexOfAny(targets);
            if (pos > -1)
            {
                var result = span[..pos];

                // Move forward
                pos++;

                // Ignore all consecutive targets
                // 忽略所有连续目标
                if (ignoreConsecutives)
                {
                    while (!EndOfStream)
                    {
                        if (pos == span.Length)
                        {
                            span = ReadBuffer();
                            if (EndOfStream) break;
                            pos = 0;
                        }

                        var b = span[pos];
                        if (targets.Contains(b))
                        {
                            pos++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Update global pos
                lastPos += pos;

                success = true;
                return result;
            }
            else
            {
                lastPos = -1;
            }

            success = false;
            return span;
        }

        /// <summary>
        /// Clear buffer
        /// 清除缓存
        /// </summary>
        public void ClearBuffer()
        {
            lastPos = -1;
            bufferBytes.Span.Clear();
        }

        /// <summary>
        /// Returns the next available byte but does not move the index
        /// 返回下一个可用字节但不移动索引
        /// </summary>
        /// <returns>Result</returns>
        public byte? Peek()
        {
            return ReadByteBase(false);
        }

        /// <summary>
        /// Read one byte
        /// 读取一个字节
        /// </summary>
        /// <returns>Result</returns>
        public byte? ReadByte()
        {
            return ReadByteBase(true);
        }

        /// <summary>
        /// Read bytes
        /// 读取多个字节
        /// </summary>
        /// <param name="count">Bytes count</param>
        /// <returns>Result</returns>
        public ReadOnlySpan<byte> ReadBytes(int count)
        {
            var w = new ArrayBufferWriter<byte>(count);

            while (count > 0)
            {
                // Read buffer, make sure one byte at least is ready
                var span = ReadBuffer();
                if (EndOfStream)
                {
                    break;
                }

                if (count <= span.Length)
                {
                    lastPos += count;
                    w.Write(span[..count]);
                    break;
                }

                // Reset to another round of reading
                lastPos = -1;
                count -= span.Length;
                w.Write(span);
            }

            return w.WrittenSpan;
        }

        /// <summary>
        /// Read bytes from range
        /// 从指定区域读取多个字节
        /// </summary>
        /// <param name="range">Bytes range</param>
        /// <returns>Result</returns>
        public ReadOnlySpan<byte> ReadBytes(ReadOnlySpan<byte> range)
        {
            var w = new ArrayBufferWriter<byte>();

            do
            {
                var span = ReadBuffer();
                if (EndOfStream) break;

                for (var i = 0; i < span.Length; i++)
                {
                    if (!range.Contains(span[i]))
                    {
                        if (i > 0)
                        {
                            w.Write(span[..i]);
                            lastPos += i;

                            // Exit directly
                            return w.WrittenSpan;
                        }
                        break;
                    }
                }

                // Reset for another round of reading
                w.Write(span);
                lastPos = -1;
            } while (true);

            return w.WrittenSpan;
        }

        /// <summary>
        /// Discard coming bytes
        /// 丢弃将读取的字节
        /// </summary>
        /// <param name="count">Bytes count</param>
        public void Discard(int count)
        {
            var span = ReadBuffer();
            if (EndOfStream) return;

            if (count <= span.Length)
            {
                // Within the buffer range
                lastPos += count;
            }
            else
            {
                count -= span.Length;

                // If the base stream can seek
                if (BaseStream.CanSeek)
                {
                    if (BaseStream.Position + count < BaseStream.Length)
                        BaseStream.Position += count;
                    else
                        BaseStream.Seek(0, SeekOrigin.End);
                }
                else
                {
                    do
                    {
                        var buffer = new byte[Math.Min(count, DefaultBufferSize)];
                        var bytesRead = BaseStream.Read(buffer);
                        if (bytesRead == 0) break;
                        count -= bytesRead;
                    }
                    while (count > 0);
                }

                // Back to ready reading status
                lastPos = -1;
            }
        }

        /// <summary>
        /// Discard consecutive bytes
        /// 丢弃连续的字节
        /// </summary>
        /// <param name="bytes">Bytes</param>
        public void Discard(ReadOnlySpan<byte> bytes)
        {
            byte? peek;
            while ((peek = Peek()) is not null)
            {
                if (bytes.Contains(peek.Value))
                {
                    Discard(1);
                }
                else
                {
                    break;
                }
            }
        }

        private byte? ReadByteBase(bool moveIndex)
        {
            // Read buffer
            var span = ReadBuffer(true);

            // No reading
            if (span.Length == 0) return null;

            if (moveIndex) lastPos++;

            return span[0];
        }

        /// <summary>
        /// Read line
        /// 读取行
        /// </summary>
        /// <returns>Bytes</returns>
        public ReadOnlySpan<byte> ReadLine()
        {
            // First try with reading from buffer success
            var span = ReadBufferLine(out var success);
            if (success) return span;

            var writer = new ArrayBufferWriter<byte>();
            do
            {
                // Write current content
                writer.Write(span);

                // Continue reading
                span = ReadBufferLine(out var newSuccess);
                if (newSuccess)
                {
                    writer.Write(span);
                    break;
                }
            }
            while (!EndOfStream);

            return writer.WrittenSpan;
        }

        /// <summary>
        /// Async read line
        /// 异步读取行
        /// </summary>
        /// <returns>Bytes</returns>
        public async Task<ReadOnlyMemory<byte>> ReadLineAsync()
        {
            // First try with reading from buffer success
            var (success, memory) = await ReadBufferLineAsync();
            if (success) return memory;

            var writer = new ArrayBufferWriter<byte>();
            do
            {
                // Write current content
                writer.Write(memory.Span);

                // Continue reading
                var (newSuccess, newMemory) = await ReadBufferLineAsync();
                if (newSuccess)
                {
                    writer.Write(newMemory.Span);
                    break;
                }
                else
                {
                    memory = newMemory;
                }
            }
            while (!EndOfStream);

            return writer.WrittenMemory;
        }

        /// <summary>
        /// Async backward read line
        /// 异步反向读取行
        /// </summary>
        /// <returns>Bytes</returns>
        public async Task<ReadOnlyMemory<byte>> BackwardReadLineAsync()
        {
            // First try with reading from buffer success
            var (success, memory) = await BackwardReadBufferLineAsync();
            if (success) return memory;

            var bytes = new List<byte>();

            do
            {
                // Write current content
                bytes.InsertRange(0, memory.ToArray());

                // Continue reading
                var (newSuccess, newMemory) = await BackwardReadBufferLineAsync();
                if (newSuccess)
                {
                    bytes.InsertRange(0, newMemory.ToArray());
                    break;
                }
                else
                {
                    memory = newMemory;
                }
            }
            while (true);

            return bytes.ToArray().AsMemory();
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

        /// <summary>
        /// Read with callback
        /// 用回调读取
        /// </summary>
        /// <param name="callback">Callback</param>
        public void ReadWhile(Func<byte, bool> callback)
        {
            do
            {
                var span = ReadBuffer();
                for (var i = 0; i < span.Length; i++)
                {
                    lastPos++;
                    if (callback(span[i])) return;
                }
            } while (!EndOfStream);
        }

        /// <summary>
        /// To stream end
        /// 到流尾端
        /// </summary>
        public void ToStreamEnd()
        {
            ClearBuffer();
            BaseStream.Seek(0, SeekOrigin.End);
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