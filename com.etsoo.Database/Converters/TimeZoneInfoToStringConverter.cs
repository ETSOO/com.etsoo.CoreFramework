using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace com.etsoo.Database.Converters
{
    /// <summary>
    /// TimeZoneInfo to string converter
    /// 时区信息转字符串转换器
    /// </summary>
    public class TimeZoneInfoToStringConverter : ValueConverter<TimeZoneInfo?, string?>
    {
        private static readonly ConverterMappingHints DefaultHints = new(size: 64);

        public TimeZoneInfoToStringConverter()
            : this(null)
        {
        }

        public TimeZoneInfoToStringConverter(ConverterMappingHints? mappingHints)
            : base(
                ToString(),
                ToTimeZoneInfo(),
                DefaultHints.With(mappingHints))
        {
        }

        public static ValueConverterInfo DefaultInfo { get; }
            = new(typeof(TimeZoneInfo), typeof(string), i => new TimeZoneInfoToStringConverter(i.MappingHints), DefaultHints);

        private static new Expression<Func<TimeZoneInfo?, string?>> ToString()
            => v => v!.Id;

        private static Expression<Func<string?, TimeZoneInfo?>> ToTimeZoneInfo()
            => v => TimeZoneUtils.GetTimeZoneBase(v);
    }
}
