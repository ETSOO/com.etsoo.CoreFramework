namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL update command attribute
    /// SQL 更新命令属性
    /// </summary>
    public class SqlUpdateCommandAttribute : SqlCommandAttribute
    {
        public SqlUpdateCommandAttribute(string tableName, NamingPolicy namingPolicy) : base(tableName, namingPolicy)
        {
        }
    }
}
