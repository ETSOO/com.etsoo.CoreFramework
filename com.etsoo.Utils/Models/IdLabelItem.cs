namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Item with id and label
    /// 带编号和标签的项目
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record IdLabelItem<T> : IdItem<T> where T : struct
    {
        /// <summary>
        /// Label
        /// 标签
        /// </summary>
        public string Label { get; set; } = null!;
    }
}
