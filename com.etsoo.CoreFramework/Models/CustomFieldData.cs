using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Custom field space (12 columns)
    /// 自定义字段空间（12列）
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<CustomFieldSpace>))]
    public enum CustomFieldSpace
    {
        Quater,
        Five,
        Half,
        Half1,
        Seven,
        Full
    }

    /// <summary>
    /// Custom field item
    /// 自定义字段项
    /// </summary>
    public record CustomFieldItem
    {
        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        public required string Type { get; init; }

        /// <summary>
        /// Field name
        /// 字段名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Label
        /// 标签
        /// </summary>
        public string? Label { get; init; }

        /// <summary>
        /// Value
        /// 值
        /// </summary>
        public required object Value { get; init; }
    }

    /// <summary>
    /// Custom field data
    /// 自定义字段数据
    /// </summary>
    public record CustomFieldData
    {
        /// <summary>
        /// Type
        /// 类型
        /// </summary>
        public required string Type { get; init; }

        /// <summary>
        /// Field name
        /// 字段名
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Options
        /// 选项
        /// </summary>
        public IEnumerable<ListType2>? Options { get; init; }

        /// <summary>
        /// Refs
        /// 引用
        /// </summary>
        public IEnumerable<object>? Refs { get; init; }

        /// <summary>
        /// Space
        /// 宽度
        /// </summary>
        public CustomFieldSpace? Space { get; init; }

        /// <summary>
        /// Grid item properties
        /// Grid Item 属性
        /// </summary>
        public Dictionary<string, object?>? GridItemProps { get; init; }

        /// <summary>
        /// Main slot properties
        /// 主 Slot 属性
        /// </summary>
        public Dictionary<string, object?>? MainSlotProps { get; init; }

        /// <summary>
        /// Label
        /// 标签
        /// </summary>
        public string? Label { get; init; }

        /// <summary>
        /// Helper text
        /// 帮助文本
        /// </summary>
        public string? HelperText { get; init; }
    }
}
