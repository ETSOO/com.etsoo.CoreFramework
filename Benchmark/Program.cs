using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main()
        {
            // BenchmarkRunner.Run<Utils.StringKeyDictionaryBM>();
            // BenchmarkRunner.Run<Utils.JsonSerializationBM>();
            // BenchmarkRunner.Run<Utils.ActionResultSerializationBM>();
            // BenchmarkRunner.Run<Utils.DictionaryDeserializationBM>();
            BenchmarkRunner.Run<Utils.StringSpanBM>();
        }
    }
}
