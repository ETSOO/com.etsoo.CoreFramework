namespace com.etsoo.Utils.Serialization.Country
{
    /// <summary>
    /// Culture item
    /// 文化项目
    /// </summary>
    public record CultureItem
    {
        /// <summary>
        /// Id, like zh-Hans-CN
        /// 编号，如zh-Hans-CN
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Two characters id, like zh
        /// 两个字符编号
        /// </summary>
        public required string Id2 { get; init; }

        /// <summary>
        /// Three characters id, like zho
        /// 三个字符编号
        /// </summary>
        public required string Id3 { get; init; }

        /// <summary>
        /// Parent culture, like zh-Hans
        /// 父文化
        /// </summary>
        public required string Parent { get; init; }

        /// <summary>
        /// Native name, like 中文(简体，中国)
        /// 原生名
        /// </summary>
        public required string NativeName { get; init; }

        /// <summary>
        /// English name, like Chinese (Simplified, China)
        /// 英文名
        /// </summary>
        public required string EnglishName { get; init; }
    }
}
