namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Model with id, label and is primary
    /// 带编号，标签和是否为默认的模型
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record IdLabelPrimaryModel<T> : IdLabelModel<T> where T : struct
    {
        /// <summary>
        /// Is primary
        /// 是否为默认
        /// </summary>
        public bool IsPrimary { get; set; }
    }
}
