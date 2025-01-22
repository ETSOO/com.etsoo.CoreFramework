using com.etsoo.CoreFramework.Authentication;
using com.etsoo.CoreFramework.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace com.etsoo.Web
{
    /// <summary>
    /// Etsoo rate limiter options
    /// 亿速思维速率限制选项
    /// </summary>
    public record EtsooRateLimiterOptions
    {
        /// <summary>
        /// API multiplier
        /// 接口倍数
        /// </summary>
        public int ApiMultiplier { get; init; } = 5;

        /// <summary>
        /// Maximum requests permit limit within the window
        /// 在一个时间窗口内的最大请求许可限制
        /// </summary>
        public int PermitLimit { get; init; } = 50;

        /// <summary>
        /// Maximum queued requests permit limit within the window
        /// 在一个时间窗口内的最大排队请求许可限制
        /// </summary>
        public int QueueLimit { get; init; } = 10;

        /// <summary>
        /// Time window of seconds
        /// 时间窗口秒数
        /// </summary>
        public int WindowSeconds { get; init; } = 5;
    }

    /// <summary>
    /// Etsoo rate limiter policy
    /// https://blog.maartenballiauw.be/post/2022/09/26/aspnet-core-rate-limiting-middleware.html
    /// 亿速思维速率限制策略
    /// </summary>
    public class EtsooRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        readonly EtsooRateLimiterOptions _options;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="options">Options</param>
        public EtsooRateLimiterPolicy(EtsooRateLimiterOptions? options = null)
        {
            _options = options ?? new EtsooRateLimiterOptions();
        }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            if (httpContext.User.Identity?.IsAuthenticated is true)
            {
                // User
                var user = MinUserToken.Create(httpContext.User);
                if (user != null)
                {
                    // Check the user is API user or not
                    var isApiUser = user.Role?.HasFlag(UserRole.API) is true;

                    var partitionKey = isApiUser ? $"{user.Id}:api" : user.Id;
                    var permitLimit = isApiUser ? _options.PermitLimit * _options.ApiMultiplier : _options.PermitLimit;
                    var queueLimit = isApiUser ? _options.QueueLimit * _options.ApiMultiplier : _options.QueueLimit;
                    var window = TimeSpan.FromSeconds(_options.WindowSeconds);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey,
                        partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = permitLimit,
                            QueueLimit = queueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            Window = window
                        }
                    );
                }
            }

            // It's not a good idea to filter by IP addres for unauthenticated users but should be controlled by Nginx Ingress in Kubernetes or other reverse proxies
            // These annotations define limits on connections and transmission rates. These can be used to mitigate DDoS Attacks.
            // https://kubernetes.github.io/ingress-nginx/user-guide/nginx-configuration/annotations/#rate-limiting
            return RateLimitPartition.GetNoLimiter("");
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } = (context, _) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return new ValueTask();
        };
    }
}
