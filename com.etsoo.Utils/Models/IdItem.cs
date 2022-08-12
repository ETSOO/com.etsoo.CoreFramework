namespace com.etsoo.Utils.Models
{
    /// <summary>
    /// Item with id
    /// 带编号的项目
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record IdItem<T> where T : struct
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public T Id { get; init; }
    }

    /// <summary>
    /// Item with id
    /// 带编号的项目
    /// </summary>
    public record IdItem
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public string Id { get; init; } = default!;
    }
}
