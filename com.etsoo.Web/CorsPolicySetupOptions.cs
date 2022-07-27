namespace com.etsoo.Web
{
    /// <summary>
    /// CORS policy setup options
    /// CORS 策略设置选项
    /// </summary>
    public record CorsPolicySetupOptions
    {
        /// <summary>
        /// Exposed headers
        /// 公开的标头
        /// </summary>
        public string[] ExposedHeaders { get; init; } = Array.Empty<string>();

        /// <summary>
        /// Include localhost addresses
        /// 包括本地主机地址
        /// </summary>
        private bool IncludeLocals { get; }

        /// <summary>
        /// Required
        /// 是否必须设置
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// Valid sites, support subdomains and http://192.168.1.*:*, IP range and all ports
        /// 有效站点，支持子域和区间IP地址和任意端口
        /// </summary>
        private string[]? Sites { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="sites">Valid sites</param>
        /// <param name="includeLocals">Include local addresses</param>
        public CorsPolicySetupOptions(string[]? sites, bool includeLocals = false)
        {
            if (sites?.Any() is true || includeLocals)
            {
                Sites = sites;
                IncludeLocals = includeLocals;

                Required = true;
            }
        }

        /// <summary>
        /// Check origin, 'true' means passing Cors policy
        /// 检查
        /// </summary>
        /// <param name="origin">Origin</param>
        /// <returns>Result</returns>
        public bool Check(string origin)
        {
            // Localhost / 127.*.*.* accepted
            if (IncludeLocals && new Uri(origin).IsLoopback) return true;

            if (Sites?.Any() is true)
            {
                var originParts = origin.Split('.');
                var len = originParts.Length;

                return Sites.Any((site) =>
                {
                    var siteParts = site.Split('.');

                    if (siteParts.Length != len) return false;

                    for (var i = 0; i < len; i++)
                    {
                        var part = siteParts[i];
                        var opart = originParts[i];
                        if (opart != part)
                        {
                            if (i == 0)
                            {
                                if (part.EndsWith('*') && opart.StartsWith(part[..^1]))
                                {
                                    continue;
                                }
                            }
                            else if (i + 1 == len)
                            {
                                if (
                                    part == "*:*" ||
                                    (part == "*" && !opart.Contains(':'))
                                    || (part.EndsWith(":*") && (opart.Equals(part[..^2]) || opart.StartsWith(part[..^1])))
                                )
                                {
                                    continue;
                                }
                            }
                            return false;
                        }
                    }

                    return true;
                });
            }

            return false;
        }
    }
}
