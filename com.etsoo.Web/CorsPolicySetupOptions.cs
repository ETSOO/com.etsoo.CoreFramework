using Microsoft.Extensions.Logging;

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
        public string[] ExposedHeaders { get; init; } = [];

        /// <summary>
        /// Logger, for debug purpose only
        /// 日志记录器，仅用于调试目的
        /// </summary>
        public ILogger? Logger { get; init; }

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
            if (sites?.Length > 0 || includeLocals)
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
            if (IncludeLocals && new Uri(origin).IsLoopback)
            {
                Logger?.LogDebug("Origin {origin} is local and IncludeLocals is true", origin);
                return true;
            }

            if (Sites?.Length > 0)
            {
                var originParts = origin.Split('.');
                var len = originParts.Length;

                return Sites.Any((site) =>
                {
                    var siteParts = site.Split('.');

                    if (siteParts.Length != len)
                    {
                        Logger?.LogDebug("Valid site {site} parts length is not equal with origin {origin}", site, origin);
                        return false;
                    }

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

                            Logger?.LogDebug("Site {site} failed origin {origin} at part {i}, {part}/{opart}", site, origin, i, part, opart);
                            return false;
                        }
                    }

                    Logger?.LogDebug("Site {site} validates origin {origin}", site, origin);
                    return true;
                });
            }

            Logger?.LogDebug("No valid sites defined");
            return false;
        }
    }
}
