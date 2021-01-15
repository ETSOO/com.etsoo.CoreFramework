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
    }
}
