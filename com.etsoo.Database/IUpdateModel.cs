namespace com.etsoo.Database
{
    /// <summary>
    /// Update model interface
    /// 更新模型接口
    /// </summary>
    public interface IUpdateModel
    {
        /// <summary>
        /// Changed fields
        /// 改变的字段
        /// </summary>
        IEnumerable<string>? ChangedFields { get; set; }
    }
}
