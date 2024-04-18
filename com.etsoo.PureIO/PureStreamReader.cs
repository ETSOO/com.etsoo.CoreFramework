using System.Buffers;
using System.Text;

namespace com.etsoo.PureIO
{
    /// <summary>
    /// Pure stream read way
    /// 纯流读取方式
    /// </summary>
    [Flags]
    public enum PureStreamReadWay : byte
    {
        /// <summary>
        /// Incudes /r, /n, /rn and ignore line ending character(s)
        /// 包括所有行尾组合，忽略行结束字符
        /// </summary>
        Default = 1,

        /// <summary>
        /// Only identify line feed as line ending character
        /// 仅识别换行符作为结束字符并忽略
        /// </summary>
        LineFeedOnly = 2,

        /// <summary>
        /// Return all characters
        /// 返回所有字符
        /// </summary>
        ReturnAll = 4
    }

    /// <summary>
    /// Pure stream reader for file bytes parser
    /// 纯流阅读器用于文件字节解析
    /// </summary>
    public class PureStreamReader : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// '\r'
        /// </summary>
        public const byte CarriageReturnByte = 13;

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
        /// Is little-endian (LE) or big-endian (BE) for digital data
        /// https://en.wikipedia.org/wiki/Endianness
        /// </summary>
        public bool IsLittleEndian { get; init; } = BitConverter.IsLittleEndian;

        /// <summary>
        /// true if the current stream position is at the end of the stream; otherwise false
        /// 如果当前流位置在流的末尾，则为真； 否则为假
        /// </summary>
        public bool EndOfStream { get; private set; }

        /// <summary>
        /// Current reading position (from 0)
        /// </summary>
        public long CurrentPosition => backward ? BaseStream.Position + lastPos : BaseStream.Position - lastCount + lastPos;

