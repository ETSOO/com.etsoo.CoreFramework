using BenchmarkDotNet.Attributes;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.String;
using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Benchmark.Utils
{
    [MValueColumn]
    public class ActionResultSerializationBM
    {
        private readonly ActionResult result;

        public ActionResultSerializationBM()
        {
            result = new ActionResult
            {
                Success = false,
                Title = "Action result test",
                TraceId = "SP1543",
                Data = new StringKeyDictionaryObject
                {
                    ["id"] = 128,
                    ["valid"] = true,
                    ["Now"] = DateTime.Now,
                    ["Items"] = new string[] { "Hello", "World" }
                }
            };
            result.AddError(new ActionResultError("Name", "Required"));
        }

        [Benchmark]
        public void JsonDirect()
        {
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                IgnoreNullValues = true
            });

            Console.WriteLine(json);
        }

        [Benchmark]
        public async Task JsonLocal()
        {
            var writer = new ArrayBufferWriter<byte>();
            await result.ToJsonAsync(writer, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                IgnoreNullValues = true
            });
            var json = Encoding.UTF8.GetString(writer.WrittenSpan);

            Console.WriteLine(json);
        }
    }
}
