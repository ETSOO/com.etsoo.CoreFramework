using BenchmarkDotNet.Running;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Serialization;

namespace Benchmark
{
    class Program
    {
        static void Main()
        {
            // BenchmarkRunner.Run<Utils.StringKeyDictionaryBM>();
            // BenchmarkRunner.Run<Utils.JsonSerializationBM>();
            BenchmarkRunner.Run<Utils.ActionResultSerializationBM>();
            //BenchmarkRunner.Run<Utils.DictionaryDeserializationBM>();
        }
    }
}
