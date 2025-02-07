namespace com.etsoo.Database
{
    /// <summary>
    /// Entity changed property
    /// 实体更改了的属性
    /// </summary>
    public record EntityChangedProperty
    {
        /// <summary>
        /// Property name
        /// 属性名称
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Original value
        /// 原值
        /// </summary>
        public object? OriginalValue { get; init; }

        /// <summary>
        /// Current value
        /// 当前值
        /// </summary>
        public object? CurrentValue { get; init; }
    }
}
