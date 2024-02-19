using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL command attribute
    /// SQL 命令属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class SqlCommandAttribute : Attribute
    {
        /// <summary>
        /// Parameter name
        /// 参数名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Column naming policy
        /// 列命名策略
        /// </summary>
        public NamingPolicy NamingPolicy { get; set; }

        /// <summary>
        /// Database name
        /// 数据库名称
        /// </summary>
        public DatabaseName Database { get; set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="tableName">Table name</param>
        public SqlCommandAttribute(string tableName, NamingPolicy namingPolicy)
        {
            TableName = tableName;
            NamingPolicy = namingPolicy;
        }
    }
}
