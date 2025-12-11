using com.etsoo.PureIO;
using com.etsoo.Utils;
using System.Text;

namespace Tests.PureIO
{
    [TestClass]
    public class PureStreamReaderTests
    {
        [TestMethod]
        public void ReadLine_FinishinTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速");
            using var reader = new PureStreamReader(stream);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("Hello，亿速", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public void ReadLine_FinishOutTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速Hello，亿速");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("Hello，亿速Hello，亿速", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public void ReadLine_WithinTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\r\n下一行数据");
            using var reader = new PureStreamReader(stream);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("Hello，亿速", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public void ReadLine_WithoutTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\r\n下一行数据");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("Hello，亿速", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public void ReadLine_TwoLinesTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\r\n下一行数据\r\n");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadLine();
            Assert.AreEqual(16, reader.CurrentPosition);
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("下一行数据", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public async Task ReadLineAsync_TwoLinesTest()
        {
            // Arrange
            await using var stream = SharedUtils.GetStream("Hello，亿速 \r\n下一行数据\r\n");
            await using var reader = new PureStreamReader(stream, 8);

            // Act
            var l1 = await reader.ReadLineAsync();
            var l2 = await reader.ReadLineAsync();

            // Assert
            Assert.AreEqual("Hello，亿速 ", Encoding.UTF8.GetString(l1.Span));
            Assert.AreEqual("下一行数据", Encoding.UTF8.GetString(l2.Span));
        }

        [TestMethod]
        public async Task BackwardReadLineAsync_TwoLinesTest()
        {
            // Arrange
            await using var stream = SharedUtils.GetStream("Hello，亿速 \r\n下一行数据");
            await using var reader = new PureStreamReader(stream, 8);

            // Read
            var r = await reader.ReadLineAsync();
            Assert.AreEqual("Hello，亿速 ", Encoding.UTF8.GetString(r.Span));

            // Act
            reader.ToStreamEnd();
            var l1 = await reader.BackwardReadLineAsync();
            Assert.AreEqual(15, reader.CurrentPosition);
            var l2 = await reader.BackwardReadLineAsync();

            // Assert
            Assert.AreEqual("下一行数据", Encoding.UTF8.GetString(l1.Span));
            Assert.AreEqual("Hello，亿速 ", Encoding.UTF8.GetString(l2.Span));
        }

        [TestMethod]
        public async Task BackwardReadLineWithEndsAsync_TwoLinesTest()
        {
            // Arrange
            await using var stream = SharedUtils.GetStream("Hello，亿速 \r\n下一行数据");
            await using var reader = new PureStreamReader(stream, 8);

            // Read
            var r = await reader.ReadLineAsync(PureStreamReadWay.ReturnAll);
            Assert.AreEqual("Hello，亿速 \r\n", Encoding.UTF8.GetString(r.Span));

            // Act
            reader.ToStreamEnd();
            var l1 = await reader.BackwardReadLineAsync(PureStreamReadWay.ReturnAll);
            Assert.AreEqual(15, reader.CurrentPosition);
            var l2 = await reader.BackwardReadLineAsync(PureStreamReadWay.ReturnAll);

            // Assert
            var bytes = Encoding.UTF8.GetBytes("\r\n下一行数据");
            Assert.IsTrue(l1.Span.SequenceEqual(bytes));
            Assert.AreEqual("Hello，亿速 ", Encoding.UTF8.GetString(l2.Span));
        }

        [TestMethod]
        public void ReadLine_StartEOLTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("\rHello，亿速");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var fistBytes = reader.ReadLine();
            var len = fistBytes.Length;
            var bytes = reader.ReadLine();
            var str = Encoding.UTF8.GetString(bytes);

            // Assert
            Assert.AreEqual(0, len);
            Assert.AreEqual("Hello，亿速", str);
        }

        [TestMethod]
        public void ReadLine_TwoLinesMoveTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello12\r\n下一行数据\n");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadLine();
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("下一行数据", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public void ReadLine_ReadByteTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var b = reader.ReadByte();

            // Assert
            Assert.AreEqual((byte)'H', b);
        }

        [TestMethod]
        public void ReadBytesWithCount()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("true false");
            using var reader = new PureStreamReader(stream);

            var bytes = reader.ReadBytes(4);
            var str = Encoding.ASCII.GetString(bytes);
            // Assert
            Assert.AreEqual("true", str);
            Assert.AreEqual((byte)' ', reader.Peek());

