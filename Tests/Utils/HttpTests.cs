using com.etsoo.HTTP;
using com.etsoo.Utils;
using System.Text.Json;

namespace Tests.Utils
{
    internal class DeleteResult
    {
        public int Id { get; init; }
        public bool IsDeleted { get; init; }
    }

    internal class Product
    {
        public int Id { get; init; }
        public string Title { get; init; } = null!;
        public decimal Price { get; init; }
    }

    internal class ProductUpdate
    {
        public string? Title { get; init; }
        public decimal? Price { get; init; }
    }

    internal class GetResult
    {
        public IEnumerable<Product> Products { get; init; } = null!;
        public int Total { get; init; }
    }

    internal class HttpClientServiceTest : HttpClientService
    {
        public HttpClientServiceTest(HttpClient client) : base(client)
        {
            client.BaseAddress = new Uri("https://dummyjson.com");
        }

        public async Task<DeleteResult?> DeleteProductAsync()
        {
            return await DeleteAsync<DeleteResult>("/products/1");
        }

        public async Task<(string?, string)> DownloadProductAsync()
        {
            await using var stream = new MemoryStream();
            var filename = await DownloadAsync("/products", stream);
            stream.Position = 0;
            return (filename, await SharedUtils.StreamToStringAsync(stream));
        }

        public async Task<(string?, long)> DownloadImageAsync()
        {
            await using var stream = new MemoryStream();
            var filename = await DownloadAsync("https://www.etsoo.com/images/logo.png", stream);
            return (filename, stream.Length);
        }

        public async Task<GetResult?> GetProductsAsync()
        {
            return await GetAsync<GetResult>("/products");
        }

        public async Task<GetResult?> GetProductsStreamAsync()
        {
            using var stream = new MemoryStream();
            await GetAsync("/products", stream);
            stream.Position = 0;
            return await JsonSerializer.DeserializeAsync<GetResult>(stream, Options);
        }

        public async Task<Product?> AddProductAsync()
        {
            var product = new Product
            {
                Id = 1001,
                Title = "New Product",
                Price = 3.15M
            };
            return await PostAsync<Product, Product>("/products/add", product);
        }

        public async Task<Product?> UpdateProductAsync()
        {
            return await PutAsync<ProductUpdate, Product>("/products/1", new ProductUpdate { Title = "New Product 1", Price = 3.16M });
        }
    }

    /// <summary>
    /// https://dummyjson.com/
    /// </summary>
    [TestClass]
    public class HttpTests
    {
        readonly HttpClientServiceTest service;

        public HttpTests()
        {
            service = new HttpClientServiceTest(new HttpClient());
        }

        [TestMethod]
        public void MimeToExtensionTests()
        {
            var ext = MimeTypeMap.TryGetExtension("application/x-perfmon");
            Assert.AreEqual(".pma", ext);

            ext = MimeTypeMap.TryGetExtension("text/html");
            Assert.AreEqual(".htm", ext);

            ext = MimeTypeMap.TryGetExtension("IMAGE/JPEG");
            Assert.AreEqual(".jpg", ext);

            ext = MimeTypeMap.TryGetExtension("IMAGE/.fail");
            Assert.IsNull(ext);
        }

        [TestMethod]
        public void ExtensionToMimeTests()
        {
            var type = MimeTypeMap.TryGetMimeType(".fail");
            Assert.IsNull(type);

            type = MimeTypeMap.TryGetMimeType(".htm");
            Assert.AreEqual("text/html", type);

            type = MimeTypeMap.TryGetMimeType(".JPG");
            Assert.AreEqual("image/jpeg", type);
        }

        [TestMethod]
        public async Task DeleteAsyncTests()
        {
            var result = await service.DeleteProductAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result?.Id);
            Assert.IsTrue(result?.IsDeleted ?? false);
        }

        [TestMethod]
        public async Task DownloadAsyncTests()
        {
            var (filename, file) = await service.DownloadProductAsync();
            Assert.AreEqual("products.json", filename);
            Assert.Contains("\"total\":", file);
        }

        [TestMethod]
        public async Task DownloadImageAsyncTests()
        {
            var (filename, length) = await service.DownloadImageAsync();
            Assert.AreEqual("logo.png", filename);
            Assert.IsGreaterThanOrEqualTo(1000, length);
        }

        [TestMethod]
        public async Task GetAsyncTests()
        {
            var result = await service.GetProductsAsync();
            Assert.AreEqual(194, result?.Total);
            Assert.AreEqual(1, result?.Products.First().Id);
            Assert.AreEqual(9.99M, result?.Products.First().Price);
        }

        [TestMethod]
        public async Task GetStreamAsyncTests()
        {
            var result = await service.GetProductsStreamAsync();
            Assert.AreEqual(194, result?.Total);
            Assert.AreEqual(1, result?.Products.First().Id);
            Assert.AreEqual(9.99M, result?.Products.First().Price);
        }

        [TestMethod]
        public async Task PostAsyncTests()
        {
            var result = await service.AddProductAsync();
            Assert.AreEqual(195, result?.Id);
            Assert.AreEqual("New Product", result?.Title);
            Assert.AreEqual(3.15M, result?.Price);
        }

        [TestMethod]
        public async Task PutAsyncTests()
        {
            var result = await service.UpdateProductAsync();
            Assert.AreEqual("New Product 1", result?.Title);
            Assert.AreEqual(3.16M, result?.Price);
        }
    }
}
