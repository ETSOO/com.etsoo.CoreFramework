namespace com.etsoo.CoreFramework.Models
{
    public interface IUpdateModel<T> where T : struct
    {
        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        T Id { get; init; }

        /// <summary>
        /// Changed fields
        /// 改变的字段
        /// </summary>
        IEnumerable<string>? ChangedFields { get; set; }
    }
}
