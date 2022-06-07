using com.etsoo.HTTP;
using com.etsoo.Utils;
using NUnit.Framework;
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

    internal class ProductStringId
    {
        public string Id { get; init; } = null!;
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
            var filename = await DownloadAsync("https://cn.etsoo.com/logo.png", stream);
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

        public async Task<ProductStringId?> UpdateProductAsync()
        {
            return await PutAsync<ProductUpdate, ProductStringId>("/products/1", new ProductUpdate { Title = "New Product 1", Price = 3.16M });
        }
    }

    /// <summary>
    /// https://dummyjson.com/
    /// </summary>
    [TestFixture]
    public class HttpTests
    {
        readonly HttpClientServiceTest service;

        public HttpTests()
        {
            service = new HttpClientServiceTest(new HttpClient());
        }

        [Test]
        public void MimeToExtensionTests()
        {
            var ext = MimeTypeMap.TryGetExtension("application/x-perfmon");
            Assert.AreEqual(".pma", ext);

            ext = MimeTypeMap.TryGetExtension("text/html");
            Assert.AreEqual(".htm", ext);

            ext = MimeTypeMap.TryGetExtension("IMAGE/JPEG");
            Assert.AreEqual(".jpe", ext);
        }

        [Test]
        public void ExtensionToMimeTests()
        {
            var type = MimeTypeMap.TryGetMimeType(".fail");
            Assert.IsNull(type);

            type = MimeTypeMap.TryGetMimeType(".htm");
            Assert.AreEqual("text/html", type);

            type = MimeTypeMap.TryGetMimeType(".JPG");
            Assert.AreEqual("image/jpeg", type);
        }

        [Test]
        public async Task DeleteAsyncTests()
        {
            var result = await service.DeleteProductAsync();
            Assert.AreEqual(1, result?.Id);
            Assert.IsTrue(result?.IsDeleted);
        }

        [Test]
        public async Task DownloadAsyncTests()
        {
            var (filename, file) = await service.DownloadProductAsync();
            Assert.AreEqual("products.json", filename);
            Assert.IsTrue(file.Contains("\"total\":"));
        }

        [Test]
        public async Task DownloadImageAsyncTests()
        {
            var (filename, length) = await service.DownloadImageAsync();
            Assert.AreEqual("logo.png", filename);
            Assert.LessOrEqual(1000, length);
        }

        [Test]
        public async Task GetAsyncTests()
        {
            var result = await service.GetProductsAsync();
            Assert.AreEqual(100, result?.Total);
            Assert.AreEqual(1, result?.Products.First().Id);
            Assert.AreEqual(549M, result?.Products.First().Price);
        }

        [Test]
        public async Task GetStreamAsyncTests()
        {
            var result = await service.GetProductsStreamAsync();
            Assert.AreEqual(100, result?.Total);
            Assert.AreEqual(1, result?.Products.First().Id);
            Assert.AreEqual(549M, result?.Products.First().Price);
        }

        [Test]
        public async Task PostAsyncTests()
        {
            var result = await service.AddProductAsync();
            Assert.AreEqual(101, result?.Id);
            Assert.AreEqual("New Product", result?.Title);
            Assert.AreEqual(3.15M, result?.Price);
        }

        [Test]
        public async Task PutAsyncTests()
        {
            var result = await service.UpdateProductAsync();
            Assert.AreEqual("New Product 1", result?.Title);
            Assert.AreEqual(3.16M, result?.Price);
        }
    }
}
