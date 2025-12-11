using com.etsoo.Database;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
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

    [TestClass]
    public class SerializationTests
    {
        private IDistributedCache CreateDistributedCache()
        {
            var services = new ServiceCollection();
            services.AddDistributedMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<IDistributedCache>();
        }

        [TestMethod]
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
            Assert.AreEqual(value, result);
            Assert.AreEqual(value, cache.GetString(key));
        }

        [TestMethod]
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

            // Assert
            Assert.AreEqual(value, result);
            Assert.IsTrue(string.IsNullOrEmpty(cache.GetString(key)));
        }

        [TestMethod]
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
            Assert.AreEqual(value, result);

            // Act
            var resultCached = await CacheFactory.DoAsync(cache, 1, () => key, async () =>
            {
                Assert.Fail("Not cached");
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.AreNotEqual(value, resultCached);
        }

        [TestMethod]
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
            Assert.IsTrue(cached);

            // Act
            var (resultCached, cached1) = await CacheFactory.DoBytesAsync(cache, 1, () => key, async () =>
            {
                Assert.Fail("Not cached");
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.IsFalse(cached1);
        }

        [TestMethod]
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
            Assert.IsTrue(cached);

            // Act
            var (resultCached, cached1) = await CacheFactory.DoBytesAsync(cache, 1, () => key, async () =>
            {
                await Task.CompletedTask;
                return value;
            });

            // Assert
            Assert.IsTrue(cached1);
        }

        [TestMethod]
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
            Assert.Contains("keys", json);
        }

        [TestMethod]
        public void DataFormatParseTests()
        {
            // For reference check
            var json = DataFormat.Json;

            var result = DataFormat.TryParse<DataFormat>(1, out var item);
            Assert.IsTrue(result);
            Assert.AreEqual(DataFormat.Json, item);

            var result1 = DataFormat.TryParse<DataFormat>(2, out var item1);
            Assert.IsFalse(result1);
            Assert.IsNull(item1);
        }

        [TestMethod]
        public void GetPropertyCaseInsensitiveTests()
        {
            var json = """{"contactTemplate": "abc"}""";
            var doc = JsonDocument.Parse(json);
            var template = doc.RootElement.GetPropertyCaseInsensitive("ContactTemplate");
            Assert.AreEqual("abc", template?.GetString());
        }

        [TestMethod]
        public void GetValueTests()
        {
            var json = """{"stringItem": "abc", "boolItem1": "true", "boolItem2": true, "intItem": 12.5}""";
            var root = JsonDocument.Parse(json).RootElement;

            var boolItem1 = root.GetProperty("boolItem1");
            Assert.IsNull(boolItem1.GetValue<bool>());
            Assert.IsTrue(boolItem1.GetValue<bool>(true));

            var boolItem2 = root.GetProperty("boolItem2");
            Assert.IsTrue(boolItem2.GetValue<bool>());

            var intItem = root.GetProperty("intItem");
            Assert.IsNull(intItem.GetValue<int>());
        }

        [TestMethod]
        public void Utf8JsonReaderGetValueTests()
        {
            var jsonFormat = DataFormat.Json;

            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var json = """{"format":1}""";
            var result = JsonSerializer.Deserialize<FormatTest>(json, options);
            Assert.IsNotNull(result);
            Assert.AreEqual(jsonFormat, result?.Format);

            var jsonResult = JsonSerializer.Serialize(result, options);
            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(json, jsonResult);
        }

        [TestMethod]
        public void GetArrayTests()
        {
            var json = """{"stringItem": "abc", "stringArray": ["a", "b", "c"], "intArray": [1, 2, "3", "a"], "object": [{"id": "1", "label": "Label"}]}""";
            var doc = JsonDocument.Parse(json);

            var stringItem = doc.RootElement.GetProperty("stringItem");
            var testArray = stringItem.GetArray<string>();
            Assert.AreEqual(0, testArray.Count());

            var stringArray = doc.RootElement.GetProperty("stringArray").GetArray<string>();
            Assert.AreEqual(3, stringArray.Count());
            Assert.AreEqual("c", stringArray.Last());

            var intArray = doc.RootElement.GetProperty("intArray").GetArray<int>();
            Assert.AreEqual(2, intArray.Count());
            Assert.AreEqual(1, intArray.First());

            var intArrayNotNull = doc.RootElement.GetProperty("intArray").GetArray<int>(true);
            Assert.AreEqual(3, intArrayNotNull.Count());
            Assert.AreEqual(3, intArrayNotNull.ElementAt(2));

            var objArray = doc.RootElement.GetProperty("object").GetArray<IdLabelItem>();
            Assert.AreEqual(1, objArray.Count());
            Assert.AreEqual("Label", objArray.First()?.Label);
        }

        [TestMethod]
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
            Assert.AreEqual("""{"a":1,"a1":999,"b":"2","d":false,"e":"2021-01-01T00:00:00","f":[1,2,3]}""", json);
        }

        [TestMethod]
        public void ToDictionaryTests()
        {
            // Arrange
            var json = """{"a":1,"a1":999,"b":"2","d":false,"e":"2021-01-01T00:00:00","f":[1,2,3]}""";
            var doc = JsonDocument.Parse(json);

            // Act
            var dic = doc.RootElement.ToDictionary();

            // Assert
            Assert.HasCount(6, dic);
            Assert.AreEqual(1, dic.Get<int>("a"));
            Assert.IsFalse(dic.Get<bool>("d"));
            Assert.AreEqual(DateTime.Parse("2021-01-01"), dic.Get<DateTime>("e"));
            Assert.AreEqual(3, dic.GetArray<int>("f").Count());
        }

        [TestMethod]
        public void FormatTemplateWithJsonTests()
        {
            // Arrange
            var tempalte = """Hello, {a}, your name is {name}, the date is {e}""";
            var json = """{"a":1,"a1":999,"b":"2","d":false,"e":"2021-01-01T00:00:00","f":[1,2,3]}""";

            // Act
            var result1 = tempalte.FormatTemplateWithJson(json, "?");
            var result2 = tempalte.FormatTemplateWithJson(json, "(empty)");

            // Assert
            Assert.AreEqual("Hello, 1, your name is ?, the date is 2021-01-01T00:00:00", result1);
            Assert.AreEqual("Hello, 1, your name is (empty), the date is 2021-01-01T00:00:00", result2);
        }
    }
}
