using com.etsoo.Utils;
using com.etsoo.Utils.Storage;
using System.Text;

namespace Tests.Utils
{
    [TestClass]
    public class LocalStorageTests
    {
        const string root = "C:\\test";
        readonly LocalStorage storage;

        public LocalStorageTests()
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }
            storage = new LocalStorage(root, "http://localhost/");
        }

        [TestMethod]
        public async Task FileExistsAsyncFalseTest()
        {
            // Act
            var result = await storage.FileExistsAsync("a/test.txt", TestContext.CancellationToken);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CommonOperationsTest()
        {
            // Act
            var file = "a/test.txt";
            var content = "Hello, world!";
            var writeResult = await storage.WriteAsync(file, new MemoryStream(Encoding.UTF8.GetBytes(content)));
            var result = await storage.FileExistsAsync(file);

            await using var readResult = await storage.ReadAsync(file, TestContext.CancellationToken);
            if (readResult != null)
            {
                var readContent = await SharedUtils.StreamToStringAsync(readResult, TestContext.CancellationToken);
                await readResult.DisposeAsync();

                Assert.AreEqual(content, readContent);
            }

            var deleteResult = await storage.DeleteAsync(file, TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(writeResult);
            Assert.IsTrue(result);
            Assert.IsTrue(deleteResult);
        }

        [TestMethod]
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
            var result = await storage.WriteAsync(file, new MemoryStream(Encoding.UTF8.GetBytes("Hello, world!")), tags: tags, cancellationToken: TestContext.CancellationToken);
            var readTags = await storage.ReadTagsAsync(file, TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(readTags);
            Assert.AreEqual(2, readTags?.Count);
        }

        [TestMethod]
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
            var result = await storage.WriteAsync(file, new MemoryStream(Encoding.UTF8.GetBytes("Hello, world!")), tags: tags, cancellationToken: TestContext.CancellationToken);
            var copyResult = await storage.CopyAsync(file, file2, tags: new Dictionary<string, string>(), deleteSource: false, TestContext.CancellationToken);
            var readTags = await storage.ReadTagsAsync(file2, TestContext.CancellationToken);
            var entries = await storage.ListEntriesAsync("/c", TestContext.CancellationToken);
            var deleteResult = await storage.DeleteFolderAsync("/c", true, TestContext.CancellationToken);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNull(readTags);
            Assert.AreEqual(2, entries?.Count());
            Assert.IsTrue(deleteResult);
        }

        public TestContext TestContext { get; set; }
    }
}
