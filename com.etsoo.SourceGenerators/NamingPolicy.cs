namespace com.etsoo.SourceGenerators
{
    /// <summary>
    /// Column naming policy
    /// 列命名策略
    /// </summary>
    public enum NamingPolicy : byte
    {
        /// <summary>
        /// Camel case
        /// 驼峰
        /// </summary>
        CamelCase = 0,

        /// <summary>
        /// Pascal case
        /// 帕斯卡
        /// </summary>
        PascalCase = 1,

        /// <summary>
        /// Snake case
        /// 蛇形
        /// </summary>
        SnakeCase = 2
    }
}
