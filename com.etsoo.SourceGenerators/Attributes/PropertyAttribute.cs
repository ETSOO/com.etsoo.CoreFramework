using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// Property custom attributes
    /// 属性自定义参数
    /// </summary>
    public class PropertyAttribute : Attribute
    {
        /// <summary>
        /// Is non-unicode
        /// 是否为非Unicode
        /// </summary>
        public bool IsAnsi { get; set; }

        /// <summary>
        /// Is fixed length (char or varchar)
        /// 是否为固定长度
        /// </summary>
        public bool FixedLength { get; set; }

        /// <summary>
        /// Length
        /// 字符串长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Ignore the property
        /// 是否忽略该属性
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Parameter name
        /// 参数名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Type name
        /// 类型名称，如 Int
        /// </summary>
        public string? TypeName { get; set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        public PropertyAttribute()
        {
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="name">Name</param>
        public PropertyAttribute(string name) => (Name) = (name);
    }
}
