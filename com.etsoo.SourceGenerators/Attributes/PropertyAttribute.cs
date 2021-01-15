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
        /// Parameter name
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type name
        /// 类型名称，如 Int
        /// </summary>
        public string TypeName { get; set; }

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
