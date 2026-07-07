namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// List type 2
    /// 列表类型 2
    /// </summary>
    public record ListType2
    {
        /// <summary>
        /// Id
        /// </summary>
        public required object Id { get; init; }

        /// <summary>
        /// Label
        /// </summary>
        public string? Label
        {
            get
            {
                return field ?? Title ?? Name;
            }
            set;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Title
        /// </summary>
        public string? Title { get; init; }
    }
}
