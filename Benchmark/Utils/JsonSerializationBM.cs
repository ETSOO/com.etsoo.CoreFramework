using BenchmarkDotNet.Attributes;
using com.etsoo.Utils.Serialization;
using System.Buffers;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Benchmark.Utils
{
    [MValueColumn]
    public class JsonSerializationBM
    {
        public class Person
        {
            public int? Id { get; init; }

            public string? Name { get; init; }

            public DateTime? Birthday { get; init; }

            public bool? Valid { get; set; }

            public int[]? Flags { get; init; }
        }

        [Benchmark]
        public void JsonDirect()
        {
            var person = new Person()
            {
                Id = 1001,
                Name = "Admin 1",
                Birthday = DateTime.Now,
                Valid = false,
                Flags = new[] { 1, 1093, 2, -1 }
            };

            var json = JsonSerializer.Serialize(person, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            Console.WriteLine(json);
        }

        public class PersonExtended : Person, ISerializable
        {
            public PersonExtended()
            {

            }

            protected PersonExtended(SerializationInfo info, StreamingContext context)
            {

            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Id", Id);
                info.AddValue("Name", Name);
                info.AddValue("Birthday", Birthday);
                info.AddValue("Valid", Valid);
                info.AddValue("Flags", Flags);
            }
        }

        [Benchmark]
        public void JsonISerializable()
        {
            var person = new PersonExtended()
            {
                Id = 1001,
                Name = "Admin 1",
                Birthday = DateTime.Now,
                Valid = false,
                Flags = new[] { 1, 1093, 2, -1 }
            };

            var json = JsonSerializer.Serialize(person, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            Console.WriteLine(json);
        }

        public class PersonLocal : Person, IJsonSerialization
        {
            public async Task ToJsonAsync(IBufferWriter<byte> writer, JsonSerializerOptions options, IEnumerable<string>? fields = null)
            {
                await using var w = options.CreateJsonWriter(writer);

                w.WriteStartObject();

                if (options.IsWritable(!Id.HasValue))
                {
                    var idName = options.ConvertName(nameof(Id));
                    if (fields == null || fields.Any(f => f.Equals(idName) || f.Equals(nameof(Id))))
                    {
                        if (!Id.HasValue)
                            w.WriteNull(idName);
                        else
                            w.WriteNumber(idName, Id.Value);
                    }
                }

                if (options.IsWritable(Name == null))
                {
                    var nameName = options.ConvertName(nameof(Name));
                    if (fields == null || fields.Any(f => f.Equals(nameName) || f.Equals(nameof(Name))))
                    {
                        if (Name == null)
                            w.WriteNull(nameName);
                        else
                            w.WriteString(nameName, Name);
                    }
                }

                if (options.IsWritable(!Birthday.HasValue))
                {
                    var birthdayName = options.ConvertName(nameof(Birthday));
                    if (fields == null || fields.Any(f => f.Equals(birthdayName) || f.Equals(nameof(Birthday))))
                    {
                        if (!Birthday.HasValue)
                            w.WriteNull(birthdayName);
                        else
                            w.WriteString(birthdayName, Birthday.Value);
                    }
                }

                if (options.IsWritable(!Valid.HasValue))
                {
                    var validName = options.ConvertName(nameof(Valid));
                    if (fields == null || fields.Any(f => f.Equals(validName) || f.Equals(nameof(Valid))))
                    {
                        if (!Valid.HasValue)
                            w.WriteNull(validName);
                        else
                            w.WriteBoolean(validName, Valid.Value);
                    }
                }

                if (options.IsWritable(Flags == null))
                {
                    var flagsName = options.ConvertName(nameof(Flags));
                    if (fields == null || fields.Any(f => f.Equals(flagsName) || f.Equals(nameof(Flags))))
                    {
                        if (Flags == null)
                            w.WriteNull(flagsName);
                        else
                        {
                            w.WriteStartArray(flagsName);
                            foreach (var f in Flags)
                            {
                                w.WriteNumberValue(f);
                            }
                            w.WriteEndArray();
                        }
                    }
                }

                w.WriteEndObject();

                await w.DisposeAsync();
            }
        }

        [Benchmark]
        public async Task JsonLocal()
        {
            var person = new PersonLocal()
            {
                Id = 1001,
                Name = "Admin 1",
                Birthday = DateTime.Now,
                Valid = false,
                Flags = new[] { 1, 1093, 2, -1 }
            };

            if (person is IJsonSerialization jl)
            {
                var writer = new ArrayBufferWriter<byte>();
                await jl.ToJsonAsync(writer, new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
                var json = Encoding.UTF8.GetString(writer.WrittenSpan);

                Console.WriteLine(json);
            }
        }
    }
}
