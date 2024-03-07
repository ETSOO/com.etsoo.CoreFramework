using com.etsoo.Database;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tests.Utils
{
    public class FormatTest
    {
        [JsonConverter(typeof(DataFormatConverter))]
        public DataFormat? Format { get; set; }
    }

    [TestFixture]
    public class SerializationTests
    {
        private IDistributedCache CreateDistributedCache()
        {
            var services = new ServiceCollection();
            services.AddDistributedMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IDistributedCache>();
        }

        [Test]
        public async Task CacheFactoryDoStringAsyncTest()
        {
            // Arrange
            var cache = CreateDistributedCache();
            var key = "key";
            var value = "value";

            // Act
            var result = await CacheFactory.DoStringAsync(cache, 1, () => key, async () =>
            {
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.That(result, Is.EqualTo(value));
            Assert.That(cache.GetString(key), Is.EqualTo(value));
        }

        [Test]
        public async Task CacheFactoryDoStringAsyncDisabledTest()
        {
            // Arrange
            var cache = CreateDistributedCache();
            var key = "key";
            var value = "value";

            // Act
            var result = await CacheFactory.DoStringAsync(cache, 0, () => key, async () =>
            {
                await Task.CompletedTask;
                return value;
            });

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(result, Is.EqualTo(value));
                Assert.That(string.IsNullOrEmpty(cache.GetString(key)), Is.True);
            });
        }

        [Test]
        public async Task CacheFactoryDoAsyncTest()
        {
            // Arrange
            var cache = CreateDistributedCache();
            var key = "key";
            var value = new UserModel
            {
                Id = 1001,
                Name = "Admin 1",
                Status = UserModel.StatusEnum.Deleted
            };

            // Act
            var result = await CacheFactory.DoAsync(cache, 1, () => key, async () =>
            {
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.That(result, Is.EqualTo(value));

            // Act
            var resultCached = await CacheFactory.DoAsync(cache, 1, () => key, async () =>
            {
                Assert.Fail("Not cached");
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.That(resultCached, Is.Not.EqualTo(value));
        }

        [Test]
        public async Task CacheFactoryDoBytesAsyncTest()
        {
            // Arrange
            var cache = CreateDistributedCache();
            var key = "key";
            var value = Encoding.UTF8.GetBytes("value");

            // Act
            var (result, cached) = await CacheFactory.DoBytesAsync(cache, 1, () => key, async () =>
            {
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.That(cached, Is.True);

            // Act
            var (resultCached, cached1) = await CacheFactory.DoBytesAsync(cache, 1, () => key, async () =>
            {
                Assert.Fail("Not cached");
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.That(cached1, Is.False);
        }

        [Test]
        public async Task CacheFactoryDoBytesAsyncEmptyTest()
        {
            // Arrange
            var cache = CreateDistributedCache();
            var key = "key";
            var value = Array.Empty<byte>();

            // Act
            var (result, cached) = await CacheFactory.DoBytesAsync(cache, 1, () => key, async () =>
            {
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.That(cached, Is.True);

            // Act
            var (resultCached, cached1) = await CacheFactory.DoBytesAsync(cache, 1, () => key, async () =>
            {
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.That(cached1, Is.True);
        }

        [Test]
        public async Task ToJsonTest()
        {
            // Arrange
            var modal = new UserModel
            {
                Id = 1001,
                Name = "Admin 1",
                Status = UserModel.StatusEnum.Deleted,
                Friends = [1, 2, 3],
                Valid = true,
                Keys = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } }
            };

            // Act
            var writer = new ArrayBufferWriter<byte>();
            await modal.ToJsonAsync(writer, new JsonSerializerOptions(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            // Assert
            Assert.That(json, Does.Contain("keys"));
        }

        [Test]
        public void DataFormatParseTests()
        {
            var result = DataFormat.TryParse<DataFormat>(1, out var item);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(item, Is.EqualTo(DataFormat.Json));
            });

            var result1 = DataFormat.TryParse<DataFormat>(2, out var item1);
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.False);
                Assert.That(item1, Is.Null);
            });
        }

        [Test]
        public void GetPropertyCaseInsensitiveTests()
        {
            var json = """{"contactTemplate": "abc"}""";
            var doc = JsonDocument.Parse(json);
            var template = doc.RootElement.GetPropertyCaseInsensitive("ContactTemplate");
            Assert.That(template?.GetString(), Is.EqualTo("abc"));
        }

        [Test]
        public void GetValueTests()
        {
            var json = """{"stringItem": "abc", "boolItem1": "true", "boolItem2": true, "intItem": 12.5}""";
            var root = JsonDocument.Parse(json).RootElement;

            var boolItem1 = root.GetProperty("boolItem1");
            Assert.Multiple(() =>
            {
                Assert.That(boolItem1.GetValue<bool>(), Is.Null);
                Assert.That(boolItem1.GetValue<bool>(true), Is.True);
            });

            var boolItem2 = root.GetProperty("boolItem2");
            Assert.That(boolItem2.GetValue<bool>(), Is.True);

            var intItem = root.GetProperty("intItem");
            Assert.That(intItem.GetValue<int>(), Is.Null);
        }

        [Test]
        public void Utf8JsonReaderGetValueTests()
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var json = """{"format":1}""";
            var result = JsonSerializer.Deserialize<FormatTest>(json, options);
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Format, Is.EqualTo(DataFormat.Json));

            var jsonResult = JsonSerializer.Serialize(result, options);
            Assert.That(jsonResult, Is.Not.Null);
            Assert.That(jsonResult, Is.EqualTo(json));
        }

        [Test]
        public void GetArrayTests()
        {
            var json = """{"stringItem": "abc", "stringArray": ["a", "b", "c"], "intArray": [1, 2, "3", "a"], "object": [{"id": "1", "label": "Label"}]}""";
            var doc = JsonDocument.Parse(json);

            var stringItem = doc.RootElement.GetProperty("stringItem");
            var testArray = stringItem.GetArray<string>();
            Assert.That(testArray.Count(), Is.EqualTo(0));

            var stringArray = doc.RootElement.GetProperty("stringArray").GetArray<string>();
            Assert.Multiple(() =>
            {
                Assert.That(stringArray.Count(), Is.EqualTo(3));
                Assert.That(stringArray.Last(), Is.EqualTo("c"));
            });

            var intArray = doc.RootElement.GetProperty("intArray").GetArray<int>();
            Assert.Multiple(() =>
            {
                Assert.That(intArray.Count(), Is.EqualTo(4));
                Assert.That(intArray.First(), Is.EqualTo(1));
                Assert.That(intArray.ElementAt(2), Is.Null);
            });

            var intArrayNotNull = doc.RootElement.GetProperty("intArray").GetArray<int>(true).WhereNotNull();
            Assert.Multiple(() =>
            {
                Assert.That(intArrayNotNull.Count(), Is.EqualTo(3));
                Assert.That(intArrayNotNull.ElementAt(2), Is.EqualTo(3));
            });

            var objArray = doc.RootElement.GetProperty("object").GetArray<IdLabelItem>();
            Assert.Multiple(() =>
            {
                Assert.That(objArray.Count(), Is.EqualTo(1));
                Assert.That(objArray.First()?.Label, Is.EqualTo("Label"));
            });
        }

        [Test]
        public void GetDictionaryJsonTests()
        {
            // Arrange
            var dic = new Dictionary<string, object>
            {
                { "a", 1 },
                { "a1", 999L },
                { "b", "2" },
                //{ "c", new { id = 3, label = "Label" } },
                { "d", false },
                { "e", DateTime.Parse("2021-01-01") },
                { "f", new int[] { 1, 2, 3 } }
            };

            // Act
            var json = JsonSerializer.Serialize(dic, CommonJsonSerializerContext.Default.DictionaryStringObject);

            // Assert
            Assert.That(json, Is.EqualTo("""{"a":1,"a1":999,"b":"2","d":false,"e":"2021-01-01T00:00:00","f":[1,2,3]}"""));
        }
    }
}
