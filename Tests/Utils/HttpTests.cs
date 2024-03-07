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

        public async Task<Product?> UpdateProductAsync()
        {
            return await PutAsync<ProductUpdate, Product>("/products/1", new ProductUpdate { Title = "New Product 1", Price = 3.16M });
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
            Assert.That(ext, Is.EqualTo(".pma"));

            ext = MimeTypeMap.TryGetExtension("text/html");
            Assert.That(ext, Is.EqualTo(".htm"));

            ext = MimeTypeMap.TryGetExtension("IMAGE/JPEG");
            Assert.That(ext, Is.EqualTo(".jpg"));

            ext = MimeTypeMap.TryGetExtension("IMAGE/.fail");
            Assert.That(ext, Is.Null);
        }

        [Test]
        public void ExtensionToMimeTests()
        {
            var type = MimeTypeMap.TryGetMimeType(".fail");
            Assert.That(type, Is.Null);

            type = MimeTypeMap.TryGetMimeType(".htm");
            Assert.That(type, Is.EqualTo("text/html"));

            type = MimeTypeMap.TryGetMimeType(".JPG");
            Assert.That(type, Is.EqualTo("image/jpeg"));
        }

        [Test]
        public async Task DeleteAsyncTests()
        {
            var result = await service.DeleteProductAsync();
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(1));
                Assert.That(result?.IsDeleted, Is.True);
            });
        }

        [Test]
        public async Task DownloadAsyncTests()
        {
            var (filename, file) = await service.DownloadProductAsync();
            Assert.Multiple(() =>
            {
                Assert.That(filename, Is.EqualTo("products.json"));
                Assert.That(file, Does.Contain("\"total\":"));
            });
        }

        [Test]
        public async Task DownloadImageAsyncTests()
        {
            var (filename, length) = await service.DownloadImageAsync();
            Assert.Multiple(() =>
            {
                Assert.That(filename, Is.EqualTo("logo.png"));
                Assert.That(length, Is.GreaterThanOrEqualTo(1000));
            });
        }

        [Test]
        public async Task GetAsyncTests()
        {
            var result = await service.GetProductsAsync();
            Assert.Multiple(() =>
            {
                Assert.That(result?.Total, Is.EqualTo(100));
                Assert.That(result?.Products.First().Id, Is.EqualTo(1));
                Assert.That(result?.Products.First().Price, Is.EqualTo(549M));
            });
        }

        [Test]
        public async Task GetStreamAsyncTests()
        {
            var result = await service.GetProductsStreamAsync();
            Assert.Multiple(() =>
            {
                Assert.That(result?.Total, Is.EqualTo(100));
                Assert.That(result?.Products.First().Id, Is.EqualTo(1));
                Assert.That(result?.Products.First().Price, Is.EqualTo(549M));
            });
        }

        [Test]
        public async Task PostAsyncTests()
        {
            var result = await service.AddProductAsync();
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(101));
                Assert.That(result?.Title, Is.EqualTo("New Product"));
                Assert.That(result?.Price, Is.EqualTo(3.15M));
            });
        }

        [Test]
        public async Task PutAsyncTests()
        {
            var result = await service.UpdateProductAsync();
            Assert.Multiple(() =>
            {
                Assert.That(result?.Title, Is.EqualTo("New Product 1"));
                Assert.That(result?.Price, Is.EqualTo(3.16M));
            });
        }
    }
}