        private readonly bool leaveOpen;
        private readonly int bufferSize;
        private readonly Memory<byte> bufferBytes;
        private bool backward = false;
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
                    return [];
                }

                backward = false;
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

                backward = false;
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
                backward = true;

                // Locate to the end
                lastPos = lastCount;
            }

            // Avoid zero bytes
            return bufferBytes[..lastPos];
        }

        private ReadOnlySpan<byte> ReadBufferLine(out bool success, PureStreamReadWay way = PureStreamReadWay.Default)
        {
            // Read buffer, make sure one byte at least is ready
            var span = ReadBuffer();
            if (EndOfStream)
            {
                success = true;
                return span;
            }

            // Search directly
            var pos = way.HasFlag(PureStreamReadWay.LineFeedOnly) ? span.IndexOf(LineFeedByte) : span.IndexOfAny(LineFeedByte, CarriageReturnByte);
            if (pos > -1)
            {
                var returnAll = way.HasFlag(PureStreamReadWay.ReturnAll);
                var result = returnAll ? span[..(pos + 1)] : span[..pos];

                // Is return byte?
                var isReturn = span[pos] == CarriageReturnByte;

                // Move forward
                pos++;
                lastPos += pos;

                // r, n or rn, consider rn case
                if (isReturn)
                {
                    if (lastPos < lastCount)
                    {
                        if (span[lastPos] == LineFeedByte)
                        {
                            lastPos++;

                            if (returnAll)
                            {
                                result = span[..(pos + 1)];
                            }
                        }
                    }
                    else
                    {
                        // Need new reading, will override the span
                        var newLen = result.Length + (returnAll ? 1 : 0);
                        Span<byte> newResult = new byte[newLen];
                        result.CopyTo(newResult);

                        if (Peek() == LineFeedByte)
                        {
                            lastPos++;

                            if (returnAll)
                            {
                                newResult[newLen - 1] = LineFeedByte;
                            }
                        }
                        else if (returnAll)
                        {
                            newResult = newResult[..^1];
                        }

                        result = newResult;
                    }
                }

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

        private async ValueTask<(bool, ReadOnlyMemory<byte>)> ReadBufferLineAsync(PureStreamReadWay way = PureStreamReadWay.Default)
        {
            // Read buffer, make sure one byte at least is ready
            var memory = await ReadBufferAsync();
            if (EndOfStream)
            {
                return (true, memory);
            }

            // Search directly
            var pos = way.HasFlag(PureStreamReadWay.LineFeedOnly) ? memory.Span.IndexOf(LineFeedByte) : memory.Span.IndexOfAny(LineFeedByte, CarriageReturnByte);
            if (pos > -1)
            {
                var returnAll = way.HasFlag(PureStreamReadWay.ReturnAll);
                var result = returnAll ? memory[..(pos + 1)] : memory[..pos];

                // Is return byte?
                var isReturn = memory.Span[pos] == CarriageReturnByte;

                // Move forward
                pos++;
                lastPos += pos;

                // r, n or rn, consider rn case
                if (isReturn)
                {
                    if (lastPos < lastCount)
                    {
                        if (Peek() == LineFeedByte)
                        {
                            lastPos++;

                            if (returnAll)
                            {
                                result = memory[..(pos + 1)];
                            }
                        }
                    }
                    else
                    {
                        // Need new reading, will override the span
                        var newLen = result.Length + (returnAll ? 1 : 0);
                        Memory<byte> newResult = new byte[newLen];
                        result.CopyTo(newResult);

                        if (Peek() == LineFeedByte)
                        {
                            lastPos++;

                            if (returnAll)
                            {
                                newResult.Span[newLen - 1] = LineFeedByte;
                            }
                        }
                        else if (returnAll)
                        {
                            newResult = newResult[..^1];
                        }

                        result = newResult;
                    }
                }

                return (true, result);
            }
            else
            {
                lastPos = -1;
            }

            return (false, memory);
        }

        private async ValueTask<(bool, ReadOnlyMemory<byte>)> BackwardReadBufferLineAsync(PureStreamReadWay way = PureStreamReadWay.Default)
        {
            // Read buffer, make sure one byte at least is ready
            var memory = await BackwardReadBufferAsync();
            if (memory.IsEmpty)
            {
                return (true, memory);
            }

            // Search directly
            var pos = way.HasFlag(PureStreamReadWay.LineFeedOnly) ? memory.Span.LastIndexOf(LineFeedByte) : memory.Span.LastIndexOfAny(LineFeedByte, CarriageReturnByte);
            if (pos > -1)
            {
                var returnAll = way.HasFlag(PureStreamReadWay.ReturnAll);
                var result = returnAll ? memory[pos..] : memory[(pos + 1)..];

                // Is line feed byte?
                var isLineFeed = memory.Span[pos] == LineFeedByte;

                // Move backward
                lastPos = pos;

                // r, n, rn, consider rn case
                if (isLineFeed)
                {
                    if (pos > 0)
                    {
                        if (memory.Span[pos] == CarriageReturnByte)
                        {
                            lastPos--;
                            if (returnAll)
                            {
                                result = memory[(pos - 1)..];
                            }
                        }
                    }
                    else
                    {
                        // Need new reading
                        var newLen = result.Length + (returnAll ? 1 : 0);
                        Memory<byte> newResult = new byte[newLen];
                        if (returnAll) result.CopyTo(newResult[1..]);
                        else result.CopyTo(newResult);

                        var newMemory = await BackwardReadBufferAsync();
                        if (!newMemory.IsEmpty && newMemory.Span[lastPos - 1] == CarriageReturnByte)
                        {
                            lastPos--;

                            if (returnAll)
                            {
                                newResult.Span[0] = CarriageReturnByte;
                            }
                        }
                        else if (returnAll)
                        {
                            newResult = newResult[..^1];
                        }

                        result = newResult;
                    }
                }

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
        /// Discard coming bytes (same as Skip)
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
        /// <param name="way">Reading way</param>
        /// <returns>Bytes</returns>
        public ReadOnlySpan<byte> ReadLine(PureStreamReadWay way = PureStreamReadWay.Default)
        {
            // First try with reading from buffer success
            var span = ReadBufferLine(out var success, way);
            if (success) return span;

            var writer = new ArrayBufferWriter<byte>();
            do
            {
                // Write current content
                writer.Write(span);

                // Continue reading
                span = ReadBufferLine(out var newSuccess, way);
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
        /// <param name="way">Reading way</param>
        /// <returns>Bytes</returns>
        public async Task<ReadOnlyMemory<byte>> ReadLineAsync(PureStreamReadWay way = PureStreamReadWay.Default)
        {
            // First try with reading from buffer success
            var (success, memory) = await ReadBufferLineAsync(way);
            if (success) return memory;

            var writer = new ArrayBufferWriter<byte>();
            do
            {
                // Write current content
                writer.Write(memory.Span);

                // Continue reading
                var (newSuccess, newMemory) = await ReadBufferLineAsync(way);
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
        /// <param name="way">Reading way</param>
        /// <returns>Bytes</returns>
        public async Task<ReadOnlyMemory<byte>> BackwardReadLineAsync(PureStreamReadWay way = PureStreamReadWay.Default)
        {
            // First try with reading from buffer success
            var (success, memory) = await BackwardReadBufferLineAsync(way);
            if (success) return memory;

            var bytes = new List<byte>();

            do
            {
                // Write current content
                bytes.InsertRange(0, memory.ToArray());

                // Continue reading
                var (newSuccess, newMemory) = await BackwardReadBufferLineAsync(way);
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
        /// Read boolean
        /// 读取布尔值
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="EndOfStreamException"></exception>
        public bool ReadBoolean()
        {
            var oneByte = ReadByte();
            if (!oneByte.HasValue)
            {
                throw new EndOfStreamException();
            }
            return oneByte.Value != 0;
        }

        /// <summary>
        /// Read two bytes
        /// 读取两个字节
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="EndOfStreamException"></exception>
        public ReadOnlySpan<byte> ReadTwoBytes()
        {
            var bytes = ReadBytes(2);
            if (bytes.Length < 2)
            {
                throw new EndOfStreamException();
            }
            return bytes;
        }

        private int ReadTwoBytesValue()
        {
            var bytes = ReadTwoBytes();
            return (bytes[0] << 8) + bytes[1];
        }

        private int ReadTwoBytesValue(bool reverse)
        {
            var bytes = ReverseBytes(ReadTwoBytes(), reverse);
            return (bytes[0] << 8) + bytes[1];
        }

        /// <summary>
        /// Read four bytes
        /// 读取四个字节
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="EndOfStreamException"></exception>
        public ReadOnlySpan<byte> ReadFourBytes()
        {
            var bytes = ReadBytes(4);
            if (bytes.Length < 4)
            {
                throw new EndOfStreamException();
            }
            return bytes;
        }

        /// <summary>
        /// Reverse bytes
        /// 反转字节数组
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Result</returns>
        public ReadOnlySpan<byte> ReverseBytes(ReadOnlySpan<byte> bytes)
        {
            return bytes.ToArray().Reverse().ToArray();
        }

        /// <summary>
        /// Reverse bytes with check
        /// 反转字节数组并检查
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <param name="reverse">Reverse</param>
        /// <returns>Result</returns>
        public ReadOnlySpan<byte> ReverseBytes(ReadOnlySpan<byte> bytes, bool reverse)
        {
            if (IsLittleEndian == reverse)
            {
                return ReverseBytes(bytes);
            }
            else
            {
                return bytes;
            }
        }

        private long ReadFourBytesValue(bool reverse)
        {
            var bytes = ReverseBytes(ReadFourBytes(), reverse);
            return (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
        }

        /// <summary>
        /// Read eight bytes
        /// 读取八个字节
        /// </summary>
        /// <returns>Result</returns>
        /// <exception cref="EndOfStreamException"></exception>
        public ReadOnlySpan<byte> ReadEightBytes()
        {
            var bytes = ReadBytes(8);
            if (bytes.Length < 8)
            {
                throw new EndOfStreamException();
            }
            return bytes;
        }

        /// <summary>
        /// Read char (2 bytes)
        /// 读取字符（2字节）
        /// </summary>
        /// <returns>Result</returns>
        public char ReadChar()
        {
            return (char)ReadTwoBytesValue();
        }

        /// <summary>
        /// Read short (2 bytes)
        /// 读取短整数（2字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        public short ReadShort(bool reverse = false)
        {
            return (short)ReadTwoBytesValue(reverse);
        }

        /// <summary>
        /// Read ushort (2 bytes)
        /// 读取无符号短整数（2字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        public ushort ReadUshort(bool reverse = false)
        {
            return (ushort)ReadTwoBytesValue(reverse);
        }

        /// <summary>
        /// Read int (4 bytes)
        /// 读取整数（4字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        public int ReadInt(bool reverse = false)
        {
            return (int)ReadFourBytesValue(reverse);
        }

        /// <summary>
        /// Read uint (4 bytes)
        /// 读取无符号整数（4字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        public uint ReadUint(bool reverse = false)
        {
            return (uint)ReadFourBytesValue(reverse);
        }

        /// <summary>
        /// Read float (4 bytes)
        /// 读取单精度浮点数（4字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        public float ReadFloat(bool reverse = false)
        {
            var bytes = ReverseBytes(ReadFourBytes(), reverse);
            return BitConverter.ToSingle(bytes);
        }

        /// <summary>
        /// Read long (8 bytes)
        /// 读取长整数（8字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        public long ReadLong(bool reverse = false)
        {
            var bytes = ReverseBytes(ReadEightBytes(), reverse);
            return BitConverter.ToInt64(bytes);
        }

        /// <summary>
        /// Read ulong (8 bytes)
        /// 读取无符号长整数（8字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        public ulong ReadUlong(bool reverse = false)
        {
            var bytes = ReverseBytes(ReadEightBytes(), reverse);
            return BitConverter.ToUInt64(bytes);
        }

        /// <summary>
        /// Read double (8 bytes)
        /// 读取双精度浮点数（8字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        public double ReadDouble(bool reverse = false)
        {
            var bytes = ReverseBytes(ReadEightBytes(), reverse);
            return BitConverter.ToDouble(bytes);
        }

        /// <summary>
        /// Read decimal (16 bytes)
        /// 读取十进制数（16字节）
        /// </summary>
        /// <param name="reverse">Reverse bytes</param>
        /// <returns>Result</returns>
        /// <exception cref="EndOfStreamException"></exception>
        public decimal ReadDecimal(bool reverse = false)
        {
            var bytes = ReverseBytes(ReadBytes(16), reverse).ToArray();
            if (bytes.Length < 16)
            {
                throw new EndOfStreamException();
            }

            return new decimal(
                BitConverter.ToInt32(bytes, 0),
                BitConverter.ToInt32(bytes, 4),
                BitConverter.ToInt32(bytes, 8),
                bytes[15] == 128,
                bytes[14]
            );
        }

        /// <summary>
        /// Read string
        /// 读取字符串
        /// </summary>
        /// <param name="count">Bytes to read</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Result</returns>
        public string ReadString(int count, Encoding? encoding = null)
        {
            var bytes = ReadBytes(count);
            return (encoding ?? Encoding.ASCII).GetString(bytes);
        }

        /// <summary>
        /// Read UTF-8 string
        /// 读取UTF-8字符串
        /// </summary>
        /// <param name="count">Bytes to read</param>
        /// <returns>Result</returns>
        public string ReadUTFString(int count)
        {
            return ReadString(count, Encoding.UTF8);
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
        /// Seek from the start
        /// 从开始位置定位
        /// </summary>
        /// <param name="offset">Offet</param>
        public void Seek(long offset)
        {
            ClearBuffer();
            BaseStream.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// Skip bytes (same as Discard)
        /// 跳过字节
        /// </summary>
        /// <param name="count">Bytes count</param>
        public void Skip(int count)
        {
            Discard(count);
        }

        /// <summary>
        /// Skip bytes with unsigned integer
        /// 跳过字节
        /// </summary>
        /// <param name="count">Bytes count</param>
        public void Skip(uint count)
        {
            if (count > int.MaxValue)
            {
                var more = (int)(count - int.MaxValue);
                Discard(more);
                count = int.MaxValue;
            }

            Discard((int)count);
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