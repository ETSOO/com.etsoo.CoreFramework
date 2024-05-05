namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL insert command attribute
    /// SQL 插入命令属性
    /// </summary>
    public sealed class SqlInsertCommandAttribute : SqlCommandAttribute
    {
        /// <summary>
        /// Primary key column name, default is 'Id'
        /// 主键列名，默认为 'Id'
        /// </summary>
        public string? PrimaryKey { get; set; }

        public SqlInsertCommandAttribute(string tableName, NamingPolicy namingPolicy) : base(tableName, namingPolicy)
        {
        }
    }
}
