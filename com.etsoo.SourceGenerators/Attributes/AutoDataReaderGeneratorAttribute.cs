using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// Auto DbDataReader object generator support attribute
    /// 自动DbDataReader对象创建者支持属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class AutoDataReaderGeneratorAttribute : Attribute
    {
        /// <summary>
        /// Is auto set datetime to Utc kind
        /// 是否设置日期时间为Utc类型
        /// </summary>
        public bool UtcDateTime { get; set; }
    }
}
