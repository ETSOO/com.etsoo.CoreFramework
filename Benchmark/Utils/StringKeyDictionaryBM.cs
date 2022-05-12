using BenchmarkDotNet.Attributes;
using com.etsoo.Utils.String;

namespace Benchmark.Utils
{
    [MValueColumn]
    public class StringKeyDictionaryBM
    {
        [Benchmark]
        public void ObjectDictionary()
        {
            var dic = new StringKeyDictionaryObject
            {
                { "int", 1 },
                { "bool", true },
                { "date", DateTime.Now },
                { "null", null }
            };

            dic.GetItem("int");
            dic.Get<bool>("bool");
            dic.Get<DateTime>("int");
        }

        [Benchmark]
        public void ObjectOnlyDictionary()
        {
            object intObj = 1;
            object boolObj = true;
            object dateObj = DateTime.Now;
            object? nullObj = null;

            var dic = new StringKeyDictionaryObject
            {
                { "int", intObj },
                { "bool", boolObj },
                { "date", dateObj },
                { "null", nullObj }
            };

            dic.GetItem("int");
            dic.Get<bool>("bool");
            dic.Get<DateTime>("int");
        }

        [Benchmark]
        public void DynamicDictionary()
        {
            var dic = new StringKeyDictionaryDynamic
            {
                { "int", 1 },
                { "bool", true },
                { "date", DateTime.Now },
                { "null", null }
            };

            dic.GetItem("int");
            dic.Get<bool>("bool");
            dic.Get<DateTime>("int");
        }

        [Benchmark]
        public void DynamicObjectDictionary()
        {
            object intObj = 1;
            object boolObj = true;
            object dateObj = DateTime.Now;
            object? nullObj = null;

            var dic = new StringKeyDictionaryDynamic
            {
                { "int", intObj },
                { "bool", boolObj },
                { "date", dateObj },
                { "null", nullObj }
            };

            dic.GetItem("int");
            dic.Get<bool>("bool");
            dic.Get<DateTime>("int");
        }
    }
}
