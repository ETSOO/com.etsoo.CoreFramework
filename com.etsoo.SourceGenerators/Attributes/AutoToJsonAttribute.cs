using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// Auto to Json attribute
    /// 自动输出Json属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class AutoToJsonAttribute : Attribute
    {
    }
}
