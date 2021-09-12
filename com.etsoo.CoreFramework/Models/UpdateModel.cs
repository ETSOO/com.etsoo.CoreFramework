namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Update model
    /// 更新模块
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record UpdateModel<T> : IdModel<T>, IUpdateModel<T> where T : struct
    {
        /// <summary>
        /// Changed fields
        /// 改变的字段
        /// </summary>
        public IEnumerable<string>? ChangedFields { get; set; }
    }
}