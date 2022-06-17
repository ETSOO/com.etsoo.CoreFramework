﻿using com.etsoo.PureIO;
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
            Assert.AreEqual("Hello，亿速", Encoding.UTF8.GetString(bytes));
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
            Assert.AreEqual("Hello，亿速Hello，亿速", Encoding.UTF8.GetString(bytes));
        }

        [Test]
        public void ReadLine_WithinTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\n下一行数据");
            using var reader = new PureStreamReader(stream);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("Hello，亿速", Encoding.UTF8.GetString(bytes));
        }

        [Test]
        public void ReadLine_WithoutTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\n下一行数据");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("Hello，亿速", Encoding.UTF8.GetString(bytes));
        }

        [Test]
        public void ReadLine_TwoLinesTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("Hello，亿速\r\n下一行数据\n");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            reader.ReadLine();
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual("下一行数据", Encoding.UTF8.GetString(bytes));
        }

        [Test]
        public void ReadLine_StartEOLTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("\r\nHello，亿速");
            using var reader = new PureStreamReader(stream, 8);

            // Act
            var fistBytes = reader.ReadLine();
            var bytes = reader.ReadLine();

            // Assert
            Assert.AreEqual(0, fistBytes.Length);
            Assert.AreEqual("Hello，亿速", Encoding.UTF8.GetString(bytes));
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
            Assert.AreEqual("下一行数据", Encoding.UTF8.GetString(bytes));
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
            Assert.AreEqual((byte)'H', b);
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

            // Assert
            Assert.AreEqual(p, b);
            Assert.AreEqual((byte)'e', c);
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
            Assert.AreEqual("Hello, 亿速", Encoding.UTF8.GetString(bytes));
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
            Assert.AreEqual("Hello, 亿速", Encoding.UTF8.GetString(bytes));
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

            // Assert
            Assert.AreEqual("Hello-", Encoding.UTF8.GetString(bytes));
            Assert.AreEqual((byte)'0', reader.Peek());
        }

        [Test]
        public void ReadToTargetsTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("[<<Hello, 亿速>>]a");
            using var reader = new PureStreamReader(stream, 8);

            var bytes = reader.ReadTo(new byte[] { (byte)'>', (byte)']' }, false);
            Assert.AreEqual("[<<Hello, 亿速", Encoding.UTF8.GetString(bytes));
            Assert.AreEqual((byte)'>', reader.Peek());
        }

        [Test]
        public void ReadToTargetsIgnoreTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("[<<Hello, 亿速>>]a");
            using var reader = new PureStreamReader(stream, 8);

            // Discard bytes
            reader.Discard(3);

            var bytes = reader.ReadTo(new byte[] { (byte)'>', (byte)']' }, true);
            Assert.AreEqual("Hello, 亿速", Encoding.UTF8.GetString(bytes));
            Assert.AreEqual((byte)'a', reader.Peek());
        }

        [Test]
        public void ReadToTargetsEmptyTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("true false");
            using var reader = new PureStreamReader(stream);

            var first = reader.Peek();
            Assert.AreEqual((byte)'t', first);

            var bytes = reader.ReadTo(new byte[] { (byte)' ' }, true);
            Assert.AreEqual("true", Encoding.UTF8.GetString(bytes));

            first = reader.Peek();
            Assert.AreEqual((byte)'f', first);

            bytes = reader.ReadTo(new byte[] { (byte)' ' }, true);
            Assert.AreEqual("false", Encoding.UTF8.GetString(bytes));
        }

        [Test]
        public void DiscardMoreReadTest()
        {
            // Arrange
            using var stream = SharedUtils.GetStream("1234567890");
            using var reader = new PureStreamReader(stream, 8);

            reader.Discard(9);

            Assert.AreEqual((byte)'0', reader.Peek());
        }
    }
}
