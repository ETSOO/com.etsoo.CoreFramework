namespace com.etsoo.SMS
{
    /// <summary>
    /// Template client interface
    /// 模板客户端接口
    /// </summary>
    public interface ITemplateClient
    {
        /// <summary>
        /// Add template
        /// 添加模板
        /// </summary>
        /// <param name="template">Template</param>
        void AddTemplate(TemplateItem template);

        /// <summary>
        /// Add templates
        /// 添加多个模板
        /// </summary>
        /// <param name="templates">Templates</param>
        void AddTemplates(IEnumerable<TemplateItem> templates);

        /// <summary>
        /// Get template
        /// 获取模板
        /// </summary>
        /// <param name="kind">Template kind</param>
        /// <param name="templateId">Template id</param>
        /// <param name="region">Country or region</param>
        /// <param name="language">Language</param>
        /// <returns>Resource</returns>
        TemplateItem? GetTemplate(TemplateKind kind, string? templateId = null, string? region = null, string? language = null);
    }
}
