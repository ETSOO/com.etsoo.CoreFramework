using com.etsoo.PureIO;
using com.etsoo.Utils;
using NUnit.Framework;
using System.Text;

namespace Tests.Web
{
    [TestFixture]
    public class PureStreamReaderTests
    {
        [Test]
        public void ReadLine_FinishinTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速");
            using var reader = new PureStreamReader(stream);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("Hello，亿速"));
        }

        [Test]
        public void ReadLine_FinishOutTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速Hello，亿速");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("Hello，亿速Hello，亿速"));
        }

        [Test]
        public void ReadLine_WithinTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\r\n下一行数据");
            using var reader = new PureStreamReader(stream);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("Hello，亿速"));
        }

        [Test]
        public void ReadLine_WithoutTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\r\n下一行数据");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("Hello，亿速"));
        }

        [Test]
        public void ReadLine_TwoLinesTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\r\n下一行数据\r\n");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadLine();
            Assert.That(reader.CurrentPosition, Is.EqualTo(16));
            var bytes = reader.ReadLine();

            // Assert
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("下一行数据"));
        }

        [Test]
        public async Task ReadLineAsync_TwoLinesTest()
        {
            // Arrange
            await using var stream = SharedUtils.GetStream("Hello，亿速 \r\n下一行数据\r\n");
            await using var reader = new PureStreamReader(stream, 8);

            // Act
            var l1 = await reader.ReadLineAsync();
            var l2 = await reader.ReadLineAsync();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(Encoding.UTF8.GetString(l1.Span), Is.EqualTo("Hello，亿速 "));
                Assert.That(Encoding.UTF8.GetString(l2.Span), Is.EqualTo("下一行数据"));
            });
        }

        [Test]
        public async Task BackwardReadLineAsync_TwoLinesTest()
        {
            // Arrange
            await using var stream = SharedUtils.GetStream("Hello，亿速 \r\n下一行数据");
            await using var reader = new PureStreamReader(stream, 8);

            // Read
            var r = await reader.ReadLineAsync();
            Assert.That(Encoding.UTF8.GetString(r.Span), Is.EqualTo("Hello，亿速 "));

            // Act
            reader.ToStreamEnd();
            var l1 = await reader.BackwardReadLineAsync();
            Assert.That(reader.CurrentPosition, Is.EqualTo(15));
            var l2 = await reader.BackwardReadLineAsync();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(Encoding.UTF8.GetString(l1.Span), Is.EqualTo("下一行数据"));
                Assert.That(Encoding.UTF8.GetString(l2.Span), Is.EqualTo("Hello，亿速 "));
            });
        }

        [Test]
        public async Task BackwardReadLineWithEndsAsync_TwoLinesTest()
        {
            // Arrange
            await using var stream = SharedUtils.GetStream("Hello，亿速 \r\n下一行数据");
            await using var reader = new PureStreamReader(stream, 8);

            // Read
            var r = await reader.ReadLineAsync(PureStreamReadWay.ReturnAll);
            Assert.That(Encoding.UTF8.GetString(r.Span), Is.EqualTo("Hello，亿速 \r\n"));

            // Act
            reader.ToStreamEnd();
            var l1 = await reader.BackwardReadLineAsync(PureStreamReadWay.ReturnAll);
            Assert.That(reader.CurrentPosition, Is.EqualTo(15));
            var l2 = await reader.BackwardReadLineAsync(PureStreamReadWay.ReturnAll);

            // Assert
            var bytes = Encoding.UTF8.GetBytes("\r\n下一行数据");
            Assert.Multiple(() =>
            {
                Assert.That(l1.Span.SequenceEqual(bytes), Is.True);
                Assert.That(Encoding.UTF8.GetString(l2.Span), Is.EqualTo("Hello，亿速 "));
            });
        }

        [Test]
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

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(len, Is.EqualTo(0));
                Assert.That(str, Is.EqualTo("Hello，亿速"));
            });
        }

        [Test]
        public void ReadLine_TwoLinesMoveTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello12\r\n下一行数据\n");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadLine();
            var bytes = reader.ReadLine();

            // Assert
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("下一行数据"));
        }

        [Test]
        public void ReadLine_ReadByteTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var b = reader.ReadByte();

            // Assert
            Assert.That(b, Is.EqualTo((byte)'H'));
        }

        [Test]
        public void ReadBytesWithCount()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("true false");
            using var reader = new PureStreamReader(stream);

            var bytes = reader.ReadBytes(4);
            var str = Encoding.ASCII.GetString(bytes);
            Assert.Multiple(() =>
            {
                Assert.That(str, Is.EqualTo("true"));
                Assert.That(reader.Peek(), Is.EqualTo((byte)' '));
            });

            reader.ReadBytes([0, 32]);
            Assert.That(reader.Peek(), Is.EqualTo((byte)'f'));
        }

        [Test]
        public void ReadBytesWithRange()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("34.>");
            using var reader = new PureStreamReader(stream);

            var bytes = reader.ReadBytes([46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57]);
            var str = Encoding.ASCII.GetString(bytes);
            Assert.Multiple(() =>
            {
                Assert.That(str, Is.EqualTo("34."));
                Assert.That(reader.Peek(), Is.EqualTo((byte)'>'));
            });
        }

        [Test]
        public void ReadBytesWithRangeMultiple()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("123456734.>");
            using var reader = new PureStreamReader(stream, 8);

            reader.Discard(6);

            var bytes = reader.ReadBytes([46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57]);
            var str = Encoding.ASCII.GetString(bytes);
            Assert.Multiple(() =>
            {
                Assert.That(str, Is.EqualTo("734."));
                Assert.That(reader.Peek(), Is.EqualTo((byte)'>'));
            });
        }

        [Test]
        public void ReadLine_PeekTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var p = reader.Peek();
            var b = reader.ReadByte();
            var c = reader.ReadByte();

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(b, Is.EqualTo(p));
                Assert.That(c, Is.EqualTo((byte)'e'));
            });
        }

        [Test]
        public void ReadToWithinTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("<Hello, 亿速>");
            using var reader = new PureStreamReader(stream);

            // Act
            reader.ReadByte();
            var bytes = reader.ReadTo((byte)'>');

            // Assert
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("Hello, 亿速"));
        }

        [Test]
        public void ReadToWithoutTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("<Hello, 亿速>");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadByte();
            var bytes = reader.ReadTo((byte)'>');

            // Assert
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("Hello, 亿速"));
        }

        [Test]
        public void ReadToMoveToNextTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("<Hello->0");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadByte();
            var bytes = reader.ReadTo((byte)'>');
            var str = Encoding.UTF8.GetString(bytes);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(str, Is.EqualTo("Hello-"));
                Assert.That(reader.Peek(), Is.EqualTo((byte)'0'));
            });
        }

        [Test]
        public void ReadToTargetsTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("[<<Hello, 亿速>>]a");
            using var reader = new PureStreamReader(stream, 8);

            var bytes = reader.ReadTo([(byte)'>', (byte)']'], false);
            var str = Encoding.UTF8.GetString(bytes);
            Assert.Multiple(() =>
            {
                Assert.That(str, Is.EqualTo("[<<Hello, 亿速"));
                Assert.That(reader.Peek(), Is.EqualTo((byte)'>'));
            });
        }

        [Test]
        public void ReadToTargetsIgnoreTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("[<<Hello, 亿速>>]a");
            using var reader = new PureStreamReader(stream, 8);

            // Discard bytes
            reader.Discard(3);

            var bytes = reader.ReadTo([(byte)'>', (byte)']'], true);
            var str = Encoding.UTF8.GetString(bytes);
            Assert.Multiple(() =>
            {
                Assert.That(str, Is.EqualTo("Hello, 亿速"));
                Assert.That(reader.Peek(), Is.EqualTo((byte)'a'));
            });
        }

        [Test]
        public void ReadToTargetsEmptyTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("true false");
            using var reader = new PureStreamReader(stream);

            var first = reader.Peek();
            Assert.That(first, Is.EqualTo((byte)'t'));

            var bytes = reader.ReadTo([(byte)' '], true);
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("true"));

            first = reader.Peek();
            Assert.That(first, Is.EqualTo((byte)'f'));

            bytes = reader.ReadTo([(byte)' '], true);
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("false"));
        }

        [Test]
        public void DiscardMoreReadTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("1234567890");
            using var reader = new PureStreamReader(stream, 8);

            reader.Discard(9);

            Assert.That(reader.Peek(), Is.EqualTo((byte)'0'));
        }

        [Test]
        public void DiscardBytesTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("1234567  \n\ra");
            using var reader = new PureStreamReader(stream, 8);

            reader.Discard(7);
            reader.Discard([32, PureStreamReader.LineFeedByte, PureStreamReader.CarriageReturnByte]);

            Assert.That(reader.Peek(), Is.EqualTo((byte)'a'));
        }

        [Test]
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
            Assert.That(reader.Peek(), Is.EqualTo((byte)'a'));
        }

        [Test]
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
            Assert.That(reader.Peek(), Is.Null);
        }

        [Test]
        public void ReadUintTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream([0, 0, 17, 14]);
            using var reader = new PureStreamReader(stream);

            // Act
            var value = reader.ReadUint();

            // Assert
            Assert.That(value, Is.EqualTo(4366));
        }

        [Test]
        public void ReadUintBETest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream([14, 17, 0, 0, 0, 0, 17, 14]);

            // Default is BE
            using var reader = new PureStreamReader(stream) { IsLittleEndian = false };

            // Act
            var value1 = reader.ReadUint();
            var value2 = reader.ReadUint(true);

            // Assert
            Assert.That(value1, Is.EqualTo(4366));
            Assert.That(value2, Is.EqualTo(4366));
        }

        [Test]
        public void ReadLongTest()
        {
            // Arrange
            long actualLong = 1;
            using var stream = SharedUtils.GetStream(BitConverter.GetBytes(actualLong));
            using var reader = new PureStreamReader(stream);

            // Act
            var value = reader.ReadLong();

            // Assert
            Assert.That(value, Is.EqualTo(actualLong));
        }

        [Test]
        public void ReadDoubleTest()
        {
            // Arrange
            double actualDouble = 12345678901.12345892;
            using var stream = SharedUtils.GetStream(BitConverter.GetBytes(actualDouble));
            using var reader = new PureStreamReader(stream);

            // Act
            var value = reader.ReadDouble();

            // Assert
            Assert.That(value, Is.EqualTo(actualDouble));
        }
    }
}
