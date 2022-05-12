using BenchmarkDotNet.Attributes;

namespace Benchmark.Utils
{
    [MValueColumn]
    public class StringSpanBM
    {
        private const string INPUT = "ghhrkkbghhrkkbghhrkkbghhrkkbghhrkkbbkkkkkrtzzeed";

        [Benchmark]
        public string? TraditionalAgregateWay()
        {
            var result = INPUT.Aggregate(new List<char>(), (seed, item) =>
            {
                if (seed.Count == 0 || seed.Last() != item) seed.Add(item);
                return seed;
            });
            return string.Concat(result);
        }

        [Benchmark]
        public string? TraditionalWhereWay()
        {
            var result = string.Concat(INPUT.Where((c, index) => index == 0 || INPUT[index - 1] != c));
            return result;
        }

        [Benchmark]
        public string? SpanWay()
        {
            var chars = INPUT.AsSpan();
            char? last = null;
            var index = 0;
            var newIndex = 0;
            var len = 0;
            Span<char> newChars = new char[chars.Length];

            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (last == null || !last.Value.Equals(c))
                {
                    last = c;
                    len++;
                }
                else
                {
                    chars.Slice(index, len).CopyTo(newChars.Slice(newIndex, len));
                    newIndex += len;
                    index = i + 1;
                    len = 0;
                }
            }

            chars.Slice(index, len).CopyTo(newChars.Slice(newIndex, len));

            return newChars.Trim('\0').ToString();
        }
    }
}
