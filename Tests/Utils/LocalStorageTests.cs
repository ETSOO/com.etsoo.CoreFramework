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
            Directory.Delete(root, true);
            storage = new LocalStorage(root, "http://localhost/");
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
            var file = "a/test.txt";
            var content = "Hello, world!";
            var writeResult = await storage.WriteAsync(file, new MemoryStream(Encoding.UTF8.GetBytes(content)));
            var result = await storage.FileExistsAsync(file);

            await using var readResult = await storage.ReadAsync(file);
            if (readResult != null)
            {
                var readContent = await SharedUtils.StreamToStringAsync(readResult);
                await readResult.DisposeAsync();

                Assert.That(readContent, Is.EqualTo(content));
            }

            var deleteResult = await storage.DeleteAsync(file);

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
            var file = "/b/test.txt";
            var result = await storage.WriteAsync(file, new MemoryStream(Encoding.UTF8.GetBytes("Hello, world!")), tags: tags);
            var readTags = await storage.ReadTagsAsync(file);

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
            var file = "/c/test.txt";
            var file2 = "/c/test2.txt";
            var result = await storage.WriteAsync(file, new MemoryStream(Encoding.UTF8.GetBytes("Hello, world!")), tags: tags);
            var copyResult = await storage.CopyAsync(file, file2, tags: new Dictionary<string, string>(), deleteSource: false);
            var readTags = await storage.ReadTagsAsync(file2);
            var entries = await storage.ListEntriesAsync("/c");
            var deleteResult = await storage.DeleteFolderAsync("/c", true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(readTags, Is.Null);
                Assert.That(entries?.Count(), Is.EqualTo(2));
                Assert.That(deleteResult, Is.True);
            });
        }
    }
}
