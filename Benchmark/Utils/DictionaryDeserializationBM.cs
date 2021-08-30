using BenchmarkDotNet.Attributes;
using com.etsoo.Utils.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Benchmark.Utils
{
    [MValueColumn]
    public class DictionaryDeserializationBM
    {
        private class TestModal
        {
            public int Id { get; init; }
            public bool Valid { get; init; }
            public DateTime? Now { get; init; }
            public string[]? Items { get; init; }
            public Guid Guid { get; init; }

            public TestModal()
            {

            }

            public TestModal(StringKeyDictionaryObject dic)
            {
                Id = dic.GetExact<int>("id");
                Valid = dic.GetExact<bool>("valid");
                Now = dic.GetExact<DateTime>("now");
                Items = dic.GetExact<string[]>("items");
                Guid = dic.GetExact<Guid>("guid");
            }
        }

        readonly StringKeyDictionaryObject dic;

        public DictionaryDeserializationBM()
        {
            dic = new StringKeyDictionaryObject
            {
                ["id"] = 128,
                ["valid"] = true,
                ["Now"] = DateTime.Now,
                ["Items"] = new string[] { "Hello", "World" }
            };
        }

        [Benchmark]
        public void CodeParse()
        {
            _ = new TestModal(dic);
        }

        [Benchmark]
        public void JsonParse()
        {
            var json = JsonSerializer.Serialize(dic, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            JsonSerializer.Deserialize<TestModal>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            });
        }

        private T? Parse<T>()
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type, dic);
            return (T?)obj;
        }

        [Benchmark]
        public void ReflectionParse()
        {
            Parse<TestModal>();
        }
    }
}
