using com.etsoo.Utils;
using com.etsoo.Utils.Storage;
using NUnit.Framework;
using System.Text;

namespace Tests.Utils
{
    [TestFixture]
    public class LocalStorageTests
    {
        const string root = "C:\\test";
        readonly LocalStorage storage;

        public LocalStorageTests()
        {
            storage = new LocalStorage(root, "http://localhost/");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Directory.Delete(root, true);
        }

        [Test]
        public async Task FileExistsAsyncFalseTest()
        {
            // Act
            var result = await storage.FileExistsAsync("a/test.txt");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CommonOperationsTest()
        {
            // Act
            var content = "Hello, world!";
            var writeResult = await storage.WriteAsync("a/test.txt", new MemoryStream(Encoding.UTF8.GetBytes(content)));
            var result = await storage.FileExistsAsync("/a/test.txt");

            await using var readResult = await storage.ReadAsync("/a/test.txt");
            if (readResult != null)
            {
                var readBytes = await SharedUtils.StreamToBytesAsync(readResult);
                await readResult.DisposeAsync();

                Assert.That(Encoding.UTF8.GetString(readBytes.Span), Is.EqualTo(content));
            }

            var deleteResult = await storage.DeleteAsync("/a/test.txt");

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(writeResult, Is.True);
                Assert.That(result, Is.True);
                Assert.That(deleteResult, Is.True);
            });
        }

        [Test]
        public async Task TagsTest()
        {
            // Arrange
            var tags = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            // Act
            var result = await storage.WriteAsync("/b/test.txt", new MemoryStream(Encoding.UTF8.GetBytes("Hello, world!")), tags: tags);
            var readTags = await storage.ReadTagsAsync("/b/test.txt");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(readTags, Is.Not.Null);
                Assert.That(readTags?.Count, Is.EqualTo(2));
            });
        }

        [Test]
        public async Task NoTagsTest()
        {
            // Arrange
            var tags = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            // Act
            var result = await storage.WriteAsync("/c/test.txt", new MemoryStream(Encoding.UTF8.GetBytes("Hello, world!")), tags: tags);
            var copyResult = await storage.CopyAsync("/c/test.txt", "/c/test2.txt", tags: new Dictionary<string, string>(), deleteSource: false);
            var readTags = await storage.ReadTagsAsync("/c/test2.txt");
            var entries = await storage.ListEntriesAsync("/c");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(readTags, Is.Null);
                Assert.That(entries?.Count(), Is.EqualTo(2));
            });
        }
    }
}
