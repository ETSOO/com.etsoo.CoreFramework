namespace com.etsoo.Utils.Net.SMS
{
    /// <summary>
    /// Template abstract client
    /// 模板抽象客户端
    /// </summary>
    public abstract class TemplateClient : HttpClientService, ITemplateClient
    {
        private readonly List<TemplateItem> allTemplates = new();

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="client">HTTP client</param>
        public TemplateClient(HttpClient client) : base(client)
        {
        }

        /// <summary>
        /// Add template
        /// 添加模板
        /// </summary>
        /// <param name="template">Template</param>
        public void AddTemplate(TemplateItem template)
        {
            allTemplates.Add(template);
        }

        /// <summary>
        /// Add templates
        /// 添加多个模板
        /// </summary>
        /// <param name="templates">Templates</param>
        public void AddTemplates(IEnumerable<TemplateItem> templates)
        {
            allTemplates.AddRange(templates);
        }

        /// <summary>
        /// Get template
        /// 获取模板
        /// </summary>
        /// <param name="kind">Template kind</param>
        /// <param name="templateId">Template id</param>
        /// <param name="region">Country or region</param>
        /// <param name="language">Language</param>
        /// <returns>Resource</returns>
        public TemplateItem? GetTemplate(TemplateKind kind, string? templateId = null, string? region = null, string? language = null)
        {
            if (!string.IsNullOrEmpty(templateId))
            {
                // Exact match for the template id
                return allTemplates.FirstOrDefault(t => t.Kind == kind && t.TemplateId == templateId);
            }

            if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(language))
            {
                // Return first default item
                return allTemplates.FirstOrDefault(t => t.Kind == kind && t.Default && t.Region == null && t.Language == null);
            }

            var subTemplates = allTemplates.Where(t => t.Kind == kind);

            if (!string.IsNullOrEmpty(region))
            {
                // All countries or the specific country
                subTemplates = subTemplates.Where(r => r.Region == null || r.Region == region).OrderByDescending(r => r.Region == region).ThenByDescending(r => r.Default);
            }

            if (!string.IsNullOrEmpty(language))
            {
                // All languages or the specific language
                subTemplates = subTemplates.Where(r => r.Language == null || r.Language == language).OrderByDescending(r => r.Language == language).ThenByDescending(r => r.Default);
            }

            return subTemplates.FirstOrDefault();
        }
    }
}
