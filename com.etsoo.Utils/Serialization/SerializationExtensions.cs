﻿using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Serialization extensions
    /// 序列化扩展
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Create Utf8 Json writer
        /// Utf8 Json创建器
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="writer">Buffer writer</param>
        /// <returns></returns>
        public static Utf8JsonWriter CreateJsonWriter(this JsonSerializerOptions options, IBufferWriter<byte> writer)
        {
            return new Utf8JsonWriter(writer, new JsonWriterOptions { Encoder = options.Encoder, Indented = options.WriteIndented });
        }

        /// <summary>
        /// Convert name
        /// 转化名称
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="name">Name</param>
        /// <returns>Converted name</returns>
        public static string ConvertName(this JsonSerializerOptions options, string name)
        {
            var pnp = options.PropertyNamingPolicy;

            if (pnp == null)
            {
                return name;
            }

            return pnp.ConvertName(name);
        }

        /// <summary>
        /// Convert IDictionary key name
        /// 转化字段键名称
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="name">Name</param>
        /// <returns>Converted name</returns>
        public static string ConvertKeyName(this JsonSerializerOptions options, string name)
        {
            var pnp = options.DictionaryKeyPolicy;

            if (pnp == null)
            {
                return name;
            }

            return pnp.ConvertName(name);
        }

        /// <summary>
        /// Is Json serialization writable
        /// 是否支持Json序列化输出
        /// </summary>
        /// <param name="options">Options</param>
        /// <param name="isNull">Is null value</param>
        /// <param name="isField">Is field or property</param>
        /// <param name="isReadonly">Is readonly modifier</param>
        /// <returns>Writable</returns>
        public static bool IsWritable(this JsonSerializerOptions options, bool isNull, bool isField = false, bool isReadonly = false)
        {
            if (options.DefaultIgnoreCondition == JsonIgnoreCondition.Always)
                return false;

            if ((options.IgnoreNullValues || options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull) && isNull)
                return false;

            if (isField)
            {
                if (!options.IncludeFields)
                    return false;

                if (options.IgnoreReadOnlyFields && isReadonly)
                    return false;
            }

            if (options.IgnoreReadOnlyProperties && isReadonly)
                return false;

            return true;
        }
    }
}
