namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Item with id, label and is primary
    /// 带编号、标签和是否为默认的项目
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record IdLabelPrimaryItem<T> : IdLabelItem<T> where T : struct
    {
        /// <summary>
        /// Is primary item
        /// 是否为默认项目
        /// </summary>
        public bool? IsPrimary {  get; set; }
    }
}
