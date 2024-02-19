using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL select column attribute
    /// SQL选择列属性
    /// </summary>
    public class SqlSelectColumnAttribute : Attribute
    {
        /// <summary>
        /// Column prefix
        /// 列前缀
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Column calculated function
        /// 列计算函数
        /// </summary>
        public string? Function { get; set; }
    }
}
