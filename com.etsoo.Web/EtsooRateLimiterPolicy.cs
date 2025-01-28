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
        /// Anonymous user maximum requests limit, -1 means no limit, 0 means login required
        /// 匿名用户最大请求限制，-1表示无限制，0表示需要登录
        /// </summary>
        public int AnonymousLimit { get; init; } = -1;

        /// <summary>
        /// API multiplier
        /// 接口倍数
        /// </summary>
        public int ApiMultiplier { get; init; } = 5;

        /// <summary>
        /// Delete requests limit
        /// 删除请求限制
        /// </summary>
        public int DeleteLimit { get; init; } = 2;

        /// <summary>
        /// Maximum requests permit limit within the window
        /// 在一个时间窗口内的最大请求许可限制
        /// </summary>
        public int PermitLimit { get; init; } = 60;

        /// <summary>
        /// Maximum queued requests permit limit within the window
        /// 在一个时间窗口内的最大排队请求许可限制
        /// </summary>
        public int QueueLimit { get; init; } = 5;

        /// <summary>
        /// Time window of minutes
        /// 时间窗口分钟数
        /// </summary>
        public int WindowMinutes { get; init; } = 2;

        /// <summary>
        /// Segments per window
        /// 在一个时间窗口内的段数
        /// </summary>
        public int SegmentsPerWindow { get; init; } = 4;
    }

    /// <summary>
    /// Etsoo rate limiter policy
    /// https://blog.maartenballiauw.be/post/2022/09/26/aspnet-core-rate-limiting-middleware.html
    /// 亿速思维速率限制策略
    /// </summary>
    public class EtsooRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        readonly EtsooRateLimiterOptions _options;
        readonly TimeSpan _window;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="options">Options</param>
        public EtsooRateLimiterPolicy(EtsooRateLimiterOptions? options = null)
        {
            _options = options ?? new EtsooRateLimiterOptions();
            _window = TimeSpan.FromMinutes(_options.WindowMinutes);
        }

        RateLimitPartition<string> CreateDeletePartition(string key)
        {
            return RateLimitPartition.GetSlidingWindowLimiter(
                key,
                partition => new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = _options.DeleteLimit,
                    Window = _window,
                    SegmentsPerWindow = _options.SegmentsPerWindow
                }
            );
        }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            var window = TimeSpan.FromMinutes(_options.WindowMinutes);

            if (httpContext.User.Identity?.IsAuthenticated is true)
            {
                // User
                var user = MinUserToken.Create(httpContext.User);
                if (user != null)
                {
                    // Limit Delete requests rate
                    if (httpContext.Request.Method == "DELETE" && _options.DeleteLimit > 0)
                    {
                        return CreateDeletePartition($"{user.Id}:delete");
                    }

                    // Check the user is API user or not
                    var isApiUser = user.Role?.HasFlag(UserRole.API) is true;

                    var partitionKey = isApiUser ? $"{user.Id}:api" : user.Id;
                    var permitLimit = isApiUser ? _options.PermitLimit * _options.ApiMultiplier : _options.PermitLimit;
                    var queueLimit = isApiUser ? _options.QueueLimit * _options.ApiMultiplier : _options.QueueLimit;

                    // Sliding Window smoothly handles requests over time by considering requests within the "sliding" window
                    // When SegmentsPerWindow is 3, will calculate the past 3 segments to decide the rate limit
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey,
                        partition => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = _options.PermitLimit,
                            QueueLimit = _options.QueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            Window = window,
                            SegmentsPerWindow = _options.SegmentsPerWindow
                        }
                    );
                }
            }

            var anonymousLimit = _options.AnonymousLimit;
            if (anonymousLimit == 0)
            {
                throw new UnauthorizedAccessException("Login Required for Rate Limit");
            }

            var anonymousKey = httpContext.Request.Host.ToString();

            // Limit Delete requests rate
            if (httpContext.Request.Method == "DELETE" && _options.DeleteLimit > 0)
            {
                return CreateDeletePartition($"{anonymousKey}:delete");
            }

            // It's not a good idea to filter by IP addres for unauthenticated users but should be controlled by Nginx Ingress in Kubernetes or other reverse proxies
            // These annotations define limits on connections and transmission rates. These can be used to mitigate DDoS Attacks.
            // https://kubernetes.github.io/ingress-nginx/user-guide/nginx-configuration/annotations/#rate-limiting
            if (anonymousLimit < 0)
            {
                return RateLimitPartition.GetNoLimiter(anonymousKey);
            }
            else
            {
                return RateLimitPartition.GetSlidingWindowLimiter(
                    anonymousKey,
                    partition => new SlidingWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = anonymousLimit,
                        Window = window,
                        SegmentsPerWindow = _options.SegmentsPerWindow
                    }
                );
            }
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } = (context, _) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return new ValueTask();
        };
    }
}
