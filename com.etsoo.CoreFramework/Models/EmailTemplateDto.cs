namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Email template data
    /// 邮件模板数据
    /// </summary>
    public record EmailTemplateDto
    {
        /// <summary>
        /// Subject
        /// 主题
        /// </summary>
        public required string Subject { get; init; }

        /// <summary>
        /// Template
        /// 模板
        /// </summary>
        public required string Template { get; init; }

        /// <summary>
        /// Is razor template
        /// 是否为 Razor 模板
        /// </summary>
        public bool? IsRazor { get; init; }

        /// <summary>
        /// Cc
        /// 抄送
        /// </summary>
        public IEnumerable<string>? Cc { get; init; }

        /// <summary>
        /// Bcc
        /// 密送
        /// </summary>
        public IEnumerable<string>? Bcc { get; init; }
    }
}
