namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL delete command attribute
    /// SQL 删除命令属性
    /// </summary>
    public sealed class SqlDeleteCommandAttribute : SqlCommandAttribute
    {
        public SqlDeleteCommandAttribute(string tableName, NamingPolicy namingPolicy) : base(tableName, namingPolicy)
        {
        }
    }
}
