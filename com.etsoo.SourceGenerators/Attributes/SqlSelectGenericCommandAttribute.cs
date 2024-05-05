namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL select command attribute
    /// SQL 选择命令属性
    /// </summary>
    public sealed class SqlSelectGenericCommandAttribute : SqlCommandAttribute
    {
        /// <summary>
        /// JSON naming policy
        /// JSON字段命名策略
        /// </summary>
        public NamingPolicy JsonNamingPolicy { get; set; } = NamingPolicy.CamelCase;

        public SqlSelectGenericCommandAttribute(string tableName, NamingPolicy namingPolicy) : base(tableName, namingPolicy)
        {
        }
    }
}
