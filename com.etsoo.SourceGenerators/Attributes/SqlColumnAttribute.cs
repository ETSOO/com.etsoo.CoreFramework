using System;

namespace com.etsoo.SourceGenerators.Attributes
{
    /// <summary>
    /// SQL column attribute
    /// SQL列属性
    /// </summary>
    public class SqlColumnAttribute : Attribute
    {
        /// <summary>
        /// Tabl column name
        /// 表格列名
        /// </summary>
        public string? ColumnName { get; set; }

        /// <summary>
        /// Ignore the property
        /// 是否忽略该属性
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Keep null value
        /// 保留空值
        /// </summary>
        public bool KeepNull { get; set; }

        /// <summary>
        /// Key column, used for update filter
        /// 关键列，用于更新过滤
        /// </summary>
        public bool Key { get; set; }

        /// <summary>
        /// SQL query sign
        /// SQL查询符号
        /// </summary>
        public SqlQuerySign QuerySign { get; set; }
    }
}