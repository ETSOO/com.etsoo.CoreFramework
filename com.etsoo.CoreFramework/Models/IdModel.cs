namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Model with id
    /// 带编号的模型
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record IdModel<T> where T : struct
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        public T Id { get; init; }
    }
}