            reader.ReadBytes([0, 32]);
            Assert.AreEqual((byte)'f', reader.Peek());
        }

        [TestMethod]
        public void ReadBytesWithRange()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("34.>");
            using var reader = new PureStreamReader(stream);

            var bytes = reader.ReadBytes([46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57]);
            var str = Encoding.ASCII.GetString(bytes);

            // Assert
            Assert.AreEqual("34.", str);
            Assert.AreEqual((byte)'>', reader.Peek());
        }

        [TestMethod]
        public void ReadBytesWithRangeMultiple()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("123456734.>");
            using var reader = new PureStreamReader(stream, 8);

            reader.Discard(6);

            var bytes = reader.ReadBytes([46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57]);
            var str = Encoding.ASCII.GetString(bytes);

            // Assert
            Assert.AreEqual("734.", str);
            Assert.AreEqual((byte)'>', reader.Peek());
        }

        [TestMethod]
        public void ReadLine_PeekTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var p = reader.Peek();
            var b = reader.ReadByte();
            var c = reader.ReadByte();

            // Assert
            Assert.AreEqual(b, p);
            Assert.AreEqual((byte)'e', c);
        }

        [TestMethod]
        public void ReadToWithinTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("<Hello, 亿速>");
            using var reader = new PureStreamReader(stream);

            // Act
            reader.ReadByte();
            var bytes = reader.ReadTo((byte)'>');

            // Assert
            Assert.AreEqual("Hello, 亿速", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public void ReadToWithoutTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("<Hello, 亿速>");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadByte();
            var bytes = reader.ReadTo((byte)'>');

            // Assert
            Assert.AreEqual("Hello, 亿速", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public void ReadToMoveToNextTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("<Hello->0");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadByte();
            var bytes = reader.ReadTo((byte)'>');
            var str = Encoding.UTF8.GetString(bytes);

            // Assert
            Assert.AreEqual("Hello-", str);
            Assert.AreEqual((byte)'0', reader.Peek());
        }

        [TestMethod]
        public void ReadToTargetsTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("[<<Hello, 亿速>>]a");
            using var reader = new PureStreamReader(stream, 8);

            var bytes = reader.ReadTo([(byte)'>', (byte)']'], false);
            var str = Encoding.UTF8.GetString(bytes);

            // Assert
            Assert.AreEqual("[<<Hello, 亿速", str);
            Assert.AreEqual((byte)'>', reader.Peek());
        }

        [TestMethod]
        public void ReadToTargetsIgnoreTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("[<<Hello, 亿速>>]a");
            using var reader = new PureStreamReader(stream, 8);

            // Discard bytes
            reader.Discard(3);

            var bytes = reader.ReadTo([(byte)'>', (byte)']'], true);
            var str = Encoding.UTF8.GetString(bytes);

            // Assert
            Assert.AreEqual("Hello, 亿速", str);
            Assert.AreEqual((byte)'a', reader.Peek());
        }

        [TestMethod]
        public void ReadToTargetsEmptyTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("true false");
            using var reader = new PureStreamReader(stream);

            var first = reader.Peek();
            Assert.AreEqual((byte)'t', first);

            var bytes = reader.ReadTo([(byte)' '], true);
            Assert.AreEqual("true", Encoding.UTF8.GetString(bytes));

            first = reader.Peek();
            Assert.AreEqual((byte)'f', first);

            bytes = reader.ReadTo([(byte)' '], true);
            Assert.AreEqual("false", Encoding.UTF8.GetString(bytes));
        }

        [TestMethod]
        public void DiscardMoreReadTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("1234567890");
            using var reader = new PureStreamReader(stream, 8);

            reader.Discard(9);

            Assert.AreEqual((byte)'0', reader.Peek());
        }

        [TestMethod]
        public void DiscardBytesTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("1234567  \n\ra");
            using var reader = new PureStreamReader(stream, 8);

            reader.Discard(7);
            reader.Discard([32, PureStreamReader.LineFeedByte, PureStreamReader.CarriageReturnByte]);

            Assert.AreEqual((byte)'a', reader.Peek());
        }

        [TestMethod]
        public void ReadWhileTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("1234567890+abc");
            using var reader = new PureStreamReader(stream, 8);
            reader.ReadWhile((one) =>
            {
                if (one == (byte)'+') return true;
                return false;
            });
            Assert.AreEqual((byte)'a', reader.Peek());
        }

        [TestMethod]
        public void ReadWhileFailedTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("1234567890+abc");
            using var reader = new PureStreamReader(stream, 8);
            reader.ReadWhile((one) =>
            {
                if (one == (byte)'-') return true;
                return false;
            });
            Assert.IsNull(reader.Peek());
        }

        [TestMethod]
        public void ReadByteAndSbyteTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream([128, 128]);
            using var reader = new PureStreamReader(stream);

            // Act
            var value1 = reader.ReadOne();
            var value2 = reader.ReadSbyte();

            // Assert
            Assert.AreEqual(128, value1);
            Assert.AreEqual(-128, value2);
        }

        [TestMethod]
        public void ReadNegativeIntTest()
        {
            // Arrange
            var negativeInt = -4366;
            var bytes = BitConverter.GetBytes(negativeInt).Reverse().ToArray();
            using var stream = SharedUtils.GetStream(bytes);
            using var reader = new PureStreamReader(stream)
            {
                IsLittleEndian = false
            };

            // Act
            var value = reader.ReadInt();

            // Assert
            Assert.AreEqual(negativeInt, value);
        }

        [TestMethod]
        public void ReadUintTest()
        {
            // Arrange
            uint uintValue = 4366;
            var bytes = BitConverter.GetBytes(uintValue).Reverse().ToArray();
            using var stream = SharedUtils.GetStream(bytes);
            using var reader = new PureStreamReader(stream);

            // Act
            var value = reader.ReadUint();

            // Assert
            bytes.SequenceEqual<byte>([14, 17, 0, 0]);
            Assert.AreEqual((uint)4366, value);
        }

        [TestMethod]
        public void ReadUintBETest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream([0, 0, 17, 14, 14, 17, 0, 0]);

            // Default is BE
            using var reader = new PureStreamReader(stream);

            // Act
            var value1 = reader.ReadUint();
            var value2 = reader.ReadUint(true);

            // Assert
            Assert.AreEqual((uint)4366, value1);
            Assert.AreEqual((uint)4366, value2);
        }

        [TestMethod]
        public void SeekAndCurrentPositionTest()
        {
            // Arrange
            byte[] bytes = [116, 116, 99, 102, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 32, 0, 0, 1, 188, 68, 83, 73, 71, 0, 0, 37, 192, 1, 44, 121, 116, 0, 1, 0, 0, 0, 25, 1, 0, 0, 4, 0, 144, 71, 68, 69, 70, 224, 155];
            using var stream = SharedUtils.GetStream(bytes);
            using var sr = new PureStreamReader(stream);

            // Act
            var mainTag = sr.ReadString(4);
            sr.Skip(4);
            var numFonts = sr.ReadUint();

            var offsets = new uint[numFonts];
            for (var f = 0; f < numFonts; f++)
            {
                offsets[f] = sr.ReadUint();
            }

            var currentPos = sr.CurrentPosition;
            sr.Seek(offsets[0]);
            var newPos = sr.CurrentPosition;

            var ttId = sr.ReadInt();
            var tableCount = sr.ReadUshort();
            sr.Skip(6);
            var tag = sr.ReadString(4);

            var a = Encoding.ASCII.GetString(bytes);

            // Assert
            Assert.AreEqual("ttcf", mainTag);
            Assert.AreEqual((uint)2, numFonts);
            Assert.AreEqual(20, currentPos);
            Assert.AreEqual(32, newPos);
            Assert.AreEqual(0x00010000, ttId);
            Assert.AreEqual(25, tableCount);
            Assert.AreEqual("GDEF", tag);
        }

        [TestMethod]
        public void ReadLongTest()
        {
            // Arrange
            long expectedLong = 1;
            using var stream = SharedUtils.GetStream(BitConverter.GetBytes(expectedLong));
            using var reader = new PureStreamReader(stream)
            {
                IsLittleEndian = true
            };

            // Act
            var value = reader.ReadLong();

            // Assert
            Assert.AreEqual(expectedLong, value);
        }

        [TestMethod]
        public void ReadDoubleTest()
        {
            // Arrange
            double expectedDouble = 12345678901.12345892;
            using var stream = SharedUtils.GetStream(BitConverter.GetBytes(expectedDouble));
            using var reader = new PureStreamReader(stream)
            {
                IsLittleEndian = true
            };

            // Act
            var value = reader.ReadDouble();

            // Assert
            Assert.AreEqual(expectedDouble, value);
        }
    }
}
