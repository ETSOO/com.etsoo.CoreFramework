using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL select result attribute
    /// SQL 选择结果属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SqlSelectResultAttribute : Attribute
    {
        /// <summary>
        /// Is JSON object
        /// 是否为JSON对象
        /// </summary>
        public bool IsObject { get; set; }

        /// <summary>
        /// Result type
        /// 结果类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Property name
        /// 属性名称
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="type">Result type</param>
        public SqlSelectResultAttribute(Type type)
        {
            Type = type;
        }
    }
}
