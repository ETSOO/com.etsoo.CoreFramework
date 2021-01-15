using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// Auto Dictionary object generator support attribute
    /// 自动Dictionary对象创建者支持属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class AutoDictionaryGeneratorAttribute : Attribute
    {
        /// <summary>
        /// Data field name is snake case
        /// 数据字段名称为蛇形命名
        /// </summary>
        public bool SnakeCase { get; set; }
    }
}
