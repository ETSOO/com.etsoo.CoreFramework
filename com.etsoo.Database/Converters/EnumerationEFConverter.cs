using com.etsoo.Utils;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace com.etsoo.Database.Converters
{
    /// <summary>
    /// Enumeration EF Converter
    /// </summary>
    /// <typeparam name="E">Model side generic type</typeparam>
    /// <typeparam name="T">Database side generic type</typeparam>
    public class EnumerationEFConverter<E, T> : ValueConverter<E?, T?>
        where E : Enumeration<T>
        where T : struct, IComparable
    {
        public EnumerationEFConverter()
            : this(null)
        {
        }

        public EnumerationEFConverter(ConverterMappingHints? mappingHints)
            : base(FromEnumeration(), ToEnumeration(), mappingHints)
        {
        }

        public static ValueConverterInfo DefaultInfo { get; }
            = new(typeof(E), typeof(T), i => new EnumerationEFConverter<E, T>(i.MappingHints));

        private static Expression<Func<E?, T?>> FromEnumeration()
            => v => v == null ? null : v.Value;

        private static Expression<Func<T?, E?>> ToEnumeration()
            => v => Enumeration<T>.Parse<E>(v);
    }
}
