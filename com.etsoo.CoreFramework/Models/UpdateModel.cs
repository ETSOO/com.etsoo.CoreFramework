namespace com.etsoo.CoreFramework.Models
{
    public record UpdateModel<T> : IdModel<T>, IUpdateModel<T> where T : struct
    {
        /// <summary>
        /// Changed fields
        /// 改变的字段
        /// </summary>
        public IEnumerable<string>? ChangedFields { get; set; }
    }
}