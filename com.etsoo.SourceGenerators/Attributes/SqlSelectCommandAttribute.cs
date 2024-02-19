namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL select command attribute
    /// SQL 选择命令属性
    /// </summary>
    public class SqlSelectCommandAttribute : SqlCommandAttribute
    {
        /// <summary>
        /// JSON naming policy
        /// JSON字段命名策略
        /// </summary>
        public NamingPolicy JsonNamingPolicy { get; set; } = NamingPolicy.CamelCase;

        /// <summary>
        /// Is JSON object
        /// 是否为JSON对象
        /// </summary>
        public bool IsObject { get; set; }

        public SqlSelectCommandAttribute(string tableName, NamingPolicy namingPolicy) : base(tableName, namingPolicy)
        {
        }
    }
}
