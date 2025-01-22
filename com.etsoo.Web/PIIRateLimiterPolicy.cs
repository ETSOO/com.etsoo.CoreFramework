using com.etsoo.CoreFramework.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace com.etsoo.Web
{
    /// <summary>
    /// PII private API rate limiter options
    /// PII 隐私接口速率限制选项
    /// </summary>
    public record PIIApiRateLimiterOptions
    {
        /// <summary>
        /// Maximum tokens within the window
        /// 在一个时间窗口内最大的令牌数
        /// </summary>
        public int TokenLimit { get; init; } = 120;

        /// <summary>
        /// Replenishment tokens per perid
        /// 一个时间窗口内的补货令牌书 
        /// </summary>
        public int TokensPerPeriod { get; init; } = 60;

        /// <summary>
        /// Replenishment minutes
        /// 补货分钟数
        /// </summary>
        public int ReplenishmentMinutes { get; init; } = 30;
    }

    /// <summary>
    /// PII private API rate limiter policy, such as limiting browsing user and customer information
    /// https://blog.maartenballiauw.be/post/2022/09/26/aspnet-core-rate-limiting-middleware.html
    /// PII 隐私接口速率限制策略，比如限制浏览用户和客户信息
    /// </summary>
    public class PIIApiRateLimiterPolicy : IRateLimiterPolicy<string>
    {
        readonly PIIApiRateLimiterOptions _options;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="options">Options</param>
        public PIIApiRateLimiterPolicy(PIIApiRateLimiterOptions? options = null)
        {
            _options = options ?? new PIIApiRateLimiterOptions();
        }

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            if (httpContext.User.Identity?.IsAuthenticated is true)
            {
                // User
                var user = MinUserToken.Create(httpContext.User);
                if (user != null)
                {
                    var period = TimeSpan.FromMinutes(_options.ReplenishmentMinutes);

                    return RateLimitPartition.GetTokenBucketLimiter(
                        user.Id,
                        partition => new TokenBucketRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            TokenLimit = _options.TokenLimit,
                            TokensPerPeriod = _options.TokensPerPeriod,
                            ReplenishmentPeriod = period,
                        }
                    );
                }
            }

            return RateLimitPartition.GetFixedWindowLimiter(
                "",
                partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 1,
                    Window = TimeSpan.FromDays(1)
                }
            );
        }

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } = (context, _) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return new ValueTask();
        };
    }
}
