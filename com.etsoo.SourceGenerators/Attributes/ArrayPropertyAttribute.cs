using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// Auto DbDataReader object creator support attribute
    /// 自动DbDataReader对象创建者支持属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ArrayPropertyAttribute : Attribute
    {
        /// <summary>
        /// Splitter
        /// 分割字符
        /// </summary>
        public char Splitter { get; set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="splitter">Splitter</param>
        public ArrayPropertyAttribute(char splitter)
        {
            Splitter = splitter;
        }
    }
}
