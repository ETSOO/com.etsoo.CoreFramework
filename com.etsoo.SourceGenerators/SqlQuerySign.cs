namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// SQL query sign
    /// SQL查询符号
    /// </summary>
    public enum SqlQuerySign : byte
    {
        /// <summary>
        /// Equal
        /// 等于
        /// </summary>
        Equal = 0,

        /// <summary>
        /// Not equal
        /// 不等于
        /// </summary>
        NotEqual = 1,

        /// <summary>
        /// Greater, '>'
        /// 大于
        /// </summary>
        Greater = 2,

        /// <summary>
        /// Greater or equal, '>='
        /// 大于或等于
        /// </summary>
        GreaterOrEqual = 3,

        /// <summary>
        /// Less, '<'
        /// 小于
        /// </summary>
        Less = 4,

        /// <summary>
        /// Less or equal, '<='
        /// 小于或等于
        /// </summary>
        LessOrEqual = 5,

        /// <summary>
        /// Like
        /// 相似
        /// </summary>
        Like = 6,

        /// <summary>
        /// Not like
        /// 不相似
        /// </summary>
        NotLike = 7
    }
}
