using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// Auto support Dapper parameters attribute
    /// 自动支持输出Dapper参数属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class AutoToParametersAttribute : Attribute
    {
        /// <summary>
        /// Ignore null fields
        /// 忽略NULL字段
        /// </summary>
        public bool IgnoreNull { get; set; }

        /// <summary>
        /// Name is snake case
        /// 名称为蛇形命名
        /// </summary>
        public bool SnakeCase { get; set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="ignoreNull">Ignore null fields</param>
        public AutoToParametersAttribute(bool ignoreNull = true)
        {
            IgnoreNull = ignoreNull;
        }
    }
}
