using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace com.etsoo.Database.Converters
{
    /// <summary>
    /// Json type converter
    /// JSON类型转换器
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public class JsonTypeConverter<T> : ValueConverter<T, string?>
    {
        public JsonTypeConverter(JsonTypeInfo<T> typeInfo)
            : base(ToProvider(typeInfo), FromProvider(typeInfo))
        {
        }

        private static Expression<Func<T, string?>> ToProvider(JsonTypeInfo<T> typeInfo)
            => v => JsonSerializer.Serialize(v, typeInfo);

        private static Expression<Func<string?, T>> FromProvider(JsonTypeInfo<T> typeInfo)
            => v => (string.IsNullOrEmpty(v) ? default : JsonSerializer.Deserialize(v, typeInfo) ?? default)!;
    }
}
