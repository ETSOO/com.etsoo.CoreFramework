namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Model with id and label
    /// 带编号和标签的模型
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record IdLabelModel<T> : IdModel<T> where T : struct
    {
        /// <summary>
        /// Label
        /// 标签
        /// </summary>
        public string Label { get; set; } = null!;
    }
}
